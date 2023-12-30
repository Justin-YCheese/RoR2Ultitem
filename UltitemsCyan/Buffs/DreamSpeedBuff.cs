using R2API;
using RoR2;
using UltitemsCyan.Items.Lunar;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class DreamSpeedBuff : BuffBase
    {
        public static BuffDef buff;
        private const float dreamSpeedStack = 120f;

        public override void Init()
        {
            buff = DefineBuff("Dream Fuel buff", false, false, Color.cyan, Ultitems.Assets.DreamSpeedSprite, false);
            //Log.Info(buff.name + " Initialized");

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
                int grabCount = sender.inventory.GetItemCount(DreamFuel.item);
                args.moveSpeedMultAdd += dreamSpeedStack / 100f * grabCount;
            }
        }
    }
}