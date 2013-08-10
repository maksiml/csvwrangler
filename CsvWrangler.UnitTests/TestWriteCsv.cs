// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestWriteCsv.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Test writing to CSV file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Test writing to CSV file.
    /// </summary>
    [TestClass]
    public class TestWriteCsv
    {
        /// <summary>
        /// The steps used in tests.
        /// </summary>
        private readonly WriteTestSteps steps = new WriteTestSteps();

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void write_csv_with_header()
        {
            this.steps.given_there_is_a_list_of_same_type_items();
            this.steps.when_the_list_is_persisted_to_csv();
            this.steps.expect_csv_to_have_a_header_that_contains_expected_value();
            this.steps.expect_each_line_in_csv_to_correspond_to_the_respective_item(useHeader: true);
        }
    }

    // ReSharper restore InconsistentNaming
}
