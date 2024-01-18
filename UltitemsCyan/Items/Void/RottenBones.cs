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

        public const int rotsPerItem = 5;
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
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increase damage by <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> damage for every 3 minutes</style> passed in a stage, up to a max of <style=cIsDamage>5</style> stacks. Corrupts Birthday Candles");
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
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            //CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            //On.RoR2.Inventory.GiveItem_ItemDef_int
        }

        private void Stage_onStageStartGlobal(Stage obj)
        {
            stageStartTime = Run.instance.time;
            Log.Warning("Rotting Starts at: " + stageStartTime);
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

        // If the player picks up the item after max intervals have passed
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
                    Log.Debug("New Bone? Intervals Passed! " + behavior.IntervalsPassed);
                    ApplyRot(player, behavior.IntervalsPassed);
                }
                else
                {
                    // Apply max rot
                    Log.Debug("Pass rot Time Interval Item Pickup");
                    ApplyRot(player, rotsPerItem);
                }
            }
        }

        //
        public class RottenBonesTimedVoidBehavior : CharacterBody.ItemBehavior
        {
            private int _intervalsPassed = 0;
            public int IntervalsPassed
            {
                get { return _intervalsPassed; }
                set
                {
                    // If more intervals have passed than before
                    if (_intervalsPassed != value)
                    {
                        _intervalsPassed = value;
                        ApplyRot(body, _intervalsPassed);
                    }
                }
            }


            public void Start()
            {
                Log.Warning("Start Rotten Bones");
            }
            private void Awake()
            {
                Log.Warning("Rotten Bones AWAKE!");
            }

            private void OnEnable()
            {
                Log.Warning("Rotten Bones ENABELED!");
            }

            private void OnDisable()
            {
                Log.Warning("Rotten Bones DISED!");
            }

            private void OnDestroy()
            {
                Log.Warning("Rotten Bones GONE! DESTRO");
            }

            private void FixedUpdate()
            {
                float currentTime = Run.instance.time;
                // If more intervals have passed than currently recorded
                if (currentTime > stageStartTime + (rotTimeInterval * (IntervalsPassed + 1)))
                {
                    //Log.Warning("Rot Math: " + (currentTime - stageStartTime) + "\t/ " + rotTimeInterval + "\t = " + (int)((currentTime - stageStartTime) / rotTimeInterval));
                    IntervalsPassed++;
                    // If max rots, remove behavior by setting stacks to zero?
                    if(IntervalsPassed >= rotsPerItem)
                    {
                        Log.Debug("Pass rot Time Interval But Behavior!");
                        body.AddItemBehavior<RottenBonesTimedVoidBehavior>(0); // Not sure if removes, but at least stop FixedUpdate for this item from running
                    }
                }
            }
        }
    }
}