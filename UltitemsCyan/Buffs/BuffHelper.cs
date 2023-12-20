using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltitemsCyan.Buffs
{
    public class BuffHelper : MonoBehaviour
    {
        public static BuffDef candleBuff;
        private static float candleBuffMultiplier = 0.3f;

        public static void CreateBuffs()
        {
            Sprite candleSprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
            candleBuff = DefineBuff("Candle Buff", true, false, Color.red, candleSprite, false);

            Log.Info("Candle Buff Initialized");

            Hooks();
        }

        public static BuffDef DefineBuff(string name, bool canStack, bool isDebuff, Color color, Sprite icon, bool isHidden)
        {
            BuffDef definition = ScriptableObject.CreateInstance<BuffDef>();
            definition.name = name;
            definition.canStack = canStack;
            definition.isDebuff = isDebuff;
            definition.buffColor = color;
            definition.iconSprite = icon;
            definition.eliteDef = null;
            definition.isHidden = isHidden;
            
            ContentAddition.AddBuffDef(definition);

            return definition;
        }

        public static void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)
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
                int countCandle = attacker.GetBuffCount(candleBuff);
                if (countCandle > 0)
                {
                    Log.Debug("Birthday Candles Held: " + countCandle);
                    Log.Debug("damage:      \t" + damageInfo.damage);
                    damageInfo.damage *= (candleBuffMultiplier * countCandle) + 1;
                    Log.Debug("damage after:\t" + damageInfo.damage);
                }
            }
            // Has to be after damage changed to update damage
            orig(self, damageInfo);
        }
    }
}
