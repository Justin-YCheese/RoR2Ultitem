using Mono.Cecil.Cil;
using R2API;
using Rewired.Utils;
using RoR2;
using RoR2.Items;
using System;
using System.ComponentModel;
using System.Linq;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using UnityEngine.Networking;
using Zio;
using static Rewired.UI.ControlMapper.ControlMapper;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Lunar
{

    // TODO: check if Item classes needs to be public
    public class SilverThread : ItemBase
    {
        public static ItemDef item;

        private const int extraItemsPerStack = 1;
        private const int maxStack = 3;
        private const float percentPerStack = 50f;
        private const float deathSnapTime = 600f; // 10 minutes

        private static readonly float[] division = [1f, 0.5f, 0.25f];

        private bool inSilverAlready = false;

        public override void Init()
        {
            item = CreateItemDef(
                "SILVERTHREAD",
                "Silver Thread",
                "Gain additional items BUT die in 10 (-50% per stack) minutes. Upon death, this item will be consumed.",
                "Pick up 1 (+1 per stack) additional item when you gain items BUT die in 10 (-50% per stack) minutes after the start of each stage. Upon death, this item will be consumed.",
                "The end of the abacus of life. A King's Riches Lays before you, but at the end of a strand which has been snapped intwine.",
                ItemTier.Lunar,
                Ultitems.Assets.SilverThreadSprite,
                Ultitems.Assets.SilverThreadPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect]
            );
        }

        protected override void Hooks()
        {
            // Give Thread Item Behavior   
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            // Give Behavior at start of stage
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            // Gain additional items
            On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
            // Increase cauldron and 3D printer cost
            On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
            // Increase scrapper cost
            On.RoR2.ScrapperController.BeginScrapping += ScrapperController_BeginScrapping;
            // Prevent Mythrics from giving thread back
            //On.RoR2.ItemStealController.StolenInventoryInfo.LendStolenItem += StolenInventoryInfo_LendStolenItem;
            //IL.RoR2.GlobalEventManager.OnPlayerCharacterDeath += GlobalEventManager_OnPlayerCharacterDeath;
        }

        /*/ Do not return Thread
        private int StolenInventoryInfo_LendStolenItem(On.RoR2.ItemStealController.StolenInventoryInfo.orig_LendStolenItem orig, object self, ItemIndex itemIndex, bool useOrb, int maxStackToGive)
        {
            //controllers = self.
            if (itemIndex == item.itemIndex)
            {
                Log.Warning(" ! ! ! Don't Silver Death ! ! ! ");
                return 0;
            }
            return orig(self, itemIndex, useOrb, maxStackToGive);
        }//*/

        private void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
        {
            
            bool runOrig = true;
            if (self && activator && activator)
            {
                if (self.costType is
                    CostTypeIndex.WhiteItem or
                    CostTypeIndex.GreenItem or
                    CostTypeIndex.RedItem or
                    CostTypeIndex.BossItem or
                    CostTypeIndex.LunarItemOrEquipment
                    )
                {
                    Log.Warning("Silver Purchase check");
                    runOrig = false;
                    CharacterBody player = activator.GetComponent<CharacterBody>();
                    int grabSilverCount = Math.Min(player.inventory.GetItemCount(item), maxStack);
                    Log.Debug("Self Cost? " + self.cost + " + " + grabSilverCount);
                    self.cost *= 1 + grabSilverCount;
                    Log.Debug("New Self Cost? " + self.cost);

                    orig(self, activator);

                    self.cost /= 1 + grabSilverCount;
                    Log.Debug("Post Self Cost? " + self.cost);
                }
            }
            //
            if (runOrig)
            {
                orig(self, activator);
            }//*/
        }

        // Make Scrapper return fewer items per Silver Thread Held
        private void ScrapperController_BeginScrapping(On.RoR2.ScrapperController.orig_BeginScrapping orig, ScrapperController self, int intPickupIndex)
        {
            Log.Warning("Silver Scrapping check");
            bool runOrig = true;
            if (NetworkServer.active && self)
            {
                CharacterBody player = self.interactor.GetComponent<CharacterBody>();
                if (player && player.inventory)
                {
                    int grabSilverCount = Math.Min(player.inventory.GetItemCount(item), maxStack);
                    if (grabSilverCount > 0)
                    {
                        Log.Debug("Silver Scrapping custom function");
                        // body has a silver thread in their inventory
                        runOrig = false;

                        self.itemsEaten = 0;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(intPickupIndex));
                        if (pickupDef != null && self.interactor)
                        {
                            self.lastScrappedItemIndex = pickupDef.itemIndex;
                            int scrapCount = Mathf.Min(self.maxItemsToScrapAtATime * (1 + grabSilverCount), player.inventory.GetItemCount(pickupDef.itemIndex));
                            if (scrapCount <= grabSilverCount)
                            {
                                // not enough items to convert item, don't return anything
                                Log.Debug("Silver Scrapper Consume poor items");
                                self.itemsEaten = -1;
                            }
                            else
                            {
                                // return reduced amount
                                Log.Debug("scrapCount: " + scrapCount + " returnCount: " + (scrapCount / (1 + grabSilverCount)));
                                player.inventory.RemoveItem(pickupDef.itemIndex, scrapCount);
                                self.itemsEaten += scrapCount / (1 + grabSilverCount);
                                for (int i = 0; i < scrapCount; i++)
                                {
                                    ScrapperController.CreateItemTakenOrb(player.corePosition, self.gameObject, pickupDef.itemIndex);
                                }
                                if (self.esm)
                                {
                                    self.esm.SetNextState(new EntityStates.Scrapper.WaitToBeginScrapping());
                                }
                            }
                        }
                        /*/ Scrapper Controller Begin Scrapper
                        this.itemsEaten = 0;
                        PickupDef pickupDef = PickupCatalog.GetPickupDef(new PickupIndex(intPickupIndex));
                        if (pickupDef != null && this.interactor)
                        {
                            this.lastScrappedItemIndex = pickupDef.itemIndex;
                            CharacterBody component = this.interactor.GetComponent<CharacterBody>();
                            if (component && component.inventory)
                            {
                                int num = Mathf.Min(this.maxItemsToScrapAtATime, component.inventory.GetItemCount(pickupDef.itemIndex));
                                if (num > 0)
                                {
                                    component.inventory.RemoveItem(pickupDef.itemIndex, num);
                                    this.itemsEaten += num;
                                    for (int i = 0; i < num; i++)
                                    {
                                        ScrapperController.CreateItemTakenOrb(component.corePosition, base.gameObject, pickupDef.itemIndex);
                                    }
                                }
                            }
                        }
                        if (this.esm)
                        {
                            this.esm.SetNextState(new WaitToBeginScrapping());
                        }//*/
                    }
                }
            }
            // If checks failed, run original function
            if (runOrig)
            {
                orig(self, intPickupIndex);
            }
            
        }

        // Increase Items gained when given
        public void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)
        {
            if (NetworkServer.active && !inSilverAlready && self && itemIndex != item.itemIndex)
            {
                // Precaution incase something causes an infinity loop of items
                inSilverAlready = true;
                int grabCount = Math.Min(self.GetItemCount(item), maxStack);
                if (grabCount > 0)
                {
                    Log.Debug("Adding " + grabCount + " of " + ItemCatalog.GetItemDef(itemIndex).name + " to " + count);
                    count += grabCount;
                }
                orig(self, itemIndex, count);
                inSilverAlready = false;
            }
            else if (inSilverAlready)
            {
                Log.Warning(" * * * IN SILVER GIVE ITEM: LOOP DETECTED * * * good thing there's a inSilverAlready check");
                orig(self, itemIndex, count);
            }
            else
            {
                Log.Debug("   passing silver give item");
                orig(self, itemIndex, count);
            }
        }

        /*/
        private void GlobalEventManager_OnPlayerCharacterDeath(MonoMod.Cil.ILContext il)
        {
            il.IndexOf(new Instruction())
        }//*/

        // Stage Start Behavior? Might not need
        private void CharacterBody_onBodyStartGlobal(CharacterBody self)
        {
            if (NetworkServer.active && self && self.inventory)
            {
                self.AddItemBehavior<SilverThreadBehavior>(self.inventory.GetItemCount(item));
            }
        }

        // Add timer / update timer for silver thread
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            // Add Behavior to body (expectially if the full time intervals have passed)
            if (self && self.inventory)
            {
                int grabCount = Math.Min(self.inventory.GetItemCount(item), maxStack);
                if (grabCount > 0)
                {
                    Log.Debug("Silver Body Changed: ");
                    if (self.baseNameToken == "BROTHER_BODY_NAME")
                    {
                        Log.Warning("Mytrhics Silver Body Changed into a cadaver?");
                        deathSnapPlayer(self);
                    }
                    else
                    {
                        SilverThreadBehavior behavior = self.AddItemBehavior<SilverThreadBehavior>(grabCount);
                        behavior.enabled = true;
                    }
                }
                else
                {
                    // Remove Effect?
                }
            }
        }

        // Kill body 
        private static void deathSnapPlayer(CharacterBody body)
        {
            Log.Warning(body.GetUserName() + "'s thread snapped");
            // If has item
            int grabCount = body.inventory.GetItemCount(item);
            // Check generally even if actuall body doesn't have threads anymore
            deathCheckStealController(body);
            if (grabCount > 0)
            {
                Log.Debug("And does have thread");
                body.inventory.RemoveItem(item.itemIndex, grabCount);
                body.inventory.GiveItem(Untiered.SilverThreadConsumed.item, grabCount);
                Chat.AddMessage("Your thread snapped...");
                body.healthComponent.Suicide();
                Log.Debug("Snap!");
            }
        }

        // Death Check remove thread from stealing controller
        private static void deathCheckStealController(CharacterBody body)
        {
            ItemStealController[] controllers;

            //ItemStealController.StolenInventoryInfo[] array = this.stolenInventoryInfos;
            //NetworkBehaviour.
            //ReturnStolenItemsOnGettingHit

            ItemStealController.StolenInventoryInfo[] info = body.GetComponent<ReturnStolenItemsOnGettingHit>().itemStealController.stolenInventoryInfos;
            Log.Warning("Stolen Inventory length: " + info.Length);
            if (info != null && info.Length > 0)
            {
                foreach (ItemStealController.StolenInventoryInfo temp in info)
                {
                    Log.Debug("Stolen Thread?");
                    Inventory inventory = temp.lendeeInventory;
                    int grabCount = inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        Log.Debug("Clearing inventory of " + grabCount + " things");
                        inventory.RemoveItem(item.itemIndex, grabCount);
                        inventory.GiveItem(Untiered.SilverThreadConsumed.item, grabCount);
                        Log.Debug("New grab count: " + inventory.GetItemCount(item));
                    }
                }
                
            }
            //Log.Warning("testInv grabCount: " + test1.lendeeInventory.GetItemCount(item) + " for " + test1.networkIdentity.name);

            //controllers = body.GetComponents<ItemStealController>();
            //controllers = body.gameObject.GetComponents<ItemStealController>();
            //controllers = body.networkIdentity.GetComponents<ItemStealController>();
            controllers = body.netIdentity.GetComponents<ItemStealController>();
            //Log.Warning("Checking Steal: Null? " + controllers.IsNullOrDestroyed() + " Empty? " + (controllers.IsNullOrDestroyed() ? -1 : controllers.Length));

            // If body has an Item Steal Controller
            //if (!controllers.IsNullOrDestroyed() && controllers.Length > 0)
            //{
            //    foreach (ItemStealController controller in controllers)
            //    {
            //        Inventory inventory = controller.lendeeInventory;
            //        
            //        Log.Debug("Has " + grabCount + " Silvers");
            //        if (grabCount > 0)
            //        {
            //            Log.Debug("Clearing " + controller.networkIdentity.name + "'s controller");
            //            // Remove all Silver Threads from controller inventory
            //            inventory.RemoveItem(item.itemIndex, grabCount);
            //            inventory.GiveItem(Untiered.SilverThreadConsumed.item, grabCount);
            //        }
            //    }
            //}
        }//*/

        // Snap Timer
        public class SilverThreadBehavior : CharacterBody.ItemBehavior
        {
            private bool _intervalPassed = false;
            // Order:
            // Awake(), Enable(), Start()
            // Disable(), Destory()

            public bool IntervalPassed
            {
                get { return _intervalPassed; }
                set
                {
                    //Log.Debug("_intervalsPassed: " + _intervalsPassed);
                    // If not already the same value
                    _intervalPassed = value;
                    // If full health
                    if (_intervalPassed)
                    {
                        deathSnapPlayer(body);
                        enabled = false;
                    }
                }
            }

            private void OnDisable()
            {
                Log.Debug("Thread Disabled, now passed is false");
                IntervalPassed = false;
            }

            private void FixedUpdate()
            {
                float currentTime = Run.instance.time;
                
                // Runs stacks - 1 times
                //Log.Debug(IntervalPassed + "? division\t" + division + " : \t" + currentTime + " > \t" + (Ultitems.stageStartTime + (deathSnapTime / division)) + " = \t" + (currentTime > Ultitems.stageStartTime + (deathSnapTime / division)));
                if (stack > 0 && currentTime > Ultitems.stageStartTime + (deathSnapTime / division[Math.Min(stack - 1, 2)]))
                {
                    IntervalPassed = true;
                }
            }
        }
    }
}