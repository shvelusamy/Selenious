namespace Selenious.Project.TestSuite
{
    using NUnit.Framework;
    using Selenious.Core;
    using Selenious.Core.Project.TestData;
    using Selenious.Project.Pages;
    using Selenious.Project.TestData;

    [TestFixture]
    public class TestE2E : BaseTest
    {
        public LoginPage LoginPage => new LoginPage(this.Driver);

        public InventoryPage InventoryPage => new InventoryPage(this.Driver);

        public CartPage CartPage => new CartPage(this.Driver);

        public CheckoutPage CheckoutPage => new CheckoutPage(this.Driver);

        [Test]
        [Description("")]
        public void TestSwagLabsE2E()
        {
            this.LoginPage.Login("standard_user", "secret_sauce");

            string[] productNames = new string[] { "Sauce Labs Bolt T-Shirt", "Sauce Labs Fleece Jacket" };
            this.InventoryPage.AddToCart(productNames);

            this.InventoryPage.NavigateTo(PageOptions.Cart);
            Assert.AreEqual(this.CartPage.GetProductNames(), productNames);
            this.CartPage.ClickCheckout();

            UserInfo userInfo = new UserInfo
            {
                FirstName = "Tom",
                LastName = "Jerry",
                PostalCode = "12345",
            };

            this.CheckoutPage.DoCheckout(userInfo, productNames);
            this.CheckoutPage.AssertCheckoutComplete();
        }
    }
}