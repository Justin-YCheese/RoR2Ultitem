using R2API;
using RoR2;
using UltitemsCyan.Equipment;
using UnityEngine;
using UnityEngine.Networking;

namespace UltitemsCyan.Buffs
{
    public class YieldsBoostBuff : BuffBase
    {
        public static BuffDef buff;
        private const float multiplier = YieldSign.boostMultiplier;
        private const float horizontalMultiplier = YieldSign.boostHorizontalMultiplier;
        private const float maxMultiplier = YieldSign.boostMaxMultiplier;
        private const float minMultiplier = YieldSign.boostMinMultiplier;

        public override void Init()
        {
            buff = DefineBuff("Yields Boost Buff", false, false, Color.white, Ultitems.Assets.YieldSignSprite, false, true);
            Hooks();
        }

        protected void Hooks()
        {
            //On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (buffDef == buff)
            {
                Log.Debug("Lost Buff boost");
                YieldSign.VelocityMultiplier(ref self.characterMotor.velocity, multiplier, horizontalMultiplier, maxMultiplier, minMultiplier, self.moveSpeed);
                YieldSign.YieldAttack(self);
            }
        }
    }
}