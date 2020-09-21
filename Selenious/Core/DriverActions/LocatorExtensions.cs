namespace Selenious.Core.DriverActions
{
    using OpenQA.Selenium;
    using Selenious.Core.GlobalDataTypes;
    using Selenious.Utils;

    public static class LocatorExtensions
    {
        public static By ToBy(this ElementLocator locator)
        {
            By by;
            switch (locator.Kind)
            {
                case Locator.Id:
                    by = By.Id(locator.Value);
                    break;
                case Locator.ClassName:
                    by = By.ClassName(locator.Value);
                    break;
                case Locator.CssSelector:
                    by = By.CssSelector(locator.Value);
                    break;
                case Locator.LinkText:
                    by = By.LinkText(locator.Value);
                    break;
                case Locator.Name:
                    by = By.Name(locator.Value);
                    break;
                case Locator.PartialLinkText:
                    by = By.PartialLinkText(locator.Value);
                    break;
                case Locator.TagName:
                    by = By.TagName(locator.Value);
                    break;
                case Locator.XPath:
                    by = By.XPath(locator.Value);
                    break;
                default:
                    by = By.Id(locator.Value);
                    break;
            }

            return by;
        }
    }
}