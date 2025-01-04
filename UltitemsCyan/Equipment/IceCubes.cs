using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class IceCubes : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;
        private const float percentOfBarrier = 75f;

        public override void Init(ConfigFile configs)
        {
            string itemName = "9 Ice Cubes";
            if (!CheckItemEnabledConfig(itemName, "Equipment", configs))
            {
                return;
            }
            equipment = CreateItemDef(
                "ICECUBES",
                itemName,
                "Gain barrier on use",
                "Instantly gain <style=cIsHealing>temporary barrier</style> for <style=cIsHealing>75% of your maximum health</style>",
                "Alice that freezes forever",
                60f,
                false,
                true,
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
                CharacterBody activator = self.characterBody;
                activator.healthComponent.AddBarrier(activator.healthComponent.fullBarrier * percentOfBarrier / 100f);
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