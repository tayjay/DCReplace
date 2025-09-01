using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DCReplace
{
    public class Config : IConfig
    {
        [Description("Should the plugin force the config option disconnect_drop to false? This will prevent item duplication. Default is true.")]
        public bool ForceDisconnectDropFalse { get; set; } = true;
        
        [Description("The announcement that will be shown to the player who replaced a disconnected player. Includes variables {PLAYER}, {ROLE}, and {ZONE} which will be replaced when displayed.")]
        public string ReplacementAnnouncement { get; set; } = "<i>You have replaced a player who has disconnected.</i>";
        
        [Description("The duration in seconds that the replacement announcement will be shown for.")]
        public ushort ReplacementAnnouncementDuration { get; set; } = 5;
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = true;
    }
}