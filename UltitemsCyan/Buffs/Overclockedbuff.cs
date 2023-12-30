using R2API;
using RoR2;
using System.Drawing;
using UltitemsCyan.Items.Tier2;

namespace UltitemsCyan.Buffs
{
    public class Overclockedbuff : BuffBase
    {
        public static BuffDef buff;
        //private const float attackSpeedPerStack = 5f; // 8%
        private const float attackSpeedPerItem = 3f; // 8%

        public override void Init()
        {
            buff = DefineBuff("GPU Buff", true, false, Color.White, Ultitems.Assets.OverclockedSprite, false);
            //Log.Info(buff.name + " Initialized");

            Hooks();
        }

        protected void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (!(sender != null || !sender.inventory || !sender.HasBuff(buff)))
            {
                int buffCount = sender.GetBuffCount(buff);
                int grabCount = sender.inventory.GetItemCount(OverclockedGPU.item);
                args.attackSpeedMultAdd += attackSpeedPerItem / 100f * grabCount * buffCount; //attackSpeedPerStack / 100f * buffCount;
                //Debug.Log("Overclocked modifier: " + (attackSpeedPerItem / 100f * grabCount * buffCount));
            }
        }
    }
}
