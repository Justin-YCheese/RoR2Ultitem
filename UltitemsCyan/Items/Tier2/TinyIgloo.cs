using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine.Networking;



namespace UltitemsCyan.Items.Tier2
{
    /* Notes
     * 
     * Works with Focused Convergence,
     * 
     * TODO: Doesn't get initial increased size of stacked Warbanners
     * 
     *  Test // Void fields cells // NullWardBaseState.wardRadiusOff = 0.2f + (0.2f * FindTotalMultiplier());
     *  Test Gold Fields
     *  Test Mythrixs Pillars
     *  
     */

    public class TinyIgloo : ItemBase
    {
        private readonly struct InitialZone(NetworkBehaviour zone, float startRadius = -1f)
        {
            public readonly NetworkBehaviour zone = zone;
            public readonly float startRadius = startRadius;

            static public bool operator ==(InitialZone lInitZone, InitialZone rInitZone) { return lInitZone.zone == rInitZone.zone; }
            static public bool operator !=(InitialZone lInitZone, InitialZone rInitZone) { return lInitZone.zone != rInitZone.zone; }

            // override Equals() So InitialZone gets properly removed from zoneList
            public override bool Equals(object obj)
            {
                if (obj is InitialZone other)
                {
                    return zone == other.zone;
                }
                return false;
            } 
            public override int GetHashCode()
            {
                return zone?.GetHashCode() ?? 0;
            }
        }

        public static ItemDef item;
        private readonly List<InitialZone> zoneList = [];

        // Healing
        private const int basePercent = 30;
        private const int perStackPercent = 10;
        private const float extraZoneMul = .5f; // How much additional zones increase healing scaled off base

        // Counting Zones
        private const int maxZoneCount = 15;
        private const float radiusPerOverheal = .5f; // How much percentage of radius increase from each percentage of overheal

        // Max Zone Radius
        private const float baseMaxRadius = 60f; // +60% radius size
        private const float perStackMaxRadius = 30f;

        private float storeToFullHealth = 0f; // To store health value between functions

        public override void Init(ConfigFile configs)
        {
            const string itemName = "Tiny Igloo";
            if (!CheckItemEnabledConfig(itemName, "Green", configs))
            {
                return;
            }
            item = CreateItemDef(
                "TINYIGLOO",
                itemName,
                "Increase healing per zones occupied. Overhealing increases zone size.",
                "While in a zone, <style=cIsHealing>heal 30%</style> <style=cStack>(+10% per stack)</style> more plus half as much for each <style=cIsDamage>additional zone</style> occupied. Overhealing will <style=cIsDamage>increases the size</style> of the zone for <style=cIsHealing>50%</style> of the amount <style=cIsHealing>healed</style>. Increase max size by <style=cIsDamage>60%</style> <style=cStack>(+30% per stack)</style>.",
                "It's like a snowball effect but for zones. Get it? But there already existed a snow globe item, so I went for something similar",
                ItemTier.Tier2,
                UltAssets.TinyIglooSprite,
                UltAssets.TinyIglooPrefab,
                [ItemTag.Healing, ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.HoldoutZoneRelated]
            );
        }


        protected override void Hooks()
        {
            On.RoR2.HoldoutZoneController.OnEnable += HoldoutZoneController_OnEnable;
            //On.RoR2.HoldoutZoneController.Start += HoldoutZoneController_Start;
            On.RoR2.HoldoutZoneController.OnDisable += HoldoutZoneController_OnDisable;
            On.RoR2.BuffWard.OnEnable += BuffWard_Start;
            On.RoR2.BuffWard.OnDisable += BuffWard_OnDisable;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.HealthComponent.SendHeal += HealthComponent_SendHeal;
        }

