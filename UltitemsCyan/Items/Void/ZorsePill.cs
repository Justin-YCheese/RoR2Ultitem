using R2API;
using RoR2;
using System;
using System.Linq;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using UnityEngine.Assertions.Must;

using static RoR2.DotController;


//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{


    // * * * ~ ~ ~ * * * ~ ~ ~ * * * Change to increase TOTAL DAMAGE * * * ~ ~ ~ * * * ~ ~ ~ * * * //


    // TODO: check if Item classes needs to be public
    public class ZorsePill : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        //private const float basePercentHealth = 12f;
        //private const float percentHealthPerStack = 3f;
        //private const float bossFraction = 3f;

        private const float percentPerStack = 20f;
        public const float duration = 3f; // Any greater than 3 and the health bar visual dissapears before inflicting damage

        public override void Init()
        {
            item = CreateItemDef(
                "ZORSEPILL",
                "ZorsePill",
                "Starve enemies on hit dealing percent TOTAL damage. Corrupts all HMTs",
                "Starve an enemy for 15% (+15% per stack) of TOTAL damage. Status duration resets when reapplied. Corrupts all HMTs",
                "Get this diet pill now! Eat one and it cut's your weight down. Disclaimer: the microbes inside are definitly not eating you from the inside out.",
                ItemTier.VoidTier2,
                UltAssets.ZorsePillSprite,
                UltAssets.ZorsePillPrefab,
                [ItemTag.Damage],
                HMT.item
            );
        }

        protected override void Hooks()
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
                        Log.Debug("  ...Starving enemy with pill...");
                        // If you have fewer than the max number of downloads, then grant buff

                        //float damageMultiplier = (basePercentHealth + (percentHealthPerStack * (grabCount - 1))) / 100f;
                        InflictDotInfo inflictDotInfo = new()
                        {
                            victimObject = victim,
                            attackerObject = damageInfo.attacker,
                            //totalDamage = 0,
                            dotIndex = ZorseStarvingBuff.index,
                            duration = duration,
                            damageMultiplier = damageInfo.damage / inflictor.damage * grabCount * percentPerStack / 100f,
                            maxStacksFromAttacker = null
                        };
                        InflictDot(ref inflictDotInfo);
                        //EffectManager.SimpleEffect(biteEffect, victim.transform.position, Quaternion.identity, true);
                        //EffectManager.SimpleEffect(biteEffect, victim.transform.position, Quaternion.identity, true);
                        //victim.GetComponent<CharacterBody>().AddTimedBuff();
                    }
                }
            }
            catch (NullReferenceException)
            {
                Log.Debug(" oh...  Zorse Pill had an expected null error");
            }
        }
    }
}
