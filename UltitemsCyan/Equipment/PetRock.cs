using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class PetRock : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;
        private const float armorGained = 150f;
        private const float forceResistance = 80f;
        private const float speedPercent = 50f;

        public override void Init()
        {
            equipment = CreateItemDef(
                "PETROCK",
                "Pet Rock",
                "Gain barrier on use",
                "Instantly gain <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>80% of your maximum health</style>",
                "Alice that freezes forever",
                20f,
                false,
                true,
                UltAssets.IceCubesSprite,
                UltAssets.IceCubesPrefab
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
                Log.Debug("Pet Rock Activated");
                return true;

                
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        //damageInfo.force *= 0;
    }
}