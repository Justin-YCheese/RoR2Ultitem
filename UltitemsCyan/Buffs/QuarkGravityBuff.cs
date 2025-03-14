using R2API;
using RoR2;
using UltitemsCyan.Equipment;
using static RoR2.DotController;

namespace UltitemsCyan.Buffs
{
    public class QuarkGravityBuff : BuffBase
    {
        public static BuffDef buff;
        public static DotIndex index;

        //private readonly float duration = ZorsePill.duration;

        //private const float airSpeed = Chrysotope.airSpeed;
        private readonly float speed = OrbitalQuark.buffSpeed;
        private readonly float dampening = OrbitalQuark.buffDampening;

        //private readonly float gravity = OrbitalQuark.buffGravity;
        //private readonly int jumpCount = OrbitalQuark.buffJumps;

        public override void Init()
        {
            buff = DefineBuff("Quark Gravity Buff", false, false, UltAssets.QuarkGravitySprite, false, false, true);
            //Log.Info(buff.name + " Initialized");

            Hooks();
        }

        protected void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.CharacterBody.OnBuffFirstStackGained += CharacterBody_OnBuffFirstStackGained;
            On.RoR2.CharacterBody.OnBuffFinalStackLost += CharacterBody_OnBuffFinalStackLost;
        }

        private void CharacterBody_OnBuffFirstStackGained(On.RoR2.CharacterBody.orig_OnBuffFirstStackGained orig, CharacterBody self, BuffDef buffDef)
        {
            if (self && buffDef == buff)
            {
                //Log.Debug("Got Gravity");
                CharacterMotor chMotor = self.characterMotor;
                if (chMotor)
                {
                    CharacterGravityParameters gravPrarameters = chMotor.gravityParameters;
                    gravPrarameters.channeledAntiGravityGranterCount++;
                    chMotor.gravityParameters = gravPrarameters;
                    chMotor.velocity *= dampening;
                    self.statsDirty = true;
                }
            }
            orig(self, buffDef);
        }

        private void CharacterBody_OnBuffFinalStackLost(On.RoR2.CharacterBody.orig_OnBuffFinalStackLost orig, CharacterBody self, BuffDef buffDef)
        {
            if (self && buffDef == buff)
            {
                CharacterMotor chMotor = self.characterMotor;
                if (chMotor)
                {
                    CharacterGravityParameters gravPrarameters = chMotor.gravityParameters;
                    gravPrarameters.channeledAntiGravityGranterCount--;
                    chMotor.gravityParameters = gravPrarameters;
                    self.statsDirty = true;
                }
            }
            orig(self, buffDef);
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory && sender.teamComponent.teamIndex == TeamIndex.Player && sender.HasBuff(buff))
            {
                args.moveSpeedMultAdd += speed / 100f;
                //attackSpeedPerStack / 100f * buffCount;
                //sender.characterMotor.velocity *= .8;
                //sender.maxJumpCount += jumpCount;
                /*Log.Debug(" v v v v v New Quark Stats:" +
                    "\n\njump\t" + sender.maxJumpCount +
                    "\nGrNeu\t" + sender.characterMotor._gravityParameters.antiGravityNeutralizerCount +
                    "\nusGra\t" + sender.characterMotor.useGravity // true | false when in flying
                    );*/
            }
        }

        /*
        private void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)
        {
            orig(self);
            if (self && self.HasBuff(buff))
            {
                
                self.
                Log.Debug("Orig Bleed Chance: " + self.bleedChance);
                self.bleedChance += bleedChance;
                Log.Debug("New Bleed Chance: " + self.bleedChance);
                //Debug.Log(sender.name + "Birthday modifier: " + (rottingBuffMultiplier / 100f * buffCount));
            }
        }
        //*/
    }
}