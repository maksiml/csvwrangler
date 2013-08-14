// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvWriterOptions.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV writer options.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System.Globalization;

    /// <summary>
    /// The CSV writer options.
    /// </summary>
    public class CsvWriterOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvWriterOptions"/> class.
        /// </summary>
        public CsvWriterOptions()
        {
            this.CultureInfo = CultureInfo.InvariantCulture;
        }

        /// <summary>
        /// Gets or sets the date time format that will be used to persist dates.
        /// </summary>
        public string DateTimeFormat { get; set; }

        /// <summary>
        /// Gets or sets the culture info that will be used to serialize types that are capable of using culture info.
        /// </summary>
        public CultureInfo CultureInfo { get; set; }
    }
}
