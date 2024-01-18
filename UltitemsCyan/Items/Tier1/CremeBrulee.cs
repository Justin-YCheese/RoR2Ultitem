using R2API;
using RoR2;
using System;
using UnityEngine;
//using static RoR2.GenericPickupController;

namespace UltitemsCyan.Items.Tier1
{

    // TODO: check if Item classes needs to be public
    public class CremeBrulee : ItemBase
    {
        public static ItemDef item;
        private const float threshold = 95f;
        private const float percentHealing = 4f;
        private const float flatHealing = 16f;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "CREMEBRULEE";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Crème Brûlée");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Heal when hitting enemies above 95% health.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsHealing>Heal</style> for <style=cIsHealing>16</style> plus an additional <style=cIsHealing>4%</style> <style=cStack>(+4% per stack)</style> when dealing damage to enemies above <style=cIsDamage>95% health</style>");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Sugar Crust");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // Add text for item
            Tokens();

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier1;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.CremeBruleeSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Healing];

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        }

        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            try
            {
                // If the victum has an inventory
                // and damage isn't rejected?
                if (self && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected) // && damageInfo.damageType != DamageType.DoT
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        Log.Warning("La Creme health");
                        //Log.Debug("Health: " + self.health + " Combined Health: " + self.fullHealth + " Combined Fraction: " + self.combinedHealthFraction);
                        if (self.combinedHealthFraction >= threshold / 100f)
                        {
                            //Log.Debug("Heal Attacker, Initial: " + inflictor.healthComponent.health);
                            inflictor.healthComponent.Heal((inflictor.healthComponent.fullHealth * percentHealing / 100f * grabCount) + flatHealing, damageInfo.procChainMask);
                            Log.Debug("Healing: " + ((inflictor.healthComponent.fullHealth * percentHealing / 100f) + flatHealing));
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