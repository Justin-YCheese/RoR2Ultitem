using R2API;
using RoR2;
using UnityEngine;
using static UltitemsCyan.Items.Void.JealousFoe;

namespace UltitemsCyan.Buffs
{
    public class EyeSleepyBuff : BuffBase
    {
        public static BuffDef buff;

        public override void Init()
        {
            buff = DefineBuff("Eye Sleepy Buff", false, false, Color.white, UltAssets.EyeSleepySprite, true, false);
            Hooks();
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (self && buffDef == buff && self.inventory) // && list.Count > 0
            {
                var behavior = self.GetComponent<JealousFoeBehaviour>();
                if (behavior)
                {
                    Log.Debug("The thing sleepy did the thing ! ! !");
                    behavior.SetCollectingPhase();
                    // next is JealousFoeBehaviour GotPickup
                }
            }
        }
    }
}