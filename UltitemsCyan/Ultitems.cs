using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

using UltitemsCyan.Items.Tier1;
using UltitemsCyan.Items.Tier2;
using UltitemsCyan.Items.Tier3;
//using UltitemsCyan.Items.Equipment;
using UltitemsCyan.Items.Lunar;
//using UltitemsCyan.Items.Void;
using UltitemsCyan.Items.Untiered;

using UltitemsCyan.Buffs;
using UnityEngine.ResourceManagement.ResourceProviders;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Reflection;
using Unity.Audio;

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
        public const string PluginVersion = "0.3.4";

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

            // Load assets
            Log.Debug("Populating Assets...");
            //Assets.PopulateAssets();
            Assets.Init();

            // Add buffs to the game
            List<BuffBase> ultitemBuffs = [];
            ultitemBuffs.Add(new BirthdayBuff());
            ultitemBuffs.Add(new DreamSpeedBuff());
            ultitemBuffs.Add(new Overclockedbuff());
            //ultitemBuffs.Add(new ());
            foreach (BuffBase newBuff in ultitemBuffs)
            {
                //Log.Debug("Adding " + newItem.item.name); // This cause the mod to crash. Trying to access the name of the item definition
                Log.Debug("Adding items...");
                newBuff.Init();
            }


            Log.Debug("Buffs Done");

            // Add items to the game
            List<ItemBase> ultitemItems = [];
            //ultitems.Add(new TestItem());
            ultitemItems.Add(new BirthdayCandles());
            ultitemItems.Add(new DegreeScissors());
            ultitemItems.Add(new OverclockedGPU());
            ultitemItems.Add(new FaultyBulb());
            ultitemItems.Add(new ViralSmog());
            ultitemItems.Add(new DreamFuel());
            ultitemItems.Add(new RustedVault());
            ultitemItems.Add(new RustedVaultConsumed());
            ultitemItems.Add(new ToyRobot());

            //ultitemItems.Add(new ());
            Log.Debug("List Done");


            foreach (ItemBase newItem in ultitemItems)
            {
                //Log.Debug("Adding " + newItem.item.name); // This cause the mod to crash. Trying to access the name of the item definition
                Log.Debug("Adding items...");
                newItem.Init();
            }
            Log.Debug("Items Done");

            Log.Warning("Ultitems Cyan Done: " + PluginVersion);
        }

        //Static class for ease of access
        public static class Assets
        {
            //The mod's AssetBundle
            public static AssetBundle mainBundle;

            public static Sprite BirthdayCandleSprite;
            public static Sprite DegreeScissorsSprite;
            public static Sprite OverclockedGPUSprite;
            public static Sprite FaultyBulbSprite;
            public static Sprite ViralSmogSprite;
            public static Sprite DreamFuelSprite;
            public static Sprite RustedVaultSprite;
            public static Sprite ToyRobotSprite;
            public static Sprite RustedVaultConsumedSprite;

            public static Sprite DreamSpeedSprite;
            public static Sprite OverclockedSprite;
            public static Sprite BirthdaySprite;

            public static GameObject beltPrefab;

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
                //beltPrefab = mainBundle.LoadAsset<GameObject>("Assets/ExampleSurvivor/ExampleSurvivorAssets/mdlExampleSurvivor.prefab");
                //if (beltPrefab == null)
                //{
                //    Log.Warning("Null beltPrefab");
                //}
                BirthdayCandleSprite = mainBundle.LoadAsset<Sprite>("BirthdayCandle");
                if (BirthdayCandleSprite == null)
                {
                    Log.Debug("Null BirthdayCandle");
                }
                else
                {
                    Log.Warning("Birthday Sprite is good");
                }
                DegreeScissorsSprite = mainBundle.LoadAsset<Sprite>("DegreeScissors");
                OverclockedGPUSprite = mainBundle.LoadAsset<Sprite>("OverclockedGPU");
                FaultyBulbSprite = mainBundle.LoadAsset<Sprite>("FaultyBulb");
                ViralSmogSprite = mainBundle.LoadAsset<Sprite>("ViralSmog");
                DreamFuelSprite = mainBundle.LoadAsset<Sprite>("DreamFuel");
                RustedVaultSprite = mainBundle.LoadAsset<Sprite>("RustedVault");
                ToyRobotSprite = mainBundle.LoadAsset<Sprite>("ToyRobot");
                RustedVaultConsumedSprite = mainBundle.LoadAsset<Sprite>("RustedVaultConsumed");

                DreamSpeedSprite = mainBundle.LoadAsset<Sprite>("DreamSpeed");
                OverclockedSprite = mainBundle.LoadAsset<Sprite>("Overclocked");
                BirthdaySprite = mainBundle.LoadAsset<Sprite>("Birthday");
                //*/
            }
        }

    /*/
    public static class Assets
    {
        public static AssetBundle MainAssetBundle = null;
        public static AssetBundleProvider Provider;

        public static Sprite BirthdayCandleSprite;
        public static Sprite DegreeScissorsSprite;
        public static Sprite OverclockedGPUSprite;
        public static Sprite FaultyBulbSprite;
        public static Sprite ViralSmogSprite;
        public static Sprite DreamFuelSprite;
        public static Sprite RustedVaultSprite;
        public static Sprite ToyRobotSprite;
        public static Sprite RustedVaultConsumedSprite;

        public static Sprite DreamSpeedSprite;
        public static Sprite OverclockedSprite;
        public static Sprite BirthdaySprite;

        public static void PopulateAssets()
        {
            Log.Warning("Populating!");
            if (MainAssetBundle == null)
            {
                Log.Debug("using");
                using (Stream assetSteam = Assembly.GetExecutingAssembly().GetManifestResourceStream("UltitemsCyan.ultitemTempBundle"))
                {
                    Log.Debug("bundle...");
                    MainAssetBundle = AssetBundle.LoadFromStream(assetSteam);

                    if (MainAssetBundle == null)
                    {
                        Log.Warning("Null Bundle");
                    }

                    Provider = new AssetBundleProvider();


                    Log.Debug("1...");
                    BirthdayCandleSprite = bundle.LoadAsset<Sprite>("BirthdayCandle.png");
                    if (BirthdayCandleSprite == null)
                    {
                        Log.Warning("Null Birthday");
                    }
                    Log.Debug("2...");
                    DegreeScissorsSprite = bundle.LoadAsset<Sprite>("DegreeScissors.png");
                    Log.Debug("3...");
                    OverclockedGPUSprite = bundle.LoadAsset<Sprite>("OverclockedGPU.png");
                    Log.Debug("4...");
                    FaultyBulbSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/FaultyBulb.png");
                    if (FaultyBulbSprite == null)
                    {
                        Log.Warning("Null FaultyBulbSprite");
                    }
                    Log.Debug("5...");
                    ViralSmogSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/ViralSmog.png");
                    Log.Debug("6...");
                    DreamFuelSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/DreamFuel.png");
                    Log.Debug("7...");
                    RustedVaultSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/RustedVault.png");
                    Log.Debug("8...");
                    ToyRobotSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/ToyRobot.png");
                    Log.Debug("9...");
                    RustedVaultConsumedSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/RustedVaultConsumed.png");


                    Log.Debug("10...");
                    DreamSpeedSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/DreamSpeed.png");
                    Log.Debug("11...");
                    OverclockedSprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/Overclocked.png");
                    Log.Debug("12...");
                    BirthdaySprite = bundle.LoadAsset<Sprite>("Assets/ExampleSurvivor/ExampleSurvivorAssets/Birthday.png");
                    Log.Debug("13...");
                }
            }
            Log.Debug("14...");
        }
    }//*/
}
}