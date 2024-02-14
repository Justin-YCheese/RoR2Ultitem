using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using RoR2;
using UltitemsCyan.Items.Tier1;
using UnityEngine;
using System;

namespace UltitemsCyan.Buffs
{
    public class SlipperyGrape : BuffBase
    {
        public static BuffDef buff;
        //private const float birthdayBuffBaseMultiplier = 0.2f;
        //private const float baseTickMultiplier = FleaBag.baseTickMultiplier;
        //private const float tickPerStack = FleaBag.tickPerStack;

        public override void Init()
        {
            buff = DefineBuff("Slippery Grape Buff", true, false, Color.white, Ultitems.Assets.GrapeSprite, false, false);
            //Log.Info(buff.name + " Initialized");
            Hooks();
        }

        protected void Hooks()
        {
            //IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        /*
        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il); // Make new ILContext

            int num12 = -1;

            Log.Warning("Slippery Grape Take Damage");

            // Inject code just before damage is subtracted from health
            // Go just before the "if (num12 > 0f && this.barrier > 0f)" line, which is equal to the following instructions

            if (c.TryGotoNext(MoveType.Before,
                x => x.MatchLdfld<JunkContent>("BodyArmor"),
                x => x.MatchLdcR4(0f),
                x => x.Match(OpCodes.Ble_Un_S))
            )
            {
                Log.Debug(" * * * Start C Index: " + c.Index + " > " + c.ToString());
                // [Warning:UltitemsCyan] * **Start C Index: 1173 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-822050560::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1173, Next
                // IL_0e8a: blt.s IL_0e70
                // IL_0e8f: ldloc.s V_7

                c.Index++;


                Log.Debug(" * * * +1 Working Index: " + c.Index + " > " + c.ToString());
                // [Warning:UltitemsCyan]  * * * Working Index: 1174 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-822050560::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1174, None
                // IL_0e8f: ldloc.s V_7
                // IL_0e91: ldc.r4 0

                //c.GotoNext(MoveType.Before, x => x.MatchLdcR4(0f));
                //Log.Warning(" * * * Before LdcR4 0f: " + c.Index + " > " + c.ToString());
                // [Warning:UltitemsCyan]  * * * Before LdcR4 0f: 1174 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-822050560::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1174, Next
                // IL_0e8f: ldloc.s V_7
                // IL_0e91: ldc.r4 0

                c.Emit(OpCodes.Ldarg, 0);       // Load Health Component
                c.Emit(OpCodes.Ldloc, 1);       // Load Attacker Character Body
                c.Emit(OpCodes.Ldloc, num12);   // Load Total Damage

                // Run custom code
                c.EmitDelegate<Action<HealthComponent, CharacterBody, float>>((hc, aCb, td) =>
                {
                    
                });
            }
            else
            {
                Log.Warning("Silver cannot find '(num12 > 0f && this.barrier > 0f)'");
            }
        }
        //*/
    }
}