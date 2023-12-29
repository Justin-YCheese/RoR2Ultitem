using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class RustedVaultConsumed : ItemBase
    {
        public static ItemDef item;
        private const int quantityInVault = 16;
        private void Tokens()
        {
            string tokenPrefix = "RUSTEDVAULTCONSUMED";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Rusted Vault (Broken)");
            LanguageAPI.Add(tokenPrefix + "_PICK", "It can't protect anything anymore...");
            LanguageAPI.Add(tokenPrefix + "_DESC", "DESCRIPTION It can't protect anything anymore...");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Rusted Rusted Rusted");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // Add text for item
            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.NoTier;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist];

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // No Item Functionality
            // Hooks();

            //Log.Info("Test Item Initialized");
            Log.Warning(" Initialized: " + item.name);
        }
    }
}