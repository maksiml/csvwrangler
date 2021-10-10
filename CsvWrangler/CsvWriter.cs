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
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    using Microsoft.CSharp.RuntimeBinder;

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

            // ReSharper disable once PossibleMultipleEnumeration
            return new StringListStream(items.FirstOrDefault() is DynamicObject ? GetDynamicObjectStrings(items, options) : GetStrings<T>(items, options));
        }

        private static IEnumerator<string> GetDynamicObjectStrings<T>(IEnumerable<T> items, CsvWriterOptions options)
        {
            var stringBuilder = new StringBuilder();
            List<string> headers = null;
            foreach (var item in items)
            {
                var dynamicItem = item as DynamicObject;
                if (dynamicItem == null)
                {
                    throw new InvalidOperationException(
                        $"All objects in the collection have to be of the same type. Expected type '{typeof(DynamicObject)}' actual type '{item.GetType()}'");
                }

                if (headers == null)
                {
                    headers = dynamicItem.GetDynamicMemberNames().ToList();
                    stringBuilder.Append(string.Join(",", headers));
                }

                yield return stringBuilder.ToString();
                stringBuilder.Clear();
                stringBuilder.Append('\n');

                for (int i = 0; i < headers.Count; i++)
                {
                    object valueObject = GetPropertyValue(item, headers[i]);
                    if (valueObject is DateTime)
                    {
                        var value = (DateTime)valueObject;
                        var serializedValue = !string.IsNullOrEmpty(options.DateTimeFormat)
                                                  ? value.ToString(options.DateTimeFormat)
                                                  : value.ToString(options.CultureInfo);
                        stringBuilder.Append(serializedValue);
                    }
                    else
                    {
                        var propertyType = valueObject.GetType();
                        var toStringMethod = propertyType.GetMethod("ToString", CultureSensitiveToStringParameters);
                        if (toStringMethod != null)
                        {
                            valueObject = toStringMethod.Invoke(valueObject, new object[] { options.CultureInfo });
                        }

                        string stringValue = valueObject.ToString();
                        if (stringValue.IndexOfAny(new[] { ',', '\r', '\n' }) >= 0)
                        {
                            stringValue = string.Format("\"{0}\"", stringValue.Replace("\"", "\"\""));
                        }

                        stringBuilder.Append(stringValue);
                    }

                    if (i < headers.Count - 1)
                    {
                        stringBuilder.Append(',');
                    }
                }
            }

            yield return stringBuilder.ToString();
        }


        private static readonly Dictionary<string, CallSite<Func<CallSite, object, object>>> CallSites =
            new Dictionary<string, CallSite<Func<CallSite, object, object>>>();

        private static object GetPropertyValue(object target, string propertyName)
        {
            var key = $"{target.GetType().FullName}{propertyName}";
            if (!CallSites.ContainsKey(key))
            {
                var binder = Microsoft.CSharp.RuntimeBinder.Binder.GetMember(
                    CSharpBinderFlags.None,
                    propertyName,
                    target.GetType(),
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null), });
                var callsite = CallSite<Func<CallSite, object, object>>.Create(binder);
                CallSites[key] = callsite;
            }

            return CallSites[key].Target(CallSites[key], target);
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
                    properties = sourceType.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .Where(property => property.GetIndexParameters().Length == 0) // exclude indexers
                        .ToArray();
                    var headers = properties.Select(property => property.Name);
                    stringBuilder.Append(string.Join(",", headers));
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