        // Add or remove zones from global list
        // startRadius will be set when first added to
        private void BuffWard_Start(On.RoR2.BuffWard.orig_OnEnable orig, BuffWard self)
        {
            orig(self);
            zoneList.Add(new InitialZone(self));
            //Log.Debug(" + + + ++++++ + + + Adding B U F F | Count " + zoneList.Count + " | N E W radius = " + self.radius + " calcRadius " + self.calculatedRadius);
        }
        private void BuffWard_OnDisable(On.RoR2.BuffWard.orig_OnDisable orig, BuffWard self)
        {
            _ = zoneList.Remove(new InitialZone(self));
            //Log.Debug(" + + + +----+ + + + B U F F SUBtraction? | Count " + zoneList.Count + " | " + !zoneList.Contains(new InitialZone(self)));
            orig(self);
        }
        private void HoldoutZoneController_OnEnable(On.RoR2.HoldoutZoneController.orig_OnEnable orig, HoldoutZoneController self)
        {
            orig(self);
            zoneList.Add(new InitialZone(self));
            //Log.Debug(" + + + ++++++ + + + Adding Zone | radius = " + self.baseRadius);
        }
        private void HoldoutZoneController_OnDisable(On.RoR2.HoldoutZoneController.orig_OnDisable orig, HoldoutZoneController self)
        {
            _ = zoneList.Remove(new InitialZone(self));
            //Log.Debug(" + + + +----+ + + + Zone SUBtraction? " + !zoneList.Contains(new InitialZone(self)));
            orig(self);
        }

        // Increase amount healed
        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (nonRegen && self && self.body && self.body.inventory)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    storeToFullHealth = self.fullHealth - self.health;
                    // Increase amount healed
                    //Log.Warning("  +++  Healing igloo | initial amount = " + amount);

                    int zoneCount = GetInZoneList(self.body).Length;

                    float zoneMultiplier = zoneCount == 0 ? 0 : (zoneCount + 1) * extraZoneMul;
                    float itemMultiplier = (basePercent + (grabCount - 1) * perStackPercent) / 100f;

                    //float multiplier = 1 + (basePercent + (grabCount - 1) * perStackPercent) * zoneCount / 100f;
                    float multiplier = 1 + itemMultiplier * zoneMultiplier;
                    amount *= multiplier;
                    //Log.Debug(" ^ ^ ^ itemMultiplier = " + itemMultiplier + "\tzoneCount = " + zoneCount + "\tzoneMultiplier = " + zoneMultiplier + "\ttotalMult = " + multiplier + "\tinitialToFull = " + storeToFullHealth + "\tamount = " + amount);

                    return orig(self, amount, procChainMask, nonRegen);
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        // Get amount healed and increase zone radius
        private void HealthComponent_SendHeal(On.RoR2.HealthComponent.orig_SendHeal orig, GameObject target, float amount, bool isCrit)
        {
            //Log.Warning(" +++ Sending Healing Wishes");
            orig(target, amount, isCrit);

            CharacterBody characterBody = target.GetComponent<CharacterBody>();
            int grabCount = characterBody.inventory.GetItemCount(item);
            if (grabCount > 0)
            {
                HealthComponent healthComponent = characterBody.healthComponent;

                //Log.Debug(" +++ Send Heal +++ | Full Health = " + healthComponent.fullHealth + " ToFull = " + storeToFullHealth);

                //float overHeal = amount - Mathf.Max(Mathf.Min(amount, healthComponent.fullHealth - healthComponent.health), 0f);

                // Healed Amount - Amount to full health (min zero)
                float overHeal = Mathf.Max(amount - storeToFullHealth, 0);
                Log.Debug(" +++ Send Heal +++ | toFullHealth = " + storeToFullHealth + " Overheal: " + overHeal + " Overheal Percent: " + overHeal / healthComponent.fullHealth * 100);

                // Increase radius for over heals
                IncreaseEachRadius(GetInZoneList(characterBody), overHeal / healthComponent.fullHealth, grabCount);

                //Log.Warning("  +++  | Multiplier: " + multiplier + "\t| returnAmount " + returnAmount + "\t| overHeal " + overHeal);
                storeToFullHealth = 0;
            }
        }

