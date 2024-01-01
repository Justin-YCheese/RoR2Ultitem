using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UltitemsCyan.Items
{
    public abstract class FleaMiscPickupDef : ScriptableObject //Or just inherit from MiscPickupDef // Or BuffPickup?
    {
        // Token: 0x060018C7 RID: 6343
        public abstract void GrantPickup(ref PickupDef.GrantContext context);

        // Token: 0x060018C8 RID: 6344 RVA: 0x0006BD05 File Offset: 0x00069F05
        public virtual string GetInternalName()
        {
            return "MiscPickupIndex." + base.name;
        }

        // Token: 0x060018C9 RID: 6345 RVA: 0x0006BD18 File Offset: 0x00069F18
        public virtual PickupDef CreatePickupDef()
        {
            return new PickupDef
            {
                internalName = this.GetInternalName(),
                coinValue = this.coinValue,
                nameToken = this.nameToken,
                displayPrefab = this.displayPrefab,
                dropletDisplayPrefab = this.dropletDisplayPrefab,
                baseColor = ColorCatalog.GetColor(this.baseColor),
                darkColor = ColorCatalog.GetColor(this.darkColor),
                interactContextToken = this.interactContextToken,
                attemptGrant = new PickupDef.AttemptGrantDelegate(this.GrantPickup),
                miscPickupIndex = this.miscPickupIndex
            };
        }

        // Token: 0x04001E61 RID: 7777
        [SerializeField]
        public uint coinValue;

        // Token: 0x04001E62 RID: 7778
        [SerializeField]
        public string nameToken;

        // Token: 0x04001E63 RID: 7779
        [SerializeField]
        public GameObject displayPrefab;

        // Token: 0x04001E64 RID: 7780
        [SerializeField]
        public GameObject dropletDisplayPrefab;

        // Token: 0x04001E65 RID: 7781
        [SerializeField]
        public ColorCatalog.ColorIndex baseColor;

        // Token: 0x04001E66 RID: 7782
        [SerializeField]
        public ColorCatalog.ColorIndex darkColor;

        // Token: 0x04001E67 RID: 7783
        [SerializeField]
        public string interactContextToken;

        // Token: 0x04001E68 RID: 7784
        [NonSerialized]
        public MiscPickupIndex miscPickupIndex;
    }
}
