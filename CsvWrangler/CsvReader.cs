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
        /// Parse CSV file to list of dynamic objects.
        /// </summary>
        /// <param name="input">
        /// The input CSV stream.
        /// </param>
        /// <param name="hasHeader">
        /// The has header.
        /// </param>
        /// <returns>
        /// The list of dynamic objects produced from CSV lines.
        /// </returns>
        public static IEnumerable<dynamic> Parse(Stream input, bool hasHeader = true)
        {
            using (TextReader reader = new StreamReader(input))
            {
                const char Separator = ',';

                List<string> headers;
                string line = reader.ReadLine();
                if (line == null)
                {
                    return new dynamic[0];
                }

                if (hasHeader)
                {
                    headers = line
                                .Split(Separator)
                                .Select(header => header.ToTitleCase().Replace(" ", string.Empty))
                                .ToList();
                    for (int i = 0; i < headers.Count; i++)
                    {
                        if (!AllowedHeaderNames.IsMatch(headers[i]))
                        {
                            headers[i] = string.Format("Column{0}", i);
                        }
                    }

                    line = reader.ReadLine();
                }
                else
                {
                    headers = new List<string>();
                    int headerCount = line.Split(Separator).Length;
                    for (int i = 0; i < headerCount; i++)
                    {
                        headers.Add(string.Format("Column{0}", i));
                    }
                }

                var result = new List<dynamic>();
                while (line != null)
                {
                    string[] values = line.Split(Separator);
                    dynamic lineObject = new CsvRow(headers, values.ToList());
                    result.Add(lineObject);
                    line = reader.ReadLine();
                }

                return result;
            }
        }
    }
}
