// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvInvalidCellCountException.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Indicates mismatching number of values in the row
//   compared to number of headers.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System;

    /// <summary>
    /// Indicates mismatching number of values in the row
    /// compared to number of headers.
    /// </summary>
    public class CsvInvalidCellCountException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvInvalidCellCountException"/> class.
        /// </summary>
        /// <param name="headerCount">
        /// The header count.
        /// </param>
        /// <param name="valueCount">
        /// The value count.
        /// </param>
        public CsvInvalidCellCountException(int headerCount, int valueCount)
            : base(string.Format("Expected count of cells in the row is {0}, actual number of values is {1}", headerCount, valueCount))
        {
        }
    }
}