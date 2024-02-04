using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class SuesMandiblesConsumed : ItemBase
    {
        public static ItemDef item;
        private const float invincibilityDuration = 16f;

        private void Tokens()
        {
            string tokenPrefix = "SUESMANDIBLESCONSUMED";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Sue's Mandibles (Consumed)");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Resting in pieces");
            LanguageAPI.Add(tokenPrefix + "_DESC", "DESCRIPTION Resting in pieces");
            LanguageAPI.Add(tokenPrefix + "_LORE", "I don't know sue");

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
            itd.tier = ItemTier.NoTier;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.SuesMandiblesConsumedSprite;
            item.pickupModelPrefab = Ultitems.Assets.SuesMandiblesConsumedPrefab;

            item.canRemove = false;
            item.hidden = false;

            item.tags = [ItemTag.Utility, ItemTag.LowHealth, ItemTag.AIBlacklist];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            //Hooks();

            // Log.Info("Faulty Bulb Initialized");
            GetItemDef = item;
            Log.Warning("Initialized: " + item.name);
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