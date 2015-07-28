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
        /// Initializes a new instance of the <see cref="CsvReaderOptions"/> class.
        /// </summary>
        public CsvReaderOptions()
        {
            this.Separator = ',';
        }

        /// <summary>
        /// The resolve header name handler delegate.
        /// </summary>
        /// <param name="csvHeaderName">
        /// The CSV header name.
        /// </param>
        /// <param name="suggestedName">
        /// The name suggested by CSV reader or <b>null</b> if name cannot be devised.
        /// </param>
        /// <returns>
        /// Should return a name for the header or <b>null</b> if CSV should generate the name.
        /// </returns>
        public delegate string ResolveHeaderNameHandler(string csvHeaderName, string suggestedName);

        /// <summary>
        /// Gets or sets a value indicating whether cell count in 
        /// each row should match number of headers or number of cells
        /// in first row of CSV. When the value is <b>false</b> missing
        /// cells will be filed with empty strings.
        /// </summary>
        public bool StrictCellCount { get; set; }

        /// <summary>
        /// Gets or sets a separator character that will be used when parsing
        /// the CSV file.
        /// </summary>
        public char Separator { get; set; }

        /// <summary>
        /// Gets or sets the handler to be called when CSV header is being resolved.
        /// </summary>
        public ResolveHeaderNameHandler ResolveHeaderName { get; set; }
    }
}
