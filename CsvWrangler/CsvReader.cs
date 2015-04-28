// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvReader.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV reader.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The CSV reader.
    /// </summary>
    public class CsvReader
    {
        /// <summary>
        /// Regular expression that matches permitted names. The expression is made
        /// stricter than C# rules on purpose to avoid confusion.
        /// </summary>
        private static readonly Regex AllowedHeaderNames = new Regex("^[_a-zA-Z][_0-9a-zA-Z]*$");

        /// <summary>
        /// Matches quotes surrounding expression.
        /// </summary>
        private static readonly Regex MatchSurroundingQuotes = new Regex("^[\"|'|«](?<header>.*)[\"|'|»]$");

        /// <summary>
        /// Parse CSV file to list of dynamic objects.
        /// </summary>
        /// <param name="input">
        /// The input CSV stream.
        /// </param>
        /// <param name="hasHeader">
        /// The has header.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The list of dynamic objects produced from CSV lines.
        /// </returns>
        public static IEnumerable<dynamic> Parse(Stream input, bool hasHeader = true, CsvReaderOptions options = null)
        {
            if (options == null)
            {
                options = new CsvReaderOptions();
            }

            using (TextReader reader = new StreamReader(input))
            {
                char separator = options.Separator;

                List<string> headers = null;

                if (hasHeader)
                {
                    string line = reader.ReadLine();
                    if (line == null)
                    {
                        yield break;
                    }

                    headers = line
                                .Split(separator)
                                .Select(TransformHeaderNameToPropertyName)
                                .ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        if (!AllowedHeaderNames.IsMatch(headers[i]))
                        {
                            headers[i] = string.Format("Column{0}", i);
                        }
                    }
                }

                List<string> values = CsvParser.ParseLine(reader, separator).ToList();
                while (values.Any())
                {
                    if (headers == null)
                    {
                        headers = new List<string>();
                        var headerCount = values.Count;
                        for (int i = 0; i < headerCount; i++)
                        {
                            headers.Add(string.Format("Column{0}", i));
                        }
                    }

                    dynamic lineObject = new CsvRow(headers, values, options);
                    yield return lineObject;
                    values = CsvParser.ParseLine(reader, separator).ToList();
                }
            }
        }

        /// <summary>
        /// Transforms header name to valid C# identifier if possible.
        /// </summary>
        /// <param name="headerName">
        /// The header name.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string TransformHeaderNameToPropertyName(string headerName)
        {
            string result = headerName.ToTitleCase().Replace(" ", string.Empty);
            if (MatchSurroundingQuotes.IsMatch(headerName))
            {
                result = MatchSurroundingQuotes.Match(headerName).Groups["header"].ToString().ToTitleCase();
            }

            return result;
        }
    }
}
