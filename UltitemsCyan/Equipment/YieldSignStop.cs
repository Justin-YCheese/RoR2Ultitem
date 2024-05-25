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

        public override void Init()
        {
            equipment = CreateItemDef(
                "YIELDSIGNSTOP",
                "Yield Sign",
                "Alternate between multiplying speed and canceling it. Hit nearby enemies each time.",
                "Alternate between multipling speed by 400%, or setting it to zero. Damage nearby enemies for 300% damage.",
                "Just Stop",
                cooldown,
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
                self.characterBody.AddTimedBuff(YieldsStopBuff.buff.buffIndex, 0.01f);
                self.characterBody.inventory.SetEquipmentIndex(YieldSign.equipment.equipmentIndex);
                Log.Debug("Yields Stop");
                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }
    }
}