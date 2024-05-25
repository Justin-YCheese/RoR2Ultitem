﻿using RoR2;
using UltitemsCyan.Buffs;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltitemsCyan.Equipment
{
    // Can't affect character motor or give buffs with equipment


    // TODO: check if Item classes needs to be public
    public class YieldSign : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;

        public const float cooldown = 10f;
        public const float subCooldown = 0.1f;

        public const float boostMultiplier = 3f;
        public const float boostHorizontalMultiplier = 1.75f;
        public const float boostMinMultiplier = 4f;
        public const float boostMaxMultiplier = 8f;

        public const float stopMultiplier = -.4f;
        public const float stopHorizontalMultiplier = 1.5f;
        public const float stopMaxMultiplier = 0.8f;
        public const float stopMinMultiplier = 0.15f;

        private const float yieldDamage = 3.5f; //350%
        private const float hitForce = 2500f;
        private const int radius = 8;

        private static readonly GameObject willOWisp = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExplodeOnDeath/WilloWispDelay.prefab").WaitForCompletion();
        private static readonly GameObject explosionGolem = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();
        //AncientWispPillar ?

        public override void Init()
        {
            equipment = CreateItemDef(
                "YIELDSIGN",
                "Yield Sign",
                "Alternate between multiplying speed and canceling it. Hit nearby enemies each time.",
                "Alternate between multiplying current speed by 300% and zeroing it. Stun nearby enemies for 350% damage.",
                "Stop and go, the best of both worlds right?",
                cooldown,
                false,
                true,
                Ultitems.Assets.YieldSignSprite,
                Ultitems.Assets.YieldSignPrefab
            );
        }

        protected override void Hooks()
        {
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            On.RoR2.EquipmentSlot.RpcOnClientEquipmentActivationRecieved += EquipmentSlot_RpcOnClientEquipmentActivationRecieved;
        }

        private void EquipmentSlot_RpcOnClientEquipmentActivationRecieved(On.RoR2.EquipmentSlot.orig_RpcOnClientEquipmentActivationRecieved orig, EquipmentSlot self)
        {
            orig(self);
            Log.Debug("On Client?");
            if (self.equipmentIndex == equipment.equipmentIndex)
            {
                Log.Debug("Oh my God!");
            }
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                Log.Debug("Yields qBoost");
                self.characterBody.AddTimedBuff(YieldsBoostBuff.buff.buffIndex, 0.01f);
                self.characterBody.inventory.SetEquipmentIndex(YieldSignStop.equipment.equipmentIndex);
                Log.Debug("Yields wBoost");
                return true;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        public static void VelocityMultiplier(ref Vector3 velocity, float multiplier, float horizontalMultiplier, float maxMultiplier, float minMultiplier, float moveSpeed)
        {
            velocity *= multiplier;
            velocity.x *= horizontalMultiplier;
            velocity.z *= horizontalMultiplier;

            float maxSpeed = moveSpeed * maxMultiplier; // / forceMultiplier
            float minSpeed = moveSpeed * minMultiplier; // / forceMultiplier
            Log.Debug("Velocity exceeded bounds? | " + velocity.magnitude + " >< " + moveSpeed + " * " + maxMultiplier + " | " + velocity);// / 36
            if (velocity.magnitude > maxSpeed)
            {
                velocity = velocity.normalized * maxSpeed;
                Log.Warning("New Max Velocity | " + velocity + " mag: " + velocity.magnitude);
            }
            else if (velocity.magnitude < minSpeed)
            {
                velocity = velocity.normalized * minSpeed;
                Log.Warning("new min Velocity | " + velocity + " mag: " + velocity.magnitude);
            }
        }

        public static void YieldAttack(CharacterBody body)
        {
            body.characterMotor.disableAirControlUntilCollision = false;

            GameObject explostionObject = Object.Instantiate(willOWisp, body.transform.position, Quaternion.identity);
            DelayBlast blast = explostionObject.GetComponent<DelayBlast>();

            blast.position = body.transform.position;
            blast.attacker = body.gameObject;
            blast.inflictor = body.gameObject;
            blast.baseDamage = yieldDamage * body.damage;
            blast.baseForce = hitForce;
            //blast.bonusForce = ;
            blast.radius = radius;
            blast.maxTimer = 0f;
            blast.falloffModel = BlastAttack.FalloffModel.None;
            blast.damageColorIndex = DamageColorIndex.Fragile;
            blast.damageType = DamageType.AOE | DamageType.Stun1s;
            blast.procCoefficient = 1f;

            blast.explosionEffect = explosionGolem;
            blast.hasSpawnedDelayEffect = true;

            blast.teamFilter = new TeamFilter()
            {
                teamIndexInternal = (int)body.teamComponent.teamIndex,
                defaultTeam = TeamIndex.None,
                teamIndex = body.teamComponent.teamIndex,
                NetworkteamIndexInternal = (int)body.teamComponent.teamIndex
            };

            _ = Util.PlaySound("Play_bellBody_attackLand", body.gameObject);
            _ = Util.PlaySound("Play_bellBody_impact", body.gameObject);
        }
    }
}

/*
 *      Failed tries
 * 
 * CharacterMotor.velocity  (Fails client, changing server velocity does nothing to client, client velocity not sent to server)
 * CharacterDirection       (Fails client, direction missing y direction)
 * ItemBehavior             (Fails client, When adding behavior to client body, server body is missing behavior)
 *                          (Adding on either server or client doesn't matter because either on fails client or one cannot be detected)
 * CharacterMotor.Jump      (Fails client, Does nothing for clients)
 * Health.TryForce          (Missing y speed, Does affect client)
 * Custom prePosition       (Janky, also disableAirControlUntilCollision never actually worked for client from server)
 * Previous Position        (Client's previous Position and current position are the same on the server. Probably also janky)
 * Rigidbody                (Fails client, ApplyForce doesn't do anything to either host or client)
 * AddBuff / SetBuffCount   (Causes an error)
 * OnBuffFirstStackGained   (doesn't run on client)
 * ClientRpc                (Doesn't do anything?)
 * 
 *      Sucess
 * 
 * OnBuffFinalStackLost     (Runs on client after timed buff ends)
 */