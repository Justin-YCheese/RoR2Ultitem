using R2API;
using RoR2;
using System;
using System.Linq;
using UltitemsCyan.Buffs;
using UltitemsCyan.Items.Tier2;
using UnityEngine;
using UnityEngine.Assertions.Must;
using static UltitemsCyan.Items.Lunar.DreamFuel;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{


    // * * * ~ ~ ~ * * * ~ ~ ~ * * * Change to increase TOTAL DAMAGE * * * ~ ~ ~ * * * ~ ~ ~ * * * //


    // TODO: check if Item classes needs to be public
    public class DownloadedRAM : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;

        public const float downloadedBuffMultiplier = 10f;
        public const int downloadsPerItem = 4;

        private const float downloadChance = 15f;

        public const float notAttackingDelay = 3f;

        public override void Init()
        {
            item = CreateItemDef(
                "DOWNLOADEDRAM",
                "Downloaded RAM",
                "Chance on hit to increase damage by 10%. Stacks 4 (+4 per stack) times. Lose stacks when not attacking. <style=cIsVoid>Corrupts all Overclocked GPUs</style>.",
                "<style=cIsDamage>15%</style> chance on hit to increase damage by <style=cIsDamage>10%</style>. Maxinum cap of <style=cIsDamage>4</style> <style=cStack>(+4 per stack)</style>. Lose stacks 3 seconds after inflicting damage. <style=cIsVoid>Corrupts all Overclocked GPUs</style>.",
                "The bitter aftertaste is just the spoilage",
                ItemTier.VoidTier2,
                Ultitems.Assets.DownloadedRAMSprite,
                Ultitems.Assets.DownloadedRAMPrefab,
                [ItemTag.Damage],
                OverclockedGPU.item
            );
        }

        protected override void Hooks()
        {
            //On.RoR2.CharacterBody.OnOutOfDangerChanged += CharacterBody_OnOutOfDangerChanged;
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            orig(self);
            if (self && self.inventory)
            {
                self.AddItemBehavior<DownloadedVoidBehavior>(self.inventory.GetItemCount(item));
            }
        }

        /*/
        private void CharacterBody_OnOutOfDangerChanged(On.RoR2.CharacterBody.orig_OnOutOfDangerChanged orig, CharacterBody self)
        {
            Log.Warning(" ! Combat Changed ! ");
            try
            {
                if (self && self.outOfCombat)
                {
                    Log.Debug(self.name + "  ...Leaving Combat");
                    self.SetBuffCount(DownloadedBuff.buff.buffIndex, 0);
                }
                else
                {
                    Log.Debug(self.name + " is Entering Combat ! ! !");
                }
            }
            catch (NullReferenceException)
            {
                Log.Debug("Who Downloaded?");
                Log.Debug("Name: " + self.name);
            }
        }
        //*/

        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            try
            {
                // If the victum has an inventory
                // and damage isn't rejected?
                if (self && victim && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        //Log.Warning("RAM Download ! ! ! Def no Viris");
                        //Log.Debug("Is Crit?: " + damageInfo.crit + " Is out of combat? " + inflictor.outOfCombat);

                        //   *   *   *   ADD EFFECT   *   *   *   //

                        var behavior = inflictor.GetComponent<DownloadedVoidBehavior>();
                        behavior.enabled = true;
                        behavior.UpdateStopwatch(Run.instance.time);
                        if (Util.CheckRoll(downloadChance, inflictor.master.luck))
                        {
                            Log.Debug("downloading");
                            // If you have fewer than the max number of downloads, then grant buff
                            if(inflictor.GetBuffCount(DownloadedBuff.buff) < grabCount * downloadsPerItem)
                            {
                                inflictor.AddBuff(DownloadedBuff.buff);
                            }
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                // If error here then elite errors
                //Log.Warning("???What hit Downloading?");
                //Log.Debug("Attacker: " + damageInfo.attacker.GetComponent<CharacterBody>().name);
                //Log.Debug("Victum " + victim.name);
                //Log.Debug("CharacterBody " + victim.GetComponent<CharacterBody>().name);
                //Log.Debug("Inventory " + victim.GetComponent<CharacterBody>().inventory);
                //Log.Debug("Damage rejected? " + damageInfo.rejected);
            }
        }

        //
        public class DownloadedVoidBehavior : CharacterBody.ItemBehavior
        {
            public const float notAttackingDelay = DownloadedRAM.notAttackingDelay;
            public float attackingStopwatch = 0;
            private bool _attacking = false;

            // Order:
            // Awake(), Enable(), OnStart()
            // Disable(), Destory()

            public void UpdateStopwatch(float newTime)
            {
                //Log.Debug("New attack at " + newTime);
                attackingStopwatch = newTime;
            }

            public bool DealingDamage
            {
                get { return _attacking; }
                set
                {
                    if (_attacking != value)
                    {
                        _attacking = value;
                        Log.Warning(body.name + " attack ram toggeled!: " + _attacking);
                        // If not attacking
                        if (!_attacking)
                        {
                            //   *   *   *   REMOVE EFFECT   *   *   *   //

                            body.SetBuffCount(DownloadedBuff.buff.buffIndex, 0);
                            enabled = false;
                        }
                    }
                }
            }

            private void OnAwake()
            {
                enabled = false;
            }

            private void OnDisable()
            {
                attackingStopwatch = 0;
                _attacking = false;
            }

            private void FixedUpdate()
            {
                // If too much time has passed since last dealing damage
                //Log.Debug("RAM Times: " + attackingStopwatch);
                DealingDamage = Run.instance.time <= attackingStopwatch + notAttackingDelay;
            }
            private void OnCharacterDeathGlobal(DamageReport damageReport)
            {
                Log.Warning("RAM ran this on death?");
            }
        }
        ///
    }
}
