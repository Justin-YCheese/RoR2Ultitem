using R2API;
using RoR2;
using System;
using UltitemsCyan.Items.Tier1;
using UnityEngine;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Void
{

    // TODO: check if Item classes needs to be public
    public class DriedHam : ItemBase
    {
        public static ItemDef item;
        public static ItemDef transformItem;
        private const float threshold = 35f;
        private const float flatHealing = 4f;

        public override void Init()
        {
            item = CreateItemDef(
                "DRIEDHAM",
                "Dried Ham",
                "Heal when hitting enemies below 35% health. <style=cIsVoid>Corrupts all Crème Brûlées</style>.",
                "<style=cIsHealing>Heal</style> for <style=cIsHealing>4</style> <style=cStack>(+5 per stack)</style> when dealing damage to enemies below <style=cIsDamage>35% health</style>. <style=cIsVoid>Corrupts all Crème Brûlées</style>.",
                "The bitter aftertaste is just the spoilage",
                ItemTier.VoidTier1,
                UltAssets.DriedHamSprite,
                UltAssets.DriedHamPrefab,
                [ItemTag.Healing],
                CremeBrulee.item
            );
        }

        protected override void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            try
            {
                if (self && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        //Log.Warning("Ham health");
                        //Log.Debug("Health: " + self.health + " Combined Health: " + self.fullHealth + " Combined Fraction: " + self.combinedHealthFraction);
                        if (self.combinedHealthFraction <= threshold / 100f)
                        {
                            //Log.Debug("Heal Attacker, Initial: " + inflictor.healthComponent.health);
                            inflictor.healthComponent.Heal(flatHealing * grabCount, damageInfo.procChainMask);
                            //inflictor.healthComponent.Heal(inflictor.healthComponent.fullHealth * percentHealing / 100f * grabCount + flatHealing, damageInfo.procChainMask);
                            //Log.Debug("Healing: " + (flatHealing * grabCount));
                            // TODO Change sound
                            Util.PlaySound("Play_item_proc_thorns", inflictor.gameObject);
                        }
                    }
                }
            }
            catch (NullReferenceException)
            {
                //Log.Warning("What La Creme Hit?");
            }
            orig(self, damageInfo);
        }
    }
}