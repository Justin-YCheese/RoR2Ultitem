using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine.Networking;
using UnityEngine;
using RoR2;

namespace UltitemsCyan.Items
{
    // Token: 0x020005C5 RID: 1477
    public class FealPickup : MonoBehaviour
    {
        // Token: 0x06001AC6 RID: 6854 RVA: 0x00072EB8 File Offset: 0x000710B8
        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
            {
                SkillLocator component = other.GetComponent<SkillLocator>();
                if (component)
                {
                    alive = false;
                    component.ApplyAmmoPack();
                    EffectManager.SimpleEffect(pickupEffect, base.transform.position, Quaternion.identity, true);
                    Destroy(baseObject);
                }
            }
        }

        // Token: 0x040020E6 RID: 8422
        //[Tooltip("The base object to destroy when this pickup is consumed.")]
        public GameObject baseObject;

        // Token: 0x040020E7 RID: 8423
        //[Tooltip("The team filter object which determines who can pick up this pack.")]
        public TeamFilter teamFilter;

        // Token: 0x040020E8 RID: 8424
        public GameObject pickupEffect;

        // Token: 0x040020E9 RID: 8425
        private bool alive = true;
    }
}