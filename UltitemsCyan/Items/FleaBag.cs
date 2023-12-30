using R2API;
using RoR2;
using System;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class FleaBag : ItemBase
    {
        public static ItemDef item;
        private const float procChance = 10f;
        private void Tokens()
        {
            string tokenPrefix = "FLEABAG";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Flea Bag");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Hits can drop bags which give critical chance. Critical Strikes drop more bags.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsAttack>10%</style> chance on hit to drop a bag which gives a <style=cIsAttack>4%</style> <style=cStack>(+4% per stack)</style> <style=cIsAttack>critical chance</style> for 25 seconds. <style=cIsAttack>Critical strikes</style> are twice as likely to drop a bag.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Movie?");

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

            // Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier1;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.mysterySprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Damage];

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            try
            {
                // If the victum has an inventory
                // and damage isn't rejected?
                if (self && victim && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        bool drop = Util.CheckRoll(procChance, inflictor.master.luck);
                        if (drop)
                        {
                            PickupDef bagDrop = new()
                            {
                                baseColor = new Color(150, 30, 50),
                                darkColor = new Color(90, 10, 65),
                                //displayPrefab = Ultitems.mysteryPrefab,
                                dropletDisplayPrefab = Ultitems.mysteryPrefab,
                                
                            };
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Warning("What Dream Hit?");
                Log.Debug("Victum " + victim.name);
                Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                Log.Debug("Damage rejected? " + damageInfo.rejected);
            }
            orig(self, damageInfo, victim);
        }
    }
}