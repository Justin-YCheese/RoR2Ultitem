using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan.Items.Untiered
{

    // TODO: check if Item classes needs to be public
    public class SilverThreadConsumed : ItemBase
    {
        public static ItemDef item;

        public override void Init()
        {
            item = CreateItemDef(
                "SILVERTHREADCONSUMED",
                "Silver Thread (Snapped)",
                "Proof of death",
                "DESCRIPTION Proof of death",
                "This is a garbage death zone. How did you get here?",
                ItemTier.NoTier,
                UltAssets.SilverThreadConsumedSprite,
                UltAssets.SilverThreadConsumedPrefab,
                [ItemTag.Utility, ItemTag.AIBlacklist]
            );
        }

        protected override void Hooks() { }
    }
}