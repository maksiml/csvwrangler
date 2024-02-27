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
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
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
            bool isDictionary = false;
            bool isExpandoObject = false;
            List<object> keys = null;
            PropertyInfo[] properties = null;
            var stringBuilder = new StringBuilder();
            foreach (var item in items)
            {
                if (sourceType == null)
                {
                    isExpandoObject = item.GetType() == typeof(ExpandoObject);
                    isDictionary = item is IDictionary;
                    sourceType = item.GetType();
                    if (isDictionary || isExpandoObject)
                    {
                        keys = isExpandoObject ? GetDictionaryKeys(item as ExpandoObject) : GetDictionaryKeys((IDictionary)item);
                        stringBuilder.Append(string.Join(",", keys));
                    }
                    else
                    {
                        properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                            .Where(property => property.GetIndexParameters().Length == 0) // exclude indexers
                            .ToArray();
                        var headers = properties.Select(property => property.Name);
                        stringBuilder.Append(string.Join(",", headers));
                    }
                }

                yield return stringBuilder.ToString();
                stringBuilder.Clear();
                stringBuilder.Append('\n');

                if (isDictionary)
                {
                    AddRowFromDictionary((IDictionary)item, keys, options, stringBuilder);
                }
                else if (isExpandoObject)
                {
                    AddRowFromDictionary(item as ExpandoObject, keys, options, stringBuilder);
                }
                else
                {
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var propertyType = properties[i].PropertyType;
                        var value = properties[i].GetMethod.Invoke(item, null);
                        AppendValue(value, options, stringBuilder);

                        if (i < properties.Length - 1)
                        {
                            stringBuilder.Append(',');
                        }
                    }
                }
            }

            yield return stringBuilder.ToString();
        }

        /// <summary>
        /// Converts <paramref name="value"/> to string supporting culture through <paramref name="options"/>.
        /// </summary>
        /// <param name="value">
        /// Value to be appended to the CSV row.
        /// </param>
        /// <param name="options">
        /// Options that will be use for conversions in the context of selected culture.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder where the value will be appended.
        /// </param>
        private static void AppendValue(object value, CsvWriterOptions options, StringBuilder stringBuilder)
        {
            if (value == null)
            {
                return;
            }

            var propertyType = value.GetType();
            if (propertyType == typeof(DateTime))
            {
                var dateTime = (DateTime)value;
                var serializedValue = !string.IsNullOrEmpty(options.DateTimeFormat)
                                          ? dateTime.ToString(options.DateTimeFormat)
                                          : dateTime.ToString(options.CultureInfo);
                stringBuilder.Append(serializedValue);
            }
            else
            {
                var toStringMethod = propertyType.GetMethod("ToString", CultureSensitiveToStringParameters);

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
        }

        /// <summary>
        /// Retrieves properties from <seealso cref="ExpandoObject"/> class.
        /// </summary>
        /// <param name="item">
        /// An instance of an object from which properties will be retrieved.
        /// </param>
        /// <returns>
        /// The list of properties from <paramref name="item"/> that will be used as CSV headers.
        /// </returns>
        private static List<object> GetDictionaryKeys(ExpandoObject item)
        {
            return GetDictionaryKeys(item, i => ((IDictionary<string, object>)i).Keys);
        }

        /// <summary>
        /// Retrives list of keys from dictionary <paramref name="item"/>.
        /// </summary>
        /// <param name="item">
        /// An instance of a dictionary from which keys will be retrieved.
        /// </param>
        /// <returns>
        /// The list of keys from <paramref name="item"/> that will be used as CSV headers.
        /// </returns>
        private static List<object> GetDictionaryKeys(IDictionary item)
        {
            return GetDictionaryKeys(item, i => i.Keys);
        }

        /// <summary>
        /// Converts collection of keys to list.
        /// </summary>
        /// <typeparam name="T">
        /// The type of class from instance of which keys will be retrieved.
        /// </typeparam>
        /// <param name="item">
        /// An instance of a class from which the keys will be retrieved.
        /// </param>
        /// <param name="keyGetter">
        /// A function to retrive the keys as <see cref="IEnumerable"/> collection.
        /// </param>
        /// <returns>
        /// The list of keys from <paramref name="item"/> that will be used as CSV headers.
        /// </returns>
        private static List<object> GetDictionaryKeys<T>(T item, Func<T, IEnumerable> keyGetter)
        {
            List<object> keys = new List<object>();
            var itemKeys = keyGetter(item);
            foreach (var key in itemKeys)
            {
                keys.Add(key);
            }

            return keys;
        }

        /// <summary>
        /// Adds CSV row from the <see cref="ExpandoObject"/> instance.
        /// </summary>
        /// <param name="item">
        /// The instance of <see cref="ExpandoObject"/> who's properties will form a CSV row.
        /// </param>
        /// <param name="keys">
        /// The list of property names to be used from the <paramref name="item"/>.
        /// </param>
        /// <param name="options">
        /// Options that will be use for conversions in the context of selected culture.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder where the value will be appended.
        /// </param>
        private static void AddRowFromDictionary(ExpandoObject item, List<object> keys, CsvWriterOptions options, StringBuilder stringBuilder)
        {
            AddRowFromDictionary(item, keys, options, stringBuilder, (i, k) => ((IDictionary<string, object>)i)[(string)k]);
        }

        /// <summary>
        /// Adds CSV row from the <see cref="IDictionary"/> instance.
        /// </summary>
        /// <param name="item">
        /// The instance of <see cref="IDictionary"/> who's values will form a CSV row.
        /// </param>
        /// <param name="keys">
        /// The list of keys to be used from the <paramref name="item"/>.
        /// </param>
        /// <param name="options">
        /// Options that will be use for conversions in the context of selected culture.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder where the value will be appended.
        /// </param>
        private static void AddRowFromDictionary(IDictionary item, List<object> keys, CsvWriterOptions options, StringBuilder stringBuilder)
        {
            AddRowFromDictionary(item, keys, options, stringBuilder, (i, k) => i[k]);
        }

        /// <summary>
        /// Adds CSV row from the <paramref name="item"/> values.
        /// </summary>
        /// <typeparam name="T">
        /// The type of the item.
        /// </typeparam>
        /// <param name="item">
        /// The item who's values will form a CSV row.
        /// </param>
        /// <param name="keys">
        /// The list of keys to be used from the <paramref name="item"/>.
        /// </param>
        /// <param name="options">
        /// Options that will be use for conversions in the context of selected culture.
        /// </param>
        /// <param name="stringBuilder">
        /// The string builder where the value will be appended.
        /// </param>
        /// <param name="valueGetter">
        /// A function to retrive value from the <paramref name="item"/> by key.
        /// </param>
        private static void AddRowFromDictionary<T>(T item, List<object> keys, CsvWriterOptions options, StringBuilder stringBuilder, Func<T, object, object> valueGetter)
        {
            for (var i = 0; i < keys.Count; i++)
            {
                var key = keys[i];
                var value = valueGetter(item, key);
                AppendValue(value, options, stringBuilder);
                if (i < keys.Count - 1)
                {
                    stringBuilder.Append(',');
                }
            }
        }
    }
}
