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
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// The CSV reader.
    /// </summary>
    public class CsvReader : IDisposable
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
        /// The CSV file stream.
        /// </summary>
        private Stream stream;

        /// <summary>
        /// Prevents a default instance of the <see cref="CsvReader"/> class from being created.
        /// </summary>
        private CsvReader()
        {
        }

        /// <summary>
        /// Gets the rows read from the file.
        /// </summary>
        public IEnumerable<dynamic> Rows { get; private set; }

        /// <summary>
        /// Parses CSV file. and returns <seealso cref="CsvReader"/> initialized with the content.
        /// </summary>
        /// <param name="csvFilePath">
        /// The CSV file path.
        /// </param>
        /// <param name="hasHeader">
        /// Indicates if the file has a header.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <returns>
        /// The <seealso cref="CsvReader"/> initialized with the content of the CSV <paramref name="csvFilePath"/>.
        /// </returns>
        public static CsvReader Parse(string csvFilePath, bool hasHeader = true, CsvReaderOptions options = null)
        {
            var stream = File.OpenRead(csvFilePath);
            return new CsvReader()
                       {
                           stream = stream,
                           Rows = Parse(stream, hasHeader, options),
                       };
        }

        /// <summary>
        /// Parse CSV file to list of dynamic objects.
        /// </summary>
        /// <param name="input">
        /// The input CSV stream.
        /// </param>
        /// <param name="hasHeader">
        /// Indicates if the file has a header.
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

                List<string> headers = new List<string>();

                if (hasHeader)
                {
                    HashSet<string> duplicateHeaderTracker = new HashSet<string>();
                    var headerRowComponents = CsvParser.ParseLine(reader, options.Separator).ToList();
                    if (!headerRowComponents.Any())
                    {
                        yield break;
                    }

                    for (int i = 0; i < headerRowComponents.Count; i++)
                    {
                        string currentHeader = TransformHeaderNameToPropertyName(headerRowComponents[i], options.HeaderMatchRegex ?? MatchSurroundingQuotes);
                        currentHeader = AllowedHeaderNames.IsMatch(currentHeader) ? currentHeader : null;
                        if (options.ResolveHeaderName != null)
                        {
                            var currentHeaderReplacement = options.ResolveHeaderName(
                                headerRowComponents[i],
                                currentHeader);

                            currentHeader = string.IsNullOrEmpty(currentHeaderReplacement)
                                                ? currentHeader
                                                : currentHeaderReplacement;
                        }

                        if (string.IsNullOrEmpty(currentHeader) || duplicateHeaderTracker.Contains(currentHeader))
                        {
                            currentHeader = $"Column{i}";
                        }

                        duplicateHeaderTracker.Add(currentHeader);
                        headers.Add(currentHeader);
                    }
                }

                List<string> values = CsvParser.ParseLine(reader, separator).ToList();
                while (values.Any())
                {
                    if (!headers.Any())
                    {
                        headers = new List<string>();
                        var headerCount = values.Count;
                        for (int i = 0; i < headerCount; i++)
                        {
                            headers.Add($"Column{i}");
                        }
                    }

                    dynamic lineObject = new CsvRow(headers, values, options);
                    yield return lineObject;
                    values = CsvParser.ParseLine(reader, separator).ToList();
                }
            }
        }

        /// <inheritdoc />>
        public void Dispose()
        {
            this.stream?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Transforms header name to valid C# identifier if possible.
        /// </summary>
        /// <param name="headerName">
        /// The header name.
        /// </param>
        /// <param name="matchRegex">
        /// The custom regex to match headers.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private static string TransformHeaderNameToPropertyName(string headerName, Regex matchRegex)
        {
            string result = headerName;
            if (matchRegex.IsMatch(headerName))
            {
                result = matchRegex.Match(headerName).Groups["header"].ToString();
            }

            return result.ToTitleCase().Replace(" ", string.Empty);
        }
    }
}
