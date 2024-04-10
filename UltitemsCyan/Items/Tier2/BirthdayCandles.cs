using R2API;
using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UltitemsCyan.Buffs;


namespace UltitemsCyan.Items.Tier2
{

    // Can Buff by making buff non stackable, so always give power equal to number of candles held
    public class BirthdayCandles : ItemBase
    {
        public static ItemDef item;
        private const float birthdayDuration = 300f;

        // For Birthday Buff
        public const float birthdayBuffMultiplier = 32f;

        public override void Init()
        {
            item = CreateItemDef(
                "BIRTHDAYCANDLES",
                "Birthday Candles",
                "Temporarily deal extra damage after pickup and at the start of each stage.",
                "Increase damage by <style=cIsDamage>32%</style> <style=cStack>(+32% per stack)</style> for<style=cIsUtility>5 minutes</style>after pickup and after the start of each stage.",
                "I don't know what to get you for your birthday...",
                ItemTier.Tier2,
                Ultitems.Assets.BirthdayCandleSprite,
                Ultitems.Assets.BirthdayCandlePrefab,
                [ItemTag.Damage, ItemTag.OnStageBeginEffect]
            );
        }


        protected override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged; // Remove buff if no birthday candles
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal; // Start of stage give buff
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int; // Upon pickup give buff
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory && self.inventory.GetItemCount(item) < 1)
            {
                self.SetBuffCount(BirthdayBuff.buff.buffIndex, 0);
            }
        }

        // Start of each level (or when monsters spawn in)
        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (NetworkServer.active && self && self.inventory)
            {
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Debug("Birthday Candles On Body Start Global for " + self.GetUserName() + " | Candles: " + grabCount);
                    ApplyBirthday(self, grabCount, grabCount);
                }
            }
        }

        protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            Log.Debug("Into orig Birthday Candles");
            orig(self, itemIndex, count);
            Log.Debug("Out orig Birthday Candles");
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

                CharacterBody player = CharacterBody.readOnlyInstancesList.ToList().Find((body) => body.inventory == self);
                // If you don't have any Rotten Bones
                if (player && player.inventory.GetItemCount(Void.RottenBones.item) <= 0)
                {
                    ApplyBirthday(player, count, self.GetItemCount(item.itemIndex));
                }
            }
        }

        protected void ApplyBirthday(CharacterBody recipient, int count, int max)
        {
            for (int i = 0; i < count; i++)
            {
                recipient.AddTimedBuff(BirthdayBuff.buff, birthdayDuration, max);
            }
            Util.PlaySound("Play_item_proc_igniteOnKill", recipient.gameObject);
        }
    }
}