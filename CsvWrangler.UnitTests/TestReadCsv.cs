// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestReadCsv.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Test reading CSV files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Test reading CSV files.
    /// </summary>
    [TestClass]
    public class TestReadCsv
    {
        /// <summary>
        /// The steps.
        /// </summary>
        private readonly CsvWranglerTestsSteps steps = new CsvWranglerTestsSteps();

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

        /// <summary>
        /// Initialize test.
        /// </summary>
        [TestInitialize]
        public void InitializeTest()
        {
            this.steps.Reset();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void read_properly_formatted_csv_file()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_correct_count_of_items(useHeader: true);
            this.steps.expect_item_properties_to_correspond_to_headers();
            this.steps.expect_property_value_to_be_the_same_as_in_corresponding_cell();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void enumeration_of_item_yields_cell_values()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_enumeration_of_item_to_yield_cells(hasHeader: true);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void item_can_be_explicitly_cast_to_dictionary()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_that_each_item_can_be_cast_to_dictionary();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void item_can_be_cast_to_collection()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_that_each_item_behaves_as_collection();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void space_removed_from_header()
        {
            this.steps.given_there_is_a_csv_with_header_with_space();
            this.steps.when_csv_is_parsed();
            this.steps.expect_item_properties_to_correspond_to_headers();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void replace_headers_not_macthing_rules_with_generic_name()
        {
            this.steps.given_there_is_a_csv_with_header_not_matching_identifier_rules();
            this.steps.when_csv_is_parsed();
            this.steps.expect_item_properties_to_correspond_to_headers();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void rows_of_csv_without_headers_are_converted_to_lists()
        {
            this.steps.given_there_is_a_csv_without_header();
            this.steps.when_csv_without_header_is_parsed();
            this.steps.expect_each_row_to_be_a_list_of_string_values();
            this.steps.expect_correct_count_of_items(useHeader: false);
            this.steps.expect_item_properties_to_correspond_to_headers();
            this.steps.expect_enumeration_of_item_to_yield_cells(hasHeader: false);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void empty_file_produces_empty_list()
        {
            this.steps.given_there_is_empty_csv();
            this.steps.when_csv_is_parsed();
            this.steps.expect_zero_items_in_the_result();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void new_column_can_be_added_to_read_csv()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.when_new_column_is_added_to_read_result();
            this.steps.expect_values_set_for_new_column_to_be_retained();
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented",
            Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter",
            Justification = "Unit test naming convention.")]
        [TestMethod]
        public void column_can_be_removed_from_read_csv()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.when_column_is_removed_from_read_result();
            this.steps.expect_none_of_the_items_to_have_removed_column();
        }
    }

    // ReSharper restore InconsistentNaming
}
