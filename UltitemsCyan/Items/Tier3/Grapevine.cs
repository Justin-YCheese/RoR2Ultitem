﻿using R2API;
using RoR2;
using System;
using UltitemsCyan.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: Make better sound and visuals
    public class Grapevine : ItemBase
    {
        public static ItemDef item;
        private static GameObject GrapeOrbPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Tooth/HealPack.prefab").WaitForCompletion();
        private static GameObject GrapeEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/HealthOrbEffect.prefab").WaitForCompletion();

        private const float grapeDropChance = 20f;
        private const int maxGrapes = 20;

        public override void Init()
        {
            //loadPrefab();
            item = CreateItemDef(
                "GRAPEVINE",
                "Grapevine",
                "Kills drop orbs that block damage.",
                "20% (+20% on stack) chance on kill to grow a grape that blocks damage.",
                "If you close your eyes, you can pretend their eyeballs",
                ItemTier.Tier3,
                Ultitems.Assets.GrapevineSprite,
                Ultitems.Assets.GrapevinePrefab,
                [ItemTag.Damage]
            );
        }

        protected override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath; ;
        }

        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);
            if (self && damageReport.attacker && damageReport.attackerBody && damageReport.attackerBody.inventory)
            {
                CharacterBody killer = damageReport.attackerBody;
                CharacterBody victim = damageReport.victimBody;
                int grabCount = killer.inventory.GetItemCount(item);
                //int buffCount = killer.GetBuffCount(Buffs.OverclockedBuff.buff);
                if (grabCount > 0)
                {
                    if (Util.CheckRoll(grapeDropChance * grabCount, killer.master.luck))
                    {
                        Log.Warning("Dropping flea from " + victim.name);
                        //RoR2.BuffPickup.Instantiate(item);
                        //Util.PlaySound("Play_hermitCrab_idle_VO", victim.gameObject);
                        //Util.PlaySound("Play_hermitCrab_idle_VO", victim.gameObject);
                        SpawnOrb(victim.transform.position, victim.transform.rotation, TeamComponent.GetObjectTeam(killer.gameObject), grabCount);
                    }
                }
            }
        }

        public static void SpawnOrb(Vector3 position, Quaternion rotation, TeamIndex teamIndex, int itemCount)
        {
            GameObject orb = UnityEngine.Object.Instantiate(GrapeOrbPrefab);
            if (orb)
            {
                Log.Debug("Grape Orb is loaded");
            }

            orb.transform.position = position;
            orb.transform.rotation = rotation;
            orb.GetComponent<TeamFilter>().teamIndex = teamIndex;

            // * * Additions * * //
            VelocityRandomOnStart trajectory = orb.GetComponent<VelocityRandomOnStart>();
            trajectory.maxSpeed = 20;
            trajectory.minSpeed = 15;
            trajectory.directionMode = VelocityRandomOnStart.DirectionMode.Cone;
            trajectory.coneAngle = 1;


            HealthPickup healthComponent = orb.GetComponentInChildren<HealthPickup>();
            Log.Debug("health Component? " + healthComponent.alive);
            healthComponent.alive = false;

            //BuffPickup
            GrapePickup GrapeComponent = healthComponent.gameObject.AddComponent<GrapePickup>();
            GrapeComponent.baseObject = orb;
            GrapeComponent.teamFilter = orb.GetComponent<TeamFilter>();
            GrapeComponent.pickupEffect = GrapeEffect;

            orb.GetComponent<Rigidbody>().useGravity = true;
            orb.transform.localScale = Vector3.one * (1.5f + (itemCount / 3f));

            Log.Debug("Spawning orb at: " + orb.transform.position);
            NetworkServer.Spawn(orb);
        }
        //*/
    }
}
