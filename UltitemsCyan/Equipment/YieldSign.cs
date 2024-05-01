using HG;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UIElements;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class YieldSign : EquipmentBase
    {
        // Inflict Slowdown on self?
        public static EquipmentDef equipment;

        public const float cooldown = 6f;
        //public const float subCooldown = .1f;

        private const float speedMultiplier = 3f;
        private const float HorizontalMultiplier = 1.5f;
        private const float yieldDamage = 4f; //200%
        private const float force = 2500f;
        private const int radius = 8;

        private static GameObject willOWisp = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/ExplodeOnDeath/WilloWispDelay.prefab").WaitForCompletion();
        private static GameObject explosionGolem = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Golem/ExplosionGolem.prefab").WaitForCompletion();
        //AncientWispPillar ?

        public override void Init()
        {
            equipment = CreateItemDef(
                "YIELDSIGN",
                "Yield Sign",
                "Alternate between multiplying speed and canceling it. Hit nearby enemies each time.",
                "Alternate between multipling speed by 300%, or setting it to zero. Damage nearby enemies for 400% damage.",
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
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                CharacterBody activator = self.characterBody;
                activator.inventory.SetEquipmentIndex(YieldSignStop.equipment.equipmentIndex);
                //self.subcooldownTimer = subCooldown;
                return YieldActivated(activator, speedMultiplier);
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        public static bool YieldActivated(CharacterBody activator, float multiplier)
        {
            if (activator.characterMotor)
            {
                //Log.Debug("- First Vector: <" + activator.characterMotor.velocity.y + "," + activator.characterMotor.velocity.x + "," + activator.characterMotor.velocity.z + ">");
                //Log.Debug("- Motion: <" + activator.characterMotor.rootMotion.y + "," + activator.characterMotor.rootMotion.x + "," + activator.characterMotor.rootMotion.z + ">");
                activator.characterMotor.velocity *= multiplier;
                activator.characterMotor.velocity.x *= HorizontalMultiplier;
                activator.characterMotor.velocity.z *= HorizontalMultiplier;
                //Log.Debug(". New Vector: <" + activator.characterMotor.velocity.y + "," + activator.characterMotor.velocity.x + "," + activator.characterMotor.velocity.z + ">");

                //Log.Debug("Forced? " + activator.characterMotor.isAirControlForced);
                //activator.characterMotor.isAirControlForced = false;

                Vector3 position = activator.corePosition;

                GameObject explostionObject = UnityEngine.Object.Instantiate(willOWisp, position, Quaternion.identity);
                DelayBlast blast = explostionObject.GetComponent<DelayBlast>();
                blast.position = position;
                blast.attacker = activator.gameObject;
                blast.inflictor = activator.gameObject;
                blast.baseDamage = yieldDamage * activator.damage;
                blast.baseForce = force;
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
                    teamIndexInternal = (int)activator.teamComponent.teamIndex,
                    defaultTeam = TeamIndex.None,
                    teamIndex = activator.teamComponent.teamIndex,
                    NetworkteamIndexInternal = (int)activator.teamComponent.teamIndex
                };

                Util.PlaySound("Play_bellBody_impact", activator.gameObject);
                Util.PlaySound("Play_bellBody_attackLand", activator.gameObject);

                //Log.Debug(". new Vector: <" + activator.characterMotor.rootMotion.y + "," + activator.characterMotor.rootMotion.x + "," + activator.characterMotor.rootMotion.z + ">");
                return true;
            }
            //Util.PlaySound("Play_item_proc_iceRingSpear", self.gameObject);
            return false;
        }
    }
}