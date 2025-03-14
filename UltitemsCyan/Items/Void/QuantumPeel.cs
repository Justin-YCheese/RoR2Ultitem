using RoR2;
using BepInEx.Configuration;
using UltitemsCyan.Items.Tier3;
using R2API;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class QuantumPeel : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        // For buff calculate Logistic Growth of speed
        // Po = 20 | P(50) = 1000
        //public const double pLogiRate = 0.1565f;
        //public const double pLogiLimit = 1020f;
        //public const double pLogiConstant = 50f;

        //public const float peelSpeed = 20f;
        public const float peelSpeedMultiplier = 4f;

        //public const float peelTimer = 2f;
        //public const float slipAcceleration = 0.9f;

        public const int maxPeelStack = 20;

        public override void Init(ConfigFile configs)
        {
            const string itemName = "Quantum Peel";
            if (!CheckItemEnabledConfig(itemName, "Void", configs))
            {
                return;
            }
            item = CreateItemDef(
                "QUANTUMPEEL",
                itemName,
                "Gain speed and jump power when damaged. <style=cIsVoid>Corrupts all Viral Smogs</style>.",
                "Gain up to <style=cIsUtility>400%</style> <style=cStack>(+400% per stack)</style> <style=cIsUtility>movement speed</style> and upto <style=cIsUtility>100%</style> <style=cStack>(+100% per stack)</style> <style=cIsUtility>jump power</style> when <style=cIsHealth>missing health</style>. <style=cIsVoid>Corrupts all Viral Smogs</style>.",
                "This Tape is so slippery! And it just seems endless...\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nWait did you name it Quantum Peel because you wanted a banana item but you added too many food items?\nShhh! They'll never know as long as they never read the description...",
                ItemTier.VoidTier3,
                UltAssets.QuantumPeelSprite,
                UltAssets.QuantumPeelPrefab,
                [ItemTag.Utility],
                ViralSmog.item
            );
        }

        protected override void Hooks()
        {
            // Reset Buff when not hit for a while?
            //On.RoR2.HealthComponent.UpdateLastHitTime += HealthComponent_UpdateLastHitTime;
            //On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            //On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.HealthComponent.Heal += HealthComponent_Heal;
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self, damageInfo);
            if (self && self.body.inventory && self.body.inventory)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //Log.Debug(" D i R t Y > > > damage !");
                    self.body.statsDirty = true;
                }
            }
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            float value = orig(self, amount, procChainMask, nonRegen);
            if (self && self.body.inventory && self.body.inventory)
            {
                int grabCount = self.body.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //Log.Debug(" D i R t Y > > > heal +");
                    self.body.statsDirty = true;
                }
            }
            return value;
        }

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory && sender.characterMotor)
            {
                int grabCount = sender.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    float percent = 1 - sender.healthComponent.combinedHealthFraction;
                    if (percent < 0)
                    {
                        percent = 0;
                    }
                    float speed = percent * percent * peelSpeedMultiplier * grabCount;
                    //Log.Debug(" * * * Speed :: " + speed);
                    args.moveSpeedMultAdd += speed;
                    //Log.Debug(" * * * Base Jump :: " + sender.jumpPower);
                    args.jumpPowerMultAdd += percent;
                }
            }
        }


        /*//
        private void HealthComponent_FixedUpdate(On.RoR2.HealthComponent.orig_FixedUpdate orig, HealthComponent self)
        {
            orig(self);
            if (self.body && self.body.HasBuff(SuperBuff.buff) && self.timeSinceLastHit > superDuration)
            {
                Log.Debug("Removed bananas . . . . . . . . .");
                self.body.SetBuffCount(SuperBuff.buff.buffIndex, 0);
            }
        }//*/

        /*//
        private void HealthComponent_UpdateLastHitTime(On.RoR2.HealthComponent.orig_UpdateLastHitTime orig, HealthComponent self, float damageValue, UnityEngine.Vector3 damagePosition, bool damageIsSilent, UnityEngine.GameObject attacker)
        {
            orig(self, damageValue, damagePosition, damageIsSilent, attacker);
            if (NetworkServer.active && self.body && self.body.inventory)
            {
                CharacterBody body = self.body;
                int grabCount = body.inventory.GetItemCount(item);
                if (grabCount > 0) // && !body.HasBuff(SporkBleedBuff.buff.buffIndex)
                {
                    Log.Debug("Got B A N A N A");
                    for (int i = 0; i < grabCount; i++)
                    {
                        if (body.GetBuffCount(PeelBuff.buff.buffIndex) < maxPeelStack)
                        {
                            body.AddBuff(PeelBuff.buff);
                        }
                        // Enable Peel
                        var behavior = body.GetComponent<QuantumPeelBehaviour>();
                        behavior.ResetCounter();
                        behavior.enabled = true;
                    }
                    // Sound Effect?
                }
            }
        }//*/
        /*//
        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                self.AddItemBehavior<QuantumPeelBehaviour>(self.inventory.GetItemCount(item));
            }
        }

        private float HealthComponent_Heal(On.RoR2.HealthComponent.orig_Heal orig, HealthComponent self, float amount, ProcChainMask procChainMask, bool nonRegen)
        {
            if (self && self.body && amount > 0 && nonRegen)
            {
                Log.Debug(" :: + :: + :: + :: Healing remove Peels");
                self.body.RemoveBuff(PeelBuff.buff);
            }
            return orig(self, amount, procChainMask, nonRegen);
        }

        public class QuantumPeelBehaviour : CharacterBody.ItemBehavior
        {
            // Counter so I can increment when to remove buff
            private int counter = 1;

            private void OnAwake()
            {
                enabled = false;
            }

            public void ResetCounter()
            {
                counter = 1;
            }

            private void FixedUpdate()
            {
                // If too much time has passed since last dealing damage
                if (body.healthComponent.timeSinceLastHit > peelTimer * counter * stack && body.healthComponent.timeSinceLastHit != float.PositiveInfinity)
                {
                    Log.Debug(" :: Quantum P Time: " + body.healthComponent.timeSinceLastHit + " > " + peelTimer + " * " + counter + " * " + stack);
                    counter++;
                    body.RemoveBuff(PeelBuff.buff.buffIndex);
                    if (body.GetBuffCount(PeelBuff.buff.buffIndex) <= 0)
                    {
                        enabled = false;
                    }
                }
            }
        }
        //*/
    }
}