using IL.RoR2.Orbs;
using On.RoR2.Orbs;
using R2API;
using RoR2;
using RoR2.Orbs;
using System;
using UnityEngine;
using UnityEngine.Networking;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class FleaBag : ItemBase
    {
        public static ItemDef item;
        private const float procChance = 100f; //10f
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

            PickupDef bagDrop = new()
            {
                baseColor = new Color(150, 30, 50),
                darkColor = new Color(90, 10, 65),
                //displayPrefab = Ultitems.mysteryPrefab,
                dropletDisplayPrefab = Ultitems.mysteryPrefab,
                
            };

            //RoR2.Orbs.Add(bagDrop);

            //RoR2.Orbs.OrbManager.instance;

            //InstantiatePrefabBehavior.Instantiate(original, Transform)

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
                        bool drop = Util.CheckRoll(procChance, inflictor.master.luck);
                        if (drop)
                        {
                            Log.Debug("dropping flea");
                            //RoR2.BuffPickup.Instantiate(item);
                            SpawnOrb(victim.transform.position, victim.transform.rotation, TeamComponent.GetObjectTeam(inflictor.gameObject), 1);
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

        public static void SpawnOrb(Vector3 position, Quaternion rotation, TeamIndex teamIndex, int itemCount)
        {
            int flatHealing = 16;
            float fractionalHealing = 4f;
            float fractionalHealingPerStack = 4f;


            if (NetworkServer.active)
            {
                GameObject orb = UnityEngine.Object.Instantiate(LegacyResourcesAPI.Load<GameObject>("Prefabs/NetworkedObjects/HealPack"), position, rotation);

                orb.GetComponent<TeamFilter>().teamIndex = teamIndex;
                orb.GetComponentInChildren<HealthPickup>().flatHealing = flatHealing;
                orb.GetComponentInChildren<HealthPickup>().fractionalHealing = (fractionalHealing / 100f) + (fractionalHealingPerStack / 100f * (itemCount - 1));

                //var ror1style = orb.GetComponentInChildren<GravitatePickup>().gameObject.AddComponent<GravitatePickupRoR1Style>();

                //ror1style.targetPosition = position + rotation * Vector3.up * 4f;

                orb.GetComponent<Rigidbody>().useGravity = false;
                orb.transform.localScale = Vector3.one * (1f + orb.GetComponentInChildren<HealthPickup>().fractionalHealing);

                NetworkServer.Spawn(orb);
            }
        }

        /*/
        public class GravitatePickupRoR1Style : MonoBehaviour
        {
            public Vector3 targetPosition;
            public Vector3 targetScale;
            public Vector3 scaleDifference;
            public float lastPositionDifference = Mathf.Infinity;
            public float moveTime = 0f;
            public float moveTimeMax = 2f;
            public float floatTime = 0f;
            public float floatTimeMax = 1f;
            public bool normalBehaviour = false;
        }//*/
    }
}