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
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
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
        /// <param name="options">
        /// Options for data serialization.
        /// </param>
        /// <typeparam name="T">
        /// Type of the item in the list.
        /// </typeparam>
        /// <returns>
        /// Stream that contains the CSV.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1614:ElementParameterDocumentationMustHaveText", Justification = "Reviewed. Suppression is OK here.")]
        public static Stream ToCsv<T>(IEnumerable<T> items, CsvWriterOptions options = null)
        {
            if (options == null)
            {
                options = new CsvWriterOptions();
            }

            Type sourceType = typeof(T);
            PropertyInfo[] properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);
            var stringBuilder = new StringBuilder();
            stringBuilder.Append(string.Join(",", properties.Select(property => property.Name)));
            foreach (var item in items)
            {
                stringBuilder.Append("\n");
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].PropertyType == typeof(DateTime))
                    {
                        var value = (DateTime)properties[i].GetMethod.Invoke(item, null);
                        var serializedValue = options != null && !string.IsNullOrEmpty(options.DateTimeFormat)
                                                  ? value.ToString(options.DateTimeFormat)
                                                  : value.ToString(options.CultureInfo);
                        stringBuilder.Append(serializedValue);
                    }
                    else
                    {
                        stringBuilder.Append(properties[i].GetMethod.Invoke(item, null));
                    }
                    
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
