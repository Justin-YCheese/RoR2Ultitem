using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;
using static UltitemsCyan.Ultitems;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class XenonAmpoule : ItemBase
    {
        private static ItemDef item;
        private static GameObject shockwaveProjectile;

        public override void Init()
        {
            item = CreateItemDef(
                "XENONAMPOULE",
                "Xenon Ampoule",
                "",
                "",
                "",
                ItemTier.Tier2,
                Ultitems.Assets.XenonAmpouleSprite,
                Ultitems.Assets.XenonAmpoulePrefab,
                [ItemTag.Damage, ItemTag.EquipmentRelated]
            );
        }

        protected override void Hooks()
        {
            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            Log.Debug("Xenon Perform Equipment Action");
            bool firedEquipment = orig(self, equipmentDef);
            Log.Debug("Fired? " + firedEquipment);
            if (firedEquipment)
            {
                Log.Debug(" ? ? ? Xenon Perform Equipment Action Actually Activated?");
                CharacterBody activator = self.characterBody;
                int grabCount = activator.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug(" ! Xenon held, fire projectile");
                    /*/
                    FireProjectileInfo fireProjectileInfo = new()
                    {
                        owner = activator.gameObject,
                        damage = activator.baseDamage * 5 * grabCount,
                        position = activator.corePosition,
                        rotation = Util.QuaternionSafeLookRotation(self.inputBank.aimDirection),
                        crit = false,
                        projectilePrefab = Ultitems.mysteryPrefab
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);//*/
                }
                return true;
            }
            else
            {
                Log.Debug("X No X");
                return false;
            }

            //return orig(self, equipmentDef);
        }

        private void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot arg1, EquipmentIndex arg2)
        {
            Log.Debug("Xenon Ampoule Equipment Activated");
        }

        /*
         * 
         * 
         */
    }
}