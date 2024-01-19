using R2API;
using RoR2;
using System.Linq;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using static UltitemsCyan.Items.Lunar.DreamFuel;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class RottenBones : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        public const int rotsPerItem = 4;
        public const float rottingBuffMultiplier = 20;
        public const float rotTimeInterval = 180; // 3 minutes
        public static float stageStartTime; // measured in seconds

        private const bool isVoid = true;
        //public override bool IsVoid() { return isVoid; }

        private void Tokens()
        {
            string tokenPrefix = "ROTTENBONES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Rotten Bones");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Deal more damage over time.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increase damage by <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> damage for every 3 minutes</style> passed in a stage, up to a max of <style=cIsDamage>4</style> stacks. Corrupts Birthday Candles");
            LanguageAPI.Add(tokenPrefix + "_LORE", "The bitter aftertaste is just the spoilage");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();
            transformItem = BirthdayCandles.item;

            // Add text for item
            Tokens();

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.VoidTier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.BirthdayCandleSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Damage];

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            GetTransformItem = transformItem;
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            // TODO Add 
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            //On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        private void Stage_onStageStartGlobal(Stage obj)
        {
            stageStartTime = Run.instance.time;
            Log.Warning("Rotting Starts at: " + stageStartTime);
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            // Add Behavior to player (expectially if the full time intervals have passed)
            if (self && self.inventory)
            {
                self.AddItemBehavior<RottenBonesTimedVoidBehavior>(self.inventory.GetItemCount(item));
            }
        }
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            // Add Behavior to player (expectially if the full time intervals have passed)
            if (self && self.inventory)
            {
                if (self.inventory.GetItemCount(item) > 0)
                {
                    Log.Warning("Give Rotting Bones");
                    // If within time intervals give item behavior
                    if (Run.instance.time < stageStartTime + (rotTimeInterval * rotsPerItem))
                    {
                        RottenBonesTimedVoidBehavior behavior = self.AddItemBehavior<RottenBonesTimedVoidBehavior>(self.inventory.GetItemCount(item));
                        Log.Debug("New Bone? Intervals Passed! " + behavior.intervalsPassed);
                        ApplyRot(self, behavior.intervalsPassed);
                    }
                    else
                    {
                        // Apply max rot
                        Log.Debug("Pass rot Time Interval Item Pickup");
                        ApplyRot(self, rotsPerItem);
                    }
                }
                else
                {
                    // If player doesn't have this item anymore
                    self.AddItemBehavior<RottenBonesTimedVoidBehavior>(0);
                }
            }
        }

        private static void ApplyRot(CharacterBody player, int intervals)
        {
            int grabCount = player.inventory.GetItemCount(item);
            int maxRotStacks = grabCount * intervals;
            if (player.GetBuffCount(RottingBuff.buff) < maxRotStacks)
            {
                player.SetBuffCount(RottingBuff.buff.buffIndex, maxRotStacks);
            }
        }

        /*/ If the player picks up the item after max intervals have passed
        protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);
            if (self && itemIndex == item.itemIndex)
            {
                Log.Warning("Give Rotting Bones");
                // Finds the player that picked up the item
                CharacterBody player = CharacterBody.readOnlyInstancesList.ToList().Find((body2) => body2.inventory == self);
                // If within time intervals give item behavior
                if (Run.instance.time < stageStartTime + (rotTimeInterval * rotsPerItem))
                {
                    RottenBonesTimedVoidBehavior behavior = player.AddItemBehavior<RottenBonesTimedVoidBehavior>(player.inventory.GetItemCount(item));
                    Log.Debug("New Bone? Intervals Passed! " + behavior.intervalsPassed);
                    ApplyRot(player, behavior.intervalsPassed);
                }
                else
                {
                    // Apply max rot
                    Log.Debug("Pass rot Time Interval Item Pickup");
                    ApplyRot(player, rotsPerItem);
                }
            }
        }//*/

        //
        public class RottenBonesTimedVoidBehavior : CharacterBody.ItemBehavior
        {
            public int intervalsPassed = 0;
            // Order:
            // Awake(), Enable(), Start()
            // Disable(), Destory()

            private void OnDisable()
            {
                intervalsPassed = 0;
            }

            private void FixedUpdate()
            {
                float currentTime = Run.instance.time;
                // If more intervals have passed than currently recorded
                if (currentTime > stageStartTime + (rotTimeInterval * (intervalsPassed + 1)))
                {
                    //Log.Warning("Rot Math: " + (currentTime - stageStartTime) + "\t/ " + rotTimeInterval + "\t = " + (int)((currentTime - stageStartTime) / rotTimeInterval));
                    intervalsPassed++;
                    ApplyRot(body, intervalsPassed);
                    // If max rots, remove behavior by setting stacks to zero?
                    if (intervalsPassed >= rotsPerItem)
                    {
                        Log.Debug("Pass rot Time Interval But Behavior!");
                        body.AddItemBehavior<RottenBonesTimedVoidBehavior>(0); // Not sure if removes, but at least stop FixedUpdate for this item from running
                    }
                }
            }
        }
    }
}