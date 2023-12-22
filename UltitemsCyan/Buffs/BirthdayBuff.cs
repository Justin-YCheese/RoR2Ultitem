using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UltitemsCyan.Items;

namespace UltitemsCyan.Buffs
{
    public class BirthdayBuff : BuffBase
    {
        public static BuffDef buff;
        private const float birthdayBuffMultiplier = 0.3f;

        public override void Init()
        {
            buff = DefineBuff("Birthday Candle Buff", true, false, Color.green, Ultitems.mysterySprite, false);
            Log.Info(buff.name + " Initialized");

            Hooks();
        }

        protected void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            // If no attacker then skip
            // ? if (di == null || self.body == null || di.rejected || !di.attacker || di.inflictor == null || di.attacker == self.gameObject) ?
            if (!damageInfo.attacker)
            {
                return;
            }

            CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();

            if (attacker)
            {
                int countCandle = attacker.GetBuffCount(buff);
                if (countCandle > 0)
                {
                    Log.Debug("Birthday Candles Held: " + countCandle);
                    Log.Debug("damage:      \t" + damageInfo.damage);
                    damageInfo.damage *= (birthdayBuffMultiplier * countCandle) + 1;
                    Log.Debug("damage after:\t" + damageInfo.damage);
                }
            }
            // Has to be after damage changed to update damage
            orig(self, damageInfo);
        }
    }
}
