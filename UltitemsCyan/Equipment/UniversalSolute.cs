using RoR2;
using UltitemsCyan.Items.Untiered;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;
using UltitemsCyan.Items.Lunar;
using System;

namespace UltitemsCyan.Equipment
{
    /* NOTES
     * 
     * Removing normal
     * 
     * When dissolving a boss item, the item will still be dropped by the boss for teleporter and tricorn
     *      removing from available items will only effect command
     * If you dissolve void items, then Larva won't corrupt thoes pairs upon dying
     * 
     */

    // TODO: check if Item classes needs to be public
    public class UniversalSolute : EquipmentBase
    {
        public static EquipmentDef equipment;
        
        private const float shortCooldown = 6f;
        private const float cooldown = 60f;

        // Keeps track of the dissolved items of the current stage
        private List<ItemIndex> dissolvedList = [];

        public override void Init()
        {
            equipment = CreateItemDef(
                "UNIVERSALSOLUTE",
                "Universal Solute",
                "<style=cDeath>Erase</style> your last item from existence.",
                "<style=cDeath>Erase</style> the last item in your inventory from the run. It will no longer appear, and any instances of the items will <style=cDeath>break</style>.",
                "Everything returns to grey",
                cooldown,
                true,
                true,
                false,
                UltAssets.UniversalSoluteSprite,
                UltAssets.UniversalSolutePrefab
            );
        }

        protected override void Hooks()
        {
            // Clear dissolved so refresh between runs and stages
            // Only really need dissolved for grabing items that were already dropped but dissolved.
            // Dissolved items update by the next stage, so can clear list
            On.RoR2.Run.BeginStage += Run_BeginStage;
            // * * * Erase Items
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            // * * * Maintain removal
            // When getting a dissolved item
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            // When a chest tries dropping a dissolved item
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
            On.RoR2.OptionChestBehavior.ItemDrop += OptionChestBehavior_ItemDrop;
            //On.RoR2.PickupDropTable.GenerateDrop += PickupDropTable_GenerateDrop;
        }

        private PickupIndex PickupDropTable_GenerateDrop(On.RoR2.PickupDropTable.orig_GenerateDrop orig, PickupDropTable self, Xoroshiro128Plus rng)
        {
            PickupIndex pickup = orig(self, rng);
            if (dissolvedList.Contains(PickupCatalog.GetPickupDef(pickup).itemIndex))
            {
                Log.Debug("Pickup " + PickupCatalog.GetPickupDef(pickup).nameToken + " was dissolved...");
                return pickup;
            }
            else
            {
                return pickup;
            }
        }

        private void Run_BeginStage(On.RoR2.Run.orig_BeginStage orig, Run self)
        {
            Log.Debug("Universal Dissolved cleared");
            dissolvedList.Clear();

            orig(self);
        }

        // Delete Existing instances of the item, and remove from drops
        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                if (NetworkServer.active)
                {
                    Log.Debug("Running Solute on Server");
                }
                else
                {
                    Log.Debug("Running Solute on Client");
                }

                /*/
                if (self.gameObject && self.gameObject.name.Contains("EquipmentDrone"))
                {
                    return false;
                }//*/

                CharacterBody activator = self.characterBody;
                List<ItemIndex> itemList = activator.inventory.itemAcquisitionOrder;

