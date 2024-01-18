using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace UltitemsCyan.Items.Tier2
{

    // Can Buff by making buff non stackable, so always give power equal to number of candles held
    public class BirthdayCandles : ItemBase
    {
        public static ItemDef item;
        private const float birthdayDuration = 300f;

        // For Birthday Buff
        public const float birthdayBuffMultiplier = 32f;

        // Function
        public bool inPickupAlready = false;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "BIRTHDAYCANDLES";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Birthday Candles");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Temporarily deal extra damage after pickup and at the start of each stage.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Increase damage by <style=cIsDamage>32%</style> <style=cStack>(+32% per stack)</style> for <style=cIsUtility>5 minutes</style> after pickup and after the start of each stage.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "I don't know what to get you for your birthday...");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // tokens

            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.BirthdayCandleSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Damage, ItemTag.OnStageBeginEffect];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Birthday Candles Initialized");

            GetItemDef = item;
            Log.Warning("Initialized: " + item.name);
        }

        protected void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        // Start of each level (or when monsters spawn in)
        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            //Log.Debug("Birthday Candles On Body Start Global");
            if (self && self.inventory)
            {
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                // If grabcount is zero exits loop
                for (int i = 0; i < grabCount; i++)
                {
                    ApplyBirthday(self);
                }
            }
        }

        protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            orig(self, itemIndex, count);

            /*/
            ItemDef defineItem = ItemCatalog.GetItemDef(itemIndex);
            Sprite itemSprite = defineItem.pickupIconSprite;
            GameObject itemPrefab = defineItem.pickupModelPrefab;
            Texture itemTexture = defineItem.pickupIconTexture;//*/

            //Log.Debug("Sprite::: height - " + itemSprite.rect.m_Height + " width - " + itemSprite.rect.width + " bounds - " + itemSprite.bounds.ToString() + " border - " + itemSprite.border.ToString());
            //Log.Debug("itemTexture::: name - " + itemTexture.name + " string - " + itemTexture.ToString());

            // If item picked up is Birthday Candles and there is a character Body
            if (self && itemIndex == item.itemIndex)
            {
                Log.Warning("Give Birthday Candles");
                // Log.Debug("Count Birthday Candles on Pickup: " + count);

                CharacterBody player = CharacterBody.readOnlyInstancesList.ToList().Find((body2) => body2.inventory == self);
                // If you don't have any Rotten Bones
                if (player.inventory.GetItemCount(Void.RottenBones.item) > 0)
                {
                    ApplyBirthday(player);
                }
            }
        }

        protected void ApplyBirthday(CharacterBody recipient)
        {
            Util.PlaySound("Play_item_proc_igniteOnKill", recipient.gameObject);
            recipient.AddTimedBuffAuthority(Buffs.BirthdayBuff.buff.buffIndex, birthdayDuration);
        }
    }
}