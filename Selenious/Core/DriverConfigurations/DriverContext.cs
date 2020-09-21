namespace Selenious.Core.DriverConfigurations
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using NLog;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Chrome;
    using OpenQA.Selenium.Support.UI;
    using Selenious.Core;
    using Selenious.Core.GlobalDataTypes;
    using Selenious.Utils;
    using Selenious.Utils.Helpers;
    using Selenious.Utils.Logger;

    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Driver is disposed on test end")]
    public partial class DriverContext
    {
        private static readonly Logger Logger = LogManager.GetLogger("DRIVER");
        private TestLogger logTest;

        public string TestTitle { get; set; }

        public string ScreenShotFolder
        {
            get
            {
                return FilesHelper.GetFolder(ConfigurationManager.AppSettings["ScreenshotFolder"], this.CurrentDirectory);
            }
        }

        public string CurrentDirectory { get; set; }

        public bool IsTestFailed { get; set; }

        public TestLogger LogTest
        {
            get
            {
                return this.logTest ?? (this.logTest = new TestLogger());
            }

            set
            {
                this.logTest = value;
            }
        }

        public IWebDriver Driver { get; private set; }

        private ChromeOptions ChromeOptions
        {
            get
            {
                ChromeOptions options = new ChromeOptions();

                options.AddArgument("--ignore-certificate-errors");
                options.AddArgument("start-maximized");
                options.AddArgument("enable-automation");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-infobars");
                options.AddArgument("--disable-dev-shm-usage");
                options.AddArgument("--disable-browser-side-navigation");
                options.AddArgument("disable-features=NetworkService");
                options.AddArgument("--force-device-scale-factor=1");
                options.AddArgument("--dns-prefetch-disable");
                options.AddUserProfilePreference("profile.default_content_settings.popups", 0);
                options.AddUserProfilePreference("browser.download.folderList", 2);
                options.AddUserProfilePreference("download.prompt_for_download", false);

                return options;
            }
        }

        public Screenshot TakeScreenshot()
        {
            try
            {
                var screenshotDriver = (ITakesScreenshot)this.Driver;
                var screenshot = screenshotDriver.GetScreenshot();
                return screenshot;
            }
            catch (NullReferenceException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }
            catch (UnhandledAlertException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }
            catch (WebDriverTimeoutException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }

            return null;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Driver disposed later in stop method")]
        public void Start(BrowserType browser)
        {
            switch (browser)
            {
                case BrowserType.Chrome:
                    ChromeOptions options = this.ChromeOptions;
                    this.Driver = new ChromeDriver($"{this.CurrentDirectory}\\", options);
                    break;
                default:
                    throw new NotSupportedException(
                        string.Format(CultureInfo.CurrentCulture, "Driver {0} is not supported", BaseConfiguration.TestBrowser));
            }
        }

        public void WaitForPageLoad()
        {
            new WebDriverWait(this.Driver, TimeSpan.FromSeconds(BaseConfiguration.MediumTimeout)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
        }

        public void LoadUrl(string url)
        {
            this.Driver.Navigate().GoToUrl(url);
            this.WaitForPageLoad();
        }

        public void DeleteAllCookies()
        {
            this.Driver.Manage().Cookies.DeleteAllCookies();
        }

        public void Refresh()
        {
            this.Driver.Navigate().Refresh();
            this.WaitForPageLoad();
        }

        public void Stop()
        {
            if (this.Driver != null)
            {
                this.DeleteAllCookies();
                this.Driver.Quit();
            }
        }

        public string SaveScreenshot(ErrorDetail errorDetail, string folder, string title)
        {
            var fileName = string.Format(CultureInfo.CurrentCulture, "{0}_{1}_{2}.png", title, errorDetail.DateTime.ToString("yyyy-MM-dd HH-mm-ss-fff", CultureInfo.CurrentCulture), "browser");
            var correctFileName = Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(CultureInfo.CurrentCulture), string.Empty));
            correctFileName = Regex.Replace(correctFileName, "[^0-9a-zA-Z._]+", "_");
            correctFileName = NameHelper.ShortenFileName(folder, correctFileName, "_", 255);

            var filePath = Path.Combine(folder, correctFileName);
            Logger.Info("File path is :" + filePath);
            Logger.Info("Folder name is :" + folder);
            try
            {
                errorDetail.Screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);
                FilesHelper.WaitForFileOfGivenName(BaseConfiguration.ShortTimeout, correctFileName, folder);
                Logger.Error(CultureInfo.CurrentCulture, "Test failed: screenshot saved to {0}.", filePath);
                return filePath;
            }
            catch (NullReferenceException)
            {
                Logger.Error("Test failed but was unable to get webdriver screenshot.");
            }

            return null;
        }

        public string[] TakeAndSaveScreenshot()
        {
            List<string> filePaths = new List<string>();
            if (BaseConfiguration.FullDesktopScreenShotEnabled)
            {
                filePaths.Add(TakeScreenShot.Save(TakeScreenShot.DoIt(), ImageFormat.Png, this.ScreenShotFolder, this.TestTitle));
            }

            if (BaseConfiguration.SeleniumScreenShotEnabled)
            {
                filePaths.Add(this.SaveScreenshot(new ErrorDetail(this.TakeScreenshot(), DateTime.Now, null), this.ScreenShotFolder, this.TestTitle));
            }

            return filePaths.ToArray();
        }
    }
}