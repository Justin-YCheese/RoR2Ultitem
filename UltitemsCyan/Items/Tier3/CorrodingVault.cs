using R2API;
using RoR2;
using UltitemsCyan.Items.Untiered;
using UnityEngine;
using UnityEngine.Networking;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class CorrodingVault : ItemBase
    {
        public static ItemDef item;
        private const int minimumInVault = 16;
        private const int bonusInVault = 4;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "CORRODINGVAULT";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Corroding Vault");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Breaks at the start of the next stage. Contains white items.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "At the start of each stage, this item will <style=cIsUtility>break</style> and gives <style=cIsUtility>16 to 20</style> unique white items");
            LanguageAPI.Add(tokenPrefix + "_LORE", "This vault is sturdy, but over time the rust will just crack it open");

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

            //Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier3;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.CorrodingVaultSprite;
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
            GetItemDef = item;
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        protected void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (NetworkServer.active && self && self.inventory)
            {
                // Get number of vaults
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    Log.Warning("Rusted Vault on body start global..." + self.GetUserName());
                    // Remove a vault
                    self.inventory.RemoveItem(item);
                    // Give Consumed vault
                    self.inventory.GiveItem(CorrodingVaultConsumed.item);

                    // Get all white items
                    ItemIndex[] allWhiteItems = new ItemIndex[ItemCatalog.tier1ItemList.Count];
                    ItemCatalog.tier1ItemList.CopyTo(allWhiteItems);
                    int length = allWhiteItems.Length;

                    //Log.Debug("All White Items Length: " + length);

                    // bonus plus one because rand int not include max
                    int quantityInVault = minimumInVault + Random.Range(0, bonusInVault + 1);

                    // Error Message if there aren't enough items somehow
                    if (length < quantityInVault) { Log.Warning(" ! ! ! There aren't enough white items for Rusted Vault ! ! !"); }

                    // Give 16 different white items
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

                    Chat.AddMessage(self.GetUserName() + " got " + quantityInVault + " items from their vault");

                    //TODO Add message for number of items in vault
                    //Util.PlaySound("Play_UI_podBlastDoorOpen", self.gameObject);
                }
            }
        }
    }
}