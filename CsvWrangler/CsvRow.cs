// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvRow.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Represents row retrieved from CSV file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// Represents row retrieved from CSV file.
    /// </summary>
    internal class CsvRow : DynamicObject, IEnumerable<string>
    {
        /// <summary>
        /// The map of header names to values.
        /// </summary>
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRow"/> class.
        /// </summary>
        /// <param name="headers">
        /// The headers.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        public CsvRow(List<string> headers, List<string> values)
        {
            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                this.values.Add(header, values[i]);
            }
        }

        /// <summary>
        /// Try to get cell value given header.
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The cell value that corresponds to the header name provided in the <paramref name="binder"/>.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            string header = binder.Name;
            bool memeberExists = this.values.ContainsKey(header);
            if (memeberExists)
            {
                result = this.values[header];
            }
            else
            {
                result = null;
            }

            return memeberExists;
        }

        /// <summary>
        /// Get list of 'properties' that the object supports, list consists of header names.
        /// </summary>
        /// <returns>
        /// List of header names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.values.Keys;
        }

        /// <summary>
        /// Returns an enumerator that iterates through a values.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through a values.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.values.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a values.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through a values.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.Values.GetEnumerator();
        }
    }
}
