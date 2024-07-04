using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UltitemsCyan.Buffs;
using System;
using HG;
using static Rewired.ComponentControls.Effects.RotateAroundAxis;

namespace UltitemsCyan.Items.Tier2
{
    // TODO: check if Item classes needs to be public
    public class PigsSpork : ItemBase
    {
        public static ItemDef item;

        public const float sporkBleedChance = 100f;

        public const float sporkBaseDuration = 12f;
        //public const float sporkDurationPerStack = 5f;

        private const float bleedHealing = 3f;

        public override void Init()
        {
            item = CreateItemDef(
                "PIGSSPORK",
                "Pig's Spork",
                "Bleeds heal you. Gain 100% chance to bleed enemies at low health",
                "Bleeds heal for 3 (+3 per stack) health. Gain 100% chance to bleed when taking damage to below 25% health for 12 seconds (+12 per stack).",
                "There once was a pet named porky\nA cute and chubby pig\n\nBut the farmer broke his fork\nAnd used the spoon to dig\n\nSo he made a Sporky Spig\n",
                ItemTier.Tier3,
                UltAssets.PigsSporkSprite,
                UltAssets.PigsSporkPrefab,
                [ItemTag.Damage, ItemTag.Healing, ItemTag.LowHealth]
            );
        }

        protected override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            //On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            //On.RoR2.DotController.AddPendingDamageEntry += DotController_AddPendingDamageEntry;

            On.RoR2.DotController.EvaluateDotStacksForType += DotController_EvaluateDotStacksForType;
            On.RoR2.DotController.InflictDot_refInflictDotInfo += DotController_InflictDot_refInflictDotInfo;
            On.RoR2.DotController.OnDotStackRemovedServer += DotController_OnDotStackRemovedServer;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy; ;
        }

