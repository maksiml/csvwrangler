// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITestItemInterface.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The interface for the item that is expected from CSV.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// The interface for the item that is expected from CSV.
    /// </summary>
    public interface ITestItemInterface
    {
        /// <summary>
        /// Gets or sets value for header 1.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Test class.")]
        string Head1 { get; set; }

        /// <summary>
        /// Gets or sets value for header 2.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Test class.")]
        string Head2 { get; set; }

        /// <summary>
        /// Gets or sets value for header 3.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Test class.")]
        string Head3 { get; set; }
    }
}