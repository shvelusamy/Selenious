namespace Selenious.Project.Pages
{
    using System.Collections.Generic;
    using System.Linq;
    using Selenious.Core;
    using Selenious.Core.DriverActions;
    using Selenious.Core.DriverConfigurations;
    using Selenious.Core.GlobalDataTypes;

    public class CartPage : BasePage
    {
        private readonly ElementLocator btnCheckout = new ElementLocator(".checkout_button");
        private readonly ElementLocator txtProductName = new ElementLocator(".inventory_item_name");

        public CartPage(DriverContext driverContext)
            : base(driverContext)
        {
        }

        public int GetProductCount()
        {
            return this.Driver.GetElements(this.txtProductName).Count;
        }

        public IEnumerable<string> GetProductNames()
        {
            return this.Driver.GetElements(this.txtProductName).Select(e => e.Text);
        }

        public CartPage ClickCheckout()
        {
            this.Driver.Click(this.btnCheckout);
            this.Driver.WaitUntilElementIsFound(new ElementLocator(".checkout_info_wrapper"));
            return this;
        }
    }
}