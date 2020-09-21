namespace Selenious.Core
{
    using Selenious.Core.DriverActions;
    using Selenious.Core.GlobalDataTypes;
    using Selenious.Utils.Helpers;

    public partial class BasePage
    {
        public string GetDownloadedFile(ElementLocator locator, FileType fileExt)
        {
            string downloadFolder = BaseConfiguration.DownloadFolder;
            var currentCount = FilesHelper.CountFiles(downloadFolder, fileExt);
            this.Driver.Click(locator);
            this.Driver.WaitForPageLoad();
            string file = FilesHelper.GetDownloadedFile(downloadFolder, fileExt, currentCount, 60);
            return file;
        }
    }
}