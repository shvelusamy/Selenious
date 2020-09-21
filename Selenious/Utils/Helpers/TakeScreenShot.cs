namespace Selenious.Utils.Helpers
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Globalization;
    using System.IO;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using NLog;
    using OpenQA.Selenium;
    using Selenious.Core;
    using Selenious.Core.DriverActions;
    using Selenious.Utils;

    public static class TakeScreenShot
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Bitmap DoIt()
        {
            var screen = Screen.PrimaryScreen;
            using (var bitmap = new Bitmap(screen.Bounds.Width, screen.Bounds.Height))
            {
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    try
                    {
                        graphics.CopyFromScreen(0, 0, 0, 0, screen.Bounds.Size);
                    }
                    catch (Win32Exception)
                    {
                        Logger.Error("Win32Exception Exception, user is locked out with no access to windows desktop");
                        return null;
                    }
                }

                return (Bitmap)bitmap.Clone();
            }
        }

        public static string Save(Bitmap bitmap, ImageFormat format, string folder, string title)
        {
            var fileName = string.Format(CultureInfo.CurrentCulture, "{0}_{1}_{2}.png", title, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff", CultureInfo.CurrentCulture), "fullscreen");
            fileName = Regex.Replace(fileName, "[^0-9a-zA-Z._]+", "_");
            fileName = NameHelper.ShortenFileName(folder, fileName, "_", 255);
            var filePath = Path.Combine(folder, fileName);

            if (bitmap == null)
            {
                Logger.Error("Full screenshot is not saved");
            }
            else
            {
                bitmap.Save(filePath, format);
                bitmap.Dispose();
                FilesHelper.WaitForFileOfGivenName(BaseConfiguration.ShortTimeout, fileName, folder);
                Logger.Info(string.Format(CultureInfo.CurrentCulture, "##teamcity[publishArtifacts '{0}']", filePath));
                return filePath;
            }

            return null;
        }

        public static string TakeScreenShotOfElement(IWebElement element, string folder, string screenshotName)
        {
            return TakeScreenShotOfElement(0, 0, element, folder, screenshotName);
        }

        public static string TakeScreenShotOfElement(int iframeLocationX, int iframeLocationY, IWebElement element, string folder, string screenshotName)
        {
            var locationX = iframeLocationX;
            var locationY = iframeLocationY;

            var driver = element.ToDriver();

            var screenshotDriver = (ITakesScreenshot)driver;
            var screenshot = screenshotDriver.GetScreenshot();
            var filePath = Path.Combine(folder, DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss-fff", CultureInfo.CurrentCulture) + "temporary_fullscreen.png");
            screenshot.SaveAsFile(filePath, ScreenshotImageFormat.Png);

            if (BaseConfiguration.TestBrowser == BrowserType.Chrome)
            {
                locationX = element.Location.X + locationX;
                locationY = element.Location.Y + locationY;
            }
            else
            {
                locationX = element.Location.X;
                locationY = element.Location.Y;
            }

            var elementWidth = element.Size.Width;
            var elementHeight = element.Size.Height;

            var image = new Rectangle(locationX, locationY, elementWidth, elementHeight);
            var importFile = new Bitmap(filePath);
            string newFilePath;
            Bitmap cloneFile;
            try
            {
                newFilePath = Path.Combine(folder, screenshotName + ".png");
                cloneFile = (Bitmap)importFile.Clone(image, importFile.PixelFormat);
            }
            finally
            {
            importFile.Dispose();
            }

            cloneFile.Save(newFilePath);
            File.Delete(filePath);
            return newFilePath;
        }
    }
}
