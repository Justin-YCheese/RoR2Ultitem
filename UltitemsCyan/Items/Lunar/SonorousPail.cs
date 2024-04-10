using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;

namespace UltitemsCyan.Items.Lunar
{

    // TODO: check if Item classes needs to be public
    public class SonorousPail : ItemBase
    {
        public static ItemDef item;

        private const float attackPerWhite = 2.5f;
        private const float regenPerGreen = 0.05f;
        private const float speedPerRed = 10f;
        private const float critPerBoss = 10f;
        //private const float armourPerMisc = 2f;
        //private const float healthPerLunar = 5f;
        //private const float jumpPerLunar = 5f;
        private const float stackPercent = 20f;

        public bool inSonorousAlready = false;

        public static GameObject ShrineUseEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Common/VFX/ShrineUseEffect.prefab").WaitForCompletion();

        public override void Init()
        {
            item = CreateItemDef(
                "SONOROUSPAIL",
                "Sonorous Pail",
                "Gain stats for each item held... <style=cDeath>BUT picking up an item triggers a restack.</style>",
                "Gain <style=cIsDamage>2.5% attack</style> per common, <style=cIsHealing>0.05 regen</style> per <style=cIsHealing>uncommon</style>, <style=cIsUtility>10% speed</style> per legendary</style>, and <style=cIsDamage>10% crit</style> per <style=cIsDamage>boss</style> item <style=cStack>(+20% of each stat per stack)</style>. Trigger a <style=cDeath>restack</style> when picking up items.",
                "It's a tuning fork? no it's just a sand pail. The sand in the pail shifts with a sound which hums through it. Like a melody of waves, or to be less romantic, like a restless static.",
                ItemTier.Lunar,
                Ultitems.Assets.SandPailSprite,
                Ultitems.Assets.SandPailPrefab,
                [ItemTag.Utility]
            );
        }


