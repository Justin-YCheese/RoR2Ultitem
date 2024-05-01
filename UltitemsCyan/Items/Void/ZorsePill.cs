using R2API;
using RoR2;
using System;
using System.Linq;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UltitemsCyan.Items.Lunar.DreamFuel;
using static UltitemsCyan.Items.Void.DownloadedRAM;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{


    // * * * ~ ~ ~ * * * ~ ~ ~ * * * Change to increase TOTAL DAMAGE * * * ~ ~ ~ * * * ~ ~ ~ * * * //


    // TODO: check if Item classes needs to be public
    public class ZorsePill : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        private const float basePercentHealth = 12f;
        private const float percentHealthPerStack = 3f;
        private const float bossFraction = 1f / 3f;

        public override void Init()
        {
            item = CreateItemDef(
                "ZORSEPILL",
                "ZorsePill",
                "Starve enemies on hit dealing percent health as damage. Corrupts all HMTs",
                "Starve an enemy for 12% (+3%) of their health or 4% (+1%) for bosses. Status resets when reapplied. Corrupts Hot Mix Tape",
                "Get this diet pill now! Eat one and it cut's your weight down. Disclaimer: the microbes inside are definitly not eating you from the inside out.",
                ItemTier.VoidTier2,
                Ultitems.Assets.ZorsePillSprite,
                Ultitems.Assets.ZorsePillPrefab,
                [ItemTag.Damage],
                OverclockedGPU.item
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
