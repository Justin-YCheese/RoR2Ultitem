﻿using R2API;
using RoR2;
using System;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class PotOfRegolith : EquipmentBase
    {
        public static EquipmentDef equipment;

        private const float percentDamage = 5f;
        private const float flatDamage = 20f;

        public override void Init()
        {
            equipment = CreateItemDef(
                "POTOFREGOLITH",
                "Pot of Regolith",
                "<style=cDeath>Take damage</style> on use.",
                "Take <style=cIsHealth>20</style> plus <style=cIsHealth>5% of your current health</style> as <style=cIsDamage>damage</style>",
                "The dust is as sharp as a knife",
                2f,
                true,
                true,
                Ultitems.Assets.PotOfRegolithSprite,
                Ultitems.Assets.PotOfRegolithPrefab
            );
        }

        protected override void Hooks()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                CharacterBody activator = self.characterBody;
                DamageInfo damageSelf = new()
                {
                    crit = false, // activator.RollCrit()
                    damage = (percentDamage / 100f * activator.healthComponent.combinedHealth) + flatDamage, // + activator.baseDamage
                    procCoefficient = 100f,

                    damageType = DamageType.Generic,
                    inflictor = activator.gameObject,
                    attacker = activator.gameObject,
                    position = activator.transform.position
                };
                //if (damageSelf.crit) { damageSelf.damage *= 2; }

                Log.Debug("Activator damage: (" + (damageSelf.damage - flatDamage) + " + " + flatDamage + ")");

                activator.healthComponent.TakeDamage(damageSelf);
                //self.subcooldownTimer += 5f;
                //Log.Debug("Sub cooldown");
                Util.PlaySound("Play_imp_attack", activator.gameObject);

                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }
    }
}