using HarmonyLib;
using System;
using Timberborn.StatusSystem;

namespace Hytone.Timberborn.StatusHider
{
    internal class Patches
    {
        /// <summary>
        /// Holds the stuff related to StatusIconCycler patching
        /// </summary>
        [HarmonyPatch(typeof(StatusIconCycler), nameof(StatusIconCycler.ShowIcon))]
        public static class StatusIconCyclerPatch
        {
            /// <summary>
            /// Prefix on StatusIconCycler.ShowIcon to determine is a shown
            /// status icon should be hidden. Skips the original if the icon should be hidden
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="status"></param>
            /// <returns></returns>
            public static bool Prefix(StatusIconCycler __instance, StatusInstance status)
            {
                foreach (var buildingThings in StatusHiderPlugin.BuildingStatusThings)
                {
                    if (ShouldHideIcon(status, buildingThings.SpriteNames, buildingThings.ToggleValue, __instance))
                    {
                        return false;
                    }
                }
                foreach (var characterThings in StatusHiderPlugin.CharacterStatuses)
                {
                    if (ShouldHideIcon(status, characterThings.SpriteNames, characterThings.ToggleValue, __instance))
                    {
                        return false;
                    }
                }
                return true;
            }

            /// <summary>
            /// Hides the icon above a entity if it should be hidden.
            /// </summary>
            /// <param name="status"></param>
            /// <param name="spriteNames"></param>
            /// <param name="toggleValue"></param>
            /// <param name="__instance"></param>
            /// <returns></returns>
            private static bool ShouldHideIcon(
                StatusInstance status,
                string[] spriteNames,
                bool toggleValue,
                StatusIconCycler __instance)
            {
                // status.Sprite.name == spriteNames  
                if (Array.IndexOf(spriteNames, status.Sprite.name) > -1&& toggleValue)
                {
                    if (__instance._shownIconStatus == status)
                    {
                        __instance.HideShownIcon();
                    }
                    return true;
                }
                return false;
            }
        }
    }
}
