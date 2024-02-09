using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class CorrodingVaultConsumed : ItemBase
    {
        public static ItemDef item;

        public override void Init()
        {
            item = CreateItemDef(
                "CORRODINGVAULTCONSUMED",
                "Corroding Vault (Corroded)",
                "It can't protect anything anymore...",
                "DESCRIPTION It can't protect anything anymore...",
                "Rusted Rusted Rusted",
                ItemTier.NoTier,
                Ultitems.Assets.CorrodingVaultConsumedSprite,
                Ultitems.Assets.CorrodingVaultConsumedPrefab,
                [ItemTag.Utility, ItemTag.OnStageBeginEffect, ItemTag.AIBlacklist]
            );
        }

        protected override void Hooks() { }
    }
}