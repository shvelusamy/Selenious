namespace Selenious.Project.Pages
{
    using Selenious.Core;
    using Selenious.Core.DriverActions;
    using Selenious.Core.DriverConfigurations;
    using Selenious.Core.GlobalDataTypes;

    public class LoginPage : BasePage
    {
        private readonly ElementLocator txtUserName = new ElementLocator("#user-name");
        private readonly ElementLocator txtPassword = new ElementLocator("#password");
        private readonly ElementLocator btnLogin = new ElementLocator("#login-button");
        private readonly ElementLocator loginErrorMsg = new ElementLocator(".error-button");

        public LoginPage(DriverContext driverContext)
            : base(driverContext)
        {
        }

        public LoginPage Login(string username, string password)
        {
            this.Driver.UpdateText(this.txtUserName, username);
            this.Driver.UpdateText(this.txtPassword, password);
            this.Driver.Click(this.btnLogin);
            return this;
        }

        public string GetLoginErrorText()
        {
            return this.Driver.GetText(this.loginErrorMsg, 10);
        }
    }
}