using RoR2;

namespace UltitemsCyan.Buffs
{
    public class SuesTeethBuff : BuffBase
    {
        public static BuffDef buff;

        public override void Init()
        {
            buff = DefineBuff("Sues Teeth Buff", true, false, UltAssets.SuesTeethSprite);
            //Log.Info(buff.name + " Initialized");
            //Hooks();
        }

        /*
        protected void Hooks()
        {

        }
        */

        /*/
        public static void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            // If no attacker then skip
            // ? if (di == null || self.body == null || di.rejected || !di.attacker || di.inflictor == null || di.attacker == self.gameObject) ?
            // Bugged Code, if don't go to orig(self, damageInfo) after finding
            if (damageInfo.attacker)
            {
                CharacterBody attacker = damageInfo.attacker.GetComponent<CharacterBody>();
                int buffCount = attacker.GetBuffCount(buff);

                if (buffCount > 0)
                {
                    //Log.Debug("Birthday Candles Buffs: " + buffCount);
                    //Log.Debug("damage:      \t" + damageInfo.damage);
                    damageInfo.damage *= 1 + birthdayBuffBaseMultiplier + (rottingBuffMultiplier * buffCount);
                    //Log.Debug("damage after:\t" + damageInfo.damage);
                }
            }
            // Has to be after damage changed to update damage
            orig(self, damageInfo);
        }//*/
    }
}