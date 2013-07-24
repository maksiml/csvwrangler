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

    /// <summary>
    /// The CSV reader.
    /// </summary>
    public class CsvReader
    {
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

                string[] headers = line.Split(',');

                line = reader.ReadLine();

                var result = new List<dynamic>();
                while (line != null)
                {
                    var lineObject = new ExpandoObject() as IDictionary<string, object>;
                    string[] values = line.Split(',');
                    for (int index = 0; index < headers.Length; index++)
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
