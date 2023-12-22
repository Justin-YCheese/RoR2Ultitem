using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class FaultyBulb : ItemBase
    {
        public static ItemDef item;
        private const float dontResetFraction = 0.8f;
        private void Tokens()
        {
            string tokenPrefix = "FAULTYBULB";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Faulty Bulb");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Chance to reset cooldown.");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Have a <style=cIsUtility>20%</style> <style=cStack>(+20% per stack)</style> chance to <style=cIsUtility>reset a skill cooldown</style>.");
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

            item.pickupIconSprite = Ultitems.mysterySprite;
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
            Log.Warning("Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnSkillActivated += CharacterBody_OnSkillActivated;
        }

        protected void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)
        {
            //Log.Warning("Faulty Bulb On Skill Activated...");
            if (skill && self && self.inventory)
            {
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
                    // procChance = 100 - dontResetChance ^ n
                    procChance = 100f - procChance;
                    //Log.Debug("procChance: " + procChance);
                    bool reset = Util.CheckRoll(procChance);
                    if (reset)
                    {
                        Log.Debug("Faulty Bulb Reseting for: " + self.name);
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
                        skill.RestockSteplike();
#pragma warning restore Publicizer001 // Accessing a member that was not originally public
                    }
                }
            }
            orig(self, skill);
        }
    }
}