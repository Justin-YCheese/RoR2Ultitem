using Epic.OnlineServices;
using IL.RoR2.Orbs;
using On.RoR2.Orbs;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using System.ComponentModel;
using UltitemsCyan.Component;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Tier1
{

    // TODO: Make better sound and visuals
    public class FleaBag : ItemBase
    {
        public static ItemDef item;
        public static GameObject FleaOrb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Tooth/HealPack.prefab").WaitForCompletion();
        private const float procChance = 3f;

        // For Flea Pickup
        public const float baseBuffDuration = 15f;
        public const float buffDurationPerItem = 0f;
        public const int buffMaxStack = 15;

        // For Tick Crit Buff
        public const float baseTickMultiplier = 5f;
        public const float tickPerStack = 10f;

        private void Tokens()
        {
            // Fire flies?
            string tokenPrefix = "FLEABAG";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Flea Bag");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Hits can drop bags which give critical chance. Critical Strikes drop more bags.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsDamage>3%</style> chance on hit to drop a bag which gives a max of <style=cIsDamage>15%</style> <style=cStack>(+10% per stack)</style> <style=cIsDamage>critical chance</style> for 15 <style=cStack>(+0 per stack)</style> seconds. <style=cIsDamage>Critical strikes</style> are triply likely to drop a bag.");
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
                        Log.Warning("FleaBag on Hit");
                        bool drop;
                        if (damageInfo.crit)
                        {
                            drop = Util.CheckRoll(procChance * 2, inflictor.master.luck);
                        }
                        else
                        {
                            drop = Util.CheckRoll(procChance, inflictor.master.luck);
                        }
                        if (drop)
                        {
                            Log.Debug("dropping flea");
                            //RoR2.BuffPickup.Instantiate(item);
                            Util.PlaySound("Play_hermitCrab_idle_VO", victim.gameObject);
                            SpawnOrb(victim.transform.position, victim.transform.rotation, TeamComponent.GetObjectTeam(inflictor.gameObject), grabCount);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Warning("What Flea Hit?");
                Log.Debug("Victum " + victim.name);
                Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                Log.Debug("Damage rejected? " + damageInfo.rejected);
            }
            orig(self, damageInfo, victim);
        }

        public static void SpawnOrb(Vector3 position, Quaternion rotation, TeamIndex teamIndex, int itemCount)
        {
            if (NetworkServer.active)
            {
                GameObject orb = UnityEngine.Object.Instantiate(FleaOrb);
                if (orb)
                {
                    Log.Debug("Orb is loaded");
                }

                orb.transform.position = position;
                orb.transform.rotation = rotation;
                orb.GetComponent<TeamFilter>().teamIndex = teamIndex;

                //orb.GetComponent<TrailRenderer>().startColor = new Color(1f, 0f, 0f, 1f);
                //orb.GetComponent<TrailRenderer>().endColor = new Color(0f, 0f, 1f, 0f);

                //orb.GetComponent<DestroyOnTimer>().duration = 5f;

                //Health Pickup
                HealthPickup healthComponent = orb.GetComponentInChildren<HealthPickup>();
                Log.Debug("Orb has a Health Pickup");
                //healthComponent.flatHealing = 0;
                //healthComponent.fractionalHealing = 0;
                healthComponent.alive = false;
                //*/

                //BuffPickup
                FleaPickup FleaComponent = healthComponent.gameObject.AddComponent<FleaPickup>();

                //FleaPickup FleaComponent = orb.GetComponentInChildren<>().gameObject.AddComponent<FleaPickup>();
                FleaComponent.amount = itemCount;

                FleaComponent.baseObject = orb;
                FleaComponent.teamFilter = orb.GetComponent<TeamFilter>();
                FleaComponent.pickupEffect = null;

                orb.GetComponent<Rigidbody>().useGravity = true;
                orb.transform.localScale = Vector3.one * (.5f + itemCount / 20);

                Log.Debug("Spawning orb at: " + orb.transform.position);
                NetworkServer.Spawn(orb);
            }
        }
    }
}