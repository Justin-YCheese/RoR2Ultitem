using R2API;
using RoR2;
using System;
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
        private static GameObject FleaOrb = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Tooth/HealPack.prefab").WaitForCompletion();
        //public static GameObject FleaEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Beetle/BeetleWardOrbEffect.prefab").WaitForCompletion();
        private static GameObject FleaEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Tooth/HealthOrbFlash.prefab").WaitForCompletion();
        private const float fleaDropChance = 3f;
        private const int fleaDropCritMultiplier = 3;

        // For Flea Pickup
        public const float baseBuffDuration = 12f;
        public const float buffDurationPerItem = 0f; // Increase for buff?
        //public const int buffMaxStack = 15;

        // For Tick Crit Buff
        public const float baseTickMultiplier = 0f;
        public const float tickPerStack = 15f;

        private void Tokens()
        {
            // Fire flies?
            string tokenPrefix = "FLEABAG";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Flea Bag");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Chance on hit to drop a tick which gives critical chance. Critical Strikes drop more ticks.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsDamage>3%</style> chance on hit to drop a bag which gives a max of <style=cIsDamage>15%</style> <style=cStack>(+15% per stack)</style> <style=cIsDamage>critical chance</style> for 18 seconds. <style=cIsDamage>Critical strikes</style> are thrice as likely to drop a bag.");
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

            item.pickupIconSprite = Ultitems.Assets.FleaBagSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Damage];

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
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
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
                        //Log.Warning("FleaBag on Hit");
                        bool drop;
                        if (damageInfo.crit)
                        {
                            drop = Util.CheckRoll(fleaDropChance * fleaDropCritMultiplier, inflictor.master.luck);
                        }
                        else
                        {
                            drop = Util.CheckRoll(fleaDropChance, inflictor.master.luck);
                        }
                        if (drop)
                        {
                            Log.Warning("Dropping flea from " + victim.name);
                            //RoR2.BuffPickup.Instantiate(item);
                            Util.PlaySound("Play_hermitCrab_idle_VO", victim.gameObject);
                            Util.PlaySound("Play_hermitCrab_idle_VO", victim.gameObject);
                            SpawnOrb(victim.transform.position, victim.transform.rotation, TeamComponent.GetObjectTeam(inflictor.gameObject), grabCount);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Warning("What Flea Hit?");
                //Log.Debug("Victum " + victim.name);
                //Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                //Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                //Log.Debug("Damage rejected? " + damageInfo.rejected);
            }
        }

        public static void SpawnOrb(Vector3 position, Quaternion rotation, TeamIndex teamIndex, int itemCount)
        {
            if (NetworkServer.active)
            {
                GameObject orb = UnityEngine.Object.Instantiate(FleaOrb);
                if (orb)
                {
                    Log.Debug("Flea Orb is loaded");
                }

                orb.transform.position = position;
                orb.transform.rotation = rotation;
                orb.GetComponent<TeamFilter>().teamIndex = teamIndex;

                // * * Additions * * //
                VelocityRandomOnStart trajectory = orb.GetComponent<VelocityRandomOnStart>();
                trajectory.maxSpeed = 35;
                trajectory.minSpeed = 20;
                trajectory.directionMode = VelocityRandomOnStart.DirectionMode.Hemisphere;
                trajectory.coneAngle = 75;
                //trajectory.maxAngularSpeed = 100;

                //orb.GetComponent<ParticleSystem>().startColor = new Color(0f, 0f, 0f, 1f); // ERROR
                //orb.GetComponent<TrailRenderer>().startColor = new Color(0f, 0f, 0f, 1f); // ERROR
                //orb.GetComponent<TrailRenderer>().endColor = new Color(0f, 0f, 0f, 0f); // ERROR

                //orb.GetComponent<DestroyOnTimer>().duration = 5f;
                //Health Pickup
                HealthPickup healthComponent = orb.GetComponentInChildren<HealthPickup>();
                //Log.Debug("Orb has a Health Pickup");
                //healthComponent.flatHealing = 0;
                //healthComponent.fractionalHealing = 0;
                Log.Debug("health Component? " + healthComponent.alive);
                healthComponent.alive = false;
                //*/

                //BuffPickup
                FleaPickup FleaComponent = healthComponent.gameObject.AddComponent<FleaPickup>();

                //FleaPickup FleaComponent = orb.GetComponentInChildren<>().gameObject.AddComponent<FleaPickup>();
                FleaComponent.amount = itemCount;

                FleaComponent.baseObject = orb;
                FleaComponent.teamFilter = orb.GetComponent<TeamFilter>();
                FleaComponent.pickupEffect = FleaEffect;

                orb.GetComponent<Rigidbody>().useGravity = true;
                orb.transform.localScale = Vector3.one * (0.8f + (itemCount / 12));
                //orb.transform.localScale = Vector3.one * (.5f + itemCount / 20);

                Log.Debug("Spawning orb at: " + orb.transform.position);
                NetworkServer.Spawn(orb);
            }
        }
    }
}
