using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace UltitemsCyan.Buffs
{
    public class DreamSpeedBuff : BuffBase
    {
        public static BuffDef buff;
        //private const float dreamSpeedStack = 100;

        public override void Init()
        {
            buff = DefineBuff("Dream Fuel buff", false, false, Color.cyan, Ultitems.mysterySprite, false);
            Log.Info(buff.name + " Initialized");

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
                int grabCount = sender.inventory.GetItemCount(Items.DreamFuel.item);
                args.moveSpeedMultAdd += grabCount;
            }
        }
    }
}