                // Make delete self when you have no items?
                if (itemList.Count > 0)
                {
                    // Null if player has only untiered items, or no items
                    ItemDef lastItem = getLastItem(itemList);

                    if (lastItem)
                    {
                        Run thisRun = Run.instance;

                        if (thisRun.isRunStopwatchPaused)
                        {
                            Log.Debug("In time paused");
                            equipment.cooldown = shortCooldown;
                        }
                        else
                        {
                            Log.Debug("Outside time paused");
                            equipment.cooldown = cooldown;
                        }

                        Log.Debug("Last Item: " + lastItem.name);

                        // * * * For Every player and monster remove the item
                        foreach (CharacterMaster body in CharacterMaster.readOnlyInstancesList)
                        {
                            Log.Debug("who? " + body.name);
                            // Checks inventory in function
                            dissolveItem(body, lastItem);
                        }

                        Log.Warning(PickupCatalog.itemIndexToPickupIndex[(int)DreamFuel.item.itemIndex] + " is in? "
                            + thisRun.availableLunarCombinedDropList.Contains(PickupCatalog.itemIndexToPickupIndex[(int)DreamFuel.item.itemIndex]));

                        // * * * Remove item from pools
                        thisRun.DisableItemDrop(lastItem.itemIndex);
                        //Run.instance.DisablePickupDrop(PickupCatalog.itemIndexToPickupIndex[(int)lastItem.itemIndex]);
                        thisRun.availableItems.Remove(lastItem.itemIndex);
                        checkEmptyTierList(lastItem); // also check if empty, if so then add solute to item tier
                        dissolvedList.Add(lastItem.itemIndex);
                        thisRun.RefreshLunarCombinedDropList();

                        Log.Debug(PickupCatalog.itemIndexToPickupIndex[(int)DreamFuel.item.itemIndex] + " is in? "
                            + thisRun.availableLunarCombinedDropList.Contains(PickupCatalog.itemIndexToPickupIndex[(int)DreamFuel.item.itemIndex]));

                        /*/ Refresh chest and lunar pools
                        Log.Warning("Refresing ALL ! ! !");
                        foreach (var dropTable in PickupDropTable.instancesList)
                        {
                            Log.Debug(" . " + dropTable.GetType().ToString() + " | " + dropTable.GetPickupCount());
                        }
                        //*/



                        //Run.instance.BuildDropTable();
                        //PickupDropTable.RegenerateAll(Run.instance);




                        //foreach (var test in PickupDropTable.instancesList)
                        //{

                        //}

                        //PickupDropTable.instancesList

                        //Log.Debug(dissolvedItems.Contains(lastItem.itemIndex) + " in dessolved items");
                        Util.PlaySound("Play_minimushroom_spore_shoot", self.gameObject);

                        // * * * Remove items from shops and item pickups? Turn into solvent?

                        //Run.instance.shopPortalCount;
                        //Run.instance.
                        /*
                        var Interactors = Run.instance.GetComponents<Interactor>();
                        Log.Debug("Length: " + Interactors.Length);
                        foreach (Interactor interactor in Interactors)
                        {
                            Log.Debug("Interactor name: " + interactor.name);
                            if (interactor.GetComponent<ShopTerminalBehavior>())
                            {
                                Log.Debug("has Shop");
                            }
                        }//*/

                        //ShopTerminalBehavior.GenerateNewPickupServer(true);


                        //UpdatePickupDisplayAndAnimations()
                        //PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);

                        //DisableItemDisplay(ItemIndex itemIndex)

                        return true;
                    }
                }
                return false;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            //Log.Debug("Test Solute for item index " + itemIndex);
            if (dissolvedList.Contains(itemIndex) && self)
            {
                Log.Debug("Grabbed a disolved item...");
                Util.PlaySound("Play_minimushroom_spore_shoot", self.gameObject);
                itemIndex = GreySolvent.item.itemIndex;

            }
            Log.Warning(" ] ] ] ] ] ORIG into the Universal");
            orig(self, itemIndex, count);
            Log.Warning("  [ [ [ ORIG out to the Universal");
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            // If item in chest is in dissolved list then reroll untill it isn't
            if (dissolvedList.Count > 0 && dissolvedList.Contains(PickupCatalog.GetPickupDef(self.dropPickup).itemIndex))
            {
                Log.Debug(" // Chest Universal has a dissolved item: " + PickupCatalog.GetPickupDef(self.dropPickup).nameToken);
                // When rerolled will use updated avaialbe items list
                self.Roll();
                //Log.Debug("Is still dissolved?.. " + PickupCatalog.GetPickupDef(self.dropPickup).nameToken + " | " + dissolvedList.Contains(PickupCatalog.GetPickupDef(self.dropPickup).itemIndex));
            }
            orig(self);
        }

