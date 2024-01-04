using R2API;
using RoR2;
using UltitemsCyan.Items;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class TickCritBuff : BuffBase
    {
        public static BuffDef buff;
        //private const float birthdayBuffBaseMultiplier = 0.2f;
        private const float baseTickMultiplier = FleaBag.baseTickMultiplier;
        private const float tickPerStack = FleaBag.tickPerStack;

        public override void Init()
        {
            buff = DefineBuff("Tick Crit Buff", true, false, Color.green, Ultitems.Assets.BirthdaySprite, false);
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
                Log.Debug("Tick Crit Added " + (baseTickMultiplier + (tickPerStack * buffCount)));
                args.critAdd += baseTickMultiplier + (tickPerStack * buffCount);
                //Debug.Log(sender.name + "Birthday modifier: " + (birthdayBuffMultiplier / 100f * buffCount));
            }
        }
    }
}