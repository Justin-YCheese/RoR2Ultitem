using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Tier2
{
    // Possibly make it so that it gains 8% +2%perStack max of 32% + 40+
    // 3% +3% max of 10 stacks
    // 5% max of 6 stacks +6
    // 
    // But then would be exponential

    public class OverclockedGPU : ItemBase
    {
        public static ItemDef item;
        //private const int maxOverclockedPerStack = 6;
        private const int maxOverclocked = 10;

        private void Tokens()
        {
            string tokenPrefix = "OVERCLOCKEDGPU";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Overclocked GPU");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Increase attack speed on kill. Resets after getting hurt.");
            //LanguageAPI.Add(tokenPrefix + "_DESC", "Killing an enemy increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>5%</style>. Maximum cap of <style=cIsDamage>30%</style> <style=cStack>(+30% per stack)</style> <style=cIsDamage>attack speed</style>. Lose effect upon getting hit.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Killing an enemy increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>3%</style> <style=cStack>(+3% per stack)</style>. Maximum cap of <style=cIsDamage>10</style> buffs. Lose effect upon getting hit.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "GPU GPU");

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

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.OverclockedGPUSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Damage, ItemTag.OnKillEffect];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
            On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        }

        // Give Overclocked buff on kill
        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            //Log.Warning("Overclocking Killed");
            if (self && damageReport.attacker && damageReport.attackerBody && damageReport.attackerBody.inventory)
            {
                CharacterBody killer = damageReport.attackerBody;
                int grabCount = killer.inventory.GetItemCount(item);
                // If body has the item and has fewer than the max stack then add buff
                if (grabCount > 0 && killer.GetBuffCount(Buffs.Overclockedbuff.buff) < maxOverclocked) // maxOverclockedPerStack * grabCount
                {
                    killer.AddBuff(Buffs.Overclockedbuff.buff);
                }
            }
            // TODO check if goes in beginning or end
            orig(self, damageReport);
        }

        // Remove Overclocked buff when hit
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            //Log.Warning("Overclocking hit");
            if (self && victim && victim.GetComponent<CharacterBody>() && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
            {
                CharacterBody injured = victim.GetComponent<CharacterBody>();
                if (injured.HasBuff(Buffs.Overclockedbuff.buff))
                {
                    injured.SetBuffCount(Buffs.Overclockedbuff.buff.buffIndex, 0);
                }
            }
            orig(self, damageInfo, victim);
        }
    }
}