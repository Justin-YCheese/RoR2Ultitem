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
            string tokenPrefix = "TESX";

            LanguageAPI.Add(tokenPrefix + "_NAME", "a");
            LanguageAPI.Add(tokenPrefix + "_PICK", "b");
            LanguageAPI.Add(tokenPrefix + "_DESC", "c");
            LanguageAPI.Add(tokenPrefix + "_LORE", "d");

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

            equipment.cooldown = 3f;

            equipment.pickupIconSprite = Ultitems.mysterySprite;
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

            // Log.Info("Faulty Bulb Initialized");
            Log.Warning("Initialized: " + equipment.name);
        }

        protected void Hooks()
        {
            //EquipmentSlot.PerformEquipmentAction += new EquipmentSlot.hook_PerformEquipmentAction(this.PerformEquipmentAction);
            //EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
            //On.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted;
            //On.RoR2.EquipmentSlot.cctor += EquipmentSlot_cctor;
            //On.RoR2.EquipmentSlot.ExecuteIfReady += EquipmentSlot_ExecuteIfReady;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            Log.Debug(" ! ! ! Equipment Test PerformEquipmentAction");
            if (equipmentDef == equipment)
            {
                CharacterBody activator = self.characterBody;
                Log.Warning("Test Equipment!");
                activator.healthComponent.AddBarrier(activator.healthComponent.fullCombinedHealth);
                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        private bool EquipmentSlot_ExecuteIfReady(On.RoR2.EquipmentSlot.orig_ExecuteIfReady orig, EquipmentSlot self)
        {
            Log.Debug(" ~ ~ ~ ExecuteIfReady testing equipment?");
            if (self.equipmentIndex == equipment.equipmentIndex)
            {
                Log.Debug("My equipment is ready");
            }
            return orig(self);
        }

        private void EquipmentSlot_cctor(On.RoR2.EquipmentSlot.orig_cctor orig)
        {
            Log.Debug(" - - - cctor testing equipment?");
            orig();
        }

        private void EquipmentSlot_OnEquipmentExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            Log.Debug("Equipment Test OnEquipmentExecuted?");
            orig(self);
        }

        /*/
        private void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot arg1, EquipmentIndex arg2)
        {
            Log.Debug("Server is testing equipment?");
        }//*/
    }
}