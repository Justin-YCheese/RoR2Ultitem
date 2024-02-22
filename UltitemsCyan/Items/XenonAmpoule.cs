using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using Random = UnityEngine.Random;
using UnityEngine.ProBuilder;
using static UnityEngine.UI.Image;
using RoR2.Audio;
using System;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class XenonAmpoule : ItemBase
    {
        private static ItemDef item;


        public static GameObject TracerRailgunSuper = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/Railgunner/TracerRailgunSuper.prefab").WaitForCompletion();
        //public static GameObject Tracer2 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/OrbitalLaser/TracerAncientWisp.prefab").WaitForCompletion();
        //public static GameObject Tracer3 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageLightningLaser.prefab").WaitForCompletion();
        //public static GameObject Tracer4 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Mage/TracerMageIceLaser.prefab").WaitForCompletion();
        //public static GameObject Tracer5 = Addressables.LoadAssetAsync<GameObject>("RoR2/Junk/Commando/TracerBarrage2.prefab").WaitForCompletion();

        public static float damagePerStack = 4f;
        public static float baseDamage = 16f;

        public static float laserRadius = 2.5f;

        //public static float innerRadius = 1.75f;
        //public static float outerRadius = 3.5f;

        public static float sourDamagePercent = 20f;

        public override void Init()
        {
            damagePerStack /= 3f;   // Counter WeakPointHit and Crit bonus
            baseDamage /= 3f;       // Counter WeakPointHit and Crit bonus

            item = CreateItemDef(
                "XENONAMPOULE",
                "Xenon Ampoule",
                "Fire a critting laser when you activate your active item.",
                "Fire a critting laser for 1600% (+400% per stack) damage when you activate your active item.",
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
                    Log.Debug(" ! Xenon held, fire projectile");
                    activateAmpoule(activator, grabCount);
                }
            }
            else
            {
                Log.Warning("Xe Equipment not fired?");
            }
        }

        private static void activateAmpoule(CharacterBody activator, int grabCount)
        {
            //Log.Debug(" - - : Firing Da Lazer!");

            Ray aimRay = activator.inputBank.GetAimRay();
            float damage = activator.damage * (baseDamage + (damagePerStack * (grabCount - 1)));

            //Util.PlaySound("Play_railgunner_m2_fire", activator.gameObject);
            Util.PlaySound("Play_voidRaid_snipe_shoot_final", activator.gameObject);
            

            //float radius = 15;

            BulletAttack baseLaser = new()
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
                //damage = activator.damage * damage,
                force = 1f,
                falloffModel = BulletAttack.FalloffModel.None,
                tracerEffectPrefab = TracerRailgunSuper,
                //muzzleName = MinigunState.muzzleName,
                //hitEffectPrefab = ImpactRailgun,
                isCrit = true, // true
                HitEffectNormal = false,
                //radius = 2f,
                maxDistance = 2000f,
                smartCollision = true,
                stopperMask = LayerIndex.noDraw.mask
            };

            FireLaser(baseLaser, damage, laserRadius);
            //fireLaser(baseLaser, damage, innerRadius);
            //fireLaser(baseLaser, damage * sourDamagePercent / 100f, outerRadius);
        }

        private static void FireLaser(BulletAttack laser, float damage, float radius)
        {
            laser.damage = damage;
            laser.radius = radius;
            laser.Fire();
        }
    }
}