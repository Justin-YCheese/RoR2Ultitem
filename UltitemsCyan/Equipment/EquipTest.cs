using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class EquipTest : EquipmentBase
    {
        public static EquipmentDef equipment;

        private void Tokens()
        {
            string tokenPrefix = "ICECUBES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "9 Ice Cubes");
            LanguageAPI.Add(tokenPrefix + "_PICK", "a");
            LanguageAPI.Add(tokenPrefix + "_DESC", "b");
            LanguageAPI.Add(tokenPrefix + "_LORE", "c");

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

            Log.Debug("Init1 " + equipment.name);

            equipment.cooldown = 3f;

            equipment.pickupModelPrefab = Ultitems.mysteryPrefab;
            equipment.pickupIconSprite = Ultitems.mysterySprite;

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

            // Log.Info("Faulty Bulb Initialized");
            Log.Warning("Initialized: " + equipment.name);
        }

        protected void Hooks()
        {
            //EquipmentSlot.PerformEquipmentAction += new EquipmentSlot.hook_PerformEquipmentAction(this.PerformEquipmentAction);
        }
    }
}