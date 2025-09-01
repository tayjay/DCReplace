using System.ComponentModel;
using Exiled.API.Interfaces;

namespace DCReplace
{
    public class Config : IConfig
    {
        [Description("Should the plugin force the config option disconnect_drop to false? This will prevent item duplication. Default is true.")]
        public bool ForceDisconnectDropFalse { get; set; } = true;
        public bool IsEnabled { get; set; } = true;
        public bool Debug { get; set; } = false;
    }
}