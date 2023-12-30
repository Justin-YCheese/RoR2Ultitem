using R2API;
using RoR2;
using System.Linq;
using UnityEngine;

namespace UltitemsCyan.Items.Tier2
{

    // TODO: check if Item classes needs to be public
    public class DegreeScissors : ItemBase
    {
        public static ItemDef item;
        private const int scrapsPerScissor = 2;
        private void Tokens()
        {
            string tokenPrefix = "DEGREESCISSORS";

            // Add translation from token to string
            LanguageAPI.Add(tokenPrefix + "_NAME", "1000 Degree Scissors");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Turn consumed items into scrap.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "At the start of each stage, <style=cIsUtility>convert</style> a <style=cIsUtility>consumed</style> item into <style=cIsUtility>2 common scraps</style>. Otherwise <style=cIsUtility>convert</style> self.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "What's Youtube?");

            // Adds tokens to item
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

            item.pickupIconSprite = Ultitems.Assets.DegreeScissorsSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

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
                // Get number of scissors
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Warning("Scissors on body start global...");
                    // Get inventory
                    System.Collections.Generic.List<ItemIndex> itemsInInventory = self.inventory.itemAcquisitionOrder;

                    /*/ Print items in inventory
                    Log.Debug("Items in inventory: " + itemsInInventory.ToString());
                    foreach (ItemIndex item in itemsInInventory)
                    {
                        Log.Debug(" - " + ItemCatalog.GetItemDef(item).name);
                    }//*/

                    // Aggregate list of consumed items in inventory
                    System.Collections.Generic.List<ItemDef> consumedItems = [];
                    foreach (ItemIndex index in itemsInInventory)
                    {
                        ItemDef definition = ItemCatalog.GetItemDef(index);
                        // If item is untiered, can be removed, and not hidden
                        // Don't need to check for regenerating scrap because it is restored before this check
                        if (definition.tier.Equals(ItemTier.NoTier) && !definition.hidden) //definition.name.ToUpper().Contains("CONSUMED") // Checking for "consumed"
                        {
                            //Log.Debug("Adding consumed item " + definition.name);
                            consumedItems.Add(definition);
                        }
                    }
                    // Print consumeable items
                    Log.Debug("List of consumeable items:");
                    foreach (ItemDef item in consumedItems)
                    {
                        Log.Debug(" - " + item.name + " (" + self.inventory.GetItemCount(item) + ")");
                    }

                    // Get length of list
                    int length = consumedItems.Count;
                    //Log.Debug("Length: " + length);
                    // for each scissors in inventory remove a random consumed item
                    if (length > 0)
                    {
                        for (; grabCount > 0; grabCount--)
                        {
                            int itemPos = Random.RandomRangeInt(0, length);
                            ItemDef selectedItem = consumedItems[itemPos]; // Don't need to subtract 1 from length because it excludes the max
                            // Remove 1 consumed item
                            Log.Debug("Removing " + selectedItem.name); // + " at " + itemPos);
                            self.inventory.RemoveItem(selectedItem);

                            // Give 2 white scraps
                            self.inventory.GiveItem(ItemCatalog.FindItemIndex("ScrapWhite"), scrapsPerScissor);

                            // If ran out of that consumable item in player's inventory
                            if (self.inventory.GetItemCount(selectedItem) < 1)
                            {
                                Log.Debug("Out of " + selectedItem.name);
                                //Log.Debug("New length of " + (length - 1));
                                consumedItems.RemoveAt(itemPos);
                                length--;
                                // If list is empty break loop
                                if (length == 0)
                                {
                                    Log.Debug("Scissors can't cut Empty List");
                                    break;
                                }
                            }
                        }
                    }
                    // If after cutting there are still scissors which didn't cut anything (break out of loop or no cuttable items) then consume a scissor
                    if (grabCount > 0)
                    {
                        Log.Debug("Scissors cuts itself");
                        // Remove a scissors (Garenteed to have at least scissors)
                        self.inventory.RemoveItem(item);
                        // Give 2 white scraps
                        self.inventory.GiveItem(ItemCatalog.FindItemIndex("ScrapWhite"), scrapsPerScissor);
                    }
                }
            }
        }
    }
}