using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class InhabitedCoffinConsumed : ItemBase
    {
        public static ItemDef item;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "INHABITEDCOFFINCONSUMED";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Inhabited Coffin (Vaccant)");
            LanguageAPI.Add(tokenPrefix + "_PICK", "It has been let loose...");
            LanguageAPI.Add(tokenPrefix + "_DESC", "DESCRIPTION It has been let loose...");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Watch Out!");

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

            item.pickupIconSprite = Ultitems.Assets.VacantCoffinConsumed;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = false;
            item.hidden = false;

            item.tags = [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist];

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // No Item Functionality
            // Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            Log.Warning(" Initialized: " + item.name);
        }
    }
}