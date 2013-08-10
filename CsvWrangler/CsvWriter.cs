// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvWriter.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV writer. Converts lists of objects to CSV files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    /// <summary>
    /// The CSV writer. Converts lists of objects to CSV files.
    /// </summary>
    public class CsvWriter
    {
        /// <summary>
        /// Convert list of object of the same type to CSV.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <typeparam name="T">
        /// Type of the item in the list.
        /// </typeparam>
        /// <returns>
        /// Stream that contains the CSV.
        /// </returns>
        public static Stream ToCsv<T>(IEnumerable<T> items)
        {
            Type sourceType = typeof(T);
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Join(",", properties.Select(property => property.Name)));
            foreach (var item in items)
            {
                stringBuilder.Append("\n");
                for (int i = 0; i < properties.Length; i++)
                {
                    stringBuilder.Append(properties[i].GetMethod.Invoke(item, null));
                    if (i < properties.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
            }

            return stringBuilder.ToString().ToStream();
        }
    }
}
