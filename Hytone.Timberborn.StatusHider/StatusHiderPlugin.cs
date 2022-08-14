using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Hytone.Timberborn.StatusHider.UI;
using System.Collections.Generic;
using TimberbornAPI;
using TimberbornAPI.Common;

namespace Hytone.Timberborn.StatusHider
{
    [BepInPlugin(PluginId, PluginName, PluginVersion)]
    [BepInDependency("com.timberapi.timberapi")]
    [HarmonyPatch]
    public class StatusHiderPlugin : BaseUnityPlugin
    {
        public const string PluginId = "hytone.plugins.statushider";
        public const string PluginName = "StatusHider";
        public const string PluginVersion = "0.1.0";

        internal static ManualLogSource Log;
        private static Harmony _harmony;
        public static ConfigFile ConfigFile;

        public static List<StatusInfo> BuildingStatusThings = new List<StatusInfo>();
        public static List<StatusInfo> CharacterStatuses = new List<StatusInfo>();

        public void Awake()
        {
            Log = Logger;
            ConfigFile = Config;
            InitStatusLists();
            AddLabels();
            InitConfigs();
            // Harmony patches
            _harmony = new Harmony(PluginId);
            _harmony.PatchAll();

            TimberAPI.DependencyRegistry.AddConfigurator(new UIConfigurator(), SceneEntryPoint.Global);
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
                    new StatusInfo() { ToggleValue = false, Name = "DisableOverloadedWarehouse", Description = "Disable the icon of Overloaded Warehouse.", DefaultValue = false, SpriteName = "OverloadedWarehouse", LocKey = StatusHiderMenu.WarehouseFullOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoPower", Description = "Disable the icon of No Power.", DefaultValue = false, SpriteName = "NoPower", LocKey = StatusHiderMenu.NoPowerOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisablePause", Description = "Disable the icon of Paused buildings.", DefaultValue = false, SpriteName = "Pause", LocKey = StatusHiderMenu.PauseOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableUnreachable", Description = "Disable the icon of Unreachable Building.", DefaultValue = false, SpriteName = "UnreachableBuilding", LocKey = StatusHiderMenu.UnreachableOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableEmptyBuilding", Description = "Disable the icon of Empty Building.", DefaultValue = false, SpriteName = "Empty", LocKey = StatusHiderMenu.EmptyBuildingOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableFlooded", Description = "Disable the icon of Flooded Building.", DefaultValue = false, SpriteName = "FloodedBuilding", LocKey = StatusHiderMenu.FloodedOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoRecipeAndNothingToDo", Description = "Disable the icon of No Recipe Selected and Nothing To Do.", DefaultValue = false, SpriteName = "NothingToDo", LocKey = StatusHiderMenu.NoRecipeOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableBuildingNeedsWater", Description = "Disable the icon of Building Needs Water.", DefaultValue = false, SpriteName = "BuildingNeedsWater", LocKey = StatusHiderMenu.NeedsWaterOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableLackOfNutrients", Description = "Disable the icon of Lack Of Nutrients.", DefaultValue = false, SpriteName = "LackOfNutrients", LocKey = StatusHiderMenu.LackOfNutrientsOptionLocKey }
                            }
                        );

