using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class YieldSignStop : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;
        //private const float speedMultiplier = 4f;

        public override void Init()
        {
            equipment = CreateItemDef(
                "YIELDSIGNSTOP",
                "Yield Sign",
                "Alternate between multiplying speed and canceling it. Hit nearby enemies each time.",
                "Alternate between multipling speed by 400%, or setting it to zero. Damage nearby enemies for 300% damage.",
                "Just Stop",
                YieldSign.cooldown,
                false,
                false,
                Ultitems.Assets.YieldSignStopSprite,
                Ultitems.Assets.YieldSignStopPrefab
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
                activator.inventory.SetEquipmentIndex(YieldSign.equipment.equipmentIndex);
                //self.subcooldownTimer = YieldSign.subCooldown;
                return YieldSign.YieldActivated(activator, 0f);
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }
    }
}