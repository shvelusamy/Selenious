namespace Selenious.Core.GlobalDataTypes
{
    using System;
    using OpenQA.Selenium;

    /// <summary>
    /// Class that helps to define Kind and value for html elements.
    /// </summary>
    public class ErrorDetail
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ErrorDetail" /> class.
        /// </summary>
        /// <param name="screenshot">The screenshot.</param>
        /// <param name="dateTime">The date time.</param>
        /// <param name="exception">The exception.</param>
        public ErrorDetail(Screenshot screenshot, DateTime dateTime, Exception exception)
        {
            this.Screenshot = screenshot;
            this.DateTime = dateTime;
            this.Exception = exception;
        }

        /// <summary>
        /// Gets or sets the screenshot.
        /// </summary>
        /// <value>
        /// The screenshot.
        /// </value>
        public Screenshot Screenshot { get; set; }

        /// <summary>
        /// Gets or sets the date time.
        /// </summary>
        /// <value>
        /// The date time.
        /// </value>
        public DateTime DateTime { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        /// <value>
        /// The exception.
        /// </value>
        public Exception Exception { get; set; }
    }
}