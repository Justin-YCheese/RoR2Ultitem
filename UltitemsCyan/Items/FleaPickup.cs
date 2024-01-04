using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier

namespace UltitemsCyan.Items
{
    public class FleaPickup : MonoBehaviour
    {
        // Gets used anyways
        private void OnTriggerStay(Collider other)
        {
            //Log.Debug("On Trigger Happened!");
            if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
            {
                CharacterBody component = other.GetComponent<CharacterBody>();
                if (component)
                {
                    //TODO clear old buffs and replace with new buffs
                    Log.Debug("Flea On Trigger Happened again! " + (buffDuration + (buffDurationPerItem * stack)));

                    if(component.GetBuffCount(buffDef) < maxStack)
                    {
                        component.AddTimedBuff(buffDef.buffIndex, buffDuration + (buffDurationPerItem * stack));
                    }
                    /*/
                    int addedStacks = Math.Min(stack, maxStack - component.GetBuffCount(buffDef));
                    Log.Debug("Adding " + addedStacks + " Flea Buffs");
                    for (int i = 0; i < addedStacks; i++)
                    {
                        component.AddTimedBuff(buffDef.buffIndex, buffDuration);
                    }
                    //*/
                        //EffectManager.SpawnEffect(pickupEffect, new EffectData { origin = transform.position }, true);
                    Destroy(baseObject);
                }
            }
        }

        public BuffDef buffDef = Buffs.FleaTickBuff.buff;
        public float buffDuration = 8f;
        public float buffDurationPerItem = 6f;
        public int maxStack = 15;
        public int stack;
        
        public GameObject baseObject;
        public TeamFilter teamFilter;
        public GameObject pickupEffect;

        private bool alive = true;
    }
}
