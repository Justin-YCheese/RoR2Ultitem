using R2API;
using RoR2;
using UnityEngine;
using static UltitemsCyan.Items.Void.QuantumPeel;
using System;
using UltitemsCyan.Items.Void;

namespace UltitemsCyan.Buffs
{
    public class PeelBuff : BuffBase
    {
        public static BuffDef buff;

        public override void Init()
        {
            buff = DefineBuff("Peel buff", true, false, Color.white, UltAssets.PeelSprite, false, false);
            //Log.Info(buff.name + " Initialized");

            Hooks();
        }

        protected void Hooks()
        {
            //RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        /*
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory && sender.HasBuff(buff) && sender.characterMotor)
            {
                int buffCount = sender.GetBuffCount(buff);
                int grabCount = sender.inventory.GetItemCount(item);
                // Calculate Logistic Growth of Speed Po = 20
                //double totalSpeed = Math.E;
                //totalSpeed = 1 + (pLogiConstant * Math.Pow(totalSpeed, buffCount * -pLogiRate));
                //totalSpeed = pLogiLimit / totalSpeed * grabCount / 100;
                //Log.Debug(" s s s @ s s s | Peeling at " + totalSpeed + " but actually? " + (float)totalSpeed);
                args.moveSpeedMultAdd += peelSpeed / 100 * buffCount * grabCount;
            }
        }
        //*/
    }
}