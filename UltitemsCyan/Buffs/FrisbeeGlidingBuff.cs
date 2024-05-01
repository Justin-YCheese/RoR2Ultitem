using R2API;
using RoR2;
using UltitemsCyan.Items.Lunar;
using UltitemsCyan.Items.Tier1;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class FrisbeeGlidingBuff : BuffBase
    {
        public static BuffDef buff;
        private const float airSpeed = Frisbee.airSpeed;

        public override void Init()
        {
            buff = DefineBuff("Chrysotope Flying Buff", false, false, Color.white, Ultitems.Assets.FrisbeeGlideSprite, false, false);

            Hooks();
        }

        
        protected void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory && sender.HasBuff(buff))
            {
                int grabCount = sender.inventory.GetItemCount(Frisbee.item);
                args.moveSpeedMultAdd += airSpeed / 100f * grabCount;
            }
        }
    }
}