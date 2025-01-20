using RoR2;
using UnityEngine;
using BepInEx.Configuration;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;

namespace UltitemsCyan.Items.Tier2
{
    // Possibly make it so that it gains 8% +2%perStack max of 32% + 40+
    // 3% +3% max of 10 stacks
    // 5% max of 6 stacks +6
    // 
    // But then would be exponential

    public class TinyIgloo : ItemBase
    {
        public static ItemDef item;

        private readonly List<NetworkBehaviour> zoneList = [];

        private const int basePercent = 12;
        private const int perStackPercent = 8;

        private const int maxZoneCount = 4;
        private const int minOverHeal = 10;
        private const float radiusPerOverHeal = 0.5f;
        private const int holdoutMultipleir = 2;


        public override void Init(ConfigFile configs)
        {
            string itemName = "Tiny Igloo";
            if (!CheckItemEnabledConfig(itemName, "Green", configs))
            {
                return;
            }
            item = CreateItemDef(
                "TINYIGLOO",
                itemName,
                "Increase healing while in ",
                "Increase healing by 12% (+8% per stack) per zone you are in. Overhealing while in a zone will increase the size of the zone.",
                "GPU GPU",
                ItemTier.Tier2,
                UltAssets.CremeBruleeSprite,
                UltAssets.CremeBruleePrefab,
                [ItemTag.Healing, ItemTag.Utility, ItemTag.AIBlacklist, ItemTag.HoldoutZoneRelated]
            );
        }


        protected override void Hooks()
        {
            On.RoR2.HoldoutZoneController.OnEnable += HoldoutZoneController_OnEnable;
            On.RoR2.HoldoutZoneController.OnDisable += HoldoutZoneController_OnDisable;
            On.RoR2.BuffWard.OnEnable += BuffWard_OnEnable;
            On.RoR2.BuffWard.OnDisable += BuffWard_OnDisable;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
        }

        private void BuffWard_OnEnable(On.RoR2.BuffWard.orig_OnEnable orig, BuffWard self)
        {
            zoneList.Add(self);
            Log.Debug(" + + + ++++++ + + + Adding B U F F");
            orig(self);
        }

        private void BuffWard_OnDisable(On.RoR2.BuffWard.orig_OnDisable orig, BuffWard self)
        {
            _ = zoneList.Remove(self);
            Log.Debug(" + + + +----+ + + + B U F F Subtraction");
            orig(self);
        }

        private void HoldoutZoneController_OnEnable(On.RoR2.HoldoutZoneController.orig_OnEnable orig, HoldoutZoneController self)
        {
            zoneList.Add(self);
            Log.Debug(" + + + ++++++ + + + Adding Zone");
            orig(self);
        }
        private void HoldoutZoneController_OnDisable(On.RoR2.HoldoutZoneController.orig_OnDisable orig, HoldoutZoneController self)
        {
            _ = zoneList.Remove(self);
            Log.Debug(" + + + +----+ + + + Zone Subtraction");
            orig(self);
        }

        // Increase amount healed and increase zone radius
        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (nonRegen && self && self.body && self.body.inventory)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    // Increase amount healed
                    Log.Warning("  +++  Healing igloo");
                    NetworkBehaviour[] inZoneList = GetInZoneList(self.body);
                    int zoneCount = inZoneList.Length;

                    Log.Debug(" ^ ^ ^ zoneCount = " + zoneCount);
                    float multiplier = 1 + (basePercent + (grabCount - 1) * perStackPercent) * zoneCount / 100f;
                    amount *= multiplier;

                    // Return modified amount and get final heal amount
                    float returnAmount = orig(self, amount, procChainMask, nonRegen);

                    // Mininum amount to heal to be at full health
                    float overHeal = returnAmount - Mathf.Max(Mathf.Min(returnAmount, self.fullHealth - self.health), 0f);

                    // Increase radius for over heals
                    IncreaseRadius(inZoneList, 13f);

                    Log.Warning("  +++  | Multiplier: " + multiplier + " | amount " + amount + " | overHeal " + overHeal);

                    return returnAmount;
                }
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        // Get list of zones the body is in
        private NetworkBehaviour[] GetInZoneList(CharacterBody body)
        {
            List<NetworkBehaviour> list = new(maxZoneCount);
            //int zoneCount = 0;
            foreach (NetworkBehaviour zone in zoneList)
            {
                // Randomize list??? (otherwise prioritize oldest zone over newer zones)
                // If already counted max number of zones
                // If body is in buffward
                if (zone is BuffWard ward
                    && (body.transform.position - ward.transform.position).magnitude <= Mathf.Abs(ward.calculatedRadius))
                {
                    //Log.Debug(" . " + (body.transform.position - ward.transform.position).magnitude + " BUFF less then " + Mathf.Abs(ward.calculatedRadius));
                    list.Add(zone);
                }
                // If body is in holdout zone
                else if (zone is HoldoutZoneController holdout
                    && holdout.IsBodyInChargingRadius(body))
                {
                    //Log.Debug(" . in Holdout");
                    list.Add(zone);
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

        private void IncreaseRadius(NetworkBehaviour[] inZoneList, float amount)
        {
            Log.Debug("Increasing radius");

            foreach (NetworkBehaviour zone in inZoneList)
            {
                if (zone is BuffWard ward)
                {
                    Log.Debug(" _ _ _ _ Ward radius " + ward.radius + " increased by _" + Math.Min(amount - minOverHeal, 0f));
                    //Log.Debug(" . " + (body.transform.position - ward.transform.position).magnitude + " BUFF less then " + Mathf.Abs(ward.calculatedRadius));
                    ward.radius += Math.Min(amount - minOverHeal, 0f);
                }
                // If body is in holdout zone
                else if (zone is HoldoutZoneController holdout)
                {
                    //Log.Debug(" . in Holdout");
                    Log.Debug(" _ _ _ _ Holdout radius " + holdout.baseRadius + " increased by _" + Math.Min(amount - minOverHeal, 0f) * holdoutMultipleir);
                    holdout.baseRadius += Math.Min(amount - minOverHeal, 0f) * holdoutMultipleir;
                }
            }
            /*
            float num2 = amount;
            if (this.health < this.fullHealth)
            {
                float num3 = Mathf.Max(Mathf.Min(amount, this.fullHealth - this.health), 0f);
                num2 = amount - num3;
                this.Networkhealth = this.health + num3;
            }
            if (num2 > 0f && nonRegen && this.itemCounts.barrierOnOverHeal > 0)
            {
                float value = num2 * ((float)this.itemCounts.barrierOnOverHeal * 0.5f);
                this.AddBarrier(value);
            }
            */
        }

#pragma warning disable IDE0051 // Remove unused private members
        private void AddToRadius(NetworkBehaviour zone, float addRadius)
#pragma warning restore IDE0051 // Remove unused private members
        {
            if (zone is BuffWard ward)
            {
                ward.radius += addRadius;
            }
            else if (zone is HoldoutZoneController)
            {

            }


        }

#pragma warning disable IDE0051 // Remove unused private members
        private void HoldoutZoneController_Start(On.RoR2.HoldoutZoneController.orig_Start orig, HoldoutZoneController self)
#pragma warning restore IDE0051 // Remove unused private members
        {
            orig(self);
            Log.Debug("     |||     Created a zone     |||");
        }
    }
}