namespace Selenious.Core.DriverActions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using AutoIt;
    using NLog;
    using OpenQA.Selenium;
    using OpenQA.Selenium.Interactions;
    using OpenQA.Selenium.Support.UI;
    using Selenious.Core;
    using Selenious.Core.GlobalDataTypes;
    using Selenious.Utils;
    using Keys = OpenQA.Selenium.Keys;

    public static partial class DriverActions
    {
        private static readonly Logger Logger = LogManager.GetLogger("DRIVER");

        private static readonly double LongTimeout = BaseConfiguration.LongTimeout;
        private static readonly double MediumTimeout = BaseConfiguration.MediumTimeout;
        private static readonly double ShortTimeout = BaseConfiguration.ShortTimeout;

        public static void WaitForPageLoad(this IWebDriver driver)
        {
            WaitForPageLoad(driver, MediumTimeout);
        }

        /// <summary>
        /// Get Element command takes in the element loactor as the parameter and returns an object of type IWebElement.
        /// Various locator strategies can be used such as ID, Name, Class Name, XPATH etc, default being CSS selector.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">Searches for shadow root locator, for the passed element.If true then finds and returns the actual element.</param>
        /// <param name="timeout">waits for the element until timeout</param>
        /// <returns>return.</returns>
        public static IWebElement GetElement(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            webDriver.WaitForPageLoad();
            double timer = (timeout == -1) ? LongTimeout : timeout;

            if (IsLocatorWithShadowRoot(locator))
            {
                IWebElement rootElement = null;
                rootElement = webDriver.GetRootElement(new ElementLocator(locator.RootLocatorValue, locator.RootLocatorKind), timer);
                return rootElement.GetElement(new ElementLocator(locator.Value, locator.Kind), timeout);
            }

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
            try
            {
                try
                {
                    wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                    wait.Until(e => webDriver.FindElement(locator.ToBy()) != null);
                }
                catch (WebDriverTimeoutException ex)
                {
                    throw new TimeoutException($"Timeout while finding Element {locator.Value}." + ex.StackTrace);
                }
                catch
                {
                    wait.Until(e => webDriver.FindElement(locator.ToBy()) != null);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Timeout while finding Element '{locator.Value}'." + ex.StackTrace);
            }

            return webDriver.FindElement(locator.ToBy());
        }

        /// <summary>
        ///  Get Elements command takes in the element loactor as the parameter and returns a list of type IWebElement.
        ///  Various locator strategies can be used such as ID, Name, Class Name, XPATH etc, default being CSS Selector
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">Searches for shadow root locator, for the passed element.If true then finds and returns the list</param>
        /// <param name="timeout">waits for the element until timeout</param>
        /// <returns>return.</returns>
        public static IList<IWebElement> GetElements(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            if (IsLocatorWithShadowRoot(locator))
            {
                IWebElement rootElement = null;
                rootElement = webDriver.GetRootElement(new ElementLocator(locator.RootLocatorValue, locator.RootLocatorKind), timer);
                return rootElement.GetElements(new ElementLocator(locator.Value, locator.Kind), timeout);
            }

            try
            {
                var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
                wait.Until(elms => webDriver.FindElements(locator.ToBy()).Count >= 0);
            }
            catch
            {
            }

            return webDriver.FindElements(locator.ToBy());
        }

        /// <summary>
        ///  Scrolls into the view, where element is present, using parameter as element locator.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        public static void ScrollIntoView(this IWebDriver webDriver, ElementLocator locator)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            js.ExecuteScript($"arguments[0].scrollIntoView();", webDriver.GetElement(locator));
        }

        /// <summary>
        /// Scrolls into the view, where element is present, using parameter as element.
        /// </summary>
        /// /// <param name="webDriver">webDriver.</param>
        /// <param name="element">element.</param>
        public static void ScrollIntoView(this IWebDriver webDriver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            js.ExecuteScript($"arguments[0].scrollIntoView();", element);
        }

        /// <summary>
        /// Get Element command takes in the element loactor as the parameter and returns an object of any defined type.
        /// Various locator strategies can be used such as ID, Name, Class Name, XPATH etc, default being CSS selector.
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">timeout.</param>
        /// <returns>return.</returns>
        public static T GetElement<T>(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
            where T : class, IWebElement
        {
            IWebElement element = webDriver.GetElement(locator, timeout);
            return element.As<T>();
        }

        /// <summary>
        /// Get Element command takes in the element loactor as the parameter and returns a list of any dfined type.
        /// Various locator strategies can be used such as ID, Name, Class Name, XPATH etc, default being CSS selector.
        /// </summary>
        /// <typeparam name="T">Type T.</typeparam>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">timeout.</param>
        public static IList<T> GetElements<T>(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
            where T : class, IWebElement
        {
            IList<IWebElement> webElements = webDriver.GetElements(locator, timeout);
            return
                new ReadOnlyCollection<T>(
                    webElements.Select(e => e.As<T>()).ToList());
        }

        /// <summary>
        /// Waits for the element to be clickable until timed out. Finds out the element, using the passed locator and performs clicks on that element.
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the element until timed out.</param>
        /// <param name="scrollToView">If passed as true, then scrolls into view, where element is present. Default is false.</param>
        public static void Click(this IWebDriver webDriver, ElementLocator locator, double timeout = -1, bool scrollToView = false)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            webDriver.WaitUnitilElementClickable(locator, timeout);

            IWebElement element = webDriver.GetElement(locator, timer);
            webDriver.Click(element, scrollToView);
        }

        /// <summary>
        /// First clicks on the element, passed as the argument. Click using javascript, if the former click fails.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="element">element.</param>
        /// <param name="scrollToView">If passed as true, then scrolls into view, where element is present. Default is false.</param>
        public static void Click(this IWebDriver webDriver, IWebElement element, bool scrollToView = false)
        {
            if (scrollToView)
            {
                webDriver.ScrollIntoView(element);
            }

            try
            {
                try
                {
                    element.Click();
                }
                catch
                {
                    ((IJavaScriptExecutor)webDriver).ExecuteScript("arguments[0].click();", element);
                }
            }
            catch (StaleElementReferenceException)
            {
                webDriver.WaitForPageLoad();
                element.Click();
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to click on element." + ex.StackTrace);
            }

            webDriver.WaitForPageLoad();
        }

        /// <summary>
        ///  Double clicks on the element, whose locator is passed as the parameter.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        public static void DoubleClick(this IWebDriver webDriver, ElementLocator locator)
        {
            IWebElement element = webDriver.GetElement(locator);
            webDriver.DoubleClick(element);
        }

        /// <summary>
        ///  Double clicks on the element, whose element is passed as the parameter.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="element">element.</param>
        public static void DoubleClick(this IWebDriver webDriver, IWebElement element)
        {
            Actions builder = new Actions(webDriver);
            builder.DoubleClick(element).Perform();
        }

        /// <summary>
        /// Clicks the desired element using JavaScriptExecutor.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        public static void ClickUsingJS(this IWebDriver webDriver, ElementLocator locator)
        {
            ((IJavaScriptExecutor)webDriver).ExecuteScript($"document.querySelector('{locator.Value}').click()");
        }

        /// <summary>
        /// Clicks on the desired element, waits for that expected element to be found.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="expectedElementToBePresentAfterClick">expectedElementToBePresentAfterClick.</param>
        /// <param name="timeout">Waits for the expected element until timed out.</param>
        public static void ClickAndWaitForElement(this IWebDriver webDriver, ElementLocator locator, ElementLocator expectedElementToBePresentAfterClick, double timeout = -1)
        {
            webDriver.Click(locator, 10);
            webDriver.WaitUntilElementIsFound(expectedElementToBePresentAfterClick, timeout);
            webDriver.WaitForPageLoad();
        }

        /// <summary>
        /// Clicks the desired element from a list of elements, by passing the element locator.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="textToSearch">Text of the desired element.</param>
        /// <param name="timeout">Finds the elements until timed out.</param>
        /// <param name="exactSearch">If true, looks for the execat element text. Default is false.</param>
        /// <param name="scrollToView">If passed as true, then scrolls into view, where element is present. Default is false.</param>
        public static void ClickElementByText(this IWebDriver webDriver, ElementLocator locator, string textToSearch, double timeout = -1, bool exactSearch = true, bool scrollToView = false)
        {
            IList<IWebElement> elements = webDriver.GetElements(locator, timeout);
            if (elements.Count > 0)
            {
                IWebElement elm;
                if (exactSearch)
                {
                    elm = elements.Where(e => e.Text.ToLower().Trim() == textToSearch.ToLower().Trim()).First();
                }
                else
                {
                    elm = elements.Where(e => e.Text.ToLower().Trim().Contains(textToSearch.ToLower().Trim())).First();
                }

                webDriver.Click(elm, scrollToView);
            }
            else
            {
                throw new Exception($"Locator {locator.Value} retuned no elements.");
            }
        }

        /// <summary>
        /// Clicks the desired element from a list of elements, by passing the list of IWebElements.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="elements">elements.</param>
        /// <param name="textToSearch">Text of the desired element.</param>
        /// <param name="timeout">Finds the elements until timed out.</param>
        /// <param name="exactSearch">If true, looks for the execat element text. Default is false.</param>
        /// <param name="scrollToView">If passed as true, then scrolls into view, where element is present. Default is false.</param>
        public static void ClickElementByText(this IWebDriver webDriver, IList<IWebElement> elements, string textToSearch, double timeout = -1, bool exactSearch = true, bool scrollToView = false)
        {
            if (elements.Count > 0)
            {
                IWebElement elm;
                if (exactSearch)
                {
                    elm = elements.Where(e => e.Text.ToLower() == textToSearch.ToLower()).First();
                }
                else
                {
                    elm = elements.Where(e => e.Text.ToLower().Contains(textToSearch.ToLower())).First();
                }

                webDriver.Click(elm, scrollToView);
            }
            else
            {
                throw new Exception($"List contains no elements");
            }
        }

        /// <summary>
        /// Inputs the desired text by finding the passed element locator.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="text">Text to be updated or input.</param>
        /// <param name="timeout">Finds desired element until timed out.</param>
        public static void UpdateText(this IWebDriver webDriver, ElementLocator locator, string text, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            IWebElement txtBox = webDriver.GetElement(locator, timer);
            webDriver.UpdateText(txtBox, text);
        }

        /// <summary>
        /// Clears the already present text. Inputs the desired text. Repeats three times until successful.
        /// Uses JavaScriptExecutor if unsuccessful using former method.
        /// Thows exception if still unsuccessful.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="element">element.</param>
        /// <param name="text">Text to be updated.</param>
        public static void UpdateText(this IWebDriver webDriver, IWebElement element, string text)
        {
            try
            {
                element.Clear();
                element.SendKeys(text);

                int retry = 0;
                while (retry < 3)
                {
                    if (element.GetAttribute("value") == text)
                    {
                        break;
                    }
                    else
                    {
                        element.Clear();
                        element.SendKeys(text);

                        retry++;
                        webDriver.Sleep(1);
                    }
                }

                if (string.IsNullOrEmpty(element.GetAttribute("value")))
                {
                    webDriver.ExecuteJavaScript("arguments[0].value = arguments[1]", element, text);
                    if (string.IsNullOrEmpty(element.GetAttribute("value")))
                    {
                        throw new Exception("Element type could be different. Try changing the locator or handle via JS.");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("unable to update text: " + ex.StackTrace);
            }
        }

        /// <summary>
        /// Finds desired element and returns its text.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the element until timed out.</param>
        /// <returns>return.</returns>
        public static string GetText(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;

            IWebElement element = webDriver.GetElement(locator, timer);
            return webDriver.GetText(element);
        }

        /// <summary>
        /// Returns the text of the desired element using Text property of Selenium. If not null, then trims the text.
        /// Otherwise, returns the text using GetAttribute() property of that element, after trimming it.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="element">element.</param>
        /// <returns>return.</returns>
        public static string GetText(this IWebDriver webDriver, IWebElement element)
        {
            string text = element.Text;
            if (!string.IsNullOrEmpty(text))
            {
                return text.Trim();
            }
            else
            {
                text = element.GetAttribute("value");
                if (!string.IsNullOrEmpty(text))
                {
                    return text.Trim();
                }

                text = element.GetAttribute("innerHTML");
                if (!string.IsNullOrEmpty(text))
                {
                    return text.Trim();
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        /// <summary>
        /// Returns the index of desired text, from the passed list of elements.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="elements">List of elements.</param>
        /// <param name="text">Text whose index is to be returned.</param>
        /// <returns>return.</returns>
        public static int GetIndex(this IWebDriver webDriver, List<IWebElement> elements, string text)
        {
            foreach (IWebElement element in elements)
            {
                if (webDriver.GetText(element).ToLower().Equals(text.ToLower()))
                {
                    return elements.IndexOf(element);
                }
            }

            throw new NotFoundException($"'{text}' not found in the given list.");
        }

        /// <summary>
        /// Returns the index of desired text, from the passed element(list) locator.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="listLocator">Locator of list, which contains the desired text.</param>
        /// <param name="text">Text whose index is to be returned.</param>
        /// <returns>return.</returns>
        public static int GetIndex(this IWebDriver webDriver, ElementLocator listLocator, string text)
        {
            List<IWebElement> elements = webDriver.GetElements(listLocator).ToList();
            return webDriver.GetIndex(elements, text);
        }

        /// <summary>
        /// Method of interface JavaScriptExecutor for executing through Selenium WebDriver on selected webpage or frame or window.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="script">Desired javascript.</param>
        /// <param name="args">Arguments of the javascript, which are optional.</param>
        /// <returns>return.</returns>
        public static object ExecuteJavaScript(this IWebDriver webDriver, string script, params object[] args)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
            return js.ExecuteScript(script, args);
        }

        /// <summary>
        /// Finds the desired element, using element locator. Returns true if element is enabled.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the desired element, until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsElementEnabled(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            try
            {
                IWebElement element = webDriver.GetElement(locator, timer);
                if (element.Enabled)
                {
                    return true;
                }

                return false;
            }
            catch (WebDriverException)
            {
                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Waits till it finds the desired element, using element locator.Returns true if element is checked.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Waits for the desired element, until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsElementChecked(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            try
            {
                webDriver.WaitUntilElementIsFound(locator, timer);
                webDriver.GetElement(locator, timer).GetElement(new ElementLocator("[checked='checked']"), 1);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the desired element, using element locator. Return true if element is present.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the desired element, until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsElementPresent(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            try
            {
                IWebElement element = webDriver.GetElement(locator, timer);
                if (element != null)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Finds the desired element, using element locator. Return true if element is displayed.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the desired element, until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsElementDisplayed(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            try
            {
                IWebElement element = webDriver.GetElement(locator, timer);
                if (element.Displayed)
                {
                    return true;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the desired element, using element locator, to be clickable. Return true if element is not null.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the desired element, until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsElementClickable(this IWebDriver webDriver, ElementLocator locator, double timeout)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            try
            {
                var by = locator.ToBy();
                WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
                IWebElement element = wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(@by));

                if (element != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Waits for the desired element, using element locator, to be clickable.
        /// If former generates error, uses JavaScriptExecutor interface to fid desried element. Again waits for the desired element to be clickable.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Waits for the desired element, until timed out.</param>
        public static void WaitUnitilElementClickable(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            ElementLocator loc = GetInteractableElementLocator(locator);

            WebDriverWait wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));

            try
            {
                wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(loc.ToBy()));
            }
            catch
            {
                try
                {
                    IJavaScriptExecutor js = (IJavaScriptExecutor)webDriver;
                    js.ExecuteScript("arguments[0].scrollIntoView(true);", webDriver.FindElement(loc.ToBy()));
                    wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(loc.ToBy()));
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Return true if desired page  title is found. Otherwise, returns the actual page title as an error message.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="pageTitle">pageTitle</param>
        /// <param name="timeout">Waits for the desired page title until timed out.</param>
        /// <returns>return.</returns>
        public static bool IsPageTitle(this IWebDriver webDriver, string pageTitle, double timeout)
        {
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));

            try
            {
                wait.Until(d => d.Title.ToLower(CultureInfo.CurrentCulture) == pageTitle.ToLower(CultureInfo.CurrentCulture));
            }
            catch (WebDriverException)
            {
                Logger.Error(CultureInfo.CurrentCulture, "Actual page title is {0};", webDriver.Title);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Waits until desired element, using element locator, is not found.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">waits for the desired element until timed out.</param>
        public static void WaitUntilElementIsNoLongerFound(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? ShortTimeout : timeout;
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));

            ElementLocator loc = GetInteractableElementLocator(locator);

            try
            {
                wait.Until(driver => webDriver.GetElements(loc, timer).Count == 0);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Waits until webDriver switches to the desired frame. Waits until desired element is found on that frame.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="frameName">frameName.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Waits untill frame and element locator are timed out.</param>
        public static void WaitForIFrameToLoad(this IWebDriver webDriver, string frameName, ElementLocator locator = null, double timeout = -1)
        {
            double timer = (timeout == -1) ? BaseConfiguration.LongTimeout : timeout;

            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout));
            wait.IgnoreExceptionTypes(typeof(NoSuchFrameException), typeof(NoSuchWindowException));
            wait.Until(driver => webDriver.SwitchTo().Frame(frameName) != null);
            if (locator != null)
            {
                webDriver.WaitUntilElementIsFound(locator, timeout);
            }
        }

        /// <summary>
        /// Waits until desired element, using element locator, is found.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">waits for the desired element until timed out.</param>
        public static void WaitUntilElementIsFound(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            double timer = (timeout == -1) ? ShortTimeout : timeout;
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
            ElementLocator loc = GetInteractableElementLocator(locator);

            try
            {
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                wait.Until(driver => webDriver.GetElement(loc, timer).Displayed == true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Waits for the current url to be the desired url.
        /// </summary>
        /// <param name="webDriver">webDriver</param>
        /// <param name="urlToBe">Desired URL.</param>
        /// <param name="timeout">Waits for the url to be deisred url until timed out.</param>
        public static void WaitUntilUrlToBe(this IWebDriver webDriver, string urlToBe, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
            try
            {
                ElementLocator loc = new ElementLocator("[role='dialog']");
                wait.Until(driver => webDriver.GetCurrentUrl().Equals(urlToBe) == true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Waits for the current url to have desired url string.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="urlString">Desired url string</param>
        /// <param name="timeout">Waits for the url to have deisred url tring until timed out.</param>
        public static void WaitUntilUrlContains(this IWebDriver webDriver, string urlString, double timeout = -1)
        {
            double timer = (timeout == -1) ? MediumTimeout : timeout;
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
            try
            {
                ElementLocator loc = new ElementLocator("[role='dialog']");
                wait.Until(driver => webDriver.GetCurrentUrl().Contains(urlString) == true);
            }
            catch
            {
            }
        }

        /// <summary>
        /// Returns true if navigation to desired window is successful
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="url">url.</param>
        /// <param name="timeout">Waits for element to be displayed until timed out.</param>
        public static void SwitchToWindowUsingUrl(this IWebDriver webDriver, Uri url, double timeout)
        {
            double timer = (timeout == -1) ? ShortTimeout : timeout;
            var wait = new WebDriverWait(webDriver, TimeSpan.FromSeconds(timer));
            wait.Until(
                driver =>
                {
                    foreach (var handle in webDriver.WindowHandles)
                    {
                        webDriver.SwitchTo().Window(handle);
                        if (driver.Url.Equals(url.ToString()))
                        {
                            return true;
                        }
                    }

                    return false;
                });
        }

        /// <summary>
        /// Allows user to input the desired key.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="key">Desired key.</param>
        public static void PressKey(this IWebDriver webDriver, string key)
        {
            Actions builder = new Actions(webDriver);
            builder.SendKeys(key.ToString()).Perform();
        }

        /// <summary>
        /// Inputs arrow down key.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        public static void PressDownArrow(this IWebDriver webDriver)
        {
            Actions builder = new Actions(webDriver);
            builder.SendKeys(Keys.ArrowDown).Perform();
        }

        /// <summary>
        /// Inputs page down key.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        public static void ScrollPageDown(this IWebDriver webDriver)
        {
            Actions builder = new Actions(webDriver);
            builder.SendKeys(Keys.PageDown).Perform();
        }

        /// <summary>
        /// Returns the url of the current web page.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <returns>return.</returns>
        public static string GetCurrentUrl(this IWebDriver webDriver)
        {
            return webDriver.Url;
        }

        /// <summary>
        /// Returns the page source of the current web page in the string format
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <returns>return.</returns>
        public static string GetPageSource(this IWebDriver webDriver)
        {
            webDriver.WaitForPageLoad();
            return webDriver.PageSource;
        }

        /// <summary>
        /// Returns the
        /// </summary>
        /// <param name="webDriver"></param>
        /// <returns></returns>
        public static string GetBaseUrl(this IWebDriver webDriver)
        {
            string currentUrl = webDriver.GetCurrentUrl();
            Uri fullUrl = new Uri(currentUrl);
            string baseUrl = fullUrl.GetLeftPart(System.UriPartial.Authority);
            return baseUrl;
        }

        /// <summary>
        /// Switches the focus to the most recently opened window.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        public static void SwitchToNewTab(this IWebDriver webDriver)
        {
            webDriver.SwitchTo().Window(webDriver.WindowHandles.Last());
            webDriver.Sleep(1);
        }

        /// <summary>
        /// Switches the focus to the first opened or main window
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        public static void SwitchToMainTab(this IWebDriver webDriver)
        {
            webDriver.SwitchTo().Window(webDriver.WindowHandles.First());
            webDriver.Sleep(1);
        }

        /// <summary>
        /// Switches to the desired window.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="windowName">windowName.    </param>
        public static void SwitchToWindow(this IWebDriver webDriver, string windowName)
        {
            webDriver.SwitchTo().Window(windowName);
            webDriver.Sleep(1);
        }

        /// <summary>
        /// Finds the desired element, by passing elemnt locator and scrolls into the middle of the middle of the screen.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        public static void ScrollIntoMiddle(this IWebDriver webDriver, ElementLocator locator)
        {
            var js = (IJavaScriptExecutor)webDriver;
            var element = webDriver.ToDriver().GetElement(locator);

            var height = webDriver.Manage().Window.Size.Height;

            var hoverItem = (ILocatable)element;
            var locationY = hoverItem.LocationOnScreenOnceScrolledIntoView.Y;
            js.ExecuteScript(string.Format(CultureInfo.InvariantCulture, "javascript:window.scrollBy(0,{0})", locationY - (height / 2)));
        }

        /// <summary>
        /// mouse hovers to the  desired element.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        public static void MouseOver(this IWebDriver webDriver, ElementLocator locator)
        {
            Actions actions = new Actions(webDriver);
            actions.MoveToElement(webDriver.GetElement(locator)).Build().Perform();
            webDriver.Sleep(1);
        }

        /// <summary>
        /// Selects the desired value from a dropdown list of options.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="value">value.</param>
        public static void SelectDropDownValue(this IWebDriver webDriver, ElementLocator locator, string value)
        {
            webDriver.Click(locator);
            var options = webDriver.GetElements(new ElementLocator("select option"));
            options.Where(p => p.Text.ToLower() == value.ToLower()).First().Click();
        }

        /// <summary>
        /// Selects the desired index from a dropdown list of options.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="index">index.</param>
        public static void SelectDropDownByIndex(this IWebDriver webDriver, ElementLocator locator, int index)
        {
            webDriver.Click(locator);
            var options = webDriver.GetElements(new ElementLocator("select option"));
            webDriver.Click(options[index - 1]);
        }

        /// <summary>
        /// Suspends the current thread for the desired amount of time.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="seconds">seconds.</param>
        public static void Sleep(this IWebDriver webDriver, double seconds)
        {
            Thread.Sleep(int.Parse(seconds.ToString()) * 1000);
        }

        /// <summary>
        /// Clicks on the desired text on the button.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="buttonText">buttonText.</param>
        /// <param name="jsClick">jsClick.</param>
        public static void ClickButtonByName(this IWebDriver webDriver, string buttonText, bool jsClick = false)
        {
            ElementLocator buttonXP = new ElementLocator(string.Format("//button[text()='{0}']", buttonText), Locator.XPath);
            try
            {
                if (webDriver.IsElementDisplayed(buttonXP, 5))
                {
                    webDriver.Click(buttonXP);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Uploads the desired file, using AutoItx.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="fileName">fileName.</param>
        public static void UploadFile(this IWebDriver webDriver, string fileName)
        {
            AutoItX.WinActivate("Open");
            AutoItX.ControlGetFocus("[CLASS:#32770;Title:Open]");
            AutoItX.Sleep(2000);
            AutoItX.ControlSend("[CLASS:#32770;Title:Open]", " ", "[CLASS:Edit]", fileName, 1);
            AutoItX.Sleep(2000);
            AutoItX.ControlClick("Open", " ", "Button1");
        }

        private static void WaitForPageLoad(this IWebDriver webDriver, double timeout)
        {
            // Wait for ReadyState complete
            try
            {
                new WebDriverWait(webDriver, TimeSpan.FromSeconds(30)).Until(
                    driver =>
                    {
                        return driver is IJavaScriptExecutor jsExecutor
                               &&
                               (bool)jsExecutor.ExecuteScript(
                                   "return document.readyState").Equals("complete");
                    });
            }
            catch
            {
            }

            var javaScriptExecutor = webDriver as IJavaScriptExecutor;

            // Check if JQuery is present and has finished loading
            bool jQueryResult;
            try
            {
                jQueryResult = (bool)javaScriptExecutor.ExecuteScript("return (typeof jQuery != 'undefined');");
            }
            catch
            {
                jQueryResult = false;
            }

            if (jQueryResult)
            {
                try
                {
                    new WebDriverWait(webDriver, TimeSpan.FromSeconds(timeout)).Until(
                        driver =>
                        {
                            return driver is IJavaScriptExecutor jsExecutor
                            && (bool)jsExecutor.ExecuteScript("return (jQuery.active === 0);");
                        });
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static T As<T>(this IWebElement webElement)
            where T : class, IWebElement
        {
            var constructor = typeof(T).GetConstructor(new[] { typeof(IWebElement) });

            if (constructor != null)
            {
                return constructor.Invoke(new object[] { webElement }) as T;
            }

            throw new ArgumentNullException(string.Format(CultureInfo.CurrentCulture, "Constructor for type {0} is null.", typeof(T)));
        }

        /// <summary>
        /// Returns true if passed locator has a shadow root
        /// </summary>
        /// <param name="locator">locator.</param>
        /// <returns>return.</returns>
        private static bool IsLocatorWithShadowRoot(ElementLocator locator)
        {
            return string.IsNullOrEmpty(locator.RootLocatorValue) ? false : true;
        }

        /// <summary>
        /// Finds the root element of the desired locator executing javascript.
        /// </summary>
        /// <param name="webDriver">webDriver.</param>
        /// <param name="locator">locator.</param>
        /// <param name="timeout">Finds the desired element until timed out.</param>
        /// <returns>return.</returns>
        private static IWebElement GetRootElement(this IWebDriver webDriver, ElementLocator locator, double timeout = -1)
        {
            IWebElement element = webDriver.GetElement(locator, timeout);
            IWebElement shadowRoot = (IWebElement)((IJavaScriptExecutor)webDriver).ExecuteScript("return arguments[0].shadowRoot", element);
            return shadowRoot;
        }

        /// <summary>
        /// Returns the root locator for desired locator if shadow root is present for it.
        /// Otherwise, returns the passed locator.
        /// </summary>
        /// <param name="locator">locator.</param>
        /// <returns>return.</returns>
        private static ElementLocator GetInteractableElementLocator(ElementLocator locator)
        {
            if (IsLocatorWithShadowRoot(locator))
            {
                return new ElementLocator(locator.RootLocatorValue, locator.RootLocatorKind);
            }

            return locator;
        }
    }
}