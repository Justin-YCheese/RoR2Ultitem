﻿using BepInEx.Configuration;
using IL.RoR2.Items;
using On.RoR2.Items;
using R2API;
using RoR2;
using UltitemsCyan.Buffs;
using UnityEngine;
using static UltitemsCyan.Equipment.YieldSign;

namespace UltitemsCyan.Equipment
{
    public class YieldSignStop : EquipmentBase
    {
        public static EquipmentDef equipment;

        public override void Init(ConfigFile configs)
        {
            string itemName = "Yield Sign";
            if (!CheckItemEnabledConfig(itemName, "Equipment", configs))
            {
                return;
            }
            equipment = CreateItemDef(
                "YIELDSIGNSTOP",
                itemName,
                "Alternate between multiplying speed and canceling it. Hit nearby enemies each time.",
                "Alternate between multipling speed by 400%, or setting it to zero. Damage nearby enemies for 300% damage.",
                "Just Stop",
                cooldown,
                false,
                false,
                false,
                UltAssets.YieldSignStopSprite,
                UltAssets.YieldSignStopPrefab
            );
        }

        protected override void Hooks()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            On.RoR2.EquipmentSlot.RpcOnClientEquipmentActivationRecieved += EquipmentSlot_RpcOnClientEquipmentActivationRecieved;
        }

        private void EquipmentSlot_RpcOnClientEquipmentActivationRecieved(On.RoR2.EquipmentSlot.orig_RpcOnClientEquipmentActivationRecieved orig, EquipmentSlot self)
        {
            orig(self);
            if (self.equipmentIndex == equipment.equipmentIndex && self.characterBody && self.characterBody.characterMotor)
            {
                VelocityMultiplier(ref self.characterBody.characterMotor.velocity, stopMultiplier, stopHorizontalMultiplier, stopMaxMultiplier, stopMinMultiplier, self.characterBody.moveSpeed);
                YieldAttack(self.characterBody);
                self.characterBody.inventory.SetEquipmentIndex(YieldSign.equipment.equipmentIndex);
            }
        }

        //
        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                Log.Debug("Yields qStop");
                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }
        //*/
    }
}