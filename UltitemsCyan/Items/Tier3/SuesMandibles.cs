﻿using R2API;
using RoR2;
using UltitemsCyan.Items.Untiered;
using UnityEngine;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class SuesMandibles : ItemBase
    {
        public static ItemDef item;
        private const float effectDuration = 24f;

        private void Tokens()
        {
            string tokenPrefix = "SUESMANDIBLES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Sue's Mandibles");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Endure a killing blow then gain invulnerability and disable healing. Consumed on use.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsUtility>Upon a killing blow</style>, this item will be <style=cIsUtility>consumed</style> and you'll <style=cIsHealing>live on 1 health</style> with <style=cIsHealing>24 seconds</style> of <style=cIsHealing>invulnerability</style> and <style=cIsHealth>disabled healing</style>.");
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
            itd.tier = ItemTier.Tier3;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.SuesMandiblesSprite;
            item.pickupModelPrefab = Ultitems.Assets.SuesMandiblesPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility, ItemTag.LowHealth];

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
            orig(self, damageInfo);
            CharacterBody victim = self.GetComponent<CharacterBody>();
            // If dead after damage
            if (victim && victim.inventory && self && !self.alive && self.health <= 0)
            {
                int grabCount = victim.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Warning(" ! ! ! Killing Blow ! ! ! ");
                    Log.Debug("S Combined: " + self.combinedHealth + " FullCombined: " + self.fullCombinedHealth + " Damage: " + damageInfo.damage + " Alive? " + self.alive);
                    
                    // Regain one health
                    self.health = 1;
                    
                    // Trade Items
                    victim.inventory.RemoveItem(item);
                    victim.inventory.GiveItem(SuesMandiblesConsumed.item);

                    // Give Effects
                    //self.TriggerOneShotProtection();
                    victim.AddTimedBuffAuthority(RoR2Content.Buffs.Immune.buffIndex, effectDuration);
                    victim.AddTimedBuffAuthority(RoR2Content.Buffs.HealingDisabled.buffIndex, effectDuration); // Adds synergy with Ben's Raincoat and Genisis Loop

                    // Play Sounds
                    Util.PlaySound("Play_item_proc_ghostOnKill", victim.gameObject);
                    Util.PlaySound("Play_item_proc_ghostOnKill", victim.gameObject);
                    Util.PlaySound("Play_item_proc_phasing", victim.gameObject);
                    Util.PlaySound("Play_item_proc_phasing", victim.gameObject);
                    Util.PlaySound("Play_elite_haunt_ghost_convert", victim.gameObject);
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