using R2API;
using RoR2;
using UltitemsCyan.Items.Void;
using UnityEngine;
using static UltitemsCyan.Items.Void.JealousFoe;

namespace UltitemsCyan.Buffs
{
    public class EyeAwakeBuff : BuffBase
    {
        public static BuffDef buff;

        public override void Init()
        {
            buff = DefineBuff("Eye Awake Buff", true, false, Color.white, UltAssets.EyeAwakeSprite, false, false);
            Hooks();
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        // Last stack of awake lost, go into cooldown
        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            orig(self, buffDef);
            if (self && buffDef == buff && self.inventory) // && list.Count > 0
            {
                var behavior = self.GetComponent<JealousFoeBehaviour>();
                Log.Warning("   ?   Do you have a Jelly Foe   ?");
                if (behavior)
                {
                    Log.Debug("Oh ! !!! ! you just lost it...");
                    behavior.SetCooldownPhase();
                    // next is Sleepy LastStackRemoved
                }
            }
        }
    }
}