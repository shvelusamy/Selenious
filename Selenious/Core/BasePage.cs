namespace Selenious.Core
{
    using System;
    using OpenQA.Selenium;
    using Selenious.Core.DriverConfigurations;
    using Selenious.ExampleProject.TestData;

    public partial class BasePage
    {
        public BasePage(DriverContext driverContext)
        {
            this.Driver = driverContext.Driver;
        }

        protected IWebDriver Driver { get; set; }

        public void NavigateTo(PageOptions page)
        {
            Uri baseUrl = new Uri(BaseConfiguration.GetBaseUrl);
            switch (page)
            {
                case PageOptions.Login:
                    this.Driver.Navigate().GoToUrl(new Uri(baseUrl, "index.html"));
                    break;
                case PageOptions.Inventory:
                    this.Driver.Navigate().GoToUrl(new Uri(baseUrl, "inventory.html"));
                    break;
                case PageOptions.Cart:
                    this.Driver.Navigate().GoToUrl(new Uri(baseUrl, "cart.html"));
                    break;
            }
        }
    }
}