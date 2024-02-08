using R2API;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class PowerChips : ItemBase
    {
        public static ItemDef item;
        private const float dontResetFraction = 0.50f;

        private void Tokens()
        {
            string tokenPrefix = "POWERCHIPS";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Power Chips");
            LanguageAPI.Add(tokenPrefix + "_PICK", "<style=cDeath></style>");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsUtility></style>");
            LanguageAPI.Add(tokenPrefix + "_LORE", "");

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
            itd.tier = ItemTier.Lunar;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.PowerChipsSprite;
            item.pickupModelPrefab = Ultitems.Assets.PowerChipsPrefab;

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
            On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop;
        }

        private void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)
        {
            //self.tier1Chance = .8f for regular chest
            Log.Warning("Tier 1 Chance: " + self.tier1Chance);
            //self.dropTransform = Transform.
            Log.Debug("Player Controller: " + self.playerControllerId);

            //Log.Debug("Rolled Pickup: " + self.HasRolledPickup());
            //Log.Debug("" + self.)
        }
    }
}