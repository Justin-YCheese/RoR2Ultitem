﻿using Mono.Cecil.Cil;
using MonoMod.Cil;
using RoR2;
using System;
using UnityEngine;
using UnityEngine.Networking;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Lunar
{
    
    // TODO: check if Item classes needs to be public
    public class SilverThread : ItemBase
    {
        public static ItemDef item;

        private const int maxStack = 3;
        private const int costMultiplier = 2;
        private const int extraItemAmount = 1;

        //private const float percentPerStack = 50f;
        //private const float deathSnapTime = 600f; // 10 minutes

        private const float baseThreadChance = 50f;
        private const float stackThreadChance = 25f;

        //private bool inSilverAlready = false;

        public override void Init()
        {
            item = CreateItemDef(
                "SILVERTHREAD",
                "Silver Thread",
                "Chance to gain additional items... <style=cDeath>BUT chance of dying upon being attacked</style>. Upon death, this item will be consumed.",
                "<style=cIsUtility>50%</style> <style=cStack>(+25% chance per stack)</style> chance to pick up <style=cIsUtility>1</style> additional item. You have a chance of <style=cDeath>dying</style> equal to <style=cIsUtility>100%</style> <style=cStack>(+100% per stack)</style> health lost. <style=cIsUtility>Upon death</style>, this item will be <style=cIsUtility>consumed</style>. <style=cIsUtility>Unaffected by luck</style>.",
                "The end of the abacus of life. A King's Riches Lays before you, but at the end of a strand which has been snapped intwine.",
                ItemTier.Lunar,
                Ultitems.Assets.SilverThreadSprite,
                Ultitems.Assets.SilverThreadPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect]
            );
        }

        protected override void Hooks()
        {
            // Gain additional items
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            // Increase cauldron and 3D printer cost
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            // Increase scrapper cost
            On.RoR2.ScrapperController.BeginScrapping += ScrapperController_BeginScrapping;
            // Chance of Death
            IL.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            // Remove Item on Death
            On.RoR2.CharacterBody.OnDeathStart += CharacterBody_OnDeathStart;
        }

        private int MaxStack(Inventory inv)
        {//Test This Code
            return Math.Min(inv.GetItemCount(item), maxStack);
        }

        // Chance of Death on hit
        private void HealthComponent_TakeDamage(ILContext il)
        {
            ILCursor c = new(il); // Make new ILContext

            int num12 = -1;

            Log.Warning("Silver Thread Take Damage");

            // Inject code just before damage is subtracted from health
            // Go just before the "if (num12 > 0f && this.barrier > 0f)" line, which is equal to the following instructions

                                                                      // 1170 ldloc.s V_49 (49)             // Previous For loop k value
            if (c.TryGotoNext(MoveType.Before,                        // 1171 blt.s 1161 (0D7E) ldarg.0     // Previous For Loop branch
                x => x.MatchLdloc(out num12),                         // 1172 ldloc.s V_7 (7)
                x => x.MatchLdcR4(0f),                                // 1173 ldc.r4 0
                x => x.Match(OpCodes.Ble_Un_S),                       // 1174 ble.un.s 1200 (0DE8) ldloc.s V_7 (7)
                x => x.MatchLdarg(0),                                 // 1175 ldarg.0
                x => x.MatchLdfld<HealthComponent>("barrier"),        // 1176 ldfld float32 RoR2.HealthComponent::barrier
                x => x.MatchLdcR4(0f),                                // 1177 ldc.r4 0
                x => x.Match(OpCodes.Ble_Un_S))                       // 1178 ble.un.s 1200 (0DE8) ldloc.s V_7 (7)
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
                    CharacterBody cb = hc.body;
                    //Log.Debug("Health: " + hc.fullCombinedHealth + "\t Body: " + cb.GetUserName() + "\t Damage: " + td);
                    //Log.Warning("Damage Info " + di.ToString() + " with " + di.damage + " initial damage");
                    if (cb && cb.master && cb.master.inventory && aCb)
                    {
                        //if (aCb.master.inventory){ Log.Debug("and has inventory"); }
                        int grabCount = MaxStack(cb.master.inventory);
                        if (grabCount > 0)
                        {
                            Log.Debug("Silver Taken Damage for " + cb.GetUserName() + " with " + hc.fullCombinedHealth + "\t health");
                            Log.Debug(aCb.GetUserName() + " is attacker");
                            float deathChance = td / hc.fullCombinedHealth * 100f * grabCount;
                            if (deathChance > 100f)
                            {
                                deathChance = 100f;
                            }
                            Log.Debug("Chance of Snapping: " + deathChance);
                            if (Util.CheckRoll(deathChance, 0))
                            {
                                snapBody(cb, aCb, grabCount);
                            }
                        }
                    }
                });
            }
            else
            {
                Log.Warning("Silver cannot find '(num12 > 0f && this.barrier > 0f)'");
            }
        }

        // Kill character body
        private static void snapBody(CharacterBody body, CharacterBody killer, int grabCount)
        {
            Log.Warning(body.GetUserName() + "'s thread snapped");
            // If has item
            //int grabCount = body.inventory.GetItemCount(item);
            // Check generally even if actuall body doesn't have threads anymore
            //deathCheckStealController(body);
            //body.inventory.RemoveItem(item.itemIndex, grabCount);
            //body.inventory.GiveItem(Untiered.SilverThreadConsumed.item, grabCount);
            Chat.AddMessage("Your thread of life has snapped...");
            body.healthComponent.Suicide(killer.gameObject);
        }

        // Remove Silver on Death
        private void CharacterBody_OnDeathStart(On.RoR2.CharacterBody.orig_OnDeathStart orig, CharacterBody self)
        {
            orig(self);
            //Log.Warning("Silver Normal Death?");
            if (self && self.master && self.master.inventory)
            {
                int grabCount = self.master.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Warning("Removing Silver threads from " + self.GetUserName());
                    self.master.inventory.RemoveItem(item.itemIndex, grabCount);
                    self.master.inventory.GiveItem(Untiered.SilverThreadConsumed.item, grabCount);
                }
            }
        }

        // Increase 3D printer / Cauldron cost
        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            bool runOrig = true;
            if (NetworkServer.active && self && activator && activator)
            {
                // If an item cost other than treasure cache cost
                if (self.costType is
                    CostTypeIndex.WhiteItem or
                    CostTypeIndex.GreenItem or
                    CostTypeIndex.RedItem or
                    CostTypeIndex.BossItem or
                    CostTypeIndex.LunarItemOrEquipment
                    )
                {
                    Log.Warning("Silver Purchase check");
                    CharacterBody player = activator.GetComponent<CharacterBody>();
                    //int grabSilverCount = MaxStack(player.master.inventory);
                    if (player.master.inventory.GetItemCount(item) > 0)
                    {
                        runOrig = false;
                        Log.Debug("Self Cost? " + self.cost + " * " + costMultiplier);
                        self.cost *= costMultiplier;
                        Log.Debug("New Self Cost? " + self.cost);

                        orig(self, activator);

                        self.cost /= costMultiplier;
                        Log.Debug("Post Self Cost? " + self.cost);
                    }
                }
            }
            //
            if (runOrig)
            {
                orig(self, activator);
            }//*/
        }

        // Make Scrapper return fewer items per Silver Thread Held
        private void ScrapperController_BeginScrapping(On.RoR2.ScrapperController.orig_BeginScrapping orig, ScrapperController self, int intPickupIndex)
        {
            Log.Warning("Silver Scrapping check");
            bool runOrig = true;
            if (NetworkServer.active && self)
            {
                CharacterBody player = self.interactor.GetComponent<CharacterBody>();
                if (player && player.master.inventory)
                {
                    //int grabSilverCount = MaxStack(player.master.inventory);
                    if (player.master.inventory.GetItemCount(item) > 0)
                    {
                        Log.Debug("Silver Scrapping custom function");
                        // body has a silver thread in their inventory
                        runOrig = false;

                        self.itemsEaten = 0;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(intPickupIndex));
                        if (pickupDef != null && self.interactor)
                        {
                            self.lastScrappedItemIndex = pickupDef.itemIndex;
                            int scrapCount = Mathf.Min(self.maxItemsToScrapAtATime * costMultiplier, player.inventory.GetItemCount(pickupDef.itemIndex));
                            if (scrapCount <= costMultiplier)
                            {
                                // not enough items to convert item, don't return anything
                                Log.Debug("Silver Scrapper Consume poor items");
                                self.itemsEaten = -1;
                            }
                            else
                            {
                                // return reduced amount
                                Log.Debug("scrapCount: " + scrapCount + " returnCount: " + (scrapCount / costMultiplier));
                                player.inventory.RemoveItem(pickupDef.itemIndex, scrapCount);
                                self.itemsEaten += scrapCount / costMultiplier;
                                for (int i = 0; i < scrapCount; i++)
                                {
                                    ScrapperController.CreateItemTakenOrb(player.corePosition, self.gameObject, pickupDef.itemIndex);
                                }
                                if (self.esm)
                                {
                                    self.esm.SetNextState(new EntityStates.Scrapper.WaitToBeginScrapping());
                                }
                            }
                        }
                        /*/ Scrapper Controller Begin Scrapper
                        this.itemsEaten = 0;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(intPickupIndex));
                        if (pickupDef != null && this.interactor)
                        {
                            this.lastScrappedItemIndex = pickupDef.itemIndex;
                            CharacterBody component = this.interactor.GetComponent<CharacterBody>();
                            if (component && component.inventory)
                            {
                                int num = Mathf.Min(this.maxItemsToScrapAtATime, component.inventory.GetItemCount(pickupDef.itemIndex));
                                if (num > 0)
                                {
                                    component.inventory.RemoveItem(pickupDef.itemIndex, num);
                                    this.itemsEaten += num;
                                    for (int i = 0; i < num; i++)
                                    {
                                        ScrapperController.CreateItemTakenOrb(component.corePosition, base.gameObject, pickupDef.itemIndex);
                                    }
                                }
                            }
                        }
                        if (this.esm)
                        {
                            this.esm.SetNextState(new WaitToBeginScrapping());
                        }//*/
                    }
                }
            }
            // If checks failed, run original function
            if (runOrig)
            {
                Log.Debug("runOrig in SilverThread");
                orig(self, intPickupIndex);
                Log.Debug("runOrig out SilverThread");
            }
            
        }

        // Increase Items gained when given
        public void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            // && !inSilverAlready
            if (NetworkServer.active && count == 1 && self && ItemCatalog.GetItemDef(itemIndex).tier != ItemTier.NoTier && itemIndex != item.itemIndex)
            {
                // Precaution incase something causes an infinity loop of items
                //inSilverAlready = true;
                int grabCount = MaxStack(self);
                if (grabCount > 0)
                {
                    //Log.Debug("Thread Chance: " + (baseThreadChance + (stackThreadChance * (grabCount - 1))));
                    if (Util.CheckRoll(baseThreadChance + (stackThreadChance * (grabCount - 1))))
                    {
                        Log.Debug("Extra " + ItemCatalog.GetItemDef(itemIndex).name);
                        count += extraItemAmount;
                    }

                }
            }
            Log.Debug("SPECIAL in orig SilverThread");
            orig(self, itemIndex, count);
            Log.Debug("SPECIAL out orig SilverThread");
        }
    }
}