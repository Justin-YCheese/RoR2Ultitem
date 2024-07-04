using BepInEx.Configuration;
using R2API;
using RoR2;
using RoR2.ExpansionManagement;
using System.Linq;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public abstract class ItemBase
    {
        private readonly ExpansionDef Sovt = ExpansionCatalog.expansionDefs.FirstOrDefault(expansion => expansion.nameToken == "DLC1_NAME");
        public abstract void Init(ConfigFile configs);
        public ItemDef CreateItemDef(
            string tokenPrefix,
            string name,
            string pick,
            string desc,
            string lore,
            ItemTier tier,
            Sprite sprite,
            GameObject prefab,
            ItemTag[] tags,
            ItemDef transformItem = null)
        {
            ItemDef item = ScriptableObject.CreateInstance<ItemDef>();

            LanguageAPI.Add(tokenPrefix + "_NAME", name);
            LanguageAPI.Add(tokenPrefix + "_PICK", pick);
            LanguageAPI.Add(tokenPrefix + "_DESC", desc);
            LanguageAPI.Add(tokenPrefix + "_LORE", lore);

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";


            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = tier;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.canRemove = tier != ItemTier.NoTier;
            item.hidden = false;

            item.pickupIconSprite = sprite;
            item.pickupModelPrefab = prefab;

            item.tags = tags;

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            if (transformItem)
            {
                //Log.Warning("Transform from + " + transformItem.name);
                GetTransformItem = transformItem;
                item.requiredExpansion = Sovt;
            }
            //Log.Warning(" Initialized: " + item.name);
            return item;
        }

        public bool CheckItemEnabledConfig(string name, ConfigFile configs)
        {
            return configs.Bind(
                "Enable Items",
                "Enable " + name + "?",
                true
            ).Value;
        }

        protected abstract void Hooks();

        public ItemDef GetItemDef { get; set; }
        public EquipmentDef GetEquipmentDef { get; set; }
        public ItemDef GetTransformItem { get; set; }
    }
}