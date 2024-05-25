using R2API;
using RoR2;
using UltitemsCyan.Equipment;
using UnityEngine;
using UnityEngine.Networking;

namespace UltitemsCyan.Buffs
{
    public class YieldsStopBuff : BuffBase
    {
        public static BuffDef buff;
        private const float multiplier = YieldSign.stopMultiplier;
        private const float horizontalMultiplier = YieldSign.stopHorizontalMultiplier;
        private const float maxMultiplier = YieldSign.stopMaxMultiplier;
        private const float minMultiplier = YieldSign.stopMinMultiplier;

        public override void Init()
        {
            buff = DefineBuff("Yields Stop Buff", false, false, Color.white, Ultitems.Assets.YieldSignSprite, false, true);
            Hooks();
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (buffDef == buff)
            {
                Log.Debug("Lost Buff stop");
                YieldSign.VelocityMultiplier(ref self.characterMotor.velocity, multiplier, horizontalMultiplier, maxMultiplier, 0, self.moveSpeed);
                YieldSign.YieldAttack(self);
            }
        }
    }
}