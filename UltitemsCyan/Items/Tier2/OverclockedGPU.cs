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

        private const int maxOverclocked = 10;

        // For Overclocked Buff
        public const float buffAttackSpeedPerItem = 3f;

        private void Tokens()
        {
            string tokenPrefix = "OVERCLOCKEDGPU";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Overclocked GPU");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Increase attack speed on kill. Stacks 10 times. Resets after getting hurt.");
            //LanguageAPI.Add(tokenPrefix + "_DESC", "Killing an enemy increase <style=cIsDamage>attack speed</style> by <style=cIsDamage>5%</style>. Maximum cap of <style=cIsDamage>30%</style> <style=cStack>(+30% per stack)</style> <style=cIsDamage>attack speed</style>. Lose effect upon getting hit.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Killing an enemy increases <style=cIsDamage>attack speed</style> by <style=cIsDamage>3%</style> <style=cStack>(+3% per stack)</style>. Maximum cap of <style=cIsDamage>10</style> stacks. Lose stacks upon getting hit.");
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
            item.pickupModelPrefab = Ultitems.Assets.OverclockedGPUPrefab;

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
            GetItemDef = item;
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
                int buffCount = killer.GetBuffCount(Buffs.OverclockedBuff.buff);
                // If body has the item and has fewer than the max stack then add buff
                if (grabCount > 0 && buffCount < maxOverclocked) // maxOverclockedPerStack * grabCount
                {
                    // Don't have any buffs yet
                    if (buffCount == 0)
                    {
                        Util.PlaySound("Play_item_goldgat_windup", killer.gameObject);
                    }
                    else
                    {
                        //Play_wCrit
                        Util.PlaySound("Play_item_proc_crowbar", killer.gameObject);
                        Util.PlaySound("Play_wDroneDeath", killer.gameObject);
                    }
                    killer.AddBuff(Buffs.OverclockedBuff.buff);
                }
            }
            // TODO check if goes in beginning or end
            orig(self, damageReport);
        }

        // Remove Overclocked buff when hit
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            orig(self, damageInfo, victim);
            //Log.Warning("Overclocking hit");
            try
            {
                if (self && victim && victim.GetComponent<CharacterBody>() && !damageInfo.rejected && damageInfo.damageType != DamageType.DoT)
                {
                    CharacterBody injured = victim.GetComponent<CharacterBody>();
                    if (injured.HasBuff(Buffs.OverclockedBuff.buff))
                    {
                        Util.PlaySound("Play_item_goldgat_winddown", injured.gameObject);
                        injured.SetBuffCount(Buffs.OverclockedBuff.buff.buffIndex, 0);
                    }
                }
            }
            catch
            {
                Log.Warning("Overloading GPU Hit Error?");
            }
        }
    }
}