using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UltitemsCyan.Items.Tier1;
using UnityEngine;
using System;
using System.Reflection;
using R2API.Utils;
using Rewired.Utils;

namespace UltitemsCyan.Buffs
{
    public class SlipperyGrape : BuffBase
    {
        public static BuffDef buff;
        private const float grapeBlockChance = Items.Tier3.Grapevine.grapeBlockChance;
        //private const int maxGrapes = Items.Tier3.Grapevine.maxGrapes;
        //private const float tickPerStack = FleaBag.tickPerStack;

        public override void Init()
        {
            buff = DefineBuff("Slippery Grape Buff", true, false, Color.white, UltAssets.GrapeSprite, false, false);
            //Log.Info(buff.name + " Initialized");
            Hooks();
        }

        protected void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        //
        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il); // Make new ILContext

            //int num12 = -1;

            Log.Warning("Slippery Grape Take Damage");

            // Inject code just before damage is subtracted from health
            // Go just before the "if (num12 > 0f && this.barrier > 0f)" line, which is equal to the following instructions

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdarg(1),
                x => x.MatchLdfld<DamageInfo>("rejected"),
                x => x.Match(OpCodes.Brfalse_S))
            )
            {
                Log.Debug(" * * * Start C Index: " + c.Index + " > " + c.ToString());
                // [Debug  :UltitemsCyan]  * * * Start C Index: 258 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-1353022208::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 258, Next
                // IL_0347: blt.s IL_0330
                // IL_034c: ldarg.1

                c.Index++;


                Log.Debug(" * * * +1 Working Index: " + c.Index + " > " + c.ToString());
                // [Debug  :UltitemsCyan]  * * * +1 Working Index: 259 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-1353022208::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 259, None
                // IL_034c: ldarg.1
                // 034d: ldfld System.Boolean RoR2.DamageInfo::rejected


                //c.GotoNext(MoveType.Before, x => x.MatchLdcR4(0f));
                //Log.Warning(" * * * Before LdcR4 0f: " + c.Index + " > " + c.ToString());
                // [Warning:UltitemsCyan]  * * * Before LdcR4 0f: 1174 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-822050560::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1174, Next
                // IL_0e8f: ldloc.s V_7
                // IL_0e91: ldc.r4 0

                c.Emit(OpCodes.Ldarg, 0);   // Load Health Component
                c.Emit(OpCodes.Ldarg, 1);   // Load Damage Info

                // Run custom code
                c.EmitDelegate<Action<HealthComponent, DamageInfo>>((hc, di) =>
                {
                    //Log.Warning("Slippery Grapes Block?");
                    if (di.rejected == false)
                    {
                        CharacterBody cb = hc.body;
                        if (cb)
                        {
                            int grapes = cb.GetBuffCount(buff);     // TODO optimize with one random number generator, find location
                            for (int i = 0; i < grapes; i++)    // Ecential While loop but max 'grapes' times
                            {
                                //Log.Debug(" - " + i);
                                cb.RemoveBuff(buff);
                                if (Util.CheckRoll(grapeBlockChance, 0))
                                {
                                    Log.Debug("Slip Grape !");
                                    di.rejected = true;     // Can set value here because it's a reference. Cannot do the same for primative types?

                                    //EffectManager.SpawnEffect(HealthComponent.AssetReferences.damageRejectedPrefab, new EffectData
                                    //{
                                    //    origin = di.position
                                    //}, true);

                                    EffectManager.SpawnEffect(HealthComponent.AssetReferences.bearEffectPrefab, new EffectData
                                    {
                                        origin = di.position,
                                        rotation = Util.QuaternionSafeLookRotation((di.force != Vector3.zero) ? di.force : UnityEngine.Random.onUnitSphere),
                                        //color = new Color(9, 153, 61), // Grape Colors  Deson't Do Anything
                                        //scale = 50f
                                    }, true);
                                    return;
                                }
                            }
                        }
                    }
                    //return false;
                });

                //c.Emit(OpCodes.Stfld, typeof(DamageInfo).GetFieldCached("rejected")); * Crash
                //var field = typeof(DamageInfo).GetField("rejected", BindingFlags.FlattenHierarchy | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                //Log.Debug("Rejected?: " + field.ToString() + " | " + field.IsNullOrDestroyed()); // Not Null
                //c.Emit(OpCodes.Stfld, field); * Crash
                //c.Emit<DamageInfo>(OpCodes.Stfld, "rejected");   // Store Damage Info Rejected * Crash

                //*/
            }
            else
            {
                Log.Warning("Silver cannot find '(num12 > 0f && this.barrier > 0f)'");
            }
        }
        //*/
    }
}