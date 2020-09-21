namespace Selenious.Project.Pages
{
    using System;
    using OpenQA.Selenium;
    using Selenious.Core;
    using Selenious.Core.DriverActions;
    using Selenious.Core.DriverConfigurations;
    using Selenious.Core.GlobalDataTypes;

    public class InventoryPage : BasePage
    {
        private readonly ElementLocator btnAddToCart = new ElementLocator(".btn_inventory");
        private readonly ElementLocator txtProductName = new ElementLocator(".inventory_item_name");

        public InventoryPage(DriverContext driverContext)
            : base(driverContext)
        {
        }

        public InventoryPage AddToCart(string productName)
        {
            var itemBoxes = this.Driver.GetElements(new ElementLocator(".inventory_item"));
            if (itemBoxes.Count == 0)
            {
                throw new Exception("No products listed in inventory page.");
            }

            foreach (IWebElement item in itemBoxes)
            {
                if (string.Equals(item.GetElement(this.txtProductName).Text, productName, StringComparison.OrdinalIgnoreCase))
                {
                    this.Driver.Click(item.GetElement(this.btnAddToCart));
                    break;
                }
            }

            return this;
        }

        public InventoryPage AddToCart(string[] productNames)
        {
            if (productNames == null || productNames.Length == 0)
            {
                throw new ArgumentNullException("productNames array is missing.");
            }

            foreach (string product in productNames)
            {
                this.AddToCart(product);
            }

            return this;
        }
    }
}