using Mono.Cecil.Cil;
using MonoMod.Cil;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using UltitemsCyan.Items.Untiered;
using UnityEngine;

namespace UltitemsCyan.Items.Tier1
{
    // Currently triggers after Sue's Mandibles
    // TODO: check if Item classes needs to be public
    public class KoalaSticker : ItemBase
    {
        public static ItemDef item;
        private const float hyperbolicPercent = 12f;

        public override void Init()
        {
            item = CreateItemDef(
                "KOALASTICKER",
                "Koala Sticker",
                "Reduce the maximum damage you can take.",
                "You only take a maxinum of 90% (-12% per stack) of your health from a hit, mininum of 1.",
                "Like the bear but more consistant...   and more cute",
                ItemTier.Tier1,
                Ultitems.Assets.KoalaStickerSprite,
                Ultitems.Assets.KoalaStickerPrefab,
                [ItemTag.Utility]
            );
        }


        protected override void Hooks()
        {
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il); // Make new ILContext

            int num = -1;   //Initial Total Damage
            int num12 = -1; //New Total Damage

            // Inject code just before damage is subtracted from health
            // Go just before the "if (num12 > 0f && this.barrier > 0f)" line, which is equal to the following instructions

            Log.Warning("Koala Sticker Take Damage");

            if (c.TryGotoNext(MoveType.Before,                              // TODO make cursor search more robust
                x => x.MatchLdloc(out num),                                 // 1130 ldloc.s V_6 (6)
                x => x.MatchStloc(out num12),                               // 1130 stloc.s V_7 (7)
                x => x.MatchLdloc(out num12),                               // 1130 ldloc.s V_7 (7)
                x => x.MatchLdcR4(0f),                                      // 1131 ldc.r4 0
                x => x.Match(OpCodes.Ble_Un_S),                             // 1132 ble.un.s 1200 (0D38) ldloc.s V_7 (7)
                x => x.MatchLdarg(0),                                       // 1133 ldarg.0
                x => x.MatchLdcI4(0),                                       // 1134 ldci4.0
                x => x.MatchStfld<HealthComponent>("isShieldRegenForced")   // 1135 ldfld float32 RoR2.HealthComponent::barrier
            ))
            {

                Log.Debug(" * * * Start C Index: " + c.Index + " > " + c.ToString());
                //[Warning:UltitemsCyan] * **Start C Index: 1129 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-456176384::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1129, Next
                //IL_0e05: stfld System.Single RoR2.HealthComponent::adaptiveArmorValue
                //IL_0e0a: ldloc.s V_6

                //give_item koalasticker 100

                c.Index += 4;

                Log.Debug(" * * * +4 Working Index: " + c.Index + " > " + c.ToString());
                //[Debug  :UltitemsCyan] * **+4 Working Index: 1133 > // ILCursor: System.Void DMD<RoR2.HealthComponent::TakeDamage>?-771449600::RoR2.HealthComponent::TakeDamage(RoR2.HealthComponent,RoR2.DamageInfo), 1133, None
                //IL_0e10: ldc.r4 0
                //IL_0e15: ble.un.s IL_0e21


                c.Emit(OpCodes.Ldarg, 0);       // Load Health Component
                //c.Emit(OpCodes.Ldarg, 1);     // Load Damage Info (If Damage rejected, returned earlier)
                c.Emit(OpCodes.Ldloc, num);     // Load Total Damage

                // Run custom code
                c.EmitDelegate<Func<HealthComponent, float, float>>((hc, td) =>
                {
                    CharacterBody cb = hc.body;
                    //Log.Debug("Health: " + hc.fullCombinedHealth + "\t Body: " + cb.GetUserName() + "\t Damage: " + td);
                    if (cb.master.inventory)
                    {
                        int grabCount = cb.master.inventory.GetItemCount(item);
                        if (grabCount > 0)
                        {
                            Log.Debug("Koala Taken Damage for " + cb.GetUserName() + " with " + hc.fullCombinedHealth + "\t health");
                            //Log.Debug("Max Percent: " + ((hyperbolicPercent / 100 * grabCount) + 1) + " of " + hc.fullCombinedHealth);
                            float maxDamage = hc.fullCombinedHealth / ((hyperbolicPercent / 100 * grabCount) + 1);
                            Log.Debug("Is " + td + "\t > " + maxDamage + "?");
                            if (td > maxDamage)
                            {
                                Log.Debug("Yes");
                                return maxDamage;
                            }
                        }
                    }
                    return td;
                });

                c.Emit(OpCodes.Stloc, num12); // Store Total Damage
                //
                //}
                //else
                //{
                //    Log.Warning("Koala cannot find 'for (int k = 0; k < num15; k++){}'");
                //}
            }
            else
            {
                Log.Warning("Koala cannot find '(num12 > 0f && this.barrier > 0f)'");
            }
        }
    }
}