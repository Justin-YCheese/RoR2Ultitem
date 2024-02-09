﻿using R2API;
using RoR2;
using UltitemsCyan.Items.Untiered;
using UnityEngine;
using UnityEngine.Networking;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class CorrodingVault : ItemBase
    {
        public static ItemDef item;
        private const int minimumInVault = 16;
        private const int bonusInVault = 4;

        public override void Init()
        {
            item = CreateItemDef(
                "CORRODINGVAULT",
                "Corroding Vault",
                "Breaks at the start of the next stage. Contains white items.",
                "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>16 to 20</style> unique white items",
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
                    ;
                    ItemIndex[] allWhiteItems = new ItemIndex[ItemCatalog.tier1ItemList.Count];
                    //ItemIndex[] allWhiteItems = Run.instance.availableTier1DropList.ToArray();
                    ItemCatalog.tier1ItemList.CopyTo(allWhiteItems);
                    int length = allWhiteItems.Length;

                    //Log.Debug("All White Items Length: " + length);

                    // bonus plus one because rand int not include max
                    int quantityInVault = minimumInVault + Random.Range(0, bonusInVault + 1);

                    // Error Message if there aren't enough items somehow
                    if (length < quantityInVault) { Log.Warning(" ! ! ! There aren't enough white items for Rusted Vault ! ! !"); }

                    // Give 16 different white items
                    for (int i = 0; i < quantityInVault; i++)
                    {
                        int itemPos = Random.Range(0, length);
                        //Log.Debug("Random Position: " + itemPos);
                        Log.Debug("Random White found: " + ItemCatalog.GetItemDef(allWhiteItems[itemPos]).name);
                        self.inventory.GiveItem(allWhiteItems[itemPos]);
                        GenericPickupController.SendPickupMessage(self.master, PickupCatalog.itemIndexToPickupIndex[(int)allWhiteItems[itemPos]]);
                        // erase current item and preserve last item
                        // setting current item equal to last item and shorten length effectively moving last item to current item
                        allWhiteItems[itemPos] = allWhiteItems[length - 1];
                        length--;
                    }
                    Log.Debug(quantityInVault + " white items from vault");


                    Chat.AddMessage("You got " + quantityInVault + " items from their vault");

                    //TODO Add message for number of items in vault
                    //Util.PlaySound("Play_UI_podBlastDoorOpen", self.gameObject);
                }
            }
        }
    }
}