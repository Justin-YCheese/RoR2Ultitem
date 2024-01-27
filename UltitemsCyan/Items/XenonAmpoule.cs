using R2API;
using RoR2;
using RoR2.Projectile;
using UnityEngine;
using RoR2.Projectile;
using UnityEngine.Networking;

namespace UltitemsCyan.Items
{

    // TODO: check if Item classes needs to be public
    public class XenonAmpoule : ItemBase
    {
        private static ItemDef item;
        private static GameObject shockwaveProjectile;

        private const bool isVoid = false;
        //public override bool IsVoid() { return isVoid; }
        private void Tokens()
        {
            string tokenPrefix = "XENONAMPOULE";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Xenon Ampoule");
            LanguageAPI.Add(tokenPrefix + "_PICK", "add me");
            LanguageAPI.Add(tokenPrefix + "_DESC", "<style=cStack>text</style>");
            LanguageAPI.Add(tokenPrefix + "_LORE", "asdf");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            /*/
            shockwaveProjectile = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("prefabs/projectiles/FMJ"), "ShockwaveProjectile", true);
            model = assetBundle.LoadAsset<GameObject>("shockwaveProjectile.prefab");

            model.AddComponent<NetworkIdentity>();
            model.AddComponent<ProjectileGhostController>();

            var projectileController = shockwaveProjectile.GetComponent<ProjectileController>();
            projectileController.ghostPrefab = model;

            PrefabAPI.RegisterNetworkPrefab(shockwaveProjectile);
            ContentAddition.AddProjectile(shockwaveProjectile);
            //*/

            // Add text for item
            Tokens();

            

            // Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier2;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.XenonAmpouleSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Any];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            //Log.Info("Test Item Initialized");
            GetItemDef = item;
            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            EquipmentSlot.onServerEquipmentActivated += EquipmentSlot_onServerEquipmentActivated;
            On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        }

        private bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)
        {
            Log.Debug("Xenon Perform Equipment Action");
            bool firedEquipment = orig(self, equipmentDef);
            Log.Debug("Fired? " + firedEquipment);
            if (firedEquipment)
            {
                Log.Debug(" ? ? ? Xenon Perform Equipment Action Actually Activated?");
                CharacterBody activator = self.characterBody;
                int grabCount = activator.inventory.GetItemCount(item);
                if (grabCount > 0)
                {
                    Log.Debug(" ! Xenon held, fire projectile");
                    FireProjectileInfo fireProjectileInfo = new()
                    {
                        owner = activator.gameObject,
                        damage = activator.baseDamage * 5 * grabCount,
                        position = activator.corePosition,
                        rotation = Util.QuaternionSafeLookRotation(self.inputBank.aimDirection),
                        crit = false,
                        projectilePrefab = Ultitems.mysteryPrefab
                    };
                    ProjectileManager.instance.FireProjectile(fireProjectileInfo);
                }
                return true;
            }
            else
            {
                Log.Debug("X No X");
                return false;
            }

            //return orig(self, equipmentDef);
        }

        private void EquipmentSlot_onServerEquipmentActivated(EquipmentSlot arg1, EquipmentIndex arg2)
        {
            Log.Debug("Xenon Ampoule Equipment Activated");
        }

        /*
         * 
         * 
         */
    }
}