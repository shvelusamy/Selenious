namespace Selenious.Project.Pages
{
    using System.Linq;
    using NUnit.Framework;
    using Selenious.Core;
    using Selenious.Core.DriverActions;
    using Selenious.Core.DriverConfigurations;
    using Selenious.Core.GlobalDataTypes;
    using Selenious.Core.Project.TestData;

    public class CheckoutPage : BasePage
    {
        private readonly ElementLocator txtFirstName = new ElementLocator("#first-name");
        private readonly ElementLocator txtLastName = new ElementLocator("#last-name");
        private readonly ElementLocator txtPostalCode = new ElementLocator("#postal-code");
        private readonly ElementLocator btnContinue = new ElementLocator(".cart_button");
        private readonly ElementLocator btnFinish = new ElementLocator(".cart_button");
        private readonly ElementLocator txtProductName = new ElementLocator(".inventory_item_name");

        public CheckoutPage(DriverContext driverContext)
            : base(driverContext)
        {
        }

        public CheckoutPage DoCheckout(UserInfo userInfo, string[] productNames = null)
        {
            if (!string.IsNullOrEmpty(userInfo.FirstName))
            {
                this.Driver.UpdateText(this.txtFirstName, userInfo.FirstName);
            }

            if (!string.IsNullOrEmpty(userInfo.LastName))
            {
                this.Driver.UpdateText(this.txtLastName, userInfo.LastName);
            }

            if (!string.IsNullOrEmpty(userInfo.PostalCode))
            {
                this.Driver.UpdateText(this.txtPostalCode, userInfo.PostalCode);
            }

            this.Driver.Click(this.btnContinue);
            this.Driver.WaitUntilElementIsFound(new ElementLocator(".summary_info"));

            if (productNames != null)
            {
                Assert.AreEqual(this.Driver.GetElements(this.txtProductName).Select(e => e.Text), productNames);
            }

            this.Driver.Click(this.btnFinish);
            this.Driver.WaitUntilElementIsFound(new ElementLocator(".complete-header"));
            return this;
        }

        public void AssertCheckoutComplete()
        {
            Assert.AreEqual(this.Driver.GetText(new ElementLocator(".complete-header")), "THANK YOU FOR YOUR ORDER");
        }
    }
}