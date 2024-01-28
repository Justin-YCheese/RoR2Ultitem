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
using UltitemsCyan.Component;
using System;

namespace UltitemsCyan
{
    // Dependencies for when downloading the mod
    // For various important item methods
    [BepInDependency(ItemAPI.PluginGUID)]
    // For using Tokens
    [BepInDependency(LanguageAPI.PluginGUID)]
    // For making giving stat changes
    [BepInDependency(RecalculateStatsAPI.PluginGUID)]

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
        // The Plugin GUID should be a unique ID for this plugin,
        // which is human readable (as it is used in places like the config).
        // If we see this PluginGUID as it is on thunderstore,
        // we will deprecate this mod.
        // Change the PluginAuthor and the PluginName !
        public const string PluginGUID = PluginAuthor + "." + PluginName;
        public const string PluginAuthor = "SporkySpig";
        public const string PluginName = "UltitemsCyan";
        public const string PluginVersion = "0.6.8";
        /* Version Changes
         * Toy Robot grabs pickups by chance
         * 
         * 
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
            ultitemItems.Add(new BirthdayCandles());
            ultitemItems.Add(new DegreeScissors());
            ultitemItems.Add(new OverclockedGPU());
            //ultitemItems.Add(new FaultyLight());
            ultitemItems.Add(new ViralSmog());
            ultitemItems.Add(new DreamFuel());
            ultitemItems.Add(new CorrodingVault());
            ultitemItems.Add(new CorrodingVaultConsumed());
            ultitemItems.Add(new ToyRobot());
            ultitemItems.Add(new FleaBag());
            //ultitemItems.Add(new XenonAmpoule());
            ultitemItems.Add(new CremeBrulee());
            ultitemItems.Add(new KoalaSticker());
            ultitemItems.Add(new SuesMandibles());
            ultitemItems.Add(new SuesMandiblesConsumed());

            ultitemItems.Add(new NewBulb());

            // Equipments
            ultitemItems.Add(new IceCubes());
            ultitemItems.Add(new PotOfRegolith());

            // Void Items
            ultitemItems.Add(new DriedHam());
            ultitemItems.Add(new RottenBones());
            ultitemItems.Add(new DownloadedRAM());
            ultitemItems.Add(new InhabitedCoffin());
            ultitemItems.Add(new InhabitedCoffinConsumed());


            //ultitemItems.Add(new ());
            Log.Debug("List Done");

            int k = 0;
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

            // Add Void Transformations
            On.RoR2.Items.ContagiousItemManager.Init += ContagiousItemManager_Init;

            Log.Warning("Ultitems Cyan Done: " + PluginVersion);
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

            // Green
            public static Sprite BirthdayCandleSprite;
            public static Sprite DegreeScissorsSprite;
            public static Sprite OverclockedGPUSprite;
            public static Sprite TippedArrowSprite;

            // Red
            public static Sprite CorrodingVaultSprite;
            public static Sprite FaultyBulbSprite;
            public static Sprite GrapevineSprite;
            public static Sprite SuesMandiblesSprite;
            public static Sprite ViralSmogSprite;
            public static Sprite XenonAmpouleSprite;

            // Void
            public static Sprite DownloadedRAMSprite;
            public static Sprite DriedHamSprite;
            public static Sprite InhabitedCoffinSprite;
            public static Sprite RottenBonesSprite;
            public static Sprite TungstenRodSprite;
            public static Sprite VacantCoffinConsumed;
            public static Sprite WormHolesSprite;

            // Lunar
            public static Sprite CreatureDeckSprite;
            public static Sprite NewBulbSprite;
            public static Sprite DreamFuelSprite;
            public static Sprite PowerChipsSprite;
            public static Sprite SandPailSprite;
            public static Sprite SilverThreadSprite;

            // Untiered
            public static Sprite CorrodingVaultConsumedSprite;
            public static Sprite SuesMandiblesConsumedSprite;

            // Equipment
            public static Sprite GoopyCageSprite;
            public static Sprite IceCubesSprite;
            public static Sprite PetRockSprite;
            public static Sprite PotOfRegolithSprite;
            public static Sprite TrebuchetSprite;

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
                // White
                CremeBruleeSprite = mainBundle.LoadAsset<Sprite>("CremeBrulee");
                FleaBagSprite = mainBundle.LoadAsset<Sprite>("FleaBag");
                FlyingDiscSprite = mainBundle.LoadAsset<Sprite>("FlyingDisc");
                KoalaStickerSprite = mainBundle.LoadAsset<Sprite>("KoalaSticker");
                ToyRobotSprite = mainBundle.LoadAsset<Sprite>("ToyRobot");

                // Green
                BirthdayCandleSprite = mainBundle.LoadAsset<Sprite>("BirthdayCandle");
                DegreeScissorsSprite = mainBundle.LoadAsset<Sprite>("DegreeScissors");
                OverclockedGPUSprite = mainBundle.LoadAsset<Sprite>("OverclockedGPU");
                TippedArrowSprite = mainBundle.LoadAsset<Sprite>("TippedArrow");

                // Red
                CorrodingVaultSprite = mainBundle.LoadAsset<Sprite>("CorrodingVault");
                FaultyBulbSprite = mainBundle.LoadAsset<Sprite>("FaultyBulb");
                GrapevineSprite = mainBundle.LoadAsset<Sprite>("Grapevine");
                SuesMandiblesSprite = mainBundle.LoadAsset<Sprite>("SuesMandibles");
                ViralSmogSprite = mainBundle.LoadAsset<Sprite>("ViralSmog");
                XenonAmpouleSprite = mainBundle.LoadAsset<Sprite>("XenonAmpoule");

                // Void
                DownloadedRAMSprite = mainBundle.LoadAsset<Sprite>("DownloadedRAM");
                DriedHamSprite = mainBundle.LoadAsset<Sprite>("DriedHam");
                InhabitedCoffinSprite = mainBundle.LoadAsset<Sprite>("InhabitedCoffin");
                RottenBonesSprite = mainBundle.LoadAsset<Sprite>("RottenBones");
                TungstenRodSprite = mainBundle.LoadAsset<Sprite>("TungstenRod");
                VacantCoffinConsumed = mainBundle.LoadAsset<Sprite>("VacantCoffinConsumed");
                WormHolesSprite = mainBundle.LoadAsset<Sprite>("WormHoles");

                // Lunar
                CreatureDeckSprite = mainBundle.LoadAsset<Sprite>("CreatureDeck");
                DreamFuelSprite = mainBundle.LoadAsset<Sprite>("DreamFuel");
                NewBulbSprite = mainBundle.LoadAsset<Sprite>("NewBulb");
                PowerChipsSprite = mainBundle.LoadAsset<Sprite>("PowerChips");
                SandPailSprite = mainBundle.LoadAsset<Sprite>("SandPail");
                SilverThreadSprite = mainBundle.LoadAsset<Sprite>("SilverThread");

                // Untiered
                CorrodingVaultConsumedSprite = mainBundle.LoadAsset<Sprite>("CorrodingVaultConsumed");
                SuesMandiblesConsumedSprite = mainBundle.LoadAsset<Sprite>("SuesMandiblesConsumed");

                // Equipment
                GoopyCageSprite = mainBundle.LoadAsset<Sprite>("GoopyCage");
                IceCubesSprite = mainBundle.LoadAsset<Sprite>("IceCubes");
                PetRockSprite = mainBundle.LoadAsset<Sprite>("PetRock");
                PotOfRegolithSprite = mainBundle.LoadAsset<Sprite>("PotOfRegolith");
                TrebuchetSprite = mainBundle.LoadAsset<Sprite>("Trebuchet");

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