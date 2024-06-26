﻿using HG;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using UltitemsCyan.Items.Lunar;
using UltitemsCyan.Items.Tier1;
using UltitemsCyan.Items.Void;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Networking;
using static R2API.DotAPI;
using static RoR2.DotController;

namespace UltitemsCyan.Buffs
{
    public class ZorseStarvingBuff : BuffBase
    {
        public static BuffDef buff;
        public static DotIndex index;

        public readonly GameObject PreFractureEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/BleedOnHitVoid/PreFractureEffect.prefab").WaitForCompletion();
        public readonly GameObject FractureEffect = Addressables.LoadAssetAsync<GameObject>("RoR2/DLC1/BleedOnHitVoid/FractureImpactEffect.prefab").WaitForCompletion();

        private float duration = ZorsePill.duration;
        //private const float airSpeed = Chrysotope.airSpeed;

        public override void Init()
        {
            buff = DefineBuff("Zorse Starving Buff", false, true, Color.white, UltAssets.ZorseStarveSprite, false, false);

            DotDef dotDef = new()
            {
                associatedBuff = buff,
                damageCoefficient = 1f,
                damageColorIndex = DamageColorIndex.Void,
                interval = duration,
                resetTimerOnAdd = true,
                //terminalTimedBuff
                //terminalTimedBuffDuration
            };
            //var customDotBehaviour = DotAPI.CustomDotBehaviour
            //var customDotVisual = DotAPI.CustomDotVisual.CreateDelegate

            Log.Warning("1st Custom Count " + CustomDotCount + " | Count: " + DotIndex.Count);
            //RegisterDotDef(null);
            index = RegisterDotDef(dotDef);
            Log.Debug("2nd Custom Count " + CustomDotCount + " | Count: " + DotIndex.Count);
            
            Hooks();
        }
        
        protected void Hooks()
        {
            //On.RoR2.DotController.UpdateDotVisuals += DotController_UpdateDotVisuals;
            On.RoR2.DotController.EvaluateDotStacksForType += DotController_EvaluateDotStacksForType;
            //On.RoR2.DotController.InflictDot_refInflictDotInfo += DotController_InflictDot_refInflictDotInfo;
            //On.RoR2.DotController.GetDotDef += DotController_GetDotDef;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
            //On.RoR2.DotController.AddDot += DotController_AddDot;
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (self && buffDef == buff) // && list.Count > 0
            {
                Log.Warning("s s s Spawning Fracture Effect ! ! !");
                EffectManager.SpawnEffect(FractureEffect, new EffectData
                {
                    origin = self.corePosition,
                    scale = .2f,
                    color = new Color(0.2392f, 0.8196f, 0.917647f) // Cyan Lunar color
                }, true);
            }
            Log.Debug("OnBuffFinal Netowrking? " + NetworkServer.active);
        }

        private DotDef DotController_GetDotDef(On.RoR2.DotController.orig_GetDotDef orig, DotIndex dotIndex)
        {
            Log.Debug("\n\nGetting Dot Def " + dotIndex);
            return orig(dotIndex);
        }

        private void DotController_InflictDot_refInflictDotInfo(On.RoR2.DotController.orig_InflictDot_refInflictDotInfo orig, ref InflictDotInfo inflictDotInfo)
        {
            Log.Debug(" + * & Inflicting  - " + inflictDotInfo.dotIndex + " -  & * +");
            orig(ref inflictDotInfo);
        }

        private void DotController_AddDot(On.RoR2.DotController.orig_AddDot orig, DotController self, GameObject attackerObject, float duration, DotIndex dotIndex, float damageMultiplier, uint? maxStacksFromAttacker, float? totalDamage)
        {
            if (attackerObject)
            {
                Log.Debug("DotIndex: " + dotIndex + " | Attacker: " + attackerObject + " | total damage? " + totalDamage);
            }
            orig(self, attackerObject, duration, dotIndex, damageMultiplier, maxStacksFromAttacker, totalDamage);
        }

        /*
        private void DotController_UpdateDotVisuals(On.RoR2.DotController.orig_UpdateDotVisuals orig, DotController self)
        {
            orig(self);
            //Log.Warning("Effect 1");
            if (!self.victimBody)
            {
                return;
            }
            //Log.Debug("Effect 2 " + self.transform.position + " = " + self.victimBody.corePosition);
            //self.transform.position = self.victimBody.corePosition;
            //Log.Debug("Effect 3 " + index);
            if (self.HasDotActive(index))
            {
                Log.Debug("Effect 4");
                if (!self.preFractureEffect)
                {
                    Log.Debug("Effect 5");
                    self.preFractureEffect = PreFractureEffect;
                    return;
                }
            }
            else if (self.preFractureEffect)
            {
                Log.Debug("Effect 6");
                Destroy(self.preFractureEffect);
                self.preFractureEffect = null;
            }
            Log.Debug("Effect 7");
        }
        //*/


        private void DotController_EvaluateDotStacksForType(On.RoR2.DotController.orig_EvaluateDotStacksForType orig, DotController self, DotIndex dotIndex, float dt, out int remainingActive)
        {
            orig(self, dotIndex, dt, out remainingActive);
            Log.Debug("EvaluateDot Netowrking? " + NetworkServer.active);
        }
        //*/
    }
}