using R2API;
using RoR2;
using UnityEngine;
using UltitemsCyan.Buffs;
using System.Linq;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class BirthdayCandles : ItemBase
    {
        // Candle Buff damage multiple is in BuffHelper
        private static float stageStartDuration = 180f;
        private static float pickUpDuration = 60f;

        // Function
        public bool inPickupAlready = false;

        private void Tokens()
        {
            string tokenPrefix = "CANDLES";

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Birthday Candles");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Temporarly increase damage");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increase damage by 30% <style=cStack>(+30% per stack)</style> for 1 minute after pickup and for the first 3 minutes after the start of each stage.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "I don't know what to get you for your birthday...");
        }

        public override void Init()
        {
            name = "Birthday Candles";

            item = ScriptableObject.CreateInstance<ItemDef>();

            // tokens
            
            Tokens();

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

            item.tags = [ItemTag.Damage];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Birthday Candles Initialized");
            Log.Warning("Initialized: " + item.name);
        }

        protected override void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            //On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        // Start of each level (or when monsters spawn in)
        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            Log.Debug("Birthday Candles On Body Start Global");
            if (self && self.inventory)
            {
                
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                // If grabcount is zero exits loop
                if (grabCount > 0)
                {
                    Log.Debug("grabCount: " + grabCount);
                }
                for(int i = 0; i < grabCount; i++)
                {
                    self.AddTimedBuff(BuffHelper.candleBuff, stageStartDuration);
                }
            }
        }

        /*/ Works the first time being picked up
        protected void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                // Most recent item
                ItemIndex recentItem = self.inventory.itemAcquisitionOrder.Last<ItemIndex>();
                Log.Warning("Most recent item: " + recentItem.ToString());
                Log.Debug("Candle index: " + item.itemIndex);
                if(recentItem == item.itemIndex)
                {
                    Log.Debug("Adding buff");
                    self.AddTimedBuff(BuffHelper.candleBuff, pickUpDuration);
                }
            }
        }//*/
        
        
        protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            Log.Warning("Give Birthday");
            // If item picked up is Birthday Candles and there is a character Body
            if (self && itemIndex == item.itemIndex)
            {
                // Log.Debug("Count Birthday Candles on Pickup: " + count);

                var instances = PlayerCharacterMasterController.instances;
                foreach (var player in PlayerCharacterMasterController.instances)
                {
                    Log.Debug("Player: " + player.name);
                    if (self.Equals(player.body.inventory))
                    {
                        Log.Debug("Adding buff");
                        player.body.AddTimedBuff(BuffHelper.candleBuff, pickUpDuration);
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