using R2API;
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
        
        private void Tokens()
        {
            string tokenPrefix = "POTOFREGOLITH";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Pot of Regolith");
            LanguageAPI.Add(tokenPrefix + "_PICK", "<style=cDeath>Take damage</style> on use.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Take <style=cIsHealth>10</style> plus <style=cIsHealth>5% of your current health</style> as <style=cIsDamage>damage</style>");
            LanguageAPI.Add(tokenPrefix + "_LORE", "The dust is as sharp as a knife");

            equipment.name = tokenPrefix + "_NAME";
            equipment.nameToken = tokenPrefix + "_NAME";
            equipment.pickupToken = tokenPrefix + "_PICK";
            equipment.descriptionToken = tokenPrefix + "_DESC";
            equipment.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            equipment = ScriptableObject.CreateInstance<EquipmentDef>();

            Tokens();

            Log.Debug("Init " + equipment.name);

            equipment.cooldown = 2f;

            equipment.pickupIconSprite = Ultitems.Assets.PotOfRegolithSprite;
            equipment.pickupModelPrefab = Ultitems.mysteryPrefab;

            equipment.appearsInSinglePlayer = true;
            equipment.appearsInMultiPlayer = true;
            equipment.canDrop = true;

            equipment.enigmaCompatible = true;
            equipment.isBoss = false;
            equipment.isLunar = true;

            ItemDisplayRuleDict displayRules = new(null);
            ItemAPI.Add(new CustomEquipment(equipment, displayRules));

            // Item Functionality
            Hooks();

            GetEquipmentDef = equipment;
            Log.Warning("Initialized: " + equipment.name);
        }

        protected void Hooks()
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
                    damage = (percentDamage * activator.healthComponent.combinedHealth / 100f) + flatDamage, // + activator.baseDamage
                    procCoefficient = 100f,

                    damageType = DamageType.Generic,
                    inflictor = activator.gameObject,
                    attacker = activator.gameObject,
                    position = activator.transform.position
                };
                //if (damageSelf.crit) { damageSelf.damage *= 2; }

                Log.Debug("Activator damage: (" + (percentDamage * activator.healthComponent.combinedHealth / 100f) + " + " + flatDamage + ")");

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