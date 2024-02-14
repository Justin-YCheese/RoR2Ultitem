using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System;
using UnityEngine.ProBuilder.MeshOperations;
using UltitemsCyan.Items.Tier1;


#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier

namespace UltitemsCyan.Component
{
    public class GrapePickup : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
            {
                CharacterBody component = other.GetComponent<CharacterBody>();
                if (component)
                {
                    component.AddBuff(buffDef);
                    Destroy(baseObject);
                }
            }
        }

        private BuffDef buffDef = JunkContent.Buffs.BodyArmor;
        //private BuffDef buffDef = Buffs.SlipperyGrape.buff;

        public GameObject baseObject;
        public TeamFilter teamFilter;
        public GameObject pickupEffect;

        private bool alive = true;
    }
}
