using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace MustStashes
{
    /// <summary>
    /// Class that specifies how a setting should be displayed inside the ConfigurationManager settings window.
    /// 
    /// Usage:
    /// This class template has to be copied inside the plugin's project and referenced by its code directly.
    /// make a new instance, assign any fields that you want to override, and pass it as a tag for your setting.
    /// 
    /// If a field is null (default), it will be ignored and won't change how the setting is displayed.
    /// If a field is non-null (you assigned a value to it), it will override default behavior.
    /// </summary>
    /// 
    /// <example> 
    /// Here's an example of overriding order of settings and marking one of the settings as advanced:
    /// <code>
    /// // Override IsAdvanced and Order
    /// Config.AddSetting("X", "1", 1, new ConfigDescription("", null, new ConfigurationManagerAttributes { IsAdvanced = true, Order = 3 }));
    /// // Override only Order, IsAdvanced stays as the default value assigned by ConfigManager
    /// Config.AddSetting("X", "2", 2, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 1 }));
    /// Config.AddSetting("X", "3", 3, new ConfigDescription("", null, new ConfigurationManagerAttributes { Order = 2 }));
    /// </code>
    /// </example>
    /// 
    /// <remarks> 
    /// You can read more and see examples in the readme at https://github.com/BepInEx/BepInEx.ConfigurationManager
    /// You can optionally remove fields that you won't use from this class, it's the same as leaving them null.
    /// </remarks>
        
    public static class SettingsConfig
    {
        private const string CTG_MAIN = "Main";
        public static ConfigEntry<bool> Stash_Enable_Others; 
        public static ConfigEntry<bool> Stash_OPEN_HOST;
        public static ConfigEntry<bool> Stash_Enable_Everywhere;
        
        public static void Init(ConfigFile config)
        {
            Stash_Enable_Others = config.Bind(CTG_MAIN, "Players Can Access Other Stashes", false, "Allow players to open each other's stashes");
            Stash_OPEN_HOST = config.Bind(CTG_MAIN, "Only Open World Host Stash", false, "Keybind will only open the world host's stash");
            Stash_Enable_Everywhere = config.Bind(CTG_MAIN, "Access Stashes From Anywhere", false, "Players can also open stashes from outside of towns and cities");
        }
    }
}
