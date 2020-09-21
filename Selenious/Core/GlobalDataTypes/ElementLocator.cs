namespace Selenious.Core.GlobalDataTypes
{
    using System.Globalization;
    using Selenious.Utils;

    public class ElementLocator
    {
        public ElementLocator(string value, Locator kind = Locator.CssSelector)
        {
            this.Kind = kind;
            this.Value = value;
        }

        public ElementLocator(string rootLocator, string actualLocator, Locator rootLocatorkind = Locator.CssSelector, Locator actualLocatorkind = Locator.CssSelector)
        {
            this.RootLocatorKind = rootLocatorkind;
            this.RootLocatorValue = rootLocator;

            this.Kind = actualLocatorkind;
            this.Value = actualLocator;
        }

        public Locator RootLocatorKind { get; set; }

        public string RootLocatorValue { get; set; }

        public Locator Kind { get; set; }

        public string Value { get; set; }

        public ElementLocator Format(params object[] parameters)
        {
            return new ElementLocator(string.Format(CultureInfo.CurrentCulture, this.Value, parameters), this.Kind);
        }
    }
}