        protected override void Hooks()
        {
            //On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        }

        /*/
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (!inSonorousAlready)
            {
                if (self && self.inventory && self.inventory.GetItemCount(item) > 0)
                {
                    inSonorousAlready = true;
                    Log.Warning("Spork the inventory");
                    //SporkRestackInventory(player.inventory, new Xoroshiro128Plus(Run.instance.stageRng.nextUlong));
                    //self.inventory.ShrineRestackInventory(new Xoroshiro128Plus(Run.instance.stageRng.nextUlong));
                    SporkRestackInventory(self.inventory, self.transform.position, new Xoroshiro128Plus(Run.instance.stageRng.nextUlong));
                    Log.Debug("Effect Spork!");
                    inSonorousAlready = false;
                }
            }
        }//*/

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory) // Valid Check
            {
                Inventory inventory = sender.inventory;
                int grabCount = inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //Log.Warning("Sonorous Recalculate");
                    // Boss - Crits
                    // White - Damage
                    // Green - Healing
                    // Red - Speed
                    // VoidWhite - Damage
                    // VoidGreen - Healing
                    // VoidRed - Speed
                    // Lunar - Health? Jump Power?
                    // Untiered - Armor?
                    // Armor, Attack Speed, Health,
                    // Jump Power, Shield, Cooldowns
                    int[] statTiers = new int[5]; // 0:Misc  1:Damage  2:Healing  3:Speed  4:Crits

                    ItemIndex itemIndex = 0;
                    ItemIndex itemCount = (ItemIndex)ItemCatalog.itemCount;
                    // Go Through All Items
                    while (itemIndex < itemCount)
                    {
                        int tier = 0; // Misc
                        ItemTier itemTier = ItemCatalog.GetItemDef(itemIndex).tier;
                        if (itemTier is ItemTier.Tier1 or ItemTier.VoidTier1)
                        {
                            tier = 1; // Damage
                        }
                        else if (itemTier is ItemTier.Tier2 or ItemTier.VoidTier2)
                        {
                            tier = 2; // Healing
                        }
                        else if (itemTier is ItemTier.Tier3 or ItemTier.VoidTier3)
                        {
                            tier = 3; // Speed
                        }
                        else if (itemTier is ItemTier.Boss or ItemTier.VoidBoss)
                        {
                            tier = 4; // Crits
                        }
                        statTiers[tier] += inventory.GetItemCount(itemIndex);
                        // Check next Item
                        itemIndex++;
                    }
                    float statMultiplier = 1f + ((grabCount - 1) * stackPercent / 100f);
                    //Log.Debug("stat Multiplier: " + statMultiplier);
                    Log.Debug("Pail Damage is: " + sender.baseDamage + " + " + (statTiers[1] * attackPerWhite * statMultiplier) + "%");
                    args.damageMultAdd += statTiers[1] * attackPerWhite / 100f * statMultiplier;
                    // Regen increases per level
                    Log.Debug("Pail Regen is: " + sender.baseRegen + " + " + (statTiers[2] * (regenPerGreen + (regenPerGreen / 5 * sender.level)) * statMultiplier));
                    args.regenMultAdd += statTiers[2] * regenPerGreen * (1f + 0.2f * sender.level) * statMultiplier;
                    args.moveSpeedMultAdd += statTiers[3] * speedPerRed / 100f * statMultiplier;
                    args.critAdd += statTiers[4] * critPerBoss * statMultiplier;
                }
            }
        }

        //
        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            Log.Debug("orig IN Sonorous Pail");
            orig(self, itemIndex, count);
            Log.Debug("orig OUT Sonorous Pail");
            if (NetworkServer.active && !inSonorousAlready && self) // Hopefully fix multiple triggers and visual bug?
            {
                ItemDef iDef = ItemCatalog.GetItemDef(itemIndex);
                ItemTierDef iTierDef = ItemTierCatalog.GetItemTierDef(iDef.tier);
                // Validate check, and pass if not lunar unless is pail
                if (iDef && iTierDef && iTierDef.canRestack && (iTierDef.tier != ItemTier.Lunar || iDef == item)) // Valid Check (check iDef and iTierDef)
                {
                    inSonorousAlready = true;
                    CharacterBody player = CharacterBody.readOnlyInstancesList.ToList().Find((body) => body.inventory == self);
                    if (player && self.GetItemCount(item) > 0) // Valid Check
                    {
                        Log.Warning("Spork the inventory");
                        SporkRestackInventory(self, player.transform.position, new Xoroshiro128Plus(Run.instance.stageRng.nextUlong));
                        // Effect after restock
                        EffectManager.SpawnEffect(ShrineUseEffect, new EffectData
                        {
                            origin = player.transform.position,
                            rotation = Quaternion.identity,
                            scale = 0.5f,
                            color = new Color(0.2392f, 0.8196f, 0.917647f) // Cyan Lunar color
                        }, true);
                    }
                    inSonorousAlready = false;
                }
            }
        }//*/

        public void SporkRestackInventory(Inventory inventory, Vector3 pos, Xoroshiro128Plus rng)
        {
            Log.Debug("Restock my sporks!");
            if (!NetworkServer.active)
            {
                Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ShrineRestackInventory(Xoroshiro128Plus)' called on client");
                return;
            }
            List<ItemIndex> list = new List<ItemIndex>();
            bool flag = false;
            foreach (ItemTierDef itemTierDef in ItemTierCatalog.allItemTierDefs)
            {
                // In each tier
                Log.Debug("Which Shelf?: " + itemTierDef.tier);
                if (itemTierDef.canRestack && itemTierDef.tier != ItemTier.Lunar)
                {
                    // Record what items exist and how many items in total
                    int num = 0;
                    list.Clear();
                    for (int i = 0; i < inventory.itemStacks.Length; i++)
                    {
                        if (inventory.itemStacks[i] > 0)
                        {
                            ItemIndex itemIndex = (ItemIndex)i;
                            ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
                            if (itemTierDef.tier == itemDef.tier)
                            {
                                // Add to total items
                                num += inventory.itemStacks[i];
                                // Add to list
                                list.Add(itemIndex);
                                // Remove from inventory
                                inventory.itemAcquisitionOrder.Remove(itemIndex);
                                inventory.ResetItem(itemIndex);
                            }
                        }
                    }
                    if (list.Count > 0)
                    {
                        // Add items minus silver treads (will be countered by silver thead it self
                        inventory.GiveItem(rng.NextElementUniform(list), num); // - inventory.GetItemCount(SilverThread.item)
                        flag = true;
                    }
                }
            }
            if (flag)
            {
                inventory.SetDirtyBit(8U);
            }
        }
    }
}

/*
public void ShrineRestackInventory([NotNull] Xoroshiro128Plus rng)
{
	if (!NetworkServer.active)
	{
		Debug.LogWarning("[Server] function 'System.Void RoR2.Inventory::ShrineRestackInventory(Xoroshiro128Plus)' called on client");
		return;
	}
	List<ItemIndex> list = new List<ItemIndex>();
	bool flag = false;
	foreach (ItemTierDef itemTierDef in ItemTierCatalog.allItemTierDefs)
	{
		if (itemTierDef.canRestack)
		{
			int num = 0;
			list.Clear();
			for (int i = 0; i < this.itemStacks.Length; i++)
			{
				if (this.itemStacks[i] > 0)
				{
					ItemIndex itemIndex = (ItemIndex)i;
					ItemDef itemDef = ItemCatalog.GetItemDef(itemIndex);
					if (itemTierDef.tier == itemDef.tier)
					{
						num += this.itemStacks[i];
						list.Add(itemIndex);
						this.itemAcquisitionOrder.Remove(itemIndex);
						this.ResetItem(itemIndex);
					}
				}
			}
			if (list.Count > 0)
			{
				this.GiveItem(rng.NextElementUniform<ItemIndex>(list), num);
				flag = true;
			}
		}
	}
	if (flag)
	{
		base.SetDirtyBit(8U);
	}
}
*/