        // Get list of zones the body is in
        private InitialZone[] GetInZoneList(CharacterBody body)
        {
            List<InitialZone> list = new(maxZoneCount);
            //int zoneCount = 0;
            foreach (InitialZone initZone in zoneList)
            {
                // Randomize list??? (otherwise prioritize oldest zone over newer zones)
                // If already counted max number of zones
                // If body is in buffward
                if (initZone.zone is BuffWard ward
                    && (body.transform.position - ward.transform.position).magnitude <= Mathf.Abs(ward.calculatedRadius))
                {
                    //Log.Debug(" . " + (body.transform.position - ward.transform.position).magnitude + " BUFF less then " + Mathf.Abs(ward.calculatedRadius));
                    list.Add(initZone);
                }
                // If body is in holdout zone
                else if (initZone.zone is HoldoutZoneController holdout
                    && holdout.IsBodyInChargingRadius(body))
                {
                    //Log.Debug(" . in Holdout");
                    list.Add(initZone);
                }
                // break if already found max amount
                if (list.Count >= maxZoneCount)
                {
                    break;
                }
            }
            //Log.Debug(" ^ ^ ^ list.Count = " + list.Count);
            return [.. list];
        }

        private void IncreaseEachRadius(InitialZone[] inZoneList, float overhealPer, int grabCount)
        {
            Log.Warning(" +++++ +++++ +++++ Increasing radius");
            overhealPer *= radiusPerOverheal;
            // Try to increase size of each zone the player is in
            foreach (InitialZone initZone in inZoneList)
            {
                // If body is in buff ward
                if (initZone.zone is BuffWard ward)
                {
                    Log.Debug(" . in Ward");
                    ward.radius += RadiusChange(initZone, ward.radius, overhealPer, grabCount);
                    Log.Debug(" _ _ _ _ N E W Ward radius " + ward.radius);
                }
                // If body is in holdout zone
                else if (initZone.zone is HoldoutZoneController holdout)
                {
                    Log.Debug(" . in Holdout");
                    holdout.baseRadius += RadiusChange(initZone, holdout.baseRadius, overhealPer, grabCount);
                    Log.Debug(" _ _ _ _ N E W Ward radius " + holdout.baseRadius);
                    /*
                    Log.Debug(" . in Holdout");
                    initZone.SetStartRadius(holdout.baseRadius);
                    maxRadius *= initZone.startRadius;
                    Log.Debug(" _ _ _ _ initZone.initialRadius = " + initZone.startRadius);
                    Log.Debug(" _ _ _ _ Holdout radius " + holdout.baseRadius + " increased by " + overhealPer * 100 + "% = " + initZone.startRadius * overhealPer);
                    // Min between added radius and max radius
                    holdout.baseRadius = Mathf.Min(0, Mathf.Min(holdout.baseRadius + initZone.startRadius * overhealPer, maxRadius - holdout.baseRadius));
                    Log.Debug(" _ _ _ _ N E W Ward radius " + holdout.baseRadius + " | max: " + maxRadius);
                    */
                }
            }
        }

        private float RadiusChange(InitialZone initZone, float currentRadius, float overhealPer, int grabCount)
        {
            float maxRadius = 1 + (baseMaxRadius + (grabCount - 1) * perStackMaxRadius) / 100;
            float baseRadius = initZone.startRadius;
            // If the startRadius hasn't been initilized
            if (initZone.startRadius == -1f)
            {
                // Remove old immutable struct, replace with struct that has current Radius
                _ = zoneList.Remove(new InitialZone(initZone.zone));
                zoneList.Add(new InitialZone(initZone.zone, currentRadius));
                baseRadius = currentRadius;
                Log.Debug(" . . . initialRadius " + currentRadius);
            }
            maxRadius *= baseRadius;
            //Log.Debug(" _ _ _ _ initZone.initialRadius = " + baseRadius);
            //Log.Debug(" _ _ _ _ currentRadius " + currentRadius + " increased by " + overhealPer * 100 + "% = " + baseRadius * overhealPer + " MAX: " + maxRadius);
            //Log.Debug(" . " + (body.transform.position - ward.transform.position).magnitude + " BUFF less then " + Mathf.Abs(ward.calculatedRadius));
            // Min between added radius and max radius
            return Mathf.Max(0, Mathf.Min(baseRadius * overhealPer, maxRadius - currentRadius));
        }
    }
}