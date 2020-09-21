namespace Selenious.Core
{
    using System;
    using System.Collections.Generic;
    using NUnit.Framework;
    using NUnit.Framework.Interfaces;
    using Selenious.Core.DriverConfigurations;
    using Selenious.Utils;
    using Selenious.Utils.Helpers;
    using Selenious.Utils.Logger;

    public class BaseTest
    {
        protected DriverContext Driver { get; } = new DriverContext();

        public TestLogger LogTest
        {
            get => this.Driver.LogTest;

            set => this.Driver.LogTest = value;
        }

        [OneTimeSetUp]
        public void BeforeEachTestClass()
        {
            try
            {
                this.Driver.CurrentDirectory = TestContext.CurrentContext.TestDirectory;

                BrowserType testBrowser;
                testBrowser = BrowserType.Chrome;
                this.Driver.Start(testBrowser);

                string url = BaseConfiguration.GetBaseUrl;
                this.Driver.LoadUrl(url);
                this.Driver.WaitForPageLoad();
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
        }

        [OneTimeTearDown]
        public void AfterEachTestClass()
        {
            this.Driver.Stop();
        }

        [SetUp]
        public void BeforeEachTest()
        {
            this.Driver.TestTitle = TestContext.CurrentContext.Test.Name;
            this.LogTest.LogTestStarting(this.Driver);
        }

        [TearDown]
        public void AfterEachTest()
        {
            this.Driver.IsTestFailed = TestContext.CurrentContext.Result.Outcome.Status == TestStatus.Failed;
            var filePaths = this.SaveTestDetailsIfTestFailed(this.Driver);
            if (filePaths != null)
            {
                SaveAttachments.SaveAttachmentsToTestContext(filePaths);
            }

            this.LogTest.LogTestEnding(this.Driver);
        }

        public string[] SaveTestDetailsIfTestFailed(DriverContext driverContext)
        {
            if (driverContext.IsTestFailed)
            {
                var screenshots = driverContext.TakeAndSaveScreenshot();

                var returnList = new List<string>();
                returnList.AddRange(screenshots);

                return returnList.ToArray();
            }

            return null;
        }
    }
}