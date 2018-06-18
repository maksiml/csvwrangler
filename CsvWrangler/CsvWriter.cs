// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvWriter.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV writer. Converts lists of objects to CSV files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable UseStringInterpolation
namespace CsvWrangler
{
    using System;
    using System.Collections.Generic;
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
        /// List of parameters that is used to find ToString method that accepts culture info.
        /// </summary>
        private static readonly Type[] CultureSensitiveToStringParameters = { typeof(CultureInfo) };

        /// <summary>
        /// Converts list of object of the same type to CSV and saves to file.
        /// </summary>
        /// <param name="items">
        /// The items.
        /// </param>
        /// <param name="csvFilePath">
        /// The CSV file path.
        /// </param>
        /// <param name="options">
        /// Options for data serialization.
        /// </param>
        /// <typeparam name="T">
        /// Type of the item in the list.
        /// </typeparam>
        public static void ToCsvFile<T>(IEnumerable<T> items, string csvFilePath, CsvWriterOptions options = null)
        {
            using (var stream = ToCsv(items, options))
            using (var fileStream = File.Create(csvFilePath))
            {
                stream.CopyTo(fileStream);   
            }
        }

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
        public static Stream ToCsv<T>(IEnumerable<T> items, CsvWriterOptions options = null)
        {
            if (options == null)
            {
                options = new CsvWriterOptions();
            }

            return new StringListStream(GetStrings(items, options));
        }

        /// <summary>
        /// Convert list of objects of the same type to list of strings.
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
        /// List of serialized objects.
        /// </returns>
        private static IEnumerator<string> GetStrings<T>(IEnumerable<T> items, CsvWriterOptions options)
        {
            Type sourceType = null;
            PropertyInfo[] properties = null;
            var stringBuilder = new StringBuilder();
            foreach (var item in items)
            {
                if (sourceType == null)
                {
                    sourceType = item.GetType();
                    properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                    stringBuilder.Append(string.Join(",", properties.Select(property => property.Name)));
                }

                yield return stringBuilder.ToString();
                stringBuilder.Clear();
                stringBuilder.Append('\n');
                for (int i = 0; i < properties.Length; i++)
                {
                    if (properties[i].PropertyType == typeof(DateTime))
                    {
                        var value = (DateTime)properties[i].GetMethod.Invoke(item, null);
                        var serializedValue = !string.IsNullOrEmpty(options.DateTimeFormat)
                                                  ? value.ToString(options.DateTimeFormat)
                                                  : value.ToString(options.CultureInfo);
                        stringBuilder.Append(serializedValue);
                    }
                    else
                    {
                        var toStringMethod = properties[i].PropertyType.GetMethod("ToString", CultureSensitiveToStringParameters);
                        var value = properties[i].GetMethod.Invoke(item, null);
                        if (toStringMethod != null)
                        {
                            value = toStringMethod.Invoke(value, new object[] { options.CultureInfo });
                        }

                        string stringValue = value.ToString();
                        if (stringValue.IndexOfAny(new[] { ',', '\r', '\n' }) >= 0)
                        {
                            stringValue = string.Format("\"{0}\"", stringValue.Replace("\"", "\"\""));
                        }

                        stringBuilder.Append(stringValue);
                    }

                    if (i < properties.Length - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
            }

            yield return stringBuilder.ToString();
        }  
    }
}
