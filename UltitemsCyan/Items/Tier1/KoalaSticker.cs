using R2API;
using RoR2;
using UltitemsCyan.Items.Untiered;
using UnityEngine;

namespace UltitemsCyan.Items.Tier1
{
    // Currently triggers after Sue's Mandibles
    // TODO: check if Item classes needs to be public
    public class KoalaSticker : ItemBase
    {
        public static ItemDef item;
        private const float hyperbolicPercent = 12f;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "KOALASTICKER";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Koala Sticker");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Reduce the maxinum damage you can take");
            LanguageAPI.Add(tokenPrefix + "_DESC", "You only take a maxinum of 90% (-12% per stack) of your health from a hit, mininum of 1.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Last Stand");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier1;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            // Log.Info("Faulty Bulb Initialized");
            GetItemDef = item;
            Log.Warning("Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            float initialHealth = self.health;
            float initialShield = self.shield;
            float initialBarrier = self.barrier;
            orig(self, damageInfo);
            CharacterBody victim = self.GetComponent<CharacterBody>();
            // If dead after damage
            if (victim && victim.inventory && self && self.alive)
            {
                int grabCount = victim.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Warning("K Initial Health: " + initialHealth + " Shield: " + initialShield + " Barrier: " + initialBarrier);
                    Log.Debug("FullHealth: " + self.fullHealth + " fullShield: " + self.fullShield + " fullBarrier: " + self.fullBarrier + " fullCombined: " + self.fullCombinedHealth);
                    float percent = 1 / ((hyperbolicPercent / 100 * grabCount) + 1);
                    float maxDamage = self.fullCombinedHealth * percent;
                    float damage = initialHealth - self.health;
                    Log.Debug("Damage: " + damage);
                    if (damage > maxDamage)
                    {
                        if (maxDamage <= initialBarrier)
                        {
                            self.barrier = initialBarrier - maxDamage;
                            self.shield = self.fullShield;
                            self.health = self.fullHealth;
                        }
                        else if (maxDamage <= initialBarrier + initialShield)
                        {
                            self.shield = initialBarrier + initialShield - maxDamage;
                            self.health = self.fullHealth;
                        }
                        else
                        {
                            self.health = initialHealth - maxDamage;
                        }
                        // Floor Damage
                        Log.Debug("MaxDamage: " + maxDamage + " NewHealth: " + self.health + " | " + self.shield + " | " + self.barrier);
                    }
                }
            }
            //Log.Debug("Bye Sue");
        }
    }
    /*/
        public void RespawnExtraLife()
		{
			this.inventory.GiveItem(RoR2Content.Items.ExtraLifeConsumed, 1);
			CharacterMasterNotificationQueue.SendTransformNotification(this, RoR2Content.Items.ExtraLife.itemIndex, RoR2Content.Items.ExtraLifeConsumed.itemIndex, CharacterMasterNotificationQueue.TransformationType.Default);
			Vector3 vector = this.deathFootPosition;
			if (this.killedByUnsafeArea)
			{
				vector = (TeleportHelper.FindSafeTeleportDestination(this.deathFootPosition, this.bodyPrefab.GetComponent<CharacterBody>(), RoR2Application.rng) ?? this.deathFootPosition);
			}
			this.Respawn(vector, Quaternion.Euler(0f, UnityEngine.Random.Range(0f, 360f), 0f));
			this.GetBody().AddTimedBuff(RoR2Content.Buffs.Immune, 3f);
			GameObject gameObject = LegacyResourcesAPI.Load<GameObject>("Prefabs/Effects/HippoRezEffect");
			if (this.bodyInstanceObject)
			{
				foreach (EntityStateMachine entityStateMachine in this.bodyInstanceObject.GetComponents<EntityStateMachine>())
				{
					entityStateMachine.initialStateType = entityStateMachine.mainStateType;
				}
				if (gameObject)
				{
					EffectManager.SpawnEffect(gameObject, new EffectData
					{
						origin = vector,
						rotation = this.bodyInstanceObject.transform.rotation
					}, true);
				}
			}
		}
    //*/
}