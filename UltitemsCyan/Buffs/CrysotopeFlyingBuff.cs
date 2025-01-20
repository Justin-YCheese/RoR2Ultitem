using RoR2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class CrysotopeFlyingBuff : BuffBase
    {
        public static BuffDef buff;
        //private const float airSpeed = Chrysotope.airSpeed;

        public override void Init()
        {
            buff = DefineBuff("Crysotope Flying Buff", false, false, Color.white, UltAssets.CrysotopeFlySprite, false, false);

            //Hooks();
        }

        /*
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
        //*/
    }
}