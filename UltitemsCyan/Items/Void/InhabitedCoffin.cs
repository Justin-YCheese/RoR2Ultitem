﻿using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using System.Collections.Generic;
using UltitemsCyan.Items.Tier3;
using UltitemsCyan.Items.Untiered;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class InhabitedCoffin : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        private const float freeCoffinChance = 20f;
        private const int minimumInCoffin = 5;

        public override void Init()
        {
            item = CreateItemDef(
                "INHABITEDCOFFIN",
                "Inhabited Coffin",
                "Breaks at the start of the next stage. Contains void items. <style=cIsVoid>Corrupts all Corroding Vaults</style>.",
                "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>5</style> random void items. <style=cIsUtility>Affected by luck</style>. <style=cIsVoid>Corrupts all Corroding Vaults</style>.",
                "Something lives inside this coffin. That coffin is deeper than you think.",
                ItemTier.VoidTier3,
                Ultitems.Assets.InhabitedCoffinSprite,
                Ultitems.Assets.InhabitedCoffinPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist],
                CorrodingVault.item
            );
        }

        protected override void Hooks()
        {
            //On.RoR2.Run.BeginStage += Run_BeginStage;
            On.RoR2.Stage.BeginServer += Stage_BeginServer;
            //CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void Stage_BeginServer(On.RoR2.Stage.orig_BeginServer orig, Stage self)
        {
            orig(self);
            if (!NetworkServer.active)
            {
                Log.Debug("Running on Client... return...");
                return;
            }
            foreach (CharacterMaster master in CharacterMaster.readOnlyInstancesList)
            {
                if (master.inventory)
                {
                    // Get number of vaults
                    int grabCount = master.inventory.GetItemCount(item.itemIndex);
                    if (grabCount > 0)
                    {
                        //Log.Warning("Inhabited Coffin on body start global..." + master.name);
                        // Remove a vault
                        master.inventory.RemoveItem(item);
                        // Give Consumed vault
                        master.inventory.GiveItem(InhabitedCoffinConsumed.item);

                        // All Void Items
                        List<PickupIndex>[] allVoidDropList = [
                            Run.instance.availableVoidTier1DropList,
                            Run.instance.availableVoidTier2DropList,
                            Run.instance.availableVoidTier3DropList,
                            Run.instance.availableVoidBossDropList
                            ];

                        int length = allVoidDropList[0].Count + allVoidDropList[1].Count + allVoidDropList[2].Count + allVoidDropList[3].Count;
                        Log.Debug("All Void Items Length: " + length);

                        // 14 Vanilla void items
                        // 4 modded void items
                        int quantityInVault;
                        if (Util.CheckRoll(100f - freeCoffinChance, master.luck))
                        {
                            // No coffin
                            quantityInVault = minimumInCoffin;
                        }
                        else
                        {
                            // You get a free coffin
                            master.inventory.GiveItem(item);
                            //Log.Debug("- Coffin got Coffin");
                            GenericPickupController.SendPickupMessage(master, PickupCatalog.itemIndexToPickupIndex[(int)item.itemIndex]);
                            quantityInVault = minimumInCoffin - 1;
                        }

                        Xoroshiro128Plus rng = new(Run.instance.stageRng.nextUlong);

                        for (int i = 0; i < quantityInVault; i++)
                        {
                            int randItemPos = rng.RangeInt(0, length);
                            PickupIndex foundItem = getVoidItem(allVoidDropList, randItemPos);

                            master.inventory.GiveItem(PickupCatalog.GetPickupDef(foundItem).itemIndex);
                            GenericPickupController.SendPickupMessage(master, foundItem);
                        }
                    }
                }
            }
        }

        private PickupIndex getVoidItem(List<PickupIndex>[] items, int index)
        {
            // Iterate the different tiers
            for(int i = 0; i < items.Length; i++)
            {
                // If index greater than current tier, subract count and iterate to next tier
                if (index >= items[i].Count)
                {
                    index -= items[i].Count;
                }
                else
                {
                    return items[i][index];
                }
            }
            return PickupIndex.none;
        }
    }
}