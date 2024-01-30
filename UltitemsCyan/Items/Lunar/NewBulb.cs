using R2API;
using RoR2;
using System;
using System.Linq;
using UnityEngine;

namespace UltitemsCyan.Items.Lunar
{

    // TODO: check if Item classes needs to be public
    public class NewBulb : ItemBase
    {
        public static ItemDef item;
        private const float dontResetFraction = 0.50f;

        private void Tokens()
        {
            string tokenPrefix = "NEWBULB";

            LanguageAPI.Add(tokenPrefix + "_NAME", "New Bulb");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Chance to instantly reset a skill after it's used but triples all cooldown");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Have a <style=cIsUtility>50%</style> <style=cStack>(+50% per stack)</style> chance to <style=cIsUtility>reset a skill cooldown</style> but</style> <style=cStack>(Triple all cooldowns per stack)</style>");
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
            itd.tier = ItemTier.Lunar;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.NewBulbSprite;
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
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
            On.RoR2.GenericSkill.CalculateFinalRechargeInterval += GenericSkill_CalculateFinalRechargeInterval;
        }

        private float GenericSkill_CalculateFinalRechargeInterval(On.RoR2.GenericSkill.orig_CalculateFinalRechargeInterval orig, GenericSkill self)
        {
            return self.baseRechargeInterval > 0 ? Mathf.Max(0.5f, self.baseRechargeInterval * self.cooldownScale - self.flatCooldownReduction) : 0;
        }

        protected void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            //Log.Warning("Faulty Bulb On Skill Activated...");
            if (skill && skill.skillDef.baseRechargeInterval > 0 && self && self.inventory)
            {
                //Log.Debug("Cooldown remain: " + skill.cooldownRemaining + " Scale: " + skill.cooldownScale + " Base Interval: " + skill.skillDef.baseRechargeInterval + " Reset Cooldown?: " + skill.skillDef.resetCooldownTimerOnUse);
                int grabCount = self.inventory.GetItemCount(item.itemIndex); // Change Luck
                if (grabCount > 0)
                {
                    grabCount += (int)self.master.luck;
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
                    bool reset = Util.CheckRoll(procChance);
                    if (reset)
                    {
                        Log.Debug("New Bulb Reseting for: " + self.GetUserName());
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
                    else
                    {
                        //Log.Debug("Cooldowb Scale for: " + self.name);
                        //skill.cooldownScale = 2^grabCount;
                    }
                }
            }
            orig(self, skill);
        }
        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                int grabCount = sender.inventory.GetItemCount(item);
                if(grabCount > 0)
                {
                    int increase = 1;
                    for (int i = 0; i < grabCount;i++)
                    {
                        increase *= 3;
                    }
                    increase--;
                    //Log.Debug("New Bulb Cooldown Extend? " + (increase + 1));
                    args.cooldownMultAdd += increase;
                }
            }
        }
    }
}