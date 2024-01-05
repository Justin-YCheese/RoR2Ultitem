using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class FleaTickBuff : BuffBase
    {
        public static BuffDef buff;
        //private const float birthdayBuffBaseMultiplier = 0.2f;
        private const float FleaTickMultiplier = 8f;

        public override void Init()
        {
            buff = DefineBuff("Tick Crit Buff", true, false, Color.red, Ultitems.mysterySprite, false);
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
                Log.Debug("Flea Tick Crit Added " + (FleaTickMultiplier * buffCount));
                args.critAdd += FleaTickMultiplier * buffCount;
                //Debug.Log(sender.name + "Birthday modifier: " + (birthdayBuffMultiplier / 100f * buffCount));
            }
        }
    }
}