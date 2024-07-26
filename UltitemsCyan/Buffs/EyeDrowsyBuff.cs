using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class EyeDrowsyBuff : BuffBase
    {
        public static BuffDef buff;

        public override void Init()
        {
            buff = DefineBuff("Eye Drowsy Buff", true, false, Color.white, UltAssets.EyeDrowsySprite, false, false);
            Hooks();
        }

        protected void Hooks()
        {
            //RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            //On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
            //On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }
    }
}