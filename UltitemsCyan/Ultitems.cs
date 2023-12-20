using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UltitemsCyan.Items;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UltitemsCyan.Buffs;
using System.Collections.Generic;

namespace UltitemsCyan
{
    //Initialize R2API: ItemAPI
    [BepInDependency(ItemAPI.PluginGUID)]
    //Initialize R2API: LanguageAPI
    [BepInDependency(LanguageAPI.PluginGUID)]

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
        public const string PluginVersion = "1.0.0";

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

            // Add buffs to the game
            BuffHelper.CreateBuffs();
            Log.Debug("Buffs Done");

            // Add items to the game
            Log.Debug("Test 1");
            List<ItemBase> ultitems = [];
            //ultitems.Add(new TestItem());
            Log.Debug("Test 2");
            ultitems.Add(new BirthdayCandles());
            Log.Debug("Test 3");
            ultitems.Add(new FaultyBulb());
            Log.Debug("Test 4");
            Log.Debug("Listed Items:");
            //ultitems.Add(new ());

            foreach (ItemBase item in ultitems)
            {
                Log.Debug("Test 5");
                Log.Debug("Adding items...");
                item.Init();
            }
            Log.Debug("Items Done");

            Log.Info("Ultitems Cyan Done");
        }
    }
}
