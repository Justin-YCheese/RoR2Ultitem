using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;

namespace UltitemsCyan.Items.Tier2
{
    // TODO: check if Item classes needs to be public
    public class XenonAmpoule : ItemBase
    {
        private static ItemDef item;

        public static GameObject TracerRailgun = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgun.prefab").WaitForCompletion();
        public static GameObject TracerRailgunCryo = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunCryo.prefab").WaitForCompletion();
        public static GameObject TracerRailgunSuper = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion();

        //public static GameObject Tracer2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/OrbitalLaser/TracerAncientWisp.prefab").WaitForCompletion();
        //public static GameObject Tracer3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageLightningLaser.prefab").WaitForCompletion();
        //public static GameObject Tracer4 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageIceLaser.prefab").WaitForCompletion();
        //public static GameObject Tracer5 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Commando/TracerBarrage2.prefab").WaitForCompletion();

        public static float damagePerStack = 4f;
        public static float baseDamage = 16f;

        public static float laserRadius = 2.5f;

        public static float shortLaserRadius = 1f;
        public static float shortDamagePercent = 50f;

        public static float longLaserRadius = 3.2f;

        public static float shortCooldown = 30f;    // Less than this and has half damage
        public static float normalCooldown = 80f;   // Greater than this, and multiple cooldown by cooldown / 80

        public override void Init()
        {
            damagePerStack /= 3f;   // Counter WeakPointHit and Crit bonus
            baseDamage /= 3f;       // Counter WeakPointHit and Crit bonus

            //TracerRailgunSuper

            item = CreateItemDef(
                "XENONAMPOULE",
                "Xenon Ampoule",
                "Activating your Equipment also fires a laser",
                "Activating your Equipment also fires a <style=cIsDamage>critting laser</style> for <style=cIsDamage>1600%</style> <style=cStack>(+400% per stack)</style> base damage. The damage scales with an equipment's cooldown.",
                "It's Purple because I messed up. Xenon is supposed to be more blue than hyrdogen, but I wanted an X name. Sorry.",
                ItemTier.Tier2,
                Ultitems.Assets.XenonAmpouleSprite,
                Ultitems.Assets.XenonAmpoulePrefab,
                [ItemTag.Damage, ItemTag.EquipmentRelated]
            );
        }

        protected override void Hooks()
        {
            //EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated; // Doesn't work?
            //On.RoR2.EquipmentSlot.PerformEquipmentAction // For Equipment Activation (if used, has a chance of returning before eaching hooked code)
            On.RoR2.EquipmentSlot.OnEquipmentExecuted += EquipmentSlot_OnEquipmentExecuted; // For If the Equipment was fired
        }

        private void EquipmentSlot_OnEquipmentExecuted(On.RoR2.EquipmentSlot.orig_OnEquipmentExecuted orig, EquipmentSlot self)
        {
            orig(self);

            //Log.Debug("Xenon Perform Equipment Action");
            if (NetworkServer.active && self.characterBody && self.inventory)
            {
                //Log.Debug(" ? ? ? Xenon Perform Equipment Action Actually Activated?");
                CharacterBody activator = self.characterBody;
                int grabCount = activator.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    // Cooldown after reduction from items
                    //Log.Debug(" ! Xe Fire | Cooldown: " + self.cooldownTimer);

                    Ray aimRay = activator.inputBank.GetAimRay();
                    //float damage = activator.damage * (baseDamage + (damagePerStack * (grabCount - 1)));
                    float damage;
                    float radius;
                    GameObject tracer;

                    // Get Default Cooldown of Item
                    float cooldown = EquipmentCatalog.GetEquipmentDef(self.equipmentIndex).cooldown;

                    if (cooldown < shortCooldown)
                    {
                        //Log.Debug("Short");
                        Util.PlaySound("Play_railgunner_m2_fire", activator.gameObject);
                        tracer = TracerRailgun;
                        damage = (baseDamage + damagePerStack * (grabCount - 1)) * shortDamagePercent / 100f;//  * (self.cooldownTimer / 45f)
                        radius = shortLaserRadius;
                    }
                    else if (cooldown < normalCooldown)
                    {
                        //Log.Debug("Normal");
                        Util.PlaySound("Play_voidRaid_snipe_shoot_final", activator.gameObject);
                        tracer = TracerRailgunCryo;
                        damage = baseDamage + damagePerStack * (grabCount - 1);
                        radius = laserRadius;
                    }
                    else
                    {
                        //Log.Debug("Long");
                        Util.PlaySound("Play_voidRaid_snipe_shoot_final", activator.gameObject);
                        tracer = TracerRailgunSuper;
                        damage = (baseDamage + damagePerStack * (grabCount - 1)) * (cooldown / normalCooldown);
                        radius = longLaserRadius;
                    }

                    Log.Debug((baseDamage + damagePerStack * (grabCount - 1)) + " * " + (cooldown / normalCooldown) + " | " + damage);

                    // Create and Fire Laser
                    new BulletAttack
                    {
                        owner = activator.gameObject,
                        weapon = activator.gameObject,
                        origin = aimRay.origin,
                        aimVector = aimRay.direction,
                        minSpread = 0f,
                        maxSpread = 0f,
                        bulletCount = 1U,
                        procCoefficient = 2f,
                        damageType = DamageType.WeakPointHit,
                        damage = activator.damage * damage,
                        force = 1f,
                        falloffModel = BulletAttack.FalloffModel.None,
                        //muzzleName = MinigunState.muzzleName,
                        //hitEffectPrefab = ImpactRailgun,
                        tracerEffectPrefab = tracer,
                        isCrit = true, // true
                        HitEffectNormal = false,
                        radius = radius,
                        maxDistance = 2000f,
                        smartCollision = true,
                        stopperMask = LayerIndex.noDraw.mask
                    }.Fire();
                }
            }
            else
            {
                Log.Warning("Xe Equipment not fired?");
            }
        }
    }
}