using R2API;
using RoR2;
using UnityEngine;

namespace UltitemsCyan
{

    // TODO: check if Item classes needs to be public
    public abstract class VoidBase : ItemBase
    {
        
        ItemDef item;
        //public abstract bool IsVoid();

        // protected abstract void Hooks();


        // Start of the Run
        // Used to reset some parameters
        //     Hook:
        // On.RoR2.Run.Start += Run_Start
        //protected void Run_Start(On.RoR2.Run.orig_Start orig, Run self)


        // On kill effects
        // On Death effects? (i.e. Dio Bear)
        //
        //     Hook:
        // On.RoR2.GlobalEventManager.OnCharacterDeath += GlobalEventManager_OnCharacterDeath;
        //protected void GlobalEventManager_OnCharacterDeath(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)


        // Also on kill effect?
        //
        //     Hook:
        // On.RoR2.CharacterBody.HandleOnKillEffectsServer += CharacterBody_OnKillEffects;
        //protected void CharacterBody_HandleOnKillEffectsServer(On.RoR2.CharacterBody.orig_HandleOnKillEffectsServer orig, CharacterBody self, DamageReport damageReport)


        // For items which increase a character stat, passive effects (i.e. hopoo feather or pearl)
        // or possible one time effects when picked up or removed 
        //
        //     Hook:
        // On.RoR2.CharacterBody.OnInventoryChanged += CharacterBody_OnInventoryChanged;
        //protected void CharacterBody_OnInventoryChanged(On.RoR2.CharacterBody.orig_OnInventoryChanged orig, CharacterBody self)


        // For when a chest is openned and generates items (i.e. eulogy zero)
        //
        //     Hook:
        // On.RoR2.ChestBehavior.ItemDrop += ChestBehavior_ItemDrop
        //protected void ChestBehavior_ItemDrop(On.RoR2.ChestBehavior.orig_ItemDrop orig, ChestBehavior self)


        // On damage taken (For handeling when damage is actually taken, i.e. tougher times or power flask)
        //
        //     Hook:
        // On.RoR2.HealthComponent.TakeDamage += HealthComponent_TakeDamage;
        //protected void HealthComponent_TakeDamage(On.RoR2.HealthComponent.orig_TakeDamage orig, RoR2.HealthComponent self, RoR2.DamageInfo damageInfo)


        // On enemy hit (For checking procs, i.e. bleeding or atg missle)
        //
        //     Hook:
        // On.RoR2.GlobalEventManager.OnHitEnemy += GlobalEventManager_OnHitEnemy;
        //protected void GlobalEventManager_OnHitEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)


        // When you use your equipment
        //
        //     Hook:
        // On.RoR2.EquipmentSlot.PerformEquipmentAction += EquipmentSlot_PerformEquipmentAction;
        //protected bool EquipmentSlot_PerformEquipmentAction(On.RoR2.EquipmentSlot.orig_PerformEquipmentAction orig, EquipmentSlot self, EquipmentDef equipmentDef)


        // Whenever the player or an enemy spawns into the stage
        //
        //     Hook:
        // CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        //protected void CharacterBody_onBodyStartGlobal(CharacterBody self)


        // When interacting with anything?
        //
        //     Hook:
        // On.RoR2.GlobalEventManager.OnInteractionBegin += GlobalEventManager_OnInteractionBegin;
        //protected void GlobalEventManager_OnInteractionBegin(On.RoR2.GlobalEventManager.orig_OnInteractionBegin orig, GlobalEventManager self, Interactor interactor, IInteractable interactable, GameObject interactableObject)


        // When a skill is activated
        //
        //     Hook:
        // On.RoR2.CharacterBody.OnSkillActivated += FireProjectile;
        //protected void CharacterBody_OnSkillActivated(On.RoR2.CharacterBody.orig_OnSkillActivated orig, CharacterBody self, GenericSkill skill)


        // When a skill is executed
        //
        //     Hook:
        // On.RoR2.Skills.SkillDef.OnExecute += SkillDef_OnExecute;
        //protected void SkillDef_OnExecute(On.RoR2.Skills.SkillDef.orig_OnExecute orig, RoR2.Skills.SkillDef self, GenericSkill skillSlot)


        // Interacting with teleporter
        //
        //     Hook:
        // On.RoR2.TeleporterInteraction.OnInteractionBegin += TeleporterInteraction_OnInteractionBegin;
        //protected void TeleporterInteraction_OnInteractionBegin(On.RoR2.TeleporterInteraction.orig_OnInteractionBegin orig, TeleporterInteraction self, Interactor activator);


        // Spawning Boss
        //
        //     Hook:
        // On.RoR2.CombatDirector.SetNextSpawnAsBoss += CombatDirector_SetNextSpawnAsBoss;
        //protected void CombatDirector_SetNextSpawnAsBoss(On.RoR2.CombatDirector.orig_SetNextSpawnAsBoss orig, CombatDirector self);


        // When purchasing something
        //
        //     Hook:
        // On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin;
        //protected void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)


        // When recieving money
        //
        //     Hook:
        // On.RoR2.CharacterMaster.GiveMoney += CharacterMaster_GiveMoney;
        //protected void CharacterMaster_GiveMoney(On.RoR2.CharacterMaster.orig_GiveMoney orig, CharacterMaster self, uint amount)


        // RecalculateStats of player, usually if stat increase isn't from picking up the item (i.e. Infusion)
        //
        //     Hook:
        // On.RoR2.CharacterBody.RecalculateStats += CharacterBody_RecalculateStats;
        //protected void CharacterBody_RecalculateStats(On.RoR2.CharacterBody.orig_RecalculateStats orig, CharacterBody self)


        // Hook to various event triggers like the newt spawning detection
        //
        //     Hook:
        // On.RoR2.OnPlayerEnterEvent.OnTriggerEnter += OnPlayerEnterEvent_OnTriggerEnter;
        //protected void OnPlayerEnterEvent_OnTriggerEnter(On.RoR2.OnPlayerEnterEvent.orig_OnTriggerEnter orig, OnPlayerEnterEvent self, Collider other)


        // Can change the new price of a shrine of chance
        //
        //     Hook:
        // On.RoR2.ShrineChanceBehavior.FixedUpdate += ShrineChanceBehavior_FixedUpdate;
        //protected void ShrineChanceBehavior_FixedUpdate(On.RoR2.ShrineChanceBehavior.orig_FixedUpdate orig, ShrineChanceBehavior self)


        // When the player picks up a specific item (count is how many items were picked up)
        // Called after itemDef
        // 
        //     Hook:
        // On.RoR2.Inventory.GiveItem_ItemIndex_int += Inventory_GiveItem_ItemIndex_int;
        //protected void Inventory_GiveItem_ItemIndex_int(On.RoR2.Inventory.orig_GiveItem_ItemIndex_int orig, Inventory self, ItemIndex itemIndex, int count)


        // When the player is given a specific item
        //
        //     Hook:
        // On.RoR2.Inventory.GiveItem_ItemDef_int += Inventory_GiveItem_ItemDef_int;
        //protected void Inventory_GiveItem_ItemDef_int(On.RoR2.Inventory.orig_GiveItem_ItemDef_int orig, Inventory self, ItemDef itemDef, int count)


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //


        // 
        //
        //     Hook:
        // On.RoR2.
        //
    }
}