using R2API;
using RoR2;
using UltitemsCyan.Items.Untiered;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class CorrodingVault : ItemBase
    {
        public static ItemDef item;
        private const int quantityInVault = 15;
        //private const int bonusInVault = 0;

        public override void Init()
        {
            item = CreateItemDef(
                "CORRODINGVAULT",
                "Corroding Vault",
                "Breaks at the start of the next stage. Contains white items.",
                "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>15</style> unique white items",
                "This vault is sturdy, but over time the rust will just crack it open",
                ItemTier.Tier3,
                Ultitems.Assets.CorrodingVaultSprite,
                Ultitems.Assets.CorrodingVaultPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist]
            );
        }


        protected override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (NetworkServer.active && self && self.inventory)
            {
                // Get number of vaults
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Warning("Rusted Vault on body start global..." + self.GetUserName());
                    // Remove a vault
                    self.inventory.RemoveItem(item);
                    // Give Consumed vault
                    self.inventory.GiveItem(CorrodingVaultConsumed.item);

                    // Get all white items
                    
                    PickupIndex[] allWhiteItems = new PickupIndex[Run.instance.availableTier1DropList.Count];

                    Run.instance.availableTier1DropList.CopyTo(allWhiteItems);
                    
                    //ItemIndex[] allWhiteItems = Run.instance.availableTier1DropList.ToArray();
                    //ItemCatalog.tier1ItemList.CopyTo(allWhiteItems);
                    int length = allWhiteItems.Length;

                    //Log.Debug("All White Items Length: " + length);
                    Xoroshiro128Plus rng = new(Run.instance.stageRng.nextUlong);

                    // bonus plus one because rand int exclude max
                    //int quantityInVault = minimumInVault; // + rng.RangeInt(0, bonusInVault + 1);

                    // Error Message if there aren't enough items somehow (Universal Solute)
                    //if (length < quantityInVault) { Log.Warning(" ! ! ! There aren't enough white items for Rusted Vault ! ! !"); }

                    // Give 16 different white items
                    for (int i = 0; i < quantityInVault; i++)
                    {
                        int randItemPos = rng.RangeInt(0, length);

                        //Log.Debug("Random Position: " + randItemPos + " / " + length);
                        //printArray(allWhiteItems);
                        ItemIndex foundItem = PickupCatalog.GetPickupDef(allWhiteItems[randItemPos]).itemIndex;
                        //ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(allWhiteItems[i]).itemIndex).name;
                        //Log.Debug(allWhiteItems[i] + " | " + PickupCatalog.GetPickupDef(allWhiteItems[itemPos]).itemIndex + " | " + ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(allWhiteItems[itemPos]).itemIndex) + " | " + ItemCatalog.GetItemDef(PickupCatalog.GetPickupDef(allWhiteItems[itemPos]).itemIndex).name);

                        //Log.Debug("Random White found: " + foundItem + " | " + ItemCatalog.GetItemDef(foundItem).name);
                        self.inventory.GiveItem(foundItem);
                        GenericPickupController.SendPickupMessage(self.master, allWhiteItems[randItemPos]);
                        // erase current item with last listed item
                        // setting current item equal to last item and shorten length effectively moving last item to current item
                        allWhiteItems[randItemPos] = allWhiteItems[length - 1];
                        length--;
                        // Ran out of white items, reset pool
                        if (length == 0)
                        {
                            Log.Debug("Ran out of white items...   Reseting Pool");
                            length = allWhiteItems.Length;
                            Run.instance.availableTier1DropList.CopyTo(allWhiteItems);
                        }
                        
                    }
                    //Log.Debug(quantityInVault + " white items from vault");
                    //Chat.AddMessage("You got " + quantityInVault + " items from their vault");
                    //TODO Add message for number of items in vault
                    //Util.PlaySound("Play_UI_podBlastDoorOpen", self.gameObject);
                }
            }
        }

        private void printArray(PickupIndex[] array)
        {
            int length = array.Length;
            for (int i = 0; i < length; i++)
            {
                Log.Debug(i + ": " + PickupCatalog.GetPickupDef(array[i]).itemIndex);
            }
        }
    }
}