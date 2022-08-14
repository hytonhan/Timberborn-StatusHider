using HarmonyLib;
using Timberborn.MainMenuScene;
using Timberborn.Options;
using UnityEngine.UIElements;

namespace Hytone.Timberborn.StatusHider.UI
{
    internal class UIPatches
    {
        /// <summary>
        /// Patch to show Status Icon Options on the In game Menu
        /// </summary>
        [HarmonyPatch(typeof(OptionsBox), "GetPanel")]
        public static class InGameMenuPanelPatch
        {
            public static void Postfix(ref VisualElement __result)
            {
                VisualElement root = __result.Query("OptionsBox");
                Button button = new Button() { classList = { "menu-button" } };
                button.text = "Status Hider Options";
                button.clicked += StatusHiderMenu.OpenOptionsDelegate;
                root.Insert(6, button);
            }
        }

        /// <summary>
        /// Patch to show Status Icon Options on Main Menu
        /// </summary>
        [HarmonyPatch(typeof(MainMenuPanel), "GetPanel")]
        public static class MainMenuPanelPatch
        {
            public static void Postfix(ref VisualElement __result)
            {
                VisualElement root = __result.Query("MainMenuPanel");
                Button button = new Button() { classList = { "menu-button" } };
                button.text = "Status Hider Options";
                button.clicked += StatusHiderMenu.OpenOptionsDelegate;
                root.Insert(6, button);
            }
        }
    }
}
