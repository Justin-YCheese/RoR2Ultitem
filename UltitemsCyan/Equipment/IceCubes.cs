﻿using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class IceCubes : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;
        private const float fractionOfBarrier = 0.8f;


        private void Tokens()
        {
            string tokenPrefix = "ICECUBES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "9 Ice Cubes");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Gain temporary barrier on use");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Instantly gain <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>80% of your maximum health</style>");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Alice that freezes forever");

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

            equipment.cooldown = 60f;

            equipment.pickupIconSprite = Ultitems.Assets.IceCubesSprite;
            equipment.pickupModelPrefab = Ultitems.mysteryPrefab;

            equipment.appearsInSinglePlayer = true;
            equipment.appearsInMultiPlayer = true;
            equipment.canDrop = true;

            equipment.enigmaCompatible = true;
            equipment.isBoss = false;
            equipment.isLunar = false;

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
                
                activator.healthComponent.AddBarrier(activator.healthComponent.fullBarrier * fractionOfBarrier);
                //self.subcooldownTimer += 5f;
                //Log.Debug("Sub cooldown");
                Util.PlaySound("Play_item_proc_iceRingSpear", self.gameObject);
                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }
    }
}