using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class SilverThreadConsumed : ItemBase
    {
        public static ItemDef item;

        private void Tokens()
        {
            string tokenPrefix = "SILVERTHREADCONSUMED";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Silver Thread (Snapped)");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Proof of death");
            LanguageAPI.Add(tokenPrefix + "_DESC", "DESCRIPTION Proof of death");
            LanguageAPI.Add(tokenPrefix + "_LORE", "This is a garbage death zone. How did you get here?");

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

            item.pickupIconSprite = Ultitems.Assets.InhabitedCoffinConsumedSprite;
            item.pickupModelPrefab = Ultitems.Assets.SilverThreadPrefab;

            item.canRemove = false;
            item.hidden = false;

            item.tags = [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist];

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
}