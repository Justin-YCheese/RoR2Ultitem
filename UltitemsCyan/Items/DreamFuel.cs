using R2API;
using RoR2;
using System;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class DreamFuel : ItemBase
    {
        public static ItemDef item;
        //private const float dreamSpeed = 1; // 200f for +200% speed
        private void Tokens()
        {
            string tokenPrefix = "DREAMFUEL";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Dream Fuel");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Increase speed at full health <style=cDeath>BUT get rooted when hit</style>.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "While at <style=cIsHealth>full health</style> increase <style=cIsUtility>movement speed</style> by <style=cIsUtility>200%</style> <style=cStack>(+200% per stack)</style>. You get <style=cIsHealth>rooted</style> for 2 seconds <style=cStack>(+2 per stack)</style> when hit.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Nightmare fuel");

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
            itd.tier = ItemTier.Lunar;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Ultitems.DefDreamFuel = item;

            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            //RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }



        // Detect change which may include Dream Fuel
        protected void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                self.AddItemBehavior<UltitemsDreamFuelBehaviour>(self.inventory.GetItemCount(item));
            }
            orig(self);
        }//*/

        public class UltitemsDreamFuelBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _isFullHealth = false;
            public bool isFullHealth
            {
                get { return _isFullHealth; }
                set
                {
                    // If not already the same value
                    if (_isFullHealth != value)
                    {
                        _isFullHealth = value;
                        // If full health
                        if (_isFullHealth)
                        {
                            // Don't know why I would need to check for NetworkServer active
                            // This ensures that the following code only runs as the host
                            //if (NetworkServer.active)
                            body.AddBuff(Buffs.DreamSpeedBuff.buff);
                        }
                        else
                        {
                            body.RemoveBuff(Buffs.DreamSpeedBuff.buff);
                        }
                    }
                }
            }

            // If player is at full health
            public void FixedUpdate()
            {
                if (healthComponent)
                {
                    isFullHealth = healthComponent.combinedHealthFraction == 1;
                }
            }

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }


            public void OnDestroy()
            {
                isFullHealth = false;
            }
        }

        /*/private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                int grabCount = sender.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug("Base Movement : " + sender.baseMoveSpeed);
                    Log.Debug("Movement Speed: " + sender.moveSpeed);

                    args.moveSpeedMultAdd += grabCount;

                    Log.Debug("post Base Movement : " + sender.baseMoveSpeed);
                    Log.Debug("post Movement Speed: " + sender.moveSpeed);
                }
            }
        }//*/

        /*/private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                int grabCount = self.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug("Dream Fuel recalculate stats");
                    Log.Debug("Base Movement : " + self.baseMoveSpeed);
                    Log.Debug("Movement Speed: " + self.moveSpeed);

                    self.move *= grabCount + 1;
                }
            }
            orig(self);
        }//*/



        // Root when hit
        protected void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            try
            {
                // If the victum has an inventory
                // and damage isn't rejected?
                Log.Warning("Dream Hit!");

                Log.Debug("Victum " + victim.name);
                Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                Log.Debug("Damage rejected? " + damageInfo.rejected);
                if (victim && victim.GetComponent<CharacterBody>() && victim.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    Log.Debug("In Loop...");
                    CharacterBody injured = victim.GetComponent<CharacterBody>();
                    Log.Debug("got injured... " + injured.name);
                    int grabCount = injured.inventory.GetItemCount(item);
                    Log.Debug("grabCount... " + grabCount);
                    if (grabCount > 0)
                    {
                        Log.Debug("...");
                        injured.AddTimedBuff(RoR2Content.Buffs.LunarSecondaryRoot, 2f * grabCount);
                        
                        Log.Debug("... Done?");
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Warning("What Hit?");
            }
            orig(self, damageInfo, victim);
        }
    }
}