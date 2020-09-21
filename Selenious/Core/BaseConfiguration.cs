namespace Selenious.Core
{
    using System;
    using System.Configuration;
    using System.Globalization;
    using System.IO;
    using NLog;
    using NUnit.Framework;
    using Selenious.Utils;

    public static class BaseConfiguration
    {
        private static readonly string CurrentDirectory = TestContext.CurrentContext.TestDirectory;

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static BrowserType TestBrowser
        {
            get
            {
                bool supportedBrowser = Enum.TryParse(ConfigurationManager.AppSettings["browser"], out BrowserType browserType);

                if (supportedBrowser)
                {
                    return browserType;
                }

                return BrowserType.None;
            }
        }

        public static string GetBaseUrl
        {
            get
            {
                string baseUrl;

                if (!string.IsNullOrEmpty(Properties.TestSettings.Default.Env))
                {
                    baseUrl = ConfigurationManager.AppSettings[Properties.TestSettings.Default.Env].ToString();
                }
                else
                {
                    baseUrl = ConfigurationManager.AppSettings["Dev"].ToString();
                }

                Logger.Info("Base URL is: " + baseUrl);
                return baseUrl;
            }
        }

        public static string DownloadFolder
        {
            get
            {
                string downloadFolder;

                if (UseCurrentDirectory)
                {
                    downloadFolder = Path.Combine(CurrentDirectory, ConfigurationManager.AppSettings["DownloadFolder"]);
                }
                else
                {
                    downloadFolder = ConfigurationManager.AppSettings["DownloadFolder"];
                }

                return Path.Combine(downloadFolder, TestContext.CurrentContext.Test.ClassName.Split('.')[2]);
            }
        }

        public static double MediumTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["mediumTimeout"], CultureInfo.CurrentCulture);
            }
        }

        public static double LongTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["longTimeout"], CultureInfo.CurrentCulture);
            }
        }

        public static double ShortTimeout
        {
            get
            {
                return Convert.ToDouble(ConfigurationManager.AppSettings["shortTimeout"], CultureInfo.CurrentCulture);
            }
        }

        public static bool FullDesktopScreenShotEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["FullDesktopScreenShotEnabled"]))
                {
                    return false;
                }

                if (string.Equals(ConfigurationManager.AppSettings["FullDesktopScreenShotEnabled"].ToString(), "true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
        }

        public static bool SeleniumScreenShotEnabled
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["SeleniumScreenShotEnabled"]))
                {
                    return true;
                }

                if (string.Equals(ConfigurationManager.AppSettings["SeleniumScreenShotEnabled"], "true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
        }

        public static bool UseCurrentDirectory
        {
            get
            {
                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["UseCurrentDirectory"]))
                {
                    return false;
                }

                if (string.Equals(ConfigurationManager.AppSettings["UseCurrentDirectory"], "true", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                return false;
            }
        }
    }
}