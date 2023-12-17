using System;
using System.Collections.Generic;
using System.Text;
using R2API;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltitemsCyan.Items
{
    
    // TODO: check if Item classes needs to be public
    public class ItemBase
    {
        private static ItemDef item;

        public void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // TODO: turn into tokens
            // strings
            item.name = "EXAMPLE_ITEM_NAME";
            item.nameToken = "EXAMPLE_ITEM_NAME";
            item.pickupToken = "EXAMPLE_ITEM_PICKUP";
            item.descriptionToken = "EXAMPLE_ITEM_DESC";
            item.loreToken = "EXAMPLE_ITEM_LORE";

            // tier
            ItemTierDef itd = new() { tier = ItemTier.Tier1 };
            item._itemTierDef = itd;

            item.pickupIconSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            item.pickupModelPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Any];

            // TODO: Turn tokens into strings
            // AddTokens();

            var displayRules = new ItemDisplayRuleDict(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            On.RoR2.CharacterBody.HandleOnKillEffectsServer += CharacterBody_HandleOnKillEffectsServer;
        }

        private void CharacterBody_HandleOnKillEffectsServer(On.RoR2.CharacterBody.orig_HandleOnKillEffectsServer orig, CharacterBody self, DamageReport damageReport)
        {
            // If a character was killed by the world, we shouldn't do anything.
            if (!damageReport.attacker || !damageReport.attackerBody)
            {
                return;
            }

            var attackerCharacterBody = damageReport.attackerBody;

            // We need an inventory to do check for our item
            if (attackerCharacterBody.inventory)
            {
                // Store the amount of our item we have
                var garbCount = attackerCharacterBody.inventory.GetItemCount(item.itemIndex);
                if (garbCount > 0)
                {
                    // Since we passed all checks, we now give our attacker the cloaked buff.
                    // Note how we are scaling the buff duration depending on the number of the custom item in our inventory.
                    for (int i = 0; i < garbCount; i++)
                        attackerCharacterBody.AddBuff(RoR2Content.Buffs.BanditSkull);
                }
            }
        }

    }
}
