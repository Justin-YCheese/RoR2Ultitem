using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class OverclockedGPU : ItemBase
    {
        public static ItemDef item;
        private void Tokens()
        {
            string tokenPrefix = "OVERCLOCKEDGPU";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Overclocked GPU");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Increase attack speed on kill.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Killing an enemy increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>8%</style>. Maximum cap of <style=isDamage>32%</style> <style=cStack>(+32% per stack)</style> <style=cIsDamage>attack speed</style>. Lose effect upon getting hit.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "GPU GPU");

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
            itd.tier = ItemTier.Tier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Damage, ItemTag.OnKillEffect];

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