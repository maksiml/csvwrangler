// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestCsvRow.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Tests that CSV Row behaves as expected of the collection and dictionary.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Tests that CSV Row behaves as expected of the collection and dictionary.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class TestCsvRow
    {
        /// <summary>
        /// The steps.
        /// </summary>
        private readonly ReadTestsSteps steps = new ReadTestsSteps();

        /// <summary>
        /// Initialize test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.steps.Reset();
        }

        [TestMethod]
        public void new_column_can_be_added_to_read_csv()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.when_new_column_is_added_to_read_result();
            this.steps.expect_values_set_for_new_column_to_be_retained();
        }

        [TestMethod]
        public void column_can_be_removed_from_read_csv()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.when_column_is_removed_from_read_result();
            this.steps.expect_none_of_the_items_to_have_removed_column();
        }

        [TestMethod]
        public void cell_values_can_be_updated_in_read_data()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_cell_data_can_be_updated();
        }

        [TestMethod]
        public void cells_values_can_be_enumerated()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_cell_values_can_be_enumerated();
        }
    }

    // ReSharper restore InconsistentNaming
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}