            CharacterStatuses.AddRange(
                new List<StatusInfo>()
                {
                    new StatusInfo() { ToggleValue = false, Name = "DisableBroken", Description = "Disable the icon of Broken.", DefaultValue = false, SpriteName = "Broken", LocKey = StatusHiderMenu.BrokenOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableBrokenTeeth", Description = "Disable the icon of Broken Teeth.", DefaultValue = false, SpriteName = "BrokenTeeth", LocKey = StatusHiderMenu.BrokenTeethOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableDeath", Description = "Disable the icon of Death.", DefaultValue = false, SpriteName = "Death", LocKey = StatusHiderMenu.DeathOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableExhaustion", Description = "Disable the icon of Exhaustion.", DefaultValue = false, SpriteName = "Exhaustion", LocKey = StatusHiderMenu.ExhaustionOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableFood", Description = "Disable the icon of Hunger.", DefaultValue = false, SpriteName = "Food", LocKey = StatusHiderMenu.FoodOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableInjury", Description = "Disable the icon of Injury.", DefaultValue = false, SpriteName = "Injury", LocKey = StatusHiderMenu.InjuryOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableNoControlSignal", Description = "Disable the icon of No Control Signal.", DefaultValue = false, SpriteName = "NoControlSignal", LocKey = StatusHiderMenu.NoControlSignalOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableOutOfEnergy", Description = "Disable the icon of Out Of Energy.", DefaultValue = false, SpriteName = "OutOfEnergy", LocKey = StatusHiderMenu.OutOfEnergyOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableOutOfFuel", Description = "Disable the icon of Out Of Fuel.", DefaultValue = false, SpriteName = "OutOfFuel", LocKey = StatusHiderMenu.OutOfFuelOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisablePollutedMechanisms", Description = "Disable the icon of Polluted Mechanisms.", DefaultValue = false, SpriteName = "PollutedMechanisms", LocKey = StatusHiderMenu.PollutedMechanismsOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisablePollutionPoisoning", Description = "Disable the icon of Pollution Poisoning.", DefaultValue = false, SpriteName = "PollutionPoisoning", LocKey = StatusHiderMenu.PollutionPoisoningOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableStranded", Description = "Disable the icon of Stranded.", DefaultValue = false, SpriteName = "Stranded", LocKey = StatusHiderMenu.StrandedOptionLocKey },
                    new StatusInfo() { ToggleValue = false, Name = "DisableWater", Description = "Disable the icon of Thirst.", DefaultValue = false, SpriteName = "Water", LocKey = StatusHiderMenu.WaterOptionLocKey }
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
        /// <param name="flagField"></param>
        /// <param name="name"></param>
        /// <param name="description"></param>
        /// <param name="defaultValue"></param>
        public static void InitConfig(ref bool flagField, string name, string description, bool defaultValue)
        {
            flagField = ConfigFile.Bind("General",
                                        name,
                                        defaultValue,
                                        description)
                                  .Value;
        }

        /// <summary>
        /// Adds localization labels used by the mod to the game
        /// </summary>
        private void AddLabels()
        {
            TimberAPI.Localization.AddLabel(StatusHiderMenu.MenuHeaderLocKey, "Status Hider Options");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.BuildingsHeaderLocKey, "Building Statuses");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.CharactersHeaderLocKey, "Character Statuses");

            TimberAPI.Localization.AddLabel(StatusHiderMenu.WarehouseFullOptionLocKey, "Disable Warehouse Full icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.PauseOptionLocKey, "Disable Paused bulding icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.NoPowerOptionLocKey, "Disable Unpowered building icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.UnreachableOptionLocKey, "Disable Unreachable building icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.EmptyBuildingOptionLocKey, "Disable Empty Building icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.FloodedOptionLocKey, "Disable Flooded building icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.NoRecipeOptionLocKey, "Disable Nothing To Do/No Recipe icons.");
            //TimberAPI.Localization.AddLabel(StatusHiderMenu.NothingToDoOptionLocKey, "Disable Nothing to do icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.NeedsWaterOptionLocKey, "Disable Needs water icons.");

            TimberAPI.Localization.AddLabel(StatusHiderMenu.BrokenOptionLocKey, "Disable Broken icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.BrokenTeethOptionLocKey, "Disable Broken Teeth icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.DeathOptionLocKey, "Disable Death icons");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.ExhaustionOptionLocKey, "Disable Exhaustion icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.FoodOptionLocKey, "Disable Hunger icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.LackOfNutrientsOptionLocKey, "Disable Lack of Nutrients icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.InjuryOptionLocKey, "Disable Injury icons");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.NoControlSignalOptionLocKey, "Disable No Control Signal icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.OutOfEnergyOptionLocKey, "Disable Out of Energy icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.OutOfFuelOptionLocKey, "Disable Out of Fuel icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.PollutedMechanismsOptionLocKey, "Disable Polluted Mechanisms icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.PollutionPoisoningOptionLocKey, "Disable Pollution Poisoning icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.StrandedOptionLocKey, "Disable Stranded icons.");
            TimberAPI.Localization.AddLabel(StatusHiderMenu.WaterOptionLocKey, "Disable Thirst icons.");
        }
    }
}