namespace Selenious.Utils.Helpers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using NLog;

    public static class NameHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Returns a string containing random characters, of the desired length.
        /// </summary>
        public static string RandomName(int length)
        {
            const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

            var randomString = new StringBuilder();
            var random = new Random();

            for (int i = 0; i < length; i++)
            {
                randomString.Append(Chars[random.Next(Chars.Length)]);
            }

            return randomString.ToString();
        }

        /// <summary>
        /// Encodes all the characters in the desired string into a sequence of bytes
        /// converts this sequence of bytes to its equivalent string representation that is encoded with base-64 digits.
        /// </summary>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// Converts the desired string, which encodes binary data as base-64 digits, to an equivalent 8-bit unsigned integer array.
        /// Decodes a range of bytes from the array into a string.
        /// </summary>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }

        /// <summary>
        /// Shortens the desired filename, when foldername and filename exceeds the maximum length and replaces the desired pattern with empty string.
        /// Shortens the desired filename, when foldername and filename exceeds the maximum length.
        /// </summary>
        public static string ShortenFileName(string folder, string fileName, string pattern, int maxLength)
        {
            while (((folder + fileName).Length > maxLength) && fileName.Contains(pattern))
            {
                Regex rgx = new Regex(pattern);
                fileName = rgx.Replace(fileName, string.Empty, 1);
            }

            if ((folder + fileName).Length > 255)
            {
                Logger.Error(CultureInfo.CurrentCulture, "Length of the file full name is over {0} characters, try to shorten the name of tests", maxLength);
            }

            return fileName;
        }

        /// <summary>
        /// Removes special character and space in a given string
        /// </summary>
        public static string Sanitize(string name)
        {
            Regex rgx = new Regex("[^a-zA-Z0-9]");
            return rgx.Replace(name, string.Empty);
        }

        /// <summary>
        /// Adds space before each capital letter in a given input string
        /// </summary>
        /// <param name="input"></param>
        /// <returns> String </returns>
        public static string SplitCamelCase(string input)
        {
            return string.Concat(input.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
        }
    }
}