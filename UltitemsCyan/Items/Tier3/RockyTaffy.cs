﻿using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using BepInEx.Configuration;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    // Fix with transcendance
    // num59 += (float)num4 * 0.08f * this.maxHealth;

    public class RockyTaffy : ItemBase
    {
        public static ItemDef item;
        private const float shieldPercent = 32f;

        public static GameObject CaptainBodyArmorBlockEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Captain/CaptainBodyArmorBlockEffect.prefab").WaitForCompletion();

        public override void Init(ConfigFile configs)
        {
            const string itemName = "Rocky Taffy";
            if (!CheckItemEnabledConfig(itemName, "Red", configs))
            {
                return;
            }
            item = CreateItemDef(
                "ROCKYTAFFY",
                itemName,
                "Gain a recharging shield. Buff gives a stable barrier without your shield. Buff gained with full shield.",
                "Gain a <style=cIsHealing>shield</style> equal to <style=cIsHealing>32%</style> <style=cStack>(+32% per stack)</style> of your maximum health. On losing your shield with the buff, gain a <style=cIsHealing>stable barrier</style> for 100% of your <style=cIsHealing>max shield</style>. No barrier decay without a shield and regain buff with a full shield.",
                "This vault is sturdy, but over time the rust will just crack it open. Oh wait this is the wrong description...\n" +
                "Give me a second...\n\num...\n\nSomething about laughing but harder so for shields? I don't know...",
                ItemTier.Tier3,
                UltAssets.RockyTaffySprite,
                UltAssets.RockyTaffyPrefab,
                [ItemTag.Utility, ItemTag.Healing]
            );
        }

        protected override void Hooks()
        {
            // Add Shield
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            // Grant Barrier when losing shield
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            // No Barrier Decay without shield
            On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            // Add Recalculate Stats on shield lost / gained
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        // Add Shield
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                int grabCount = sender.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //Log.Debug("Taffy On the rocks | Health: " + sender.healthComponent.fullHealth);
                    args.baseShieldAdd += sender.healthComponent.fullHealth * (shieldPercent / 100f * grabCount);
                }
            }
        }
        //*/

        // Grant Barrier on Losing Shield
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            bool runOrig = true;

            if (self && self.body && self.body.inventory)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    runOrig = false;

                    //Log.Debug("   Got Taffy!");
                    bool initialShield = self.shield > 0;

                    orig(self, damageInfo);

                    bool newShield = self.shield > 0;

                    if (initialShield && !newShield && self.body.HasBuff(Buffs.TaffyChewBuff.buff))
                    {
                        //Log.Debug("Taffy Shield lost! Gain Barrier");
                        self.body.RemoveBuff(Buffs.TaffyChewBuff.buff);
                        self.AddBarrier(self.fullShield);
                        _ = Util.PlaySound("Play_gup_death", self.body.gameObject);
                        //self.body.statsDirty = true;
                        //self.body.RecalculateStats();
                    }
                }
            }

            if (runOrig)
            {
                orig(self, damageInfo);
            }
        }

        // Remove Barrier Decay with no shield (Changeing barrier decay in On function doesn't seem to work)
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);

            // bool runOrig = true;
            if (self && self.inventory)
            {
                int grabCount = self.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //runOrig = false;
                    //self.maxShield += self.healthComponent.fullHealth * (shieldPercent / 100f * grabCount);

                    // Shield Decay
                    if (self.healthComponent.shield <= 0)
                    {
                        self.barrierDecayRate = 0;
                    }
                    //Log.Debug("After Barrier Decay: " + self.barrierDecayRate);
                    //Log.Debug("v Shield new: " + args.shieldMultAdd);
                    //args.baseShieldAdd += sender.healthComponent.fullHealth * (shieldPercent / 100f * grabCount);
                    //self.moveSpeed = 0;
                }
            }

            //if (runOrig)
            //{
            //    orig(self);
            //}
        }

        // Add Recalculate Stats on shield lost / gained
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                _ = self.AddItemBehavior<RockyTaffyBehaviour>(self.inventory.GetItemCount(item));
            }
        }

        // Recalculate stats when losing or gaining shield
        public class RockyTaffyBehaviour : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _hasShield = true;
            private bool _ifFullShield = false; // False so when you first get full shield then can get buff immediantly
            public bool HasShield
            {
                get { return _hasShield; }
                set
                {
                    // If not already the same value
                    if (_hasShield != value)
                    {
                        _hasShield = value;
                        //Log.Debug(_hasShield + " > Sticky Taffy Dirty Stats (has shield changed) | Shield: " + healthComponent.shield + " vs Full? " + healthComponent.fullShield);
                        body.statsDirty = true;
                    }
                }
            }
            public bool IfFullShield
            {
                get { return _ifFullShield; }
                set
                {
                    // If not already the same value
                    if (_ifFullShield != value)
                    {
                        _ifFullShield = value;
                        if (_ifFullShield)
                        {
                            body.AddBuff(Buffs.TaffyChewBuff.buff);
                            _ = Util.PlaySound("Play_captain_m2_tazer_bounce", body.gameObject);
                        }
                    }
                }
            }

            // If player shield stat changed
            public void FixedUpdate()
            {
                if (healthComponent)
                {
                    HasShield = healthComponent.shield > 0;
                    IfFullShield = healthComponent.shield == healthComponent.fullShield;
                }
            }

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            public void OnDestroy()
            {
                body.RemoveBuff(Buffs.TaffyChewBuff.buff);
                HasShield = true;
                IfFullShield = false;
            }
        }
    }
}