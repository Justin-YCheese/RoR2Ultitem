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
        private const float threshold = 30f;
        private const float percentHealing = 0f;
        private const float flatHealing = 4f;

        private void Tokens()
        {
            string tokenPrefix = "DRIEDHAM";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Dried Ham");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Heal when hitting enemies below 30% health.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsHealing>Heal</style> for <style=cIsHealing>4</style> <style=cStack>(+4 per stack)</style> when dealing damage to enemies below <style=cIsDamage>30% health</style>. Corrupts all Crème Brûlée.");
            //LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cIsHealing>Heal</style> for <style=cIsHealing>1%</style> plus an additional <style=cIsHealing>4</style> <style=cStack>(+4 per stack)</style> when dealing damage to enemies below <style=cIsDamage>30% health</style>. Corrupts all Crème Brûlée.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "The bitter aftertaste is just the spoilage");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();
            transformItem = CremeBrulee.item;

            // Add text for item
            Tokens();

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.VoidTier1;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.DriedHamSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;

            item.tags = [ItemTag.Healing];

            // ~ * ~ * ~ * ~ * ~ Void Stuff ~ * ~ * ~ * ~ * ~ //


            //item.requiredExpansion = ExpansionCatalog.expansionDefs.FirstOrDefault(x => x.nameToken == "DLC1_NAME");
            /*/
            voidLink = new()
            {
                itemDef1 = transformPair,
                itemDef2 = item
            };//*/
            //Ultitems.CorruptionPairs.Add(voidLink);

            ItemDisplayRuleDict displayRules = new(null);

            
            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            GetTransformItem = transformItem;
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
                if (self && damageInfo.attacker.GetComponent<CharacterBody>() && damageInfo.attacker.GetComponent<CharacterBody>().inventory && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody inflictor = damageInfo.attacker.GetComponent<CharacterBody>();
                    int grabCount = inflictor.inventory.GetItemCount(item);
                    if (grabCount > 0)
                    {
                        Log.Warning("Ham health");
                        //Log.Debug("Health: " + self.health + " Combined Health: " + self.fullHealth + " Combined Fraction: " + self.combinedHealthFraction);
                        if (self.combinedHealthFraction <= threshold / 100f)
                        {
                            //Log.Debug("Heal Attacker, Initial: " + inflictor.healthComponent.health);
                            inflictor.healthComponent.Heal(inflictor.healthComponent.fullHealth * percentHealing / 100f + flatHealing * grabCount, damageInfo.procChainMask);
                            //inflictor.healthComponent.Heal(inflictor.healthComponent.fullHealth * percentHealing / 100f * grabCount + flatHealing, damageInfo.procChainMask);
                            Log.Debug("Healing: " + (inflictor.healthComponent.fullHealth * percentHealing / 100f + flatHealing));
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