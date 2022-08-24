namespace Hytone.Timberborn.StatusHider
{
    /// <summary>
    /// A class that holds relevant data used in status related stuff
    /// </summary>
    public class StatusInfo
    {
        public bool ToggleValue;

        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Array of possible spritenames of the status.
        /// Array allows the mod to work both on stable and experimental
        /// if the spritenames have changed.
        /// eg. "Hunger" in 0.1.5 -> "Food" in 0.2.X
        /// </summary>
        public string[] SpriteNames { get; set; }

        public bool DefaultValue { get; set; }

        public string LocKey { get; set; }

        public string ToggleName
        {
            get { return $"{Name}Toggle"; }
        }
    }
}
