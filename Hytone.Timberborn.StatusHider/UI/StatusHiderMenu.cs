using System;
using TimberApi.UiBuilderSystem;
using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.CoreUI;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.Length.Unit;

namespace Hytone.Timberborn.StatusHider.UI
{
    public class StatusHiderMenu : IPanelController
    {
        public static Action OpenOptionsDelegate;
        private readonly PanelStack _panelStack;
        private readonly UIBuilder _uiBuilder;

        public static readonly string MenuHeaderLocKey = "statushider.menuheader";
        public static readonly string BuildingsHeaderLocKey = "statushider.buildingsheader";
        public static readonly string CharactersHeaderLocKey = "statushider.charactersheader";

        public static readonly string WarehouseFullOptionLocKey = "statushider.warehousefulloption";
        public static readonly string PauseOptionLocKey = "statushider.disablepauseoption";
        public static readonly string NoPowerOptionLocKey = "statushider.nopoweroption";
        public static readonly string UnreachableOptionLocKey = "statushider.ureachableoption";
        public static readonly string EmptyBuildingOptionLocKey = "statushider.emptybuildingoption";
        public static readonly string FloodedOptionLocKey = "statushider.floodedoption";
        public static readonly string NoRecipeOptionLocKey = "statushider.norecipeoption";
        public static readonly string NeedsWaterOptionLocKey = "statushider.needswateroption";
        public static readonly string NotEnoughScienceOptionLocKey = "statushider.notenoughscience";

        public static readonly string BrokenOptionLocKey = "statushider.brokenoption";
        public static readonly string BrokenTeethOptionLocKey = "statushider.brokenteethoption";
        public static readonly string DeathOptionLocKey = "statushider.deathoption";
        public static readonly string ExhaustionOptionLocKey = "statushider.exhaustionoption";
        public static readonly string FoodOptionLocKey = "statushider.foodoption";
        public static readonly string LackOfNutrientsOptionLocKey = "statushider.lackofnutrientsoption";
        public static readonly string InjuryOptionLocKey = "statushider.injuryoption";
        public static readonly string NoControlSignalOptionLocKey = "statushider.nocontrolsignaloption";
        public static readonly string OutOfEnergyOptionLocKey = "statushider.outofenergyoption";
        public static readonly string OutOfFuelOptionLocKey = "statushider.outoffueloption";
        public static readonly string PollutedMechanismsOptionLocKey = "statushider.pollutedmechanismsoption";
        public static readonly string PollutionPoisoningOptionLocKey = "statushider.pollutionpoisoningoption";
        public static readonly string StrandedOptionLocKey = "statushider.strandedoption";
        public static readonly string WaterOptionLocKey = "statushider.wateroption";

        public StatusHiderMenu(
            UIBuilder uiBuilder,
            PanelStack panelStack)
        {
            _uiBuilder = uiBuilder;
            _panelStack = panelStack;
            OpenOptionsDelegate = OpenOptionsPanel;
        }

        private void OpenOptionsPanel()
        {
            _panelStack.HideAndPush(this);
        }

        /// <summary>
        /// Create the Options Panel
        /// </summary>
        /// <returns></returns>
        public VisualElement GetPanel()
        {
            UIBoxBuilder boxBuilder = _uiBuilder.CreateBoxBuilder()
                .SetHeight(new Length(600, Pixel))
                .SetWidth(new Length(600, Pixel))
                .ModifyScrollView(builder => builder.SetName("elementPreview"));

            VisualElementBuilder menuContent = InitMenuPanelWithHeader();
            AddBuildinTogglesToMenuContent(menuContent);
            AddCharacterToggleToMenuContent(menuContent);
            boxBuilder.AddComponent(menuContent.Build());

            VisualElement root = boxBuilder.AddCloseButton("CloseButton").SetBoxInCenter().AddHeader(MenuHeaderLocKey).BuildAndInitialize();
            root.Q<Button>("CloseButton").clicked += OnUICancelled;

            foreach (var thing in StatusHiderPlugin.BuildingStatusThings)
            {
                root.Q<Toggle>(thing.ToggleName).RegisterValueChangedCallback((changeEvent) => OptionToggled(thing, changeEvent));
                root.Q<Toggle>(thing.ToggleName).value = thing.ToggleValue;
            }
            foreach (var thing in StatusHiderPlugin.CharacterStatuses)
            {
                root.Q<Toggle>(thing.ToggleName).RegisterValueChangedCallback((changeEvent) => OptionToggled(thing, changeEvent));
                root.Q<Toggle>(thing.ToggleName).value = thing.ToggleValue;
            }

            return root;
        }

        /// <summary>
        /// Adds Character related Status toggles to menu panel
        /// </summary>
        /// <param name="menuContent"></param>
        private static void AddCharacterToggleToMenuContent(VisualElementBuilder menuContent)
        {
            menuContent.AddPreset(factory => factory.Labels().DefaultBig(CharactersHeaderLocKey, builder: builder => builder.SetStyle(style => { style.alignSelf = Align.Center; style.marginBottom = new Length(10, Pixel); })));
            foreach (var thing in StatusHiderPlugin.CharacterStatuses)
            {
                menuContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: thing.LocKey, name: thing.ToggleName, builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            }
        }

        /// <summary>
        /// Adds Building realted Status toggles to menu panel
        /// </summary>
        /// <param name="menuContent"></param>
        private static void AddBuildinTogglesToMenuContent(VisualElementBuilder menuContent)
        {
            menuContent.AddPreset(factory => factory.Labels().DefaultBig(BuildingsHeaderLocKey, builder: builder => builder.SetStyle(style => { style.alignSelf = Align.Center; style.marginBottom = new Length(10, Pixel); })));
            foreach (var thing in StatusHiderPlugin.BuildingStatusThings)
            {
                menuContent.AddPreset(factory => factory.Toggles().Checkbox(locKey: thing.LocKey, name: thing.ToggleName, builder: builder => builder.SetStyle(style => style.marginBottom = new Length(10, Pixel))));
            }
        }

        /// <summary>
        /// Initializes a menu panel with a header for Status toggles
        /// </summary>
        /// <returns></returns>
        private VisualElementBuilder InitMenuPanelWithHeader()
        {
            var menuContent = _uiBuilder.CreateComponentBuilder().CreateVisualElement();
            menuContent.AddPreset(factory => factory.Labels().DefaultHeader(MenuHeaderLocKey, builder: builder => builder.SetStyle(style => { style.alignSelf = Align.Center; style.marginBottom = new Length(10, Pixel); })));
            return menuContent;
        }

        /// <summary>
        /// Stores the new value when a Toggle is toggled
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="changeEvent"></param>
        private void OptionToggled(StatusInfo thing, ChangeEvent<bool> changeEvent)
        {
            thing.ToggleValue = changeEvent.newValue;
            if(!StatusHiderPlugin.ConfigFile.TryGetEntry<bool>("General",
                                                               thing.Name,
                                                               out var setting))
            {
                StatusHiderPlugin.Log.LogInfo($"Config \"{thing.Name}\" didn't exist. Creating.");
                StatusHiderPlugin.InitConfig(ref thing.ToggleValue,
                                             thing.Name,
                                             thing.Description,
                                             thing.DefaultValue);
            }
            setting.Value = changeEvent.newValue;
            StatusHiderPlugin.ConfigFile
                             .Save();
        }

        public bool OnUIConfirmed()
        {
            return false;
        }

        public void OnUICancelled()
        {
            _panelStack.Pop(this);
        }

    }
}
