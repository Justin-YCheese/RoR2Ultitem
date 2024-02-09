using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class InhabitedCoffinConsumed : ItemBase
    {
        public static ItemDef item;

        public override void Init()
        {
            item = CreateItemDef(
                "INHABITEDCOFFINCONSUMED",
                "Inhabited Coffin (Vaccant)",
                "It has been let loose...",
                "DESCRIPTION It has been let loose...",
                "Watch Out!",
                ItemTier.NoTier,
                Ultitems.Assets.InhabitedCoffinConsumedSprite,
                Ultitems.Assets.InhabitedCoffinConsumedPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist]
            );
        }

        protected override void Hooks() { }
    }
}