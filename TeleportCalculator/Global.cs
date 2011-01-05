using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TeleportCalculator
{
    internal static class Global
    {
        private static readonly string costsCompatibility = "1.0";
        private static readonly string translationsCompatibility = "1.0";
        private static readonly string settingsCompatibility = "1.0";

        internal static string CostsCompatibility
        {
            get
            {
                return costsCompatibility;
            }
        }

        internal static string TranslationsCompatibility
        {
            get
            {
                return translationsCompatibility;
            }
        }

        internal static string SettingsCompatibility
        {
            get
            {
                return settingsCompatibility;
            }
        }
    }
}
