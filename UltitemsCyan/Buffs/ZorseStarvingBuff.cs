using RoR2;
using UltitemsCyan.Items.Void;
using UnityEngine;
using UnityEngine.AddressableAssets;
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

        private readonly float duration = ZorsePill.duration;
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

            //Log.Warning("1st Custom Count " + CustomDotCount + " | Count: " + DotIndex.Count);
            //RegisterDotDef(null);
            index = RegisterDotDef(dotDef);
            //Log.Debug("2nd Custom Count " + CustomDotCount + " | Count: " + DotIndex.Count);

            Hooks();
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (self && buffDef == buff) // && list.Count > 0
            {
                //Log.Warning("s s s Spawning Fracture Effect ! ! !");
                EffectManager.SpawnEffect(FractureEffect, new EffectData
                {
                    origin = self.corePosition,
                    scale = .2f,
                    color = new Color(0.2392f, 0.8196f, 0.917647f) // Cyan Lunar color
                }, true);
            }
            //Log.Debug("OnBuffFinal Netowrking? " + NetworkServer.active);
        }
    }
}