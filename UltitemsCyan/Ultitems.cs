using BepInEx;

using R2API;

using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

using UltitemsCyan.Items.Tier1;
using UltitemsCyan.Items.Tier2;
using UltitemsCyan.Items.Tier3;
using UltitemsCyan.Items.Lunar;
using UltitemsCyan.Items.Void;
using UltitemsCyan.Items.Untiered;
using UltitemsCyan.Equipment;
using UltitemsCyan.Buffs;

using System.Linq;
//using HarmonyLib;

// Unused?
using UnityEngine.ResourceManagement.ResourceProviders;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;
using Unity.Audio;
using R2API.Utils;
using BepInEx.Configuration;
using System;
using UltitemsCyan.Items;

namespace UltitemsCyan
{
    // Dependencies for when downloading the mod
    // For various important item methods
    [BepInDependency(ItemAPI.PluginGUID)]
    // For using Tokens
    [BepInDependency(LanguageAPI.PluginGUID)]
    // For making giving stat changes
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]
    // For using custom prefabs
    [BepInDependency(PrefabAPI.PluginGUID)]
    // This attribute is required, and lists metadata for your plugin.
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]

    // TODO: Check if I need this for my mod specifically 
    // [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]



    // This is the main declaration of our plugin class.
    // BepInEx searches for all classes inheriting from BaseUnityPlugin to initialize on startup.
    // BaseUnityPlugin itself inherits from MonoBehaviour,
    // so you can use this as a reference for what you can declare and use in your plugin class
    // More information in the Unity Docs: https://docs.unity3d.com/ScriptReference/MonoBehaviour.html
    public class Ultitems : BaseUnityPlugin
    {
        public static float stageStartTime; // measured in seconds

        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "SporkySpig";
        public const string PluginName = "UltitemsCyan";
        public const string PluginVersion = "0.7.10";

        public const string PluginSuffix = "Seed RNG for Vault and Coffin";
        /* Version Changes     Old Git v7.8
         * Added Rocky Taffy v7.9
         * Fixed Rocky Taffy Check inventory bug
         * Various Item fixes for, Xenon and Sliver Thread rework
         * Fixed Double cost for thread v7.10
         * Seed RNG for Vault and Coffin
         * 
         * 
         * 
         * 
         * 
         */

        public static List<ItemDef.Pair> CorruptionPairs = [];
        public static PluginInfo PInfo { get; private set; }

        public static Sprite mysterySprite = Addressables.LoadAssetAsync<Sprite>("RoR2/Base/Common/MiscIcons/texMysteryIcon.png").WaitForCompletion();
        public static GameObject mysteryPrefab = Addressables.LoadAssetAsync<GameObject>("RoR2/Base/Mystery/PickupMystery.prefab").WaitForCompletion();

        /* TODO: Add Assets and Config File
        // assets
        public static AssetBundle resources;

        public static GameObject cardPrefab;
        public static GameObject smallPrefab;

        public static Vector3 scaleTo;

        // config file
        private static ConfigFile cfgFile;
        //*/

        public void Awake()
        {
            // Init our logging class so that we can properly log for debugging
            Log.Init(Logger);
            PInfo = Info;

            // Load assets TODO when making a final version
            //Log.Debug("Populating Assets...");
            //Assets.PopulateAssets();
            Assets.Init();

            // Add buffs to the game
            List<BuffBase> ultitemBuffs = [];
            ultitemBuffs.Add(new BirthdayBuff());
            ultitemBuffs.Add(new DreamSpeedBuff());
            ultitemBuffs.Add(new OverclockedBuff());
            ultitemBuffs.Add(new TickCritBuff());
            ultitemBuffs.Add(new RottingBuff());
            ultitemBuffs.Add(new DownloadedBuff());
            ultitemBuffs.Add(new SlipperyGrape());
            //ultitemBuffs.Add(new ());
            foreach (BuffBase newBuff in ultitemBuffs)
            {
                //Log.Debug("Adding " + newItem.item.name); // This cause the mod to crash. Trying to access the name of the item definition
                //Log.Debug("Adding buffs...");
                newBuff.Init();
            }
            Log.Debug("Buffs Done");

            // Add items to the game
            // Tiered Items
            List<ItemBase> ultitemItems = [];
            //ultitems.Add(new TestItem());

            // First Priority

            // Untiered
            ultitemItems.Add(new CorrodingVaultConsumed());
            ultitemItems.Add(new InhabitedCoffinConsumed());
            ultitemItems.Add(new SuesMandiblesConsumed());
            ultitemItems.Add(new SilverThreadConsumed());

            // White
            ultitemItems.Add(new CremeBrulee());
            ultitemItems.Add(new KoalaSticker());
            ultitemItems.Add(new ToyRobot());
            ultitemItems.Add(new FleaBag());

            // Green
            ultitemItems.Add(new BirthdayCandles());
            //ultitemItems.Add(new DegreeScissors()); // Last Priority
            ultitemItems.Add(new OverclockedGPU());
            ultitemItems.Add(new XenonAmpoule());

            // Red
            ultitemItems.Add(new ViralSmog());
            ultitemItems.Add(new SuesMandibles());
            ultitemItems.Add(new CorrodingVault());
            ultitemItems.Add(new RockyTaffy());
            ultitemItems.Add(new Grapevine());

            // Lunar Items
            ultitemItems.Add(new DreamFuel());
            ultitemItems.Add(new UltravioletBulb());
            //ultitemItems.Add(new PowerChip());
            ultitemItems.Add(new SilverThread()); // Need to be before Sonorous Pail?
            ultitemItems.Add(new SonorousPail());

            // Equipments
            ultitemItems.Add(new IceCubes());
            ultitemItems.Add(new PotOfRegolith());

            // Void Items
            ultitemItems.Add(new DriedHam());
            ultitemItems.Add(new RottenBones());
            ultitemItems.Add(new DownloadedRAM());
            ultitemItems.Add(new InhabitedCoffin());
            //ultitemItems.Add(new InhabitedCoffinConsumed()); // Untiered

            // Last Priority
            ultitemItems.Add(new DegreeScissors()); // After Vault and Coffin to grab consumed items

            //ultitemItems.Add(new ());
            Log.Debug("List Done");

            foreach (ItemBase newItem in ultitemItems)
            {
                newItem.Init();
                // If a void item (which always transforms other items) then add to corruption pair list
                if (newItem.GetTransformItem)
                {
                    //Log.Warning("Adding Void Transformation to list!");
                    CorruptionPairs.Add(new()
                    {
                        itemDef1 = newItem.GetTransformItem,
                        itemDef2 = newItem.GetItemDef,
                    });
                }
            }
            Log.Debug("Items Done");


            // Add Hooks
            Stage.onStageStartGlobal += Stage_onStageStartGlobal;
            On.RoR2.Items.ContagiousItemManager.Init += ContagiousItemManager_Init;


            Log.Warning("Ultitems Cyan Done: " + PluginVersion + " <- " + PluginSuffix);
        }


        private void Stage_onStageStartGlobal(Stage obj)
        {
            stageStartTime = Run.instance.time;
            Log.Warning("Ultitem Starts at: " + stageStartTime);
        }


        // Add Void Pairs
        public void ContagiousItemManager_Init(On.RoR2.Items.ContagiousItemManager.orig_Init orig)
        {
            // Add ultiCorruptionPairs to base game corruption pairs
            Log.Warning("Ultitem Create Void Transformations!");
            List<ItemDef.Pair> voidPairs = ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem].ToList(); // Collection Expression?
            /*/
            Log.Debug("Base Void Items:");
            printPairList(voidPairs);
            //*/
            Log.Debug("My Void Items:");
            printPairList(CorruptionPairs);

            ItemCatalog.itemRelationships[DLC1Content.ItemRelationshipTypes.ContagiousItem] = voidPairs.Union(CorruptionPairs).ToArray();
            Log.Debug("Error?");
            orig();
        }

        private void printPairList(List<ItemDef.Pair> list)
        {
            foreach (ItemDef.Pair pair in list)
            {
                Log.Debug(". " + pair.itemDef1.name + " -> " + pair.itemDef2.name);
            }
            Log.Debug("end");
        }

        //Static class for ease of access
        public static class Assets
        {
            //The mod's AssetBundle
            public static AssetBundle mainBundle;

            // White
            public static Sprite CremeBruleeSprite;
            public static Sprite FleaBagSprite;
            public static Sprite FlyingDiscSprite;
            public static Sprite KoalaStickerSprite;
            public static Sprite ToyRobotSprite;
            public static GameObject CremeBruleePrefab;
            public static GameObject FleaBagPrefab;
            public static GameObject FlyingDiscPrefab;
            public static GameObject KoalaStickerPrefab;
            public static GameObject ToyRobotPrefab;

            // Green
            public static Sprite BirthdayCandleSprite;
            public static Sprite DegreeScissorsSprite;
            public static Sprite OverclockedGPUSprite;
            public static Sprite TippedArrowSprite;
            public static GameObject BirthdayCandlePrefab;
            public static GameObject DegreeScissorsPrefab;
            public static GameObject OverclockedGPUPrefab;
            public static GameObject TippedArrowPrefab;

            // Red
            public static Sprite CorrodingVaultSprite;
            public static Sprite FaultyBulbSprite;
            public static Sprite GrapevineSprite;
            public static Sprite SuesMandiblesSprite;
            public static Sprite ViralSmogSprite;
            public static Sprite XenonAmpouleSprite;
            public static GameObject CorrodingVaultPrefab;
            public static GameObject FaultyBulbPrefab;
            public static GameObject GrapevinePrefab;
            public static GameObject SuesMandiblesPrefab;
            public static GameObject ViralSmogPrefab;
            public static GameObject XenonAmpoulePrefab;

            // Void
            public static Sprite DownloadedRAMSprite;
            public static Sprite DriedHamSprite;
            public static Sprite InhabitedCoffinSprite;
            public static Sprite RottenBonesSprite;
            public static Sprite TungstenRodSprite;
            public static Sprite InhabitedCoffinConsumedSprite;
            public static Sprite WormHolesSprite;
            public static GameObject DownloadedRAMPrefab;
            public static GameObject DriedHamPrefab;
            public static GameObject InhabitedCoffinPrefab;
            public static GameObject RottenBonesPrefab;
            public static GameObject TungstenRodPrefab;
            public static GameObject InhabitedCoffinConsumedPrefab;
            public static GameObject WormHolesPrefab;

            // Lunar
            public static Sprite CreatureDeckSprite;
            public static Sprite NewBulbSprite;
            public static Sprite DreamFuelSprite;
            public static Sprite PowerChipsSprite;
            public static Sprite SandPailSprite;
            public static Sprite SilverThreadSprite;
            public static GameObject CreatureDeckPrefab;
            public static GameObject NewBulbPrefab;
            public static GameObject DreamFuelPrefab;
            public static GameObject PowerChipsPrefab;
            public static GameObject SandPailPrefab;
            public static GameObject SilverThreadPrefab;

            // Untiered
            public static Sprite CorrodingVaultConsumedSprite;
            public static Sprite SuesMandiblesConsumedSprite;
            public static GameObject CorrodingVaultConsumedPrefab;
            public static GameObject SuesMandiblesConsumedPrefab;

            // Equipment
            public static Sprite GoopyCageSprite;
            public static Sprite IceCubesSprite;
            public static Sprite PetRockSprite;
            public static Sprite PotOfRegolithSprite;
            public static Sprite TrebuchetSprite;
            public static GameObject GoopyCagePrefab;
            public static GameObject IceCubesPrefab;
            public static GameObject PetRockPrefab;
            public static GameObject PotOfRegolithPrefab;
            public static GameObject TrebuchetPrefab;

            // Buffs
            public static Sprite BirthdaySprite;
            public static Sprite DownloadedSprite;
            public static Sprite DreamSpeedSprite;
            public static Sprite GrapeSprite;
            public static Sprite OverclockedSprite;
            public static Sprite RottingSprite;
            public static Sprite TickCritSprite;

            //A constant of the AssetBundle's name.
            public const string bundleName = "ultitembundle";
            // Not necesary, but useful if you want to store the bundle on its own folder.
            public const string assetBundleFolder = "UltitemAssetBundle";

            //The direct path to your AssetBundle
            public static string AssetBundlePath
            {
                get
                {
                    //This returns the path to your assetbundle assuming said bundle is on the same folder as your DLL. If you have your bundle in a folder, you can uncomment the statement below this one.
                    //return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(PInfo.Location), bundleName);
                    return System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(PInfo.Location)), assetBundleFolder, bundleName);
                }
            }

            public static void Init()
            {
                //Loads the assetBundle from the Path, and stores it in the static field.
                mainBundle = AssetBundle.LoadFromFile(AssetBundlePath);
                if (mainBundle == null)
                {
                    Log.Warning("Null mainBundle");
                }

                float localScale = 0.2f;

                CremeBruleeSprite = mainBundle.LoadAsset<Sprite>("CremeBrulee.png");
                FleaBagSprite = mainBundle.LoadAsset<Sprite>("FleaBag.png");
                FlyingDiscSprite = mainBundle.LoadAsset<Sprite>("FlyingDisc.png");
                KoalaStickerSprite = mainBundle.LoadAsset<Sprite>("KoalaSticker.png");
                ToyRobotSprite = mainBundle.LoadAsset<Sprite>("ToyRobot.png");
                CremeBruleePrefab = mainBundle.LoadAsset<GameObject>("CremeBrulee.prefab");
                FleaBagPrefab = mainBundle.LoadAsset<GameObject>("FleaBag.prefab");
                FlyingDiscPrefab = mainBundle.LoadAsset<GameObject>("FlyingDisc.prefab");
                KoalaStickerPrefab = mainBundle.LoadAsset<GameObject>("KoalaSticker.prefab");
                ToyRobotPrefab = mainBundle.LoadAsset<GameObject>("ToyRobot.prefab");

                CremeBruleePrefab.transform.localScale = Vector3.up * localScale;
                FleaBagPrefab.transform.localScale = Vector3.up * localScale;
                FlyingDiscPrefab.transform.localScale = Vector3.up * localScale;
                KoalaStickerPrefab.transform.localScale = Vector3.up * localScale;
                ToyRobotPrefab.transform.localScale = Vector3.up * localScale;

                // Green
                BirthdayCandleSprite = mainBundle.LoadAsset<Sprite>("BirthdayCandle.png");
                DegreeScissorsSprite = mainBundle.LoadAsset<Sprite>("DegreeScissors.png");
                OverclockedGPUSprite = mainBundle.LoadAsset<Sprite>("OverclockedGPU.png");
                TippedArrowSprite = mainBundle.LoadAsset<Sprite>("TippedArrow.png");
                BirthdayCandlePrefab = mainBundle.LoadAsset<GameObject>("BirthdayCandle.prefab");
                DegreeScissorsPrefab = mainBundle.LoadAsset<GameObject>("DegreeScissors.prefab");
                OverclockedGPUPrefab = mainBundle.LoadAsset<GameObject>("OverclockedGPU.prefab");
                TippedArrowPrefab = mainBundle.LoadAsset<GameObject>("TippedArrow.prefab");

                BirthdayCandlePrefab.transform.localScale = Vector3.up * localScale;
                DegreeScissorsPrefab.transform.localScale = Vector3.up * localScale;
                OverclockedGPUPrefab.transform.localScale = Vector3.up * localScale;
                TippedArrowPrefab.transform.localScale = Vector3.up * localScale;

                // Red
                CorrodingVaultSprite = mainBundle.LoadAsset<Sprite>("CorrodingVault.png");
                FaultyBulbSprite = mainBundle.LoadAsset<Sprite>("FaultyBulb.png");
                GrapevineSprite = mainBundle.LoadAsset<Sprite>("Grapevine.png");
                SuesMandiblesSprite = mainBundle.LoadAsset<Sprite>("SuesMandibles.png");
                ViralSmogSprite = mainBundle.LoadAsset<Sprite>("ViralSmog.png");
                XenonAmpouleSprite = mainBundle.LoadAsset<Sprite>("XenonAmpoule.png");
                CorrodingVaultPrefab = mainBundle.LoadAsset<GameObject>("CorrodingVault.prefab");
                FaultyBulbPrefab = mainBundle.LoadAsset<GameObject>("FaultyBulb.prefab");
                GrapevinePrefab = mainBundle.LoadAsset<GameObject>("Grapevine.prefab");
                SuesMandiblesPrefab = mainBundle.LoadAsset<GameObject>("SuesMandibles.prefab");
                ViralSmogPrefab = mainBundle.LoadAsset<GameObject>("ViralSmog.prefab");
                XenonAmpoulePrefab = mainBundle.LoadAsset<GameObject>("XenonAmpoule.prefab");

                CorrodingVaultPrefab.transform.localScale = Vector3.up * localScale;
                FaultyBulbPrefab.transform.localScale = Vector3.up * localScale;
                GrapevinePrefab.transform.localScale = Vector3.up * localScale;
                SuesMandiblesPrefab.transform.localScale = Vector3.up * localScale;
                ViralSmogPrefab.transform.localScale = Vector3.up * localScale;
                XenonAmpoulePrefab.transform.localScale = Vector3.up * localScale;

                // Void
                DownloadedRAMSprite = mainBundle.LoadAsset<Sprite>("DownloadedRAM.png");
                DriedHamSprite = mainBundle.LoadAsset<Sprite>("DriedHam.png");
                InhabitedCoffinSprite = mainBundle.LoadAsset<Sprite>("InhabitedCoffin.png");
                RottenBonesSprite = mainBundle.LoadAsset<Sprite>("RottenBones.png");
                TungstenRodSprite = mainBundle.LoadAsset<Sprite>("TungstenRod.png");
                InhabitedCoffinConsumedSprite = mainBundle.LoadAsset<Sprite>("VacantCoffinConsumed.png");
                WormHolesSprite = mainBundle.LoadAsset<Sprite>("WormHoles.png");
                DownloadedRAMPrefab = mainBundle.LoadAsset<GameObject>("DownloadedRAM.prefab");
                DriedHamPrefab = mainBundle.LoadAsset<GameObject>("DriedHam.prefab");
                InhabitedCoffinPrefab = mainBundle.LoadAsset<GameObject>("InhabitedCoffin.prefab");
                RottenBonesPrefab = mainBundle.LoadAsset<GameObject>("RottenBones.prefab");
                TungstenRodPrefab = mainBundle.LoadAsset<GameObject>("TungstenRod.prefab");
                InhabitedCoffinConsumedPrefab = mainBundle.LoadAsset<GameObject>("VacantCoffinConsumed.prefab");
                WormHolesPrefab = mainBundle.LoadAsset<GameObject>("WormHoles.prefab");

                DownloadedRAMPrefab.transform.localScale = Vector3.up * localScale;
                DriedHamPrefab.transform.localScale = Vector3.up * localScale;
                InhabitedCoffinPrefab.transform.localScale = Vector3.up * localScale;
                RottenBonesPrefab.transform.localScale = Vector3.up * localScale;
                TungstenRodPrefab.transform.localScale = Vector3.up * localScale;
                InhabitedCoffinConsumedPrefab.transform.localScale = Vector3.up * localScale;
                WormHolesPrefab.transform.localScale = Vector3.up * localScale;

                // Lunar
                CreatureDeckSprite = mainBundle.LoadAsset<Sprite>("CreatureDeck.png");
                DreamFuelSprite = mainBundle.LoadAsset<Sprite>("DreamFuel.png");
                NewBulbSprite = mainBundle.LoadAsset<Sprite>("NewBulb.png");
                PowerChipsSprite = mainBundle.LoadAsset<Sprite>("PowerChips.png");
                SandPailSprite = mainBundle.LoadAsset<Sprite>("SandPail.png");
                SilverThreadSprite = mainBundle.LoadAsset<Sprite>("SilverThread.png");
                CreatureDeckPrefab = mainBundle.LoadAsset<GameObject>("CreatureDeck.prefab");
                DreamFuelPrefab = mainBundle.LoadAsset<GameObject>("DreamFuel.prefab");
                NewBulbPrefab = mainBundle.LoadAsset<GameObject>("NewBulb.prefab");
                PowerChipsPrefab = mainBundle.LoadAsset<GameObject>("PowerChips.prefab");
                SandPailPrefab = mainBundle.LoadAsset<GameObject>("SandPail.prefab");
                SilverThreadPrefab = mainBundle.LoadAsset<GameObject>("SilverThread.prefab");

                CreatureDeckPrefab.transform.localScale = Vector3.up * localScale;
                DreamFuelPrefab.transform.localScale = Vector3.up * localScale;
                NewBulbPrefab.transform.localScale = Vector3.up * localScale;
                PowerChipsPrefab.transform.localScale = Vector3.up * localScale;
                SandPailPrefab.transform.localScale = Vector3.up * localScale;
                SilverThreadPrefab.transform.localScale = Vector3.up * localScale;

                // Untiered
                CorrodingVaultConsumedSprite = mainBundle.LoadAsset<Sprite>("CorrodingVaultConsumed.png");
                SuesMandiblesConsumedSprite = mainBundle.LoadAsset<Sprite>("SuesMandiblesConsumed.png");
                CorrodingVaultConsumedPrefab = mainBundle.LoadAsset<GameObject>("CorrodingVaultConsumed.prefab");
                SuesMandiblesConsumedPrefab = mainBundle.LoadAsset<GameObject>("SuesMandiblesConsumed.prefab");

                CorrodingVaultConsumedPrefab.transform.localScale = Vector3.up * localScale;
                SuesMandiblesConsumedPrefab.transform.localScale = Vector3.up * localScale;

                // Equipment
                GoopyCageSprite = mainBundle.LoadAsset<Sprite>("GoopyCage.png");
                IceCubesSprite = mainBundle.LoadAsset<Sprite>("IceCubes.png");
                PetRockSprite = mainBundle.LoadAsset<Sprite>("PetRock.png");
                PotOfRegolithSprite = mainBundle.LoadAsset<Sprite>("PotOfRegolith.png");
                TrebuchetSprite = mainBundle.LoadAsset<Sprite>("Trebuchet.png");
                GoopyCagePrefab = mainBundle.LoadAsset<GameObject>("GoopyCage.prefab");
                IceCubesPrefab = mainBundle.LoadAsset<GameObject>("IceCubes.prefab");
                PetRockPrefab = mainBundle.LoadAsset<GameObject>("PetRock.prefab");
                PotOfRegolithPrefab = mainBundle.LoadAsset<GameObject>("PotOfRegolith.prefab");
                TrebuchetPrefab = mainBundle.LoadAsset<GameObject>("Trebuchet.prefab");

                GoopyCagePrefab.transform.localScale = Vector3.up * localScale;
                IceCubesPrefab.transform.localScale = Vector3.up * localScale;
                PetRockPrefab.transform.localScale = Vector3.up * localScale;
                PotOfRegolithPrefab.transform.localScale = Vector3.up * localScale;
                TrebuchetPrefab.transform.localScale = Vector3.up * localScale;

                // Buffs
                BirthdaySprite = mainBundle.LoadAsset<Sprite>("Birthday");
                DownloadedSprite = mainBundle.LoadAsset<Sprite>("Downloaded");
                DreamSpeedSprite = mainBundle.LoadAsset<Sprite>("DreamSpeed");
                GrapeSprite = mainBundle.LoadAsset<Sprite>("Grape");
                OverclockedSprite = mainBundle.LoadAsset<Sprite>("Overclocked");
                RottingSprite = mainBundle.LoadAsset<Sprite>("Rotting");
                TickCritSprite = mainBundle.LoadAsset<Sprite>("TickCrit");

                /*/
                DegreeScissorsSprite = mainBundle.LoadAsset<Sprite>("DegreeScissors");
                OverclockedGPUSprite = mainBundle.LoadAsset<Sprite>("OverclockedGPU");
                FaultyBulbSprite = mainBundle.LoadAsset<Sprite>("FaultyBulb");
                ViralSmogSprite = mainBundle.LoadAsset<Sprite>("ViralSmog");
                DreamFuelSprite = mainBundle.LoadAsset<Sprite>("DreamFuel");
                CorrodingVaultSprite = mainBundle.LoadAsset<Sprite>("CorrodingVault");
                ToyRobotSprite = mainBundle.LoadAsset<Sprite>("ToyRobot");
                RustedVaultConsumedSprite = mainBundle.LoadAsset<Sprite>("CorrodingVaultConsumed");
                FleaBagSprite = mainBundle.LoadAsset<Sprite>("FleaBag");
                CremeBruleeSprite = mainBundle.LoadAsset<Sprite>("CremeBrulee");
                SuesMandiblesSprite = mainBundle.LoadAsset<Sprite>("SuesMandibles");
                SuesMandiblesConsumedSprite = mainBundle.LoadAsset<Sprite>("SuesMandiblesConsumed");

                IceCubesSprite = mainBundle.LoadAsset<Sprite>("IceCubes");
                PotOfRegolithSprite = mainBundle.LoadAsset<Sprite>("PotOfRegolith");

                DreamSpeedSprite = mainBundle.LoadAsset<Sprite>("DreamSpeed");
                OverclockedSprite = mainBundle.LoadAsset<Sprite>("Overclocked");
                BirthdaySprite = mainBundle.LoadAsset<Sprite>("Birthday");
                TickCritSprite = mainBundle.LoadAsset<Sprite>("TickCrit");
                //*/
            }
        }
    }
}