﻿using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Tier3
{

    // TODO: check if Item classes needs to be public
    public class ViralSmog : ItemBase
    {
        public static ItemDef item;
        private const float speedPerStackStatus = 25f;

        public override void Init()
        {
            item = CreateItemDef(
                "VIRALSMOG",
                "Viral Smog",
                "Increase speed per unique status effect.",
                "Increases <style=cIsUtility>movement speed</style> by <style=cIsUtility>25%</style> <style=cStack>(+25% per stack)</style> per <style=cIsDamage>unique status</style> you have.",
                "Illness",
                ItemTier.Tier3,
                Ultitems.Assets.ViralSmogSprite,
                Ultitems.Assets.ViralSmogPrefab,
                [ItemTag.Utility]
            );
        }

        protected override void Hooks()
        {
            // Perhaps I don't need this?
            //On.RoR2.CharacterBody.UpdateBuffs += CharacterBody_UpdateBuffs;
            RecalculateStatsAPI.GetStatCoefficients += RecalculateStatsAPI_GetStatCoefficients;
        }

        //TODO add recalculate on get status effect?

        private void RecalculateStatsAPI_GetStatCoefficients(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender && sender.inventory)
            {
                int grabCount = sender.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    //Log.Warning("Viral Smog Active");

                    // Get active buffs list and how many in that list is actually active (length)
                    int activeBuffLength = sender.activeBuffsListCount;
                    BuffIndex[] activeBuffs = sender.activeBuffsList;
                    //Log.Debug("Active Count Length: " + activeBuffLength);

                    int nonCooldownBuffs = activeBuffLength;
                    for (int i = 0; i < activeBuffLength; i++)
                    {
                        BuffDef buffDef = BuffCatalog.GetBuffDef(activeBuffs[i]);
                        if (buffDef.isCooldown)
                        {
                            nonCooldownBuffs--;
                            //Log.Debug(" ~ Cooldown: " + buffDef.name + "\tnew nonCool Count: " + nonCooldownBuffs);
                        }
                        else
                        {
                            //Log.Debug("Not Cool: " + buffDef.name);
                        }
                    }
                    //Log.Debug("Viral Smog\nCount: " + nonCooldownBuffs + "\n"
                    //    + "Speed from Virus: " + speedPerStackStatus / 100f * nonCooldownBuffs * grabCount);
                    // Gives 30% speed per status per item
                    if (activeBuffLength > 0)
                    {
                        args.moveSpeedMultAdd += speedPerStackStatus / 100f * nonCooldownBuffs * grabCount;
                    }
                    else
                    {
                        //Log.Debug("Didn't apply speed buff");
                    }
                }
            }
        }
    }
}