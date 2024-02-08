using Newtonsoft.Json.Utilities;
using R2API;
using RoR2;
using System.Collections.Generic;
using UltitemsCyan.Items.Tier3;
using UltitemsCyan.Items.Untiered;
using UnityEngine;
using UnityEngine.Timeline;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class InhabitedCoffin : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        private const float noFreeCoffinChance = 88f;
        private const int minimumInCoffin = 5;
        private const int bonusInCoffin = 0;

        private void Tokens()
        {
            string tokenPrefix = "INHABITEDCOFFIN";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Inhabited Coffin");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Breaks at the start of the next stage. Contains void items. <style=cIsVoid>Corrupts all Corroding Vaults</style>.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>6</style> random void items. <style=cIsVoid>Corrupts all Corroding Vaults</style>.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Something lives inside this coffin. That coffin is deeper than you think.");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();
            transformItem = CorrodingVault.item;

            // Add text for item
            Tokens();

            //Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.VoidTier3;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.InhabitedCoffinSprite;
            item.pickupModelPrefab = Ultitems.Assets.InhabitedCoffinPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            GetTransformItem = transformItem;

            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (self && self.inventory)
            {
                // Get number of vaults
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Warning("Inhabited Coffin on body start global..." + self.GetUserName());
                    // Remove a vault
                    self.inventory.RemoveItem(item);
                    // Give Consumed vault
                    self.inventory.GiveItem(InhabitedCoffinConsumed.item);

                    // Get Void Items
                    List<ItemIndex> voidItemsList = new List<ItemIndex>();
                    ItemDef.Pair[] voidCorruptions = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem];
                    foreach (ItemDef.Pair pair in voidCorruptions)
                    {
                        bool added = voidItemsList.AddDistinct(pair.itemDef2.itemIndex);
                        //Log.Debug(". Adding " + pair.itemDef2.name + "? " + added);
                    }

                    // All Void Items
                    ItemIndex[] allVoidItems = voidItemsList.ToArray();
                    int length = allVoidItems.Length;
                    Log.Debug("All Void Items Length: " + length);

                    // 14 Vanilla void items
                    // 4 modded void items
                    int quantityInVault;
                    if (Util.CheckRoll(noFreeCoffinChance, self.master.luck))
                    {
                        // No coffin
                        quantityInVault = minimumInCoffin;
                    }
                    else
                    {
                        // You get a free coffin
                        self.inventory.GiveItem(item);
                        Log.Debug("- Coffin got Coffin");
                        GenericPickupController.SendPickupMessage(self.master, PickupCatalog.itemIndexToPickupIndex[(int)item.itemIndex]);
                        quantityInVault = minimumInCoffin - 1;
                    }

                    ;// + Random.Range(0, bonusInCoffin + 1); // bonus plus one because rand int not include max

                    // Error Message if there aren't enough items somehow
                    if (length < quantityInVault) { Log.Warning(" ! ! ! There aren't enough white items for Rusted Vault ! ! !"); }

                    for (int i = 0; i < quantityInVault; i++)
                    {
                        int itemPos = Random.Range(0, length);
                        Log.Debug("- random Void found: " + ItemCatalog.GetItemDef(allVoidItems[itemPos]).name);
                        self.inventory.GiveItem(allVoidItems[itemPos]);
                        GenericPickupController.SendPickupMessage(self.master, PickupCatalog.itemIndexToPickupIndex[(int)allVoidItems[itemPos]]);
                    }

                    /*/ Give void items
                    for (int i = 0; i < quantityInVault; i++)
                    {
                        int itemPos = Random.Range(0, length);
                        //Log.Debug("Random Position: " + itemPos);
                        Log.Debug("Random White found: " + ItemCatalog.GetItemDef(allWhiteItems[itemPos]).name);
                        self.inventory.GiveItem(allWhiteItems[itemPos]);
                        // erase current item and preserve last item
                        // setting current item equal to last item and shorten length effectively moving last item to current item
                        allWhiteItems[itemPos] = allWhiteItems[length - 1];
                        length--;
                    }
                    Log.Debug(quantityInVault + " white items from vault");
                    //*/
                    //TODO Add message for number of items in vault
                }
            }
        }
    }
}