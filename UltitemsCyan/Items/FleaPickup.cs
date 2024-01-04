using UnityEngine;
using UnityEngine.Networking;
using RoR2;
using System;
using UnityEngine.ProBuilder.MeshOperations;

#pragma warning disable IDE0051 // Remove unused private members
#pragma warning disable IDE0044 // Add readonly modifier

namespace UltitemsCyan.Items
{
    public class FleaPickup : MonoBehaviour
    {
        private void OnTriggerStay(Collider other)
        {
            if (NetworkServer.active && alive && TeamComponent.GetObjectTeam(other.gameObject) == teamFilter.teamIndex)
            {
                CharacterBody component = other.GetComponent<CharacterBody>();
                if (component)
                {
                    int amountOfStacks = Math.Min(amount, maxStack);
                    float duration = baseBuffDuration + (buffDurationPerItem * amount);
                    Log.Debug("Flea On Trigger Happened! amount: " + amountOfStacks + " duration: " + duration);
                    for (int i = 0; i < amountOfStacks; i++)
                    {
                        Log.Debug(" . add tick " + i);
                        component.AddTimedBuff(buffDef, duration, amountOfStacks);
                    }
                    //EffectManager.SpawnEffect(pickupEffect, new EffectData { origin = transform.position }, true);
                    Destroy(baseObject);
                }
            }
        }

        private BuffDef buffDef = Buffs.TickCritBuff.buff;
        private float baseBuffDuration = FleaBag.baseBuffDuration;
        private float buffDurationPerItem = FleaBag.buffDurationPerItem;
        private int maxStack = FleaBag.buffMaxStack;
        public int amount;
        
        public GameObject baseObject;
        public TeamFilter teamFilter;
        public GameObject pickupEffect;

        private bool alive = true;
    }
}
