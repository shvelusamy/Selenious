namespace Selenious.Utils.Helpers
{
    using System;
    using System.Collections.Generic;

    public static class NumberHelper
    {
        /// <summary>
        /// Returns a list of integers containing random numbers, of the desired length.
        /// </summary>
        public static IList<int> GenerateRandomNumbers(int length, int min = 1, int max = 1000)
        {
            IList<int> generatedNumber = new List<int>();
            Random randmoNmber = new Random();
            for (int i = 0; i < length; i++)
            {
                generatedNumber.Add(randmoNmber.Next(min, max));
            }

            return generatedNumber;
        }
     }
}