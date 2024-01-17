using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UltitemsCyan;
using RoR2;

namespace UltitemsCyan.Component
{
    [HarmonyPatch]
    public static class HarmonyHelper
    {
        // Harmony is to change or "patch" the game code with your own code
        // Using it here because to add void transformations,
        // the Item Catalog has to first be made which happenes after initilizing the mod
        [HarmonyPrefix, HarmonyPatch(typeof(RoR2.Items.ContagiousItemManager), nameof(RoR2.Items.ContagiousItemManager.Init))]
        public static void CreateVoidTransformations()
        {
            // Add ultiCorruptionPairs to base game corruption pairs
            Log.Warning("Create Void Transformations ! ! ! ! ! ! ~ ! ! ! ! ! ! ~ ! ! ! ! ! ! ~ ! ! ! ! ! ! ~ ! ! ! ! ! ! ");
            //ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem];
            //Log.Debug("What? [" + ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].ToString() + "] ");
            //Log.Debug("First: " + ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem][0]);
            List<ItemDef.Pair> voidPairs = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].ToList(); // Collection Expression?
            Log.Debug("What do you know there was an error");
            //ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = voidPairs.Union(Ultitems.CorruptionPairs).ToArray();

            On.RoR2.Items.ContagiousItemManager.Init += ContagiousItemManager_Init;
        }

        private static void ContagiousItemManager_Init(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
        {
            throw new NotImplementedException();
        }
    }
}
