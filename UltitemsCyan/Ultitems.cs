using BepInEx;
using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System.Collections.Generic;

using UltitemsCyan.Items;
using UltitemsCyan.Buffs;

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
            List<BuffBase> ultitemBuffs = [];
            ultitemBuffs.Add(new BirthdayBuff());
            ultitemBuffs.Add(new DreamSpeedBuff());
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
            ultitemItems.Add(new DreamFuel());
            ultitemItems.Add(new FaultyBulb());
            //ultitems.Add(new ());
            Log.Debug("List Done");


            foreach (ItemBase newItem in ultitemItems)
            {
                //Log.Debug("Adding " + newItem.item.name); // This cause the mod to crash. Trying to access the name of the item definition
                Log.Debug("Adding items...");
                newItem.Init();
            }
            Log.Debug("Items Done");

            Log.Warning("Ultitems Cyan Done");
        }
    }
}
