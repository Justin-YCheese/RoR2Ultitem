using R2API;
using RoR2;
using UltitemsCyan.Items;
using UltitemsCyan.Items.Tier2;
using UnityEngine;

namespace UltitemsCyan.Buffs
{
    public class OverclockedBuff : BuffBase
    {
        public static BuffDef buff;
        private const float attackSpeedPerItem = OverclockedGPU.buffAttackSpeedPerItem;

        public override void Init()
        {
            buff = DefineBuff("GPU Buff", true, false, Color.white, Ultitems.Assets.OverclockedSprite, false, false);
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
                int buffCount = sender.GetBuffCount(buff);
                int grabCount = sender.inventory.GetItemCount(OverclockedGPU.item);
                //Log.Debug("Overclocked Amount: " + (attackSpeedPerItem / 100f * grabCount * buffCount));
                args.attackSpeedMultAdd += attackSpeedPerItem / 100f * grabCount * buffCount; //attackSpeedPerStack / 100f * buffCount;
                //Debug.Log("Overclocked modifier: " + (attackSpeedPerItem / 100f * grabCount * buffCount));
            }
        }
    }
}
