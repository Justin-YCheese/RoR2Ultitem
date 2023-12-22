using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class ViralSmog : ItemBase
    {
        public static ItemDef item;
        private void Tokens()
        {
            string tokenPrefix = "VIRALSMOG";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Viral Smog");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Increase speed per status effect.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>30%</style> <style=cStack>(+30% per stack)</style> per <style=cIsDamage>unique status</style> you have.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Illness");

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
            itd.tier = ItemTier.Tier3;
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

            //Log.Info("Test Item Initialized");
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            
        }
    }
}