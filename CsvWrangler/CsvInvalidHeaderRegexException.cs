// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvInvalidHeaderRegexException.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Indicates that provided header parsing regex is not valid.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System;

    /// <summary>
    /// Indicates that provided header parsing regex is not valid.
    /// </summary>
    public class CsvInvalidHeaderRegexException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvInvalidHeaderRegexException"/> class.
        /// </summary>
        /// <param name="error">
        /// The error.
        /// </param>
        public CsvInvalidHeaderRegexException(string error)
            : base($"Provided header matching regex is not acceptable: '{error}'")
        {
        }
    }
}