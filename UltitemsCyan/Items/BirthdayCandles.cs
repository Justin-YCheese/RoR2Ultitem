﻿using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class BirthdayCandles : ItemBase
    {
        public static ItemDef item;
        // Candle Buff damage multiple is in BuffHelper
        private const float stageStartDuration = 180f;
        private const float pickUpDuration = 180f;

        // Function
        public bool inPickupAlready = false;

        private void Tokens()
        {
            string tokenPrefix = "BIRTHDAYCANDLES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Birthday Candles");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Temporarily deal extra damage.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increase damage by <style=cIsDamage>30%</style> <style=cStack>(+30% per stack)</style> for <style=cIsUtility>3 minutes</style> after pickup and after the start of each stage.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "I don't know what to get you for your birthday...");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // tokens
            
            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Damage, ItemTag.OnStageBeginEffect];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Birthday Candles Initialized");

            Log.Warning("Initialized: " + item.name);
        }

        protected void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        // Start of each level (or when monsters spawn in)
        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            //Log.Debug("Birthday Candles On Body Start Global");
            if (self && self.inventory)
            {
                
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                // If grabcount is zero exits loop
                for(int i = 0; i < grabCount; i++)
                {
                    self.AddTimedBuff(Buffs.BirthdayBuff.buff, stageStartDuration);
                }
            }
        }
        
        protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            //Log.Warning("Give Birthday");
            // If item picked up is Birthday Candles and there is a character Body
            if (self && itemIndex == item.itemIndex)
            {
                // Log.Debug("Count Birthday Candles on Pickup: " + count);

                // Compares current inventory with all player's inventory to find the player who picked up the item
                var instances = PlayerCharacterMasterController.instances;
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    Log.Debug("Birthday Player: " + player.name);
                    if (self.Equals(player.body.inventory))
                    {
                        Log.Debug("Adding buff");
                        player.body.AddTimedBuff(Buffs.BirthdayBuff.buff, pickUpDuration);
                    }
                }


                /*/
                CharacterBody owner = self.GetComponentInParent<CharacterBody>(); // Doesn't get Character Body

                if (owner)
                {
                    Log.Debug("Birthday Boy: " + owner.name);
                    owner.AddTimedBuff(BuffHelper.candleBuff, pickUpDuration);
                }
                //*/
            }
            orig(self, itemIndex, count);
        }
    }
}