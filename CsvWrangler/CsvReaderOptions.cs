// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvReaderOptions.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV reader options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    /// <summary>
    /// The CSV reader options.
    /// </summary>
    public class CsvReaderOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether cell count in 
        /// each row should match number of headers or number of cells
        /// in first row of CSV. When the value is <b>false</b> missing
        /// cells will be filed with empty strings.
        /// </summary>
        public bool StrictCellCount { get; set; }
    }
}
