using RoR2;
using UltitemsCyan.Items.Untiered;
using System.Collections.Generic;

namespace UltitemsCyan.Equipment
{

    // TODO: check if Item classes needs to be public
    public class UniversalSolute : EquipmentBase
    {
        public static EquipmentDef equipment;

        private const float cooldown = 1f;
        private List<ItemIndex> dissolvedItems = [];

        public override void Init()
        {
            equipment = CreateItemDef(
                "UNIVERSALSOLUTE",
                "Universal Solute",
                "<style=cDeath>Remove</style> your last item from existence on use.",
                "<style=cDeath>Remove</style> the last item in your inventory from the run. It will no longer appear, and any instances of the items will break.",
                "Everything returns to Water",
                cooldown,
                true,
                false,
                Ultitems.Assets.UniversalSoluteSprite,
                Ultitems.Assets.UniversalSolutePrefab
            );
        }

        protected override void Hooks()
        {
            // Erase Items
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
            // Maintain removal
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int; ;
            //On.RoR2.ShopTerminalBehavior.ctor += ShopTerminalBehavior_ctor;
        }

        private void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            Log.Debug("Test Solute...");
            if (dissolvedItems.Contains(itemIndex))
            {
                Log.Debug("Grabbed a disolved item...");
                Util.PlaySound("Play_minimushroom_spore_shoot", self.gameObject);
                orig(self, UniversalSolvent.item.itemIndex, count);
            }
            else
            {
                orig(self, itemIndex, count);
            }
        }


        // Delete Existing instances of the item, and remove from drops
        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            if (equipmentDef == equipment)
            {
                CharacterBody activator = self.characterBody;
                List<ItemIndex> itemList = activator.inventory.itemAcquisitionOrder;

                // Make delete self when you have no items?
                if (itemList.Count > 0)
                {
                    // Null if player has only untiered items, or no items
                    ItemDef lastItem = getLastItem(itemList);
                    
                    if (lastItem)
                    {
                        Log.Debug("Last Item: " + lastItem.name);

                        // * * * For Every player and monster remove the item
                        foreach (CharacterBody body in CharacterBody.readOnlyInstancesList)
                        {
                            Log.Debug("who? " + body.name);
                            dissolveItem(body.inventory, lastItem);
                        }

                        // * * * Remove item from pools
                        Run.instance.DisableItemDrop(lastItem.itemIndex);
                        Run.instance.availableItems.Remove(lastItem.itemIndex);
                        // check if empty, if so then add solute to item tier
                        checkEmptyItemTier(lastItem);

                        dissolvedItems.Add(lastItem.itemIndex);
                        //Log.Debug(dissolvedItems.Contains(lastItem.itemIndex) + " in dessolved items");
                        Util.PlaySound("Play_minimushroom_spore_shoot", self.gameObject);

                        // * * * Remove items from shops and item pickups? Turn into solvent?

                        //Run.instance.shopPortalCount;
                        //Run.instance.
                        /*
                        var Interactors = Run.instance.GetComponents<Interactor>();
                        Log.Debug("Length: " + Interactors.Length);
                        foreach (Interactor interactor in Interactors)
                        {
                            Log.Debug("Interactor name: " + interactor.name);
                            if (interactor.GetComponent<ShopTerminalBehavior>())
                            {
                                Log.Debug("has Shop");
                            }
                        }//*/

                        //ShopTerminalBehavior.GenerateNewPickupServer(true);


                        //UpdatePickupDisplayAndAnimations()
                        //PickupIndex pickupIndex = PickupCatalog.FindPickupIndex(itemIndex);


                        //DisableItemDisplay(ItemIndex itemIndex)

                        return true;
                    }
                }
                return false;
            }
            else
            {
                return orig(self, equipmentDef);
            }
        }

        private ItemDef getLastItem(List<ItemIndex> list)
        {
            for(int i = list.Count - 1; i >= 0; i--)
            {
                ItemDef item = ItemCatalog.GetItemDef(list[i]);
                if (item.tier != ItemTier.NoTier)
                {
                    return item;
                }
            }
            return null;
        }

        private void dissolveItem(Inventory inventory, ItemDef item)
        {
            if (inventory)
            {
                int grabCount = inventory.GetItemCount(item);
                if (inventory.GetItemCount(item) > 0)
                {
                    inventory.RemoveItem(item, grabCount);
                    inventory.GiveItem(UniversalSolvent.item, grabCount);
                }
            }
        }

        private void checkEmptyItemTier(ItemDef item)
        {
            List<PickupIndex> list = null;
            switch (item.tier)
            {
                case ItemTier.Tier1:
                    list = Run.instance.availableTier1DropList;
                    break;
                case ItemTier.Tier2:
                    list = Run.instance.availableTier2DropList;
                    break;
                case ItemTier.Tier3:
                    list = Run.instance.availableTier3DropList;
                    break;
                case ItemTier.Lunar:
                    list = Run.instance.availableLunarItemDropList;
                    break;
                case ItemTier.Boss:
                    list = Run.instance.availableBossDropList;
                    break;
                case ItemTier.VoidTier1:
                    list = Run.instance.availableVoidTier1DropList;
                    break;
                case ItemTier.VoidTier2:
                    list = Run.instance.availableVoidTier2DropList;
                    break;
                case ItemTier.VoidTier3:
                    list = Run.instance.availableVoidTier3DropList;
                    break;
                case ItemTier.VoidBoss:
                    list = Run.instance.availableVoidBossDropList;
                    break;
                default:
                    break;
            }
            if (list.Count == 0)
            {
                Log.Debug(list.ToString() + " | replace with Solute");
                list.Add(PickupCatalog.itemIndexToPickupIndex[(int)UniversalSolvent.item.itemIndex]);
            }
        }
    }
}