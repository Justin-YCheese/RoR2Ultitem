using R2API;
using RoR2;
using UltitemsCyan.Items;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class BirthdayBuff : BuffBase
    {
        public static BuffDef buff;
        //private const float birthdayBuffBaseMultiplier = 0.2f;
        private const float birthdayBuffMultiplier = Items.Tier2.BirthdayCandles.birthdayBuffMultiplier;

        public override void Init()
        {
            buff = DefineBuff("Birthday Buff", true, false, Color.white, Ultitems.Assets.BirthdaySprite, false, false);
            //Log.Info(buff.name + " Initialized");
            Hooks();
        }

        protected void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(buff))
            {
                int buffCount = sender.GetBuffCount(buff);
                args.damageMultAdd += birthdayBuffMultiplier / 100f * buffCount;
                //Debug.Log(sender.name + "Birthday modifier: " + (rottingBuffMultiplier / 100f * buffCount));
            }
        }

        /*/
        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            // If no attacker then skip
            // ? if (di == null || self.body == null || di.rejected || !di.attacker || di.inflictor == null || di.attacker == self.gameObject) ?
            // Bugged Code, if don't go to orig(self, damageInfo) after finding
            if (damageInfo.attacker)
            {
                CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                int buffCount = attacker.GetBuffCount(buff);

                if (buffCount > 0)
                {
                    //Log.Debug("Birthday Candles Buffs: " + buffCount);
                    //Log.Debug("damage:      \t" + damageInfo.damage);
                    damageInfo.damage *= 1 + birthdayBuffBaseMultiplier + (rottingBuffMultiplier * buffCount);
                    //Log.Debug("damage after:\t" + damageInfo.damage);
                }
            }
            // Has to be after damage changed to update damage
            orig(self, damageInfo);
        }//*/
    }
}