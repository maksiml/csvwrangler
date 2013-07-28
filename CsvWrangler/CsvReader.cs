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
    using System.Dynamic;
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
        /// <returns>
        /// The list of dynamic objects produced from CSV lines.
        /// </returns>
        public static IEnumerable<dynamic> Parse(Stream input)
        {
            using (TextReader reader = new StreamReader(input))
            {
                string line = reader.ReadLine();
                if (line == null)
                {
                    return new dynamic[0];
                }

                List<string> headers = line
                                        .Split(',')
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

                var result = new List<dynamic>();
                while (line != null)
                {
                    var lineObject = new ExpandoObject() as IDictionary<string, object>;
                    string[] values = line.Split(',');
                    for (int index = 0; index < headers.Count; index++)
                    {
                        var header = headers[index];
                        lineObject.Add(header, values[index]);
                    }

                    result.Add(lineObject);

                    line = reader.ReadLine();
                }

                return result;
            }
        }
    }
}
