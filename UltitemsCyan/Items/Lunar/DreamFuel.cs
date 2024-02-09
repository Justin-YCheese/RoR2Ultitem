using R2API;
using RoR2;
using System;
using UltitemsCyan.Buffs;
using UnityEngine;

namespace UltitemsCyan.Items.Lunar
{

    // TODO: check if Item classes needs to be public
    public class DreamFuel : ItemBase
    {
        public static ItemDef item;

        // For Dream Speed Buff
        public const float dreamSpeed = 120f;

        public override void Init()
        {
            item = CreateItemDef(
                "DREAMFUEL",
                "Dream Fuel",
                "Increase speed at full health... <style=cDeath>BUT get rooted when hit.</style>",
                "While at <style=cIsHealth>full health</style> increase <style=cIsUtility>movement speed</style> by <style=cIsUtility>120%</style> <style=cStack>(+120% per stack)</style>. You get <style=cIsHealth>rooted</style> for 2 seconds <style=cStack>(+2 per stack)</style> when hit.",
                "More like Nightmare fuel",
                ItemTier.Lunar,
                Ultitems.Assets.DreamFuelSprite,
                Ultitems.Assets.DreamFuelPrefab,
                [ItemTag.Utility]
            );
        }

        protected override void Hooks()
        {

            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;

            //RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }



        // Detect change which may include Dream Fuel
        protected void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                self.AddItemBehavior<DreamFuelBehaviour>(self.inventory.GetItemCount(item));
            }
        }//*/

        // Speed at full health
        public class DreamFuelBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _isFullHealth = false;
            public bool IsFullHealth
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
                            Util.PlaySound("Play_affix_void_bug_spawn", gameObject);
                            body.AddBuff(DreamSpeedBuff.buff);
                        }
                        else
                        {
                            body.RemoveBuff(DreamSpeedBuff.buff);
                        }
                    }
                }
            }

            // If player is at full health
            public void FixedUpdate()
            {
                if (healthComponent)
                {
                    IsFullHealth = healthComponent.combinedHealthFraction >= 1;
                }
            }

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            public void OnDestroy()
            {
                IsFullHealth = false;
            }
        }

        // Root when hit
        protected void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            try
            {
                // If the victum has an inventory
                // and damage isn't rejected?
                if (self && victim && victim.GetComponent<CharacterBody>() && victim.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody injured = victim.GetComponent<CharacterBody>();
                    int grabCount = injured.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        Util.PlaySound("Play_item_lunar_secondaryReplace_explode", injured.gameObject);
                        injured.AddTimedBuffAuthority(RoR2Content.Buffs.LunarSecondaryRoot.buffIndex, 2f * grabCount);
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Warning("What Dream Hit?");
                //Log.Debug("Victum " + victim.name);
                //Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                //Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                //Log.Debug("Damage rejected? " + damageInfo.rejected);
            }
        }
    }
}