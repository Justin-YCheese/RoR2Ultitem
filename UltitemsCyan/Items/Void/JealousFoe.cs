using RoR2;
using RoR2.Projectile;
using System.ComponentModel;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier1;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class JealousFoe : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        public const float chancePerStack = 5f;
        public readonly GameObject EyeballProjectile = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/DeathProjectile/DeathProjectile.prefab").WaitForCompletion();

        public override void Init()
        {
            item = CreateItemDef(
                "JEALOUSFOE",
                "JealousFoe",
                "Chance of On-Kill effects upon grabbing pickups. <style=cIsVoid>Corrupts all Toy Robots</style>.",
                "<style=cIsDamage>5%</style> <style=cStack>(+5% per stack)</style> chance of triggering <style=cIsDamage>On-Kill</style> effects when <style=cIsDamage>grabbing pickups</style>. <style=cIsVoid>Corrupts all Toy Robots</style>.",
                "Look at it Jubilat. It just jubilant like jello jelly.",
                ItemTier.VoidTier1,
                UltAssets.JubilantFoeSprite,
                UltAssets.JubilantFoePrefab,
                [ItemTag.Utility, ItemTag.OnKillEffect],
                ToyRobot.item
            );
        }

        protected override void Hooks()
        {
            On.RoR2.HealthPickup.OnTriggerStay += HealthPickup_OnTriggerStay;
            On.RoR2.AmmoPickup.OnTriggerStay += AmmoPickup_OnTriggerStay;
            On.RoR2.BuffPickup.OnTriggerStay += BuffPickup_OnTriggerStay;
            On.RoR2.MoneyPickup.OnTriggerStay += MoneyPickup_OnTriggerStay;
        }

        private void HealthPickup_OnTriggerStay(On.RoR2.HealthPickup.orig_OnTriggerStay orig, HealthPickup self, UnityEngine.Collider other)
        {
            orig(self, other);
            GotPickup(other);
        }

        private void AmmoPickup_OnTriggerStay(On.RoR2.AmmoPickup.orig_OnTriggerStay orig, AmmoPickup self, UnityEngine.Collider other)
        {
            orig(self, other);
            GotPickup(other);
        }

        private void BuffPickup_OnTriggerStay(On.RoR2.BuffPickup.orig_OnTriggerStay orig, BuffPickup self, UnityEngine.Collider other)
        {
            orig(self, other);
            GotPickup(other);
        }

        private void MoneyPickup_OnTriggerStay(On.RoR2.MoneyPickup.orig_OnTriggerStay orig, MoneyPickup self, UnityEngine.Collider other)
        {
            orig(self, other);
            GotPickup(other);
        }

        private void GotPickup(UnityEngine.Collider other)
        {
            Log.Debug("Foe Got Pickup?");
            CharacterBody body = other.GetComponent<CharacterBody>();
            if (body && body.inventory)
            {
                int grabCount = body.inventory.GetItemCount(item);
                if (grabCount > 0 && NetworkServer.active && Util.CheckRoll(chancePerStack * grabCount, body.master.luck))
                {
                    // Spawn on kill effect
                    Log.Debug("got Chance");
                    GameObject eyeball = Object.Instantiate(EyeballProjectile, body.footPosition, Quaternion.identity);
                    eyeball.transform.localScale = new Vector3(0f, 0f, 0f);
                    eyeball.GetComponent<DeathProjectile>().baseDuration = grabCount;
                    //eyeball.GetComponent<DeathProjectile>().removalTime = grabCount;
                    Object.Destroy(eyeball.GetComponent<DestroyOnTimer>());
                    Object.Destroy(eyeball.GetComponent<DeathProjectile>());
                    Log.Debug("removing?");
                    Object.Destroy(eyeball.GetComponent<ApplyTorqueOnStart>());
                    Object.Destroy(eyeball.GetComponent<ProjectileDeployToOwner>());
                    Object.Destroy(eyeball.GetComponent<Deployable>());
                    Object.Destroy(eyeball.GetComponent<ProjectileStickOnImpact>());
                    Object.Destroy(eyeball.GetComponent<ProjectileController>());

                    Log.Debug("removing 2 ?");
                    eyeball.transform.position = body.footPosition;
                    HealthComponent health = eyeball.GetComponent<HealthComponent>();

                    DamageInfo damageInfo = new()
                    {
                        attacker = body.gameObject,
                        crit = body.RollCrit(),
                        damage = body.baseDamage,
                        position = body.footPosition,
                        procCoefficient = 0f,
                        damageType = DamageType.Generic,
                        damageColorIndex = DamageColorIndex.Item
                    };
                    DamageReport damageReport = new DamageReport(damageInfo, health, damageInfo.damage, health.combinedHealth);
                    //GlobalEventManager.instance.OnCharacterDeath(val3);
                    GlobalEventManager.instance.OnCharacterDeath(damageReport);
                    //*/
                }
            }
        }

        public class DeathProjectile : MonoBehaviour
	{
		// Token: 0x060042DD RID: 17117 RVA: 0x00115938 File Offset: 0x00113B38
		private void Awake()
		{
			//this.projectileStickOnImpactController = base.GetComponent<ProjectileStickOnImpact>();
			this.projectileController = base.GetComponent<ProjectileController>();
			this.projectileDamage = base.GetComponent<ProjectileDamage>();
			this.healthComponent = base.GetComponent<HealthComponent>();
			this.duration = this.baseDuration;
			this.fixedAge = 0f;
		}

		// Token: 0x060042DE RID: 17118 RVA: 0x0011598C File Offset: 0x00113B8C
		private void FixedUpdate()
		{
			this.fixedAge += Time.deltaTime;
			if (this.duration > 0f)
			{
				if (this.fixedAge >= 1f)
				{
					if (this.projectileStickOnImpactController.stuck)
					{
						if (this.projectileController.owner)
						{
							this.RotateDoll(UnityEngine.Random.Range(90f, 180f));
							this.SpawnTickEffect();
							if (NetworkServer.active)
							{
								DamageInfo damageInfo = new DamageInfo
								{
									attacker = this.projectileController.owner,
									crit = this.projectileDamage.crit,
									damage = this.projectileDamage.damage,
									position = base.transform.position,
									procCoefficient = this.projectileController.procCoefficient,
									damageType = this.projectileDamage.damageType,
									damageColorIndex = this.projectileDamage.damageColorIndex
								};
								HealthComponent victim = this.healthComponent;
								DamageReport damageReport = new DamageReport(damageInfo, victim, damageInfo.damage, this.healthComponent.combinedHealth);
								GlobalEventManager.instance.OnCharacterDeath(damageReport);
							}
						}
						this.duration -= 1f;
					}
					this.fixedAge = 0f;
					return;
				}
			}
			else
			{
				if (!this.doneWithRemovalEvents)
				{
					this.doneWithRemovalEvents = true;
					this.rotateObject.GetComponent<ObjectScaleCurve>().enabled = true;
				}
				if (this.fixedAge >= this.removalTime)
				{
					Util.PlaySound(this.exitSoundString, base.gameObject);
					this.shouldStopSound = false;
					UnityEngine.Object.Destroy(base.gameObject);
				}
			}
		}

		// Token: 0x060042DF RID: 17119 RVA: 0x00115B2D File Offset: 0x00113D2D
		private void OnDisable()
		{
			if (this.shouldStopSound)
			{
				Util.PlaySound(this.exitSoundString, base.gameObject);
				this.shouldStopSound = false;
			}
		}

		// Token: 0x060042E0 RID: 17120 RVA: 0x00115B50 File Offset: 0x00113D50
		public void SpawnTickEffect()
		{
			EffectData effectData = new EffectData
			{
				origin = base.transform.position,
				rotation = Quaternion.identity
			};
			EffectManager.SpawnEffect(this.OnKillTickEffect, effectData, false);
		}

		// Token: 0x060042E1 RID: 17121 RVA: 0x00115B8C File Offset: 0x00113D8C
		public void PlayStickSoundLoop()
		{
			Util.PlaySound(this.activeSoundLoopString, base.gameObject);
			this.shouldStopSound = true;
		}

		// Token: 0x060042E2 RID: 17122 RVA: 0x00115BA7 File Offset: 0x00113DA7
		public void RotateDoll(float rotationAmount)
		{
			this.rotateObject.transform.Rotate(new Vector3(0f, 0f, rotationAmount));
		}

		// Token: 0x040040D1 RID: 16593
		private ProjectileStickOnImpact projectileStickOnImpactController;

		// Token: 0x040040D2 RID: 16594
		private ProjectileController projectileController;

		// Token: 0x040040D3 RID: 16595
		private ProjectileDamage projectileDamage;

		// Token: 0x040040D4 RID: 16596
		private HealthComponent healthComponent;

		// Token: 0x040040D5 RID: 16597
		public GameObject OnKillTickEffect;

		// Token: 0x040040D6 RID: 16598
		public TeamIndex teamIndex;

		// Token: 0x040040D7 RID: 16599
		public string activeSoundLoopString;

		// Token: 0x040040D8 RID: 16600
		public string exitSoundString;

		// Token: 0x040040D9 RID: 16601
		private float duration;

		// Token: 0x040040DA RID: 16602
		private float fixedAge;

		// Token: 0x040040DB RID: 16603
		public float baseDuration = 8f;

		// Token: 0x040040DC RID: 16604
		public float radius = 500f;

		// Token: 0x040040DD RID: 16605
		public GameObject rotateObject;

		// Token: 0x040040DE RID: 16606
		private bool doneWithRemovalEvents;

		// Token: 0x040040DF RID: 16607
		public float removalTime = 1f;

		// Token: 0x040040E0 RID: 16608
		private bool shouldStopSound;
	}
    }
}