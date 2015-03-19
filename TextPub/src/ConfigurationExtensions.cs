using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TextPub
{
    internal static class ConfigurationExtensions
    {
        internal static void SaveAppSetting(this System.Configuration.Configuration configuration, string key, string value)
        {
            Contract.AssertNotNull(configuration);

            configuration.AppSettings.Settings.Remove(key);
            configuration.AppSettings.Settings.Add(key, value);
            configuration.Save();
        }

        internal static string GetAppSettingOrDefault(this System.Configuration.Configuration configuration, string key)
        {
            Contract.AssertNotNullOrWhitespace(key);

            var appSetting = configuration.AppSettings.Settings[key];
            if (appSetting != null)
            {
                return appSetting.Value;
            }

            return null;
        }
    }
}
