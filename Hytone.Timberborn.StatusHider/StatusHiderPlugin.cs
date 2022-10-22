using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Hytone.Timberborn.StatusHider.UI;
using System.Collections.Generic;
using TimberApi.ConsoleSystem;
using TimberApi.ModSystem;

namespace Hytone.Timberborn.StatusHider
{
    [BepInPlugin(PluginId, PluginName, PluginVersion)]
    //[BepInDependency("com.timberapi.timberapi")]
    [HarmonyPatch]
    public class StatusHiderPlugin : BaseUnityPlugin, IModEntrypoint
    {
        public const string PluginId = "hytone.plugins.statushider";
        public const string PluginName = "StatusHider";
        public const string PluginVersion = "2.0.1";

        internal static ManualLogSource Log;
        private static Harmony _harmony;
        public static ConfigFile ConfigFile;

        public static List<StatusInfo> BuildingStatusThings = new List<StatusInfo>();
        public static List<StatusInfo> CharacterStatuses = new List<StatusInfo>();

        public void Entry(IMod mod, IConsoleWriter consoleWriter)
        {
            Log = Logger;
            ConfigFile = Config;
            InitStatusLists();
            InitConfigs();
            _harmony = new Harmony(PluginId);
            _harmony.PatchAll();

            Log.LogInfo($"Loaded {PluginName}.");
        }

        /// <summary>
        /// Initialized the lists that hold the data related to different statuses in game
        /// </summary>
        private static void InitStatusLists()
        {
            BuildingStatusThings.AddRange(
                new List<StatusInfo>()
                {
                    new StatusInfo() { ToggleValue = false, Name = "DisableOverloadedWarehouse", Description = "Disable the icon of Overloaded Warehouse.", DefaultValue = false, SpriteNames = new string[]{"OverloadedWarehouse" }, LocKey = StatusHiderMenu.WarehouseFullOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoPower", Description = "Disable the icon of No Power.", DefaultValue = false, SpriteNames = new string[]{"NoPower" }, LocKey = StatusHiderMenu.NoPowerOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisablePause", Description = "Disable the icon of Paused buildings.", DefaultValue = false, SpriteNames = new string[]{"Pause" }, LocKey = StatusHiderMenu.PauseOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableUnreachable", Description = "Disable the icon of Unreachable Building.", DefaultValue = false, SpriteNames = new string[]{ "UnreachableObject", "UnreachableBuilding" }, LocKey = StatusHiderMenu.UnreachableOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableEmptyBuilding", Description = "Disable the icon of Empty Building.", DefaultValue = false, SpriteNames = new string[]{"Empty" }, LocKey = StatusHiderMenu.EmptyBuildingOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableFlooded", Description = "Disable the icon of Flooded Building.", DefaultValue = false, SpriteNames = new string[]{"FloodedBuilding" }, LocKey = StatusHiderMenu.FloodedOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoRecipeAndNothingToDo", Description = "Disable the icon of No Recipe Selected and Nothing To Do.", DefaultValue = false, SpriteNames = new string[]{"NothingToDo" }, LocKey = StatusHiderMenu.NoRecipeOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableBuildingNeedsWater", Description = "Disable the icon of Building Needs Water.", DefaultValue = false, SpriteNames = new string[]{"BuildingNeedsWater" }, LocKey = StatusHiderMenu.NeedsWaterOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableLackOfNutrients", Description = "Disable the icon of Lack Of Nutrients.", DefaultValue = false, SpriteNames = new string[]{"LackOfNutrients" }, LocKey = StatusHiderMenu.LackOfNutrientsOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNotEnoughScience", Description = "Disable the icon of Not Enough Science.", DefaultValue = false, SpriteNames = new string[]{"NotEnoughScience" }, LocKey = StatusHiderMenu.NotEnoughScienceOptionLocKey }
                }
            );

            CharacterStatuses.AddRange(
                new List<StatusInfo>()
                {
                    new StatusInfo() { ToggleValue = false, Name = "DisableBrokenTeeth", Description = "Disable the icon of Broken Teeth.", DefaultValue = false, SpriteNames = new string[]{"BrokenTeeth" }, LocKey = StatusHiderMenu.BrokenTeethOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableDeath", Description = "Disable the icon of Death.", DefaultValue = false, SpriteNames = new string[]{"Death" }, LocKey = StatusHiderMenu.DeathOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableExhaustion", Description = "Disable the icon of Exhaustion.", DefaultValue = false, SpriteNames = new string[]{"Exhaustion" }, LocKey = StatusHiderMenu.ExhaustionOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableFood", Description = "Disable the icon of Hunger.", DefaultValue = false, SpriteNames = new string[]{"Food", "Hunger" }, LocKey = StatusHiderMenu.FoodOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableInjury", Description = "Disable the icon of Injury.", DefaultValue = false, SpriteNames = new string[]{"Injury" }, LocKey = StatusHiderMenu.InjuryOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoControlSignal", Description = "Disable the icon of No Control Signal.", DefaultValue = false, SpriteNames = new string[]{"NoControlSignal" }, LocKey = StatusHiderMenu.NoControlSignalOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableOutOfEnergy", Description = "Disable the icon of Out Of Energy.", DefaultValue = false, SpriteNames = new string[]{"OutOfEnergy" }, LocKey = StatusHiderMenu.OutOfEnergyOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableOutOfFuel", Description = "Disable the icon of Out Of Fuel.", DefaultValue = false, SpriteNames = new string[]{"OutOfFuel" }, LocKey = StatusHiderMenu.OutOfFuelOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableStranded", Description = "Disable the icon of Stranded.", DefaultValue = false, SpriteNames = new string[]{"Stranded" }, LocKey = StatusHiderMenu.StrandedOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableWater", Description = "Disable the icon of Thirst.", DefaultValue = false, SpriteNames = new string[]{"Water", "Thirst" }, LocKey = StatusHiderMenu.WaterOptionLocKey }
                }
            );
        }

        /// <summary>
        /// Initialized the Configs that the mods uses
        /// </summary>
        private void InitConfigs()
        {
            foreach (var thing in BuildingStatusThings)
            {
                InitConfig(ref thing.ToggleValue, thing.Name, thing.Description, thing.DefaultValue);
            }
            foreach (var thing in CharacterStatuses)
            {
                InitConfig(ref thing.ToggleValue, thing.Name, thing.Description, thing.DefaultValue);
            }
        }

        /// <summary>
        /// Initialized a single config entry on the Config
        /// </summary>
        /// <param name="toggleValueField"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="defaultValue"></param>
        public static void InitConfig(ref bool toggleValueField, string name, string description, bool defaultValue)
        {
            toggleValueField = ConfigFile.Bind("General",
                                                name,
                                                defaultValue,
                                                description)
                                          .Value;
        }
    }
}