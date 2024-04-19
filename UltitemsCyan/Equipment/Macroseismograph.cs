using RoR2;
using UltitemsCyan.Items.Untiered;
using System.Collections.Generic;
using HG;
using System;
using UnityEngine;
using UnityEngine.UIElements;
using static RoR2.BlastAttack;
using UnityEngine.Networking;
using UnityEngine.AddressableAssets;
using static RoR2.BulletAttack;
using Rewired;
using static Newtonsoft.Json.Converters.DiscriminatedUnionConverter;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class Macroseismograph : EquipmentBase
    {
        public static EquipmentDef equipment;

        private const float cooldown = 300f;

        private const int radius = 180;
        private const float delay = 1.2f;
        private const float force = 7400;
        private const float earthquakeDamage = 100000;

        private static GameObject willOWisp = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExplodeOnDeath/WilloWispDelay.prefab").WaitForCompletion();

        //private static readonly GameObject warningEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Meteor/MeteorStrikePredictionEffect.prefab").WaitForCompletion();
        private static readonly GameObject warningEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/GenericDelayBlast.prefab").WaitForCompletion();
        private static readonly GameObject explostionEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/MajorAndMinorConstruct/OmniExplosionVFXMajorConstruct.prefab").WaitForCompletion();
        
        //RoR2/DLC1/VendingMachine/matVendingMachineRadiusWarning.mat
        //RoR2/Base/Meteor/matMeteorStrikeImpactIndicator.mat
        //RoR2/Base/Meteor/mdlMeteor.fbx
        //RoR2/Base/Meteor/DisplayMeteor.prefab
        //RoR2/Base/Meteor/DisplayMeteorFollower.prefab
        //RoR2/Base/Meteor/mdlMeteor.fbx
        //RoR2/Base/Meteor/MeteorStrikeImpact.prefab

        //RoR2/Base/Meteor/MeteorStrikePredictionEffect.prefab

        public override void Init()
        {
            equipment = CreateItemDef(
                "MACROSEISMOGRAPH",
                "Macroseismograph",
                "While on the ground, summons a tremendious power...  But forgo all equipments",
                "Deal 100000% damage to all enemies within 180m. But lose the ability to use equipments.",
                "This seismograph must be broken, it always reads at least 10. But there's no way that's true, I don't feel anything, yet I always try to avoid looking at it. It seems ominous, almost alive...",
                cooldown,
                true,
                true,
                Ultitems.Assets.MacroseismographSprite,
                Ultitems.Assets.MacroseismographPrefab
            );
        }

        protected override void Hooks()
        {
            // Nuke
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }


        // Delete Existing instances of the item, and remove from drops
        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                CharacterBody activator = self.characterBody;
                // If activator and (on ground or is equipment drone)
                if (activator && ((activator.characterMotor && activator.characterMotor.isGrounded) || (self.gameObject && self.gameObject.name.Contains("EquipmentDrone"))))
                {
                    Log.Warning(self.gameObject.name + " Pulled the Trigger!");
                    //Log.Warning(" | / ! Pulled the Trigger ! | / ");

                    // Get position below player
                    Vector3 position = activator?.corePosition ?? self.transform.position;
                    position.y -= 5;

                    //EffectManager.SpawnEffect(warningEffect, new EffectData
                    //{
                    //    origin = position,
                    //    scale = radius
                    //}, true);

                    // Damage Blast
                    //BlastAttack impactDamage = new()

                    Xoroshiro128Plus rng = new(Run.instance.stageRng.nextUlong);

                    //float force = rng.RangeFloat(3000f, 20000f);

                    // < 10250

                    Log.Warning(" - - - - - Force? " + force);

                    // Damage
                    GameObject explostionObject = UnityEngine.Object.Instantiate(willOWisp, position, Quaternion.identity);
                    DelayBlast blast = explostionObject.GetComponent<DelayBlast>();
                    blast.position = position;
                    blast.attacker = activator.gameObject;
                    blast.inflictor = activator.gameObject;
                    blast.baseDamage = earthquakeDamage * activator.damage;
                    blast.baseForce = 1000f;
                    //blast.bonusForce = ;
                    blast.radius = radius;
                    blast.maxTimer = delay;
                    blast.falloffModel = BlastAttack.FalloffModel.Linear;
                    blast.damageColorIndex = DamageColorIndex.Fragile;
                    blast.damageType = DamageType.AOE | DamageType.BypassBlock;
                    blast.procCoefficient = 0f;
                    //blast.delayEffect
                    blast.teamFilter = new TeamFilter()
                    {
                        teamIndexInternal = (int)activator.teamComponent.teamIndex,
                        defaultTeam = TeamIndex.None,
                        teamIndex = activator.teamComponent.teamIndex,
                        NetworkteamIndexInternal = (int)activator.teamComponent.teamIndex
                    };//*/

                    // Force
                    GameObject forceObject = UnityEngine.Object.Instantiate(willOWisp, position, Quaternion.identity);
                    DelayBlast forceBlast = forceObject.GetComponent<DelayBlast>();
                    forceBlast.position = position;
                    forceBlast.attacker = activator.gameObject;
                    forceBlast.inflictor = activator.gameObject;
                    forceBlast.baseDamage = 0f;
                    forceBlast.baseForce = force;
                    forceBlast.bonusForce = Vector3.up * force;
                    forceBlast.radius = radius;
                    forceBlast.maxTimer = delay;
                    forceBlast.falloffModel = BlastAttack.FalloffModel.Linear;
                    forceBlast.damageColorIndex = DamageColorIndex.Fragile;
                    forceBlast.damageType = DamageType.AOE | DamageType.BypassBlock;
                    forceBlast.procCoefficient = 0f;
                    //blast.delayEffect
                    forceBlast.teamFilter = new TeamFilter()
                    {
                        teamIndexInternal = 0,
                        defaultTeam = TeamIndex.None,
                        teamIndex = TeamIndex.Neutral,
                        NetworkteamIndexInternal = 0
                    };//*/

                    Util.PlaySound("Play_voidDevastator_death_vortex_explode", activator.gameObject);
                    Util.PlaySound("Play_voidDevastator_death_VO", activator.gameObject);

                    // Send notification and consume equipment
                    CharacterMasterNotificationQueue.SendTransformNotification(
                        activator.master, activator.inventory.currentEquipmentIndex, MacroseismographConsumed.equipment.equipmentIndex, CharacterMasterNotificationQueue.TransformationType.Default);
                    activator.inventory.SetEquipmentIndex(MacroseismographConsumed.equipment.equipmentIndex);

                    return true;
                }
                return false;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }


        /*
        private bool FireMeteor()
        {
	        MeteorStormController component = UnityEngine.Object.Instantiate<GameObject>(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/MeteorStorm"), this.characterBody.corePosition, Quaternion.identity).GetComponent<MeteorStormController>();
	        component.owner = base.gameObject;
	        component.ownerDamage = this.characterBody.damage;
	        component.isCrit = Util.CheckRoll(this.characterBody.crit, this.characterBody.master);
	        NetworkServer.Spawn(component.gameObject);
	        return true;
        }
        */

        public class MacroseismoController : MonoBehaviour
        {
            private readonly BlastAttack impact;
            private readonly float impactTime;

            public MacroseismoController(float delay, BlastAttack impact)
            {
                Log.Debug(" ) ) ) M A C R O started");
                this.impact = impact;
                impactTime = Run.instance.time + delay;
                //gameObject = base.gameObject;
            }

            private void FixedUpdate()
            {
                if (!NetworkServer.active)
                {
                    return;
                }
                Log.Debug(impactTime + " < " + Run.instance.time);
                if (impactTime < Run.instance.time)
                {
                    //Explostion!
                    Log.Debug(" ) ) ) M A C R O explostion ! ! !");
                    _ = impact.Fire();
                    Destroy(gameObject);
                }
            }
        }
    }
}