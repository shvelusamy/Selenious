namespace Selenious.Core.DriverActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Internal;
    using OpenQA.Selenium.Support.UI;
    using Phoenix.Core.Utils.Helpers;
    using Selenious.Core;
    using Selenious.Core.GlobalDataTypes;

    public static class ElementActions
    {
        public static IWebElement GetElement(this IWebElement element, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? BaseConfiguration.MediumTimeout : timeout;
            return element.GetElement(locator, timeout, e => e.Displayed);
        }

        public static IWebElement GetElement(this IWebElement element, ElementLocator locator, double timeout, Func<IWebElement, bool> condition)
        {
            var driver = element.ToDriver();
            var by = locator.ToBy();
            driver.WaitForPageLoad();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            wait.Until(
                    drv =>
                    {
                        var ele = element.FindElement(@by);
                        return condition(ele);
                    });

            return element.FindElement(@by);
        }

        public static IList<IWebElement> GetElements(this IWebElement element, ElementLocator locator)
        {
            return element.GetElements(locator, e => e.Displayed).ToList();
        }

        public static IList<IWebElement> GetElements(this IWebElement element, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? BaseConfiguration.MediumTimeout : timeout;
            var driver = element.ToDriver();
            var by = locator.ToBy();
            try
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timer));
                wait.Until(elms => driver.FindElements(locator.ToBy()).Count >= 0);
            }
            catch
            {
            }

            return element.FindElements(by);
        }

        public static IList<IWebElement> GetElements(this IWebElement element, ElementLocator locator, double timeout, Func<IWebElement, bool> condition, int minNumberOfElements)
        {
            IList<IWebElement> elements = null;
            WaitHelper.Wait(
                () => (elements = GetElements(element, locator, condition).ToList()).Count >= minNumberOfElements,
                TimeSpan.FromSeconds(timeout),
                "Timeout while getting elements");
            return elements;
        }

        public static IList<IWebElement> GetElements(this IWebElement element, ElementLocator locator, Func<IWebElement, bool> condition)
        {
            element.ToDriver().WaitForPageLoad();
            return element.FindElements(locator.ToBy()).Where(condition).ToList();
        }

        public static IWebDriver ToDriver(this ISearchContext webElement)
        {
            if (webElement == null)
            {
                throw new ArgumentNullException(nameof(webElement));
            }

            var wrappedElement = webElement as IWrapsDriver;
            if (wrappedElement == null)
            {
                return (IWebDriver)webElement;
            }

            return wrappedElement.WrappedDriver;
        }
    }
}