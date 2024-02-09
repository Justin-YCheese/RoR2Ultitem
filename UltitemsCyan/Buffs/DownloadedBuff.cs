using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class DownloadedBuff : BuffBase
    {
        public static BuffDef buff;
        private const float rottingBuffMultiplier = Items.Void.DownloadedRAM.downloadedBuffMultiplier;

        public override void Init()
        {
            buff = DefineBuff("Rotting Buff", true, false, Color.white, Ultitems.Assets.DownloadedSprite, false, false);
            Hooks();
        }

        protected void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.HasBuff(buff))
            {
                int buffCount = sender.GetBuffCount(buff);
                args.damageMultAdd += rottingBuffMultiplier / 100f * buffCount;
            }
        }
    }
}