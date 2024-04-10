using RoR2;
using System;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Void;
using UnityEngine.Networking;
using static UltitemsCyan.Items.Void.DownloadedRAM;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Tier1
{

    // TODO: check if Item classes needs to be public
    public class Frisbee : ItemBase
    {
        public static ItemDef item;

        public const float airSpeed = 10f;

        public const float dampeningForce = 0.96f;
        public const float riseSpeed = 2f;

        public const float baseDuration = 1f;
        public const float durationPerStack = 0.5f;

        public override void Init()
        {
            item = CreateItemDef(
                "FRISBEE",
                "Frisbee",
                "Rise and move faster after jumping. Hold jump to keep hovering.",
                "Rise slowly and move 10% (+10% per stack) faster after jumping for 1 (+0.5 per stack) seconds. Hold jump to keep hovering.",
                "Folding Flyers Falling Futher Faster For Five Far Fields",
                ItemTier.Tier1,
                Ultitems.Assets.FrisbeeSprite,
                Ultitems.Assets.FrisbeePrefab,
                [ItemTag.Utility]
            );
        }

        protected override void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.EntityStates.GenericCharacterMain.ProcessJump += GenericCharacterMain_ProcessJump;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                self.AddItemBehavior<FrisbeeBehavior>(self.inventory.GetItemCount(item));
            }
        }
        private void GenericCharacterMain_ProcessJump(On.EntityStates.GenericCharacterMain.orig_ProcessJump orig, EntityStates.GenericCharacterMain self)
        {
            if (self.characterBody && self.characterBody.inventory)
            {
                CharacterBody body = self.characterBody;
                int grabCount = body.inventory.GetItemCount(item);
                
                if (grabCount > 0 && self.hasCharacterMotor && self.jumpInputReceived)
                {
                    //Log.Warning("Is Authority? " + self.isAuthority + " Is Local? " + self.isLocalPlayer + " Is Local Authority? " + self.localPlayerAuthority + " is? " + self.rigidbody);
                    // For both host and not host
                    // True | False | True | rigidbody

                    //Log.Debug("characterMotor, jumpInput, body: " + self.hasCharacterMotor + " | " + self.jumpInputReceived + " | " + self.characterBody);
                    //Log.Debug("Jumps: " + self.characterMotor.jumpCount + " < " + self.characterBody.maxJumpCount);
                    if (self.characterMotor.jumpCount < self.characterBody.maxJumpCount)
                    {
                        //   *   *   *   ADD EFFECT   *   *   *   //

                        Log.Debug("Frisbee Jump ? ? ? adding buff for " + (baseDuration + (durationPerStack * (grabCount - 1))) + " seconds");
                        //self.characterBody.AddTimedBuffAuthority(FrisbeeFlyingBuff.buff.buffIndex, baseDuration + (durationPerStack * (grabCount - 1)));
                        
                        var behavior = self.characterBody.GetComponent<FrisbeeBehavior>();
                        behavior.enabled = true;
                        behavior.UpdateStopwatch(Run.instance.time);
                        body.SetBuffCount(FrisbeeFlyingBuff.buff.buffIndex, 1);

                        Log.Debug("Has Timed def Buff? " + self.HasBuff(FrisbeeFlyingBuff.buff));
                    }
                }
            }
            orig(self);
        }

        


        public class FrisbeeBehavior : CharacterBody.ItemBehavior
        {
            private CharacterMotor characterMotor;
            private const float baseDuration = Frisbee.baseDuration;
            private const float durationPerStack = Frisbee.durationPerStack;
            public float flyingStopwatch = 0;
            private bool _canHaveBuff = false;

            public void UpdateStopwatch(float newTime)
            {
                //Log.Debug("New attack at " + newTime);
                flyingStopwatch = newTime;
            }
            
            public bool CanHaveBuff
            {
                get { return _canHaveBuff; }
                set
                {
                    // If not already the same value
                    if (_canHaveBuff != value)
                    {
                        _canHaveBuff = value;
                        // If if air and holding jump
                        if (_canHaveBuff)
                        {
                            Log.Debug(" * * * Lift Off ! !");
                            // Add Buff if running of edge?
                            //self.characterBody.AddTimedBuff(FrisbeeFlyingBuff.buff, durationPerStack * grabCount);
                        }
                        else
                        {
                            Log.Debug("Fell? / / /");
                            //body.ClearTimedBuffs(FrisbeeFlyingBuff.buff);
                            body.SetBuffCount(FrisbeeFlyingBuff.buff.buffIndex, 0);
                        }
                    }
                }
            }

            // If player is at full health
            public void FixedUpdate()
            {
                if (characterMotor)
                {
                    CanHaveBuff = !characterMotor.isGrounded && body.inputBank.jump.down
                        && Run.instance.time <= flyingStopwatch + baseDuration + (durationPerStack * (stack - 1));
                    if (body.HasBuff(FrisbeeFlyingBuff.buff))
                    {
                        // Player is rising
                        if (body.characterMotor.velocity.y < riseSpeed)
                        {
                            //Log.Debug("Falling?: \t" + body.characterMotor.velocity.y + " = " + ((body.characterMotor.velocity.y * dampeningForce) + (riseSpeed * dampeningForce)));
                            //body.characterMotor.velocity.y -= Time.fixedDeltaTime * Physics.gravity.y * fallReducedGravity;
                            body.characterMotor.velocity.y = (body.characterMotor.velocity.y * dampeningForce) + (riseSpeed * dampeningForce);
                        }
                    }
                }
            }

            public void Start()
            {
                characterMotor = body.characterMotor;
                enabled = false;
            }

            private void OnDisable()
            {
                flyingStopwatch = 0;
                CanHaveBuff = false;
            }

            public void OnDestroy()
            {
                flyingStopwatch = 0;
                CanHaveBuff = false;
            }

        }
    }
}