using R2API;
using RoR2;

namespace UltitemsCyan.Buffs
{
    public class RottingBuff : BuffBase
    {
        public static BuffDef buff;
        private const float rottingBuffMultiplier = Items.Void.RottenBones.rottingBuffMultiplier;

        public override void Init()
        {
            buff = DefineBuff("Rotting Buff", true, false, UltAssets.RottingSprite);
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