        private void DotController_OnDotStackRemovedServer(On.RoR2.DotController.orig_OnDotStackRemovedServer orig, DotController self, object dotStack)
        {
            orig(self, dotStack);
            Log.Debug(" > > test bleed");
            DotController.DotIndex dotIndex = ((DotController.DotStack)dotStack).dotIndex;
            Log.Debug(" > > test bleed stop");
            if (self.victimBody)
            {
                if (dotIndex == DotController.DotIndex.Bleed)
                {
                    // Remove Item Behavior
                    self.victimBody.AddItemBehavior<SporkBleedBehavior>(0);
                }
            }
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            try
            {
                if (damageInfo.attacker)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    if (inflictor.inventory.GetItemCount(item) > 0 && victim && victim.GetComponent<CharacterBody>())
                    {
                        Log.Debug(" ? 4th Blood");
                        CharacterBody sporkVictim = victim.GetComponent<CharacterBody>();
                        Log.Debug(" ? 5th Blood");
                        if (sporkVictim.HasBuff(RoR2Content.Buffs.Bleeding))
                        {
                            Log.Debug(" ? 6th Blood");
                            SporkBleedBehavior behavior = sporkVictim.AddItemBehavior<SporkBleedBehavior>(1);
                            Log.Debug(" ? 7th Blood");
                            behavior.AddInflictor(inflictor);
                            Log.Debug(" ? 8th Blood");
                        }
                    }
                }
                Log.Debug(" ? Last Blood");
            }
            catch
            {
                Log.Warning("Spork On Hit Expected Error");
            }
        }

        // Add Inflictor to Victim
        // TODO supposed to be when hit and while bleeding, not if bleeding was inflicted
        private void DotController_InflictDot_refInflictDotInfo(On.RoR2.DotController.orig_InflictDot_refInflictDotInfo orig, ref InflictDotInfo inflictDotInfo)
        {
            orig(ref inflictDotInfo);
            Log.Debug("Dot Controller Spork: " + inflictDotInfo.dotIndex + " = " + DotController.DotIndex.Bleed + " | " + DotController.DotIndex.SuperBleed);
            if (inflictDotInfo.dotIndex is DotController.DotIndex.Bleed or DotController.DotIndex.SuperBleed)
            {
                CharacterBody inflictor = inflictDotInfo.attackerObject.GetComponent<CharacterBody>();
                if (inflictor.inventory.GetItemCount(item) > 0)
                {
                    CharacterBody victim = inflictDotInfo.victimObject ? inflictDotInfo.victimObject.GetComponent<CharacterBody>() : null;

                    SporkBleedBehavior behavior = victim.AddItemBehavior<SporkBleedBehavior>(1);
                    behavior.AddInflictor(inflictor);
                }
            }
        }

        // Add Heal from Bleeds
        private void DotController_EvaluateDotStacksForType(On.RoR2.DotController.orig_EvaluateDotStacksForType orig, DotController self, DotController.DotIndex dotIndex, float dt, out int remainingActive)
        {
            if (self && self.victimObject && self.victimBody &&
                dotIndex is DotController.DotIndex.Bleed or DotController.DotIndex.SuperBleed)
            {
                Log.Debug("Evaluating...");
                SporkBleedBehavior behavior = self.victimObject.GetComponent<SporkBleedBehavior>();
                if (behavior)
                {
                    CharacterBody[] inflictors = behavior.GetInflictors();
                    foreach (CharacterBody body in inflictors)
                    {
                        int grabCount = body.inventory.GetItemCount(item);
                        if (grabCount > 0)
                        {
                            //Log.Warning("Healing Inflictors ! ! ! " + body.name);
                            _ = body.healthComponent.Heal(bleedHealing * grabCount, default, true);
                        }
                    }
                }
                Log.Debug("Has inflictors?");
            }
            orig(self, dotIndex, dt, out remainingActive);
            Log.Debug(" ? but How inflictors");
        }

        /*
        private void DotController_AddPendingDamageEntry(On.RoR2.DotController.orig_AddPendingDamageEntry orig, object pendingDamages, GameObject attackerObject, float damage, DamageType damageType)
        {
            Log.Warning(" / / / Damage Type: " + damageType);
            if (damageType == DamageType.Generic)
            {
                CharacterBody body = attackerObject.GetComponent<CharacterBody>();
                int grabCount = body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug(" heal ...");
                    //body.healthComponent.Heal(grabCount * bleedHeal, new ProcChainMask());
                    body.healthComponent.Heal(grabCount * bleedHeal, default, true);
                }
            }
            orig(pendingDamages, attackerObject, damage, damageType);
        }//*/

        // Add item behavior for low health
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                _ = self.AddItemBehavior<PigsSporkBehavior>(self.inventory.GetItemCount(item));
            }
        }

        // Low health behavior
        public class PigsSporkBehavior : CharacterBody.ItemBehavior
        {
            public HealthComponent healthComponent;
            private bool _inLowHealth = false;
            public bool InLowHealth
            {
                get { return _inLowHealth; }
                set
                {
                    // If not already the same value
                    if (_inLowHealth != value)
                    {
                        Log.Debug(" /   / Low Health? " + value);
                        _inLowHealth = value;
                        // Entering Low Health
                        if (_inLowHealth)
                        {
                            //body.AddTimedBuff(SporkBleedBuff.buff, sporkBaseDuration + (sporkDurationPerStack * (stack - 1)));
                            body.AddTimedBuff(SporkBleedBuff.buff, sporkBaseDuration * stack);
                            Util.PlaySound("Play_item_void_bleedOnHit_start", body.gameObject);
                        }
                    }
                }
            }

            // If player is at full health
            public void FixedUpdate()
            {
                if (healthComponent)
                {
                    InLowHealth = healthComponent.isHealthLow;
                }
            }

            public void Start()
            {
                healthComponent = GetComponent<HealthComponent>();
            }

            public void OnDestroy()
            {
                InLowHealth = false;
            }
        }

        // Used to keep track of who heals from bleed damage
        public class SporkBleedBehavior : CharacterBody.ItemBehavior
        {
            private List<CharacterBody> _inflictors = [];

            public void AddInflictor(CharacterBody inflictor)
            {
                Log.Debug("Adding " + inflictor.name + " to inflictors");
                if (!_inflictors.Contains(inflictor))
                {
                    _inflictors.Add(inflictor);
                }
                /*//
                Log.Debug("done inflictors?");
                CharacterBody[] list = GetInflictors();
                Log.Debug(list[0] + " is in list");
                //*/
            }

            public CharacterBody[] GetInflictors()
            {
                Log.Debug("In Inflictor get method");
                return _inflictors.ToArray();
            }

            public void OnDestroy()
            {
                Log.Warning(" , Spork Bleed ended...");
                _inflictors.Clear();
            }
        }

        /*/
        private void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, Vector3 damagePosition, bool damageIsSilent, GameObject attacker)
        {
            if (NetworkServer.active && self.body && damageValue > 0f && self.isHealthLow)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug(" / / / Low spork, now drink BLOOD!");
                    PigsSporkBehavior behavior = self.body.GetComponent<PigsSporkBehavior>();
                    behavior.InLowHealth = true;
                }
            }
            orig(self, damageValue, damagePosition, damageIsSilent, attacker);
        }//*/
    }
}