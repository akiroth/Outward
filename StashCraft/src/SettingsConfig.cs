using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BepInEx.Configuration;

namespace StashCraft
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
        private const string CTG_CRAFT = "Crafting Panels";
        public static ConfigEntry<bool> cfg_Craft_Disp_Quantity;
        public static ConfigEntry<bool> cfg_Craft_Disp_Count; 
        private const string CTG_DETAILS = "Item Details Display";
        public static ConfigEntry<bool> cfg_Dets_Quantity;
        public static ConfigEntry<bool> cfg_Dets_Value;
        public static ConfigEntry<bool> cfg_Dets_Durability;
        public static ConfigEntry<bool> cfg_Dets_Perish_Days;
        public static ConfigEntry<bool> cfg_Dets_Lantern;
        private const string CTG_STASH = "Stash Crafting"; 
        public static ConfigEntry<bool> cfg_Stash_Craft; 
        public static ConfigEntry<bool> cfg_Stash_Craft_Host;
        public static ConfigEntry<bool> cfg_Stash_Craft_Any;
        private const string CTG_STASH_Ops = "Stash Options";
        public static ConfigEntry<float> cfg_Stash_Preserv;
        public static ConfigEntry<bool> cfg_Stash_Preserv_Pack;
        public static ConfigEntry<bool> cfg_Stash_Filter;

        public static void Init(ConfigFile config)
        {
            // Craft Display
            cfg_Craft_Disp_Quantity = config.Bind(CTG_CRAFT, "Display Ingredient Quantities", true, new ConfigDescription("Craft & ingredient panels show item quantities.", null, new ConfigurationManagerAttributes { Order = 1 }));
            cfg_Craft_Disp_Count = config.Bind(CTG_CRAFT, "Display Recipe Item Count", true, new ConfigDescription("Recipes show how many items you already have.", null, new ConfigurationManagerAttributes { Order = 2 }));

            // Item Details Display
            cfg_Dets_Quantity = config.Bind(CTG_DETAILS, "Display Quantities", true, new ConfigDescription("Display the total quantity (includes your stash if you can access it) in item names.", null, new ConfigurationManagerAttributes { Order = 3 }));
            cfg_Dets_Value = config.Bind(CTG_DETAILS, "Display Sell Price", true, new ConfigDescription("Display the estimated sell price (and price per lb.).", null, new ConfigurationManagerAttributes { Order = 4 }));
            cfg_Dets_Durability = config.Bind(CTG_DETAILS, "Display Durability", true, new ConfigDescription("Display the durability for perishables.", null, new ConfigurationManagerAttributes { Order = 5 }));
            cfg_Dets_Perish_Days = config.Bind(CTG_DETAILS, "Display Perish Times", true, new ConfigDescription("Convert perishables durability to game time.", null, new ConfigurationManagerAttributes { Order = 6 }));
            cfg_Dets_Lantern = config.Bind(CTG_DETAILS, "Perish Time Override", true, new ConfigDescription("Display torch & lantern durability value instead of game time.", null, new ConfigurationManagerAttributes { Order = 7 }));

            // Stash Crafting
            cfg_Stash_Craft = config.Bind(CTG_STASH, "Craft from your stash", false, new ConfigDescription("Allows you to use items from your stash to craft.", null, new ConfigurationManagerAttributes { Order = 8 }));
            cfg_Stash_Craft_Host = config.Bind(CTG_STASH, "Craft from the world host stash", false, new ConfigDescription("Instead craft from the world hosts stash.", null, new ConfigurationManagerAttributes { Order = 9 }));
            cfg_Stash_Craft_Any = config.Bind(CTG_STASH, "Usable Outside of Towns", false, new ConfigDescription("Enables the use of crafting from stashes outside of town.", null, new ConfigurationManagerAttributes { Order = 10 }));

            // Stash Options
            cfg_Stash_Filter = config.Bind(CTG_STASH_Ops, "Stash Filters", true, new ConfigDescription("Add inventory filters to the stash panel.", null, new ConfigurationManagerAttributes { Order = 12 })); 
            cfg_Stash_Preserv = config.Bind(CTG_STASH_Ops, "Stash Preservation", 0f, new ConfigDescription("Preservation for stash contents (works just like packs).", new AcceptableValueRange<float>(0f, 100f)));
            cfg_Stash_Preserv_Pack = config.Bind(CTG_STASH_Ops, "Stash Preservation from Packs", false, new ConfigDescription("Preservation determined by highest of the stash and contained packs.", null, new ConfigurationManagerAttributes { Order = 14 }));
        }
    }
}
