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

        public string SpriteName { get; set; }

        public bool DefaultValue { get; set; }

        public string LocKey { get; set; }

        public string ToggleName
        {
            get { return $"{Name}Toggle"; }
        }
    }
}
