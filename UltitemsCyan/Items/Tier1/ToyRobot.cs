using R2API;
using RoR2;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace UltitemsCyan.Items.Tier1
{

    // TODO: check if Item classes needs to be public
    public class ToyRobot : ItemBase
    {
        public static ItemDef item;
        private void Tokens()
        {
            string tokenPrefix = "TOYROBOT";

            LanguageAPI.Add(tokenPrefix + "_NAME", "Toy Robot");
            LanguageAPI.Add(tokenPrefix + "_PICK", "Grab pickups from further away");
            LanguageAPI.Add(tokenPrefix + "_DESC", "Pull in pickups from <style=cIsUtility>16m</style> <style=cStack>(+8m per stack)</style> away");
            LanguageAPI.Add(tokenPrefix + "_LORE", "They march to you like a song carriers their steps");

            item.name = tokenPrefix + "_NAME";
            item.nameToken = tokenPrefix + "_NAME";
            item.pickupToken = tokenPrefix + "_PICK";
            item.descriptionToken = tokenPrefix + "_DESC";
            item.loreToken = tokenPrefix + "_LORE";
        }

        public override void Init()
        {
            item = ScriptableObject.CreateInstance<ItemDef>();

            // Add text for item
            Tokens();

            // Log.Debug("Init " + item.name);

            // tier
            ItemTierDef itd = ScriptableObject.CreateInstance<ItemTierDef>();
            itd.tier = ItemTier.Tier1;
#pragma warning disable Publicizer001 // Accessing a member that was not originally public
            item._itemTierDef = itd;
#pragma warning restore Publicizer001 // Accessing a member that was not originally public

            item.pickupIconSprite = Ultitems.Assets.ToyRobotSprite;
            item.pickupModelPrefab = Ultitems.mysteryPrefab;

            item.canRemove = true;
            item.hidden = false;


            item.tags = [ItemTag.Utility];

            // TODO: Turn tokens into strings
            // AddTokens();

            ItemDisplayRuleDict displayRules = new(null);

            ItemAPI.Add(new CustomItem(item, displayRules));

            // Item Functionality
            Hooks();

            Log.Warning(" Initialized: " + item.name);
        }

        protected void Hooks()
        {
            On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        }

        private void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)
        {
            if (self && self.inventory)
            {
                self.AddItemBehavior<ToyRobotBehaviour>(self.inventory.GetItemCount(item));
            }
            orig(self);
        }

        public class ToyRobotBehaviour : CharacterBody.ItemBehavior
        {
            private SphereSearch sphereSearch;
            private List<Collider> colliders;
            //private float maxDistance = 0; measuring distance

            public void FixedUpdate()
            {
                if (sphereSearch == null || !body || body.transform.position == null) //Needs to be attatched to a body so we check if its null
                    return;

                sphereSearch.origin = body.transform.position;
                sphereSearch.radius = stack * 8f;

                //GravitationControllers have sphere colliders to check whenever a player is in radius no matter what...
                colliders.Clear();
                sphereSearch.RefreshCandidates().OrderCandidatesByDistance().GetColliders(colliders);
                foreach (var pickUp in colliders)
                {
                    // Get Gravitate Pickup if it has one
                    GravitatePickup gravitatePickup = pickUp.gameObject.GetComponent<GravitatePickup>();
                    if (gravitatePickup && gravitatePickup.gravitateTarget == null)
                    {
                        if (gravitatePickup.teamFilter.teamIndex == body.teamComponent.teamIndex)
                        {
                            /*/ Measure distance
                            var measureDistance = Vector3.Distance(body.transform.position, pickUp.transform.position);
                            if (maxDistance < measureDistance)
                            {
                                maxDistance = measureDistance;
                                Log.Debug("Max distance: " + maxDistance);
                            }//*/
                            //If it does not have a gravitation target, then pull in
                            gravitatePickup.gravitateTarget = body.transform;
                            //Body team = 1f
                            //Pickup team = Player
                            //gravitatePickup.maxSpeed = 40
                            //Gravitate tag: untagged
                            // 16 - 24 -
                        }
                        else
                        {
                            Log.Warning("ToyRobot Pickup is not of same team for " + body.name +
                                "\nGravitate Team: " + gravitatePickup.teamFilter.teamIndex +
                                "\nBody Team: " + body.teamComponent.teamIndex);
                        }
                    }
                }
            }

            public void Start()
            {
                colliders = new List<Collider>();
                sphereSearch = new SphereSearch()
                {
                    mask = LayerIndex.pickups.mask,
                    queryTriggerInteraction = QueryTriggerInteraction.Collide
                    //We do not need to filter by team as a gravitate pickup OnTriggerEnter already does it
                };
            }


            public void OnDestroy()
            {
                //IsFullHealth = false;
            }
        }
    }
}