        private void OptionChestBehavior_ItemDrop(On.RoR2.OptionChestBehavior.orig_ItemDrop orig, OptionChestBehavior self)
        {
            // If item in chest is in dissolved list then reroll untill it isn't
            if (dissolvedList.Count > 0)
            {
                // May be more efficent to just reroll if there are dissolved items
                self.Roll();

                /*/ If any of the items in the potential are dissolved
                foreach (PickupIndex pickup in self.generatedDrops)
                {
                    if (dissolvedList.Contains(PickupCatalog.GetPickupDef(pickup).itemIndex))
                    {
                        // Contains a dissolved item
                        self.Roll();
                        break;
                    }
                }
                //*/
                // When rerolled will use updated avaialbe items list

                //Log.Debug("Is still dissolved?.. " + PickupCatalog.GetPickupDef(self.dropPickup).nameToken + " | " + dissolvedList.Contains(PickupCatalog.GetPickupDef(self.dropPickup).itemIndex));
            }
            orig(self);
        }

        /*/
        private void ChestBehavior_Roll(On.RoR2.ChestBehavior.orig_Roll orig, ChestBehavior self)
        {
            if (self)
            {
                Log.Debug("Rolling chest's Universe for " + self.name);
            }
            else
            {
                Log.Warning("Chest Behavior can be null ?!?!?!");
            }

            orig(self);
        }
        //*/

        private ItemDef getLastItem(List<ItemIndex> list)
        {
            // Go through inventory in reverse order
            for (int i = list.Count - 1; i >= 0; i--)
            {
                ItemDef item = ItemCatalog.GetItemDef(list[i]);
                if (item.tier != ItemTier.NoTier && item.tier != ItemTier.Boss)
                {
                    // Don't dissolve world unique items
                    var tagList = item.tags.ToList();
                    if (!tagList.Contains(ItemTag.WorldUnique))
                    {
                        // return last non untiered non unique item
                        return item;
                    }
                }
            }
            // Found nothing
            return null;
        }

        private void dissolveItem(CharacterMaster body, ItemDef item)
        {
            Inventory inventory = body.inventory;
            if (inventory)
            {
                int grabCount = inventory.GetItemCount(item);
                if (inventory.GetItemCount(item) > 0)
                {
                    Log.Debug("Dissolving item into grey mush...");
                    inventory.RemoveItem(item, grabCount);
                    inventory.GiveItem(GreySolvent.item, grabCount);
                    CharacterMasterNotificationQueue.SendTransformNotification(
                        body,
                        item.itemIndex,
                        GreySolvent.item.itemIndex,
                        CharacterMasterNotificationQueue.TransformationType.Default);
                }
            }
        }

        private void checkEmptyTierList(ItemDef item)
        {
            List<PickupIndex> list = null;
            switch (item.tier)
            {
                case ItemTier.Tier1:
                    list = Run.instance.availableTier1DropList;
                    break;
                case ItemTier.Tier2:
                    list = Run.instance.availableTier2DropList;
                    break;
                case ItemTier.Tier3:
                    list = Run.instance.availableTier3DropList;
                    break;
                case ItemTier.Lunar:
                    list = Run.instance.availableLunarItemDropList;
                    break;
                case ItemTier.Boss:
                    list = Run.instance.availableBossDropList;
                    break;
                case ItemTier.VoidTier1:
                    list = Run.instance.availableVoidTier1DropList;
                    break;
                case ItemTier.VoidTier2:
                    list = Run.instance.availableVoidTier2DropList;
                    break;
                case ItemTier.VoidTier3:
                    list = Run.instance.availableVoidTier3DropList;
                    break;
                case ItemTier.VoidBoss:
                    list = Run.instance.availableVoidBossDropList;
                    break;
                default:
                    break;
            }
            if (list.Count == 0)
            {
                Log.Debug(list.ToString() + " | replace with Solute");
                list.Add(PickupCatalog.itemIndexToPickupIndex[(int)GreySolvent.item.itemIndex]);
            }
        }
    }
}