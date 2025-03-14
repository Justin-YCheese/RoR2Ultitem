using RoR2;
using System;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using BepInEx.Configuration;

using static RoR2.DotController;

//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{
    /* Notes:
     * 
     * Moves which deal zero damage will still trigger Zorse and deal a non zero amount of damage
     * Is great with KNockback fin because both multiply total damage dealt (but it's real hard to proc both)
     * 
     * Can spread with Noxious Thorns but doesn't transfer damage multiplier (only base damage transfered)
     *      Check: public void TriggerEnemyDebuffs(DamageReport damageReport)
     *             VineOrb::OnArrival | damageMultiplier = 1f
     * 
     * Use SendDamageDealt instead of DamageReport_ctor?
     * 
     * 
     * 
     * 
     * 
     */


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

        public override void Init(ConfigFile configs)
        {
            const string itemName = "ZorsePill";
            if (!CheckItemEnabledConfig(itemName, "Void", configs))
            {
                return;
            }
            item = CreateItemDef(
                "ZORSEPILL",
                itemName,
                "Starve enemies on hit to deal delayed damage. <style=cIsVoid>Corrupts all HMTs</style>.",
                "Starve an enemy for <style=cIsDamage>20%</style> <style=cStack>(+20% per stack)</style> of TOTAL damage. Status duration <style=cIsDamage>resets</style> when reapplied. <style=cIsVoid>Corrupts all HMTs</style>.",
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
            On.RoR2.DamageReport.ctor += DamageReport_ctor; // Gets TOTAL damage
            //On.RoR2.HealthComponent.SendDamageDealt += HealthComponent_SendDamageDealt;
        }

        /*
        private void HealthComponent_SendDamageDealt(On.RoR2.HealthComponent.orig_SendDamageDealt orig, DamageReport damageReport)
        {
            throw new NotImplementedException();
        }
        //*/

        private void DamageReport_ctor(On.RoR2.DamageReport.orig_ctor orig, DamageReport self, DamageInfo damageInfo, HealthComponent victim, float damageDealt, float combinedHealthBeforeDamage)
        {
            orig(self, damageInfo, victim, damageDealt, combinedHealthBeforeDamage);
            try
            {
                GameObject victimObject = victim.body.gameObject;
                // If the victum has an inventory
                // and damage isn't rejected?
                if (victimObject && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        //Log.Debug("  ...Starving enemy with reports...");
                        // If you have fewer than the max number of downloads, then grant buff

                        //float damageMultiplier = (basePercentHealth + (percentHealthPerStack * (grabCount - 1))) / 100f;
                        //Log.Debug("Damage = " + damageDealt + " | i.damage: " + inflictor.damage + " i.damageRecalc: " + inflictor.damageFromRecalculateStats
                        //    + " di.damage: " + damageInfo.damage + " di.crit: " + damageInfo.crit
                        //    + " multiplier:" + damageInfo.damage / inflictor.damageFromRecalculateStats * (damageInfo.crit ? 2 : 1) * grabCount * percentPerStack / 100f);
                        InflictDotInfo inflictDotInfo = new()
                        {
                            victimObject = victimObject,
                            attackerObject = damageInfo.attacker,
                            //totalDamage = 0,
                            dotIndex = ZorseStarvingBuff.index,
                            duration = duration,
                            //damageMultiplier = damageInfo.damage / inflictor.damage * grabCount * percentPerStack / 100f,
                            damageMultiplier = damageInfo.damage / inflictor.damageFromRecalculateStats * (damageInfo.crit ? 2 : 1) * grabCount * percentPerStack / 100f,
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
                //Log.Debug(" oh...  Zorse Pill had an expected null error");
            }
        }
    }
}
