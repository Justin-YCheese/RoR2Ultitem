/*/
using R2API;
using RoR2;
using System;
using UnityEngine;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class FaultyLight : ItemBase
    {
        public static ItemDef item;
        private const float dontResetFraction = 0.65f;

        private void Tokens()
        {
            string tokenPrefix = "FAULTYLIGHT";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Faulty Light");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Chance to reset a skill after it's used.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Have a <style=cIsUtility>35%</style> <style=cStack>(+35% per stack)</style> chance to <style=cIsUtility>reset a skill cooldown</style>.");
            LanguageAPI.Add(tokenPrefix + "_LORE", "Stacks exponetially");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            Tokens();

            Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier3;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.FaultyBulbSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            // Log.Info("Faulty Bulb Initialized");
            GetItemDef = item;
            Log.Warning("Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
        }

        protected void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            //Log.Warning("Faulty Bulb On Skill Activated...");
            if (skill && skill.skillDef.baseRechargeInterval > 0 && self && self.inventory)
            {
                //Log.Debug("Cooldown remain: " + skill.cooldownRemaining + " Scale: " + skill.cooldownScale + " Base Interval: " + skill.skillDef.baseRechargeInterval + " Reset Cooldown?: " + skill.skillDef.resetCooldownTimerOnUse);
                int grabCount = self.inventory.GetItemCount(item.itemIndex);
                if (grabCount > 0)
                {
                    //Log.Debug("garbCount: " + grabCount);
                    //Log.Debug("itemIndex: " + item.itemIndex);

                    float procChance = 100f;
                    for (int i = 0; i < grabCount; i++)
                    {
                        procChance *= dontResetFraction;
                    } 
                    // fleaDropChance = 100 - dontResetChance ^ n
                    procChance = 100f - procChance;
                    //Log.Debug("fleaDropChance: " + fleaDropChance);
                    bool reset = Util.CheckRoll(procChance, self.master.luck);
                    if (reset)
                    {
                        Log.Debug("Faulty Bulb Reseting for: " + self.GetUserName());
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                        //skill.RestockContinuous(); // Doesn't do anything?
                        //skill.RestockSteplike();
                        skill.ApplyAmmoPack();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                        Util.PlaySound("Play_mage_m2_zap", self.gameObject);
                        Util.PlaySound("Play_mage_m2_zap", self.gameObject);
                        Util.PlaySound("Play_item_proc_chain_lightning", self.gameObject);
                        Util.PlaySound("Play_item_proc_chain_lightning", self.gameObject);
                        //Util.PlaySound("Play_item_proc_chain_lightning", self.gameObject);
                        //Util.PlaySound("Play_mage_m2_impact", self.gameObject);
                        Util.PlaySound("Play_item_use_BFG_explode", self.gameObject);
                    }
                }
            }
            orig(self, skill);
        }
    }
}
//*/