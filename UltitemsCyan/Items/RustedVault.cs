﻿using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class RustedVault : ItemBase
    {
        public static ItemDef item;
        private const int quantityInVault = 16;
        private void Tokens()
        {
            string tokenPrefix = "RUSTEDVAULT";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Rusted Vault");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Breaks at the start of the next stage. Contains white items.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>16</style> unique white items");
            LanguageAPI.Add(tokenPrefix + "_LORE", "This vault is sturdy, but over time the rust will just crack it open");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // Add text for item
            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier3;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (self && self.inventory)
            {
                // Get number of scissors
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Warning("Rusted Vault on body start global...");
                    // Remove a vault
                    self.inventory.RemoveItem(item);
                    // Give Consumed vault
                    self.inventory.GiveItem(RustedVaultConsumed.item);
                    // Get all white items
                    System.Collections.Generic.List<ItemIndex> allWhiteItems = ItemCatalog.tier1ItemList;
                    int length = allWhiteItems.Count;
                    // Give 16 different white items
                    for (int i = 0; i < quantityInVault; i++)
                    {
                        int itemPos = Random.RandomRangeInt(0, length);
                        Log.Debug("Random White found: " + ItemCatalog.GetItemDef(allWhiteItems[itemPos]).name);
                        self.inventory.GiveItem(allWhiteItems[itemPos]);
                        allWhiteItems.RemoveAt(itemPos);
                    }
                }
            }
        }
    }
}