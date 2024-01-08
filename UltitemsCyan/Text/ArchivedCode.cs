using System;
using System.Collections.Generic;
using System.Text;

namespace UltitemsCyan.Text
{
    internal class ArchivedCode
    {
        /*/ To Find Death Message Order
        private void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            CharacterBody characterBody = self.GetComponent<CharacterBody>();
            Log.Warning("0.1 Sue is taking the damage");
            Log.Debug("Self: " + characterBody.name + " alive? " + characterBody.healthComponent.alive);
            orig(self, damageInfo);
            Log.Debug("0.2 Sue took damage");
            Log.Debug("Self: " + characterBody.name + " alive? " + characterBody.healthComponent.alive);
        }

        // AnythingKilling another thing
        private void CharacterBody_OnKilledOtherServer(On.RoR2.CharacterBody.orig_OnKilledOtherServer orig, CharacterBody self, DamageReport damageReport)
        {
            Log.Warning("1.1 Sue Killed Other Server");
            Log.Debug("Self: " + self.name + " - event manager");
            orig(self, damageReport);
            Log.Debug("Self: " + self.name + " - event manager");
        }
        // Global death event
        private void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            Log.Warning("1.2 Sue's Character Death");
            Log.Debug("Self: " + self.name + " - event manager");
            orig(self, damageReport);
            Log.Debug("1.5 Sue's has Death");
            Log.Debug("Character: " + self.name + " - event manager");
            
        }
        // CharacterBody death effects
        private void CharacterBody_HandleOnKillEffectsServer(On.RoR2.CharacterBody.orig_HandleOnKillEffectsServer orig, CharacterBody self, DamageReport damageReport)
        {
            Log.Warning("1.3 Sue's Handle effects");
            Log.Debug("Self: " + self.name + " alive? " + self.healthComponent.alive);
            orig(self, damageReport);
            Log.Debug("1.4 Sue's has effects hands");
            Log.Debug("Effects: " + self.name + " alive? " + self.healthComponent.alive);

        }
        // Global Handle a hit (after death effects have been checked)
        private void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            CharacterBody characterBody = victim.GetComponent<CharacterBody>();
            Log.Warning("2.1 Sue Hit the Enemy");
            Log.Debug("Self: " + characterBody.name + " alive? " + characterBody.healthComponent.alive);
            orig(self, damageInfo, victim);
            Log.Debug("2.2 Sue has hit");
            Log.Debug("Self: " + characterBody.name + " alive? " + characterBody.healthComponent.alive);
        }
        //*/
    }
}
