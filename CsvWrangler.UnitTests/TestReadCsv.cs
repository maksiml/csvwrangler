﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestReadCsv.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Test reading CSV files.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable RedundantArgumentNameForLiteralExpression
namespace CsvWrangler.UnitTests
{
    using System.Diagnostics.CodeAnalysis;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Test reading CSV files.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    public class TestReadCsv
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
        public void read_properly_formatted_csv_file()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_correct_count_of_items(useHeader: true);
            this.steps.expect_item_properties_to_correspond_to_headers();
            this.steps.expect_property_value_to_be_the_same_as_in_corresponding_cell();
        }

        [TestMethod]
        public void enumeration_of_item_yields_cell_values()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_enumeration_of_item_to_yield_cells(hasHeader: true);
        }

        [TestMethod]
        public void item_can_be_explicitly_cast_to_dictionary()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_that_each_item_can_be_cast_to_dictionary();
        }

        [TestMethod]
        public void item_can_be_cast_to_collection()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header();
            this.steps.when_csv_is_parsed();
            this.steps.expect_that_each_item_behaves_as_collection();
        }

        [TestMethod]
        public void space_removed_from_header()
        {
            this.steps.given_there_is_a_csv_with_header_with_space();
            this.steps.when_csv_is_parsed();
            this.steps.expect_item_properties_to_correspond_to_headers();
        }

        [TestMethod]
        public void surrounding_quotes_are_removed_from_header()
        {
            this.steps.given_there_is_a_csv_with_header_with_quotes();
            this.steps.when_csv_is_parsed();
            this.steps.expect_item_properties_to_correspond_to_headers();
        }

        [TestMethod]
        public void replace_headers_not_macthing_rules_with_generic_name()
        {
            this.steps.given_there_is_a_csv_with_header_not_matching_identifier_rules();
            this.steps.when_csv_is_parsed();
            this.steps.expect_item_properties_to_correspond_to_headers();
        }

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

        [TestMethod]
        public void empty_file_produces_empty_list()
        {
            this.steps.given_there_is_empty_csv();
            this.steps.when_csv_is_parsed();
            this.steps.expect_zero_items_in_the_result();
        }

        [TestMethod]
        public void rows_with_mismatched_number_of_cells_filled_with_empty_strings()
        {
            this.steps.given_there_is_a_csv_with_header_and_mismatched_row();
            this.steps.when_csv_is_parsed();
            this.steps.expect_enumeration_of_item_to_yield_cells(hasHeader: true);
        }

        [TestMethod]
        public void rows_with_mismatched_number_of_cells_fail_read_with_strict_option()
        {
            this.steps.given_there_is_a_csv_with_header_and_mismatched_row();
            this.steps.when_csv_is_parsed(new CsvReaderOptions { StrictCellCount = true });
            this.steps.expect_invalid_cell_count_exception();
        }

        [TestMethod]
        public void data_is_read_line_by_line()
        {
            this.steps.given_there_is_a_csv_with_header_and_mismatched_row();

            // The data here is formed in such way that exception would happen only when second line is read.
            this.steps.when_first_line_is_read();
            this.steps.expect_there_are_no_exceptions();
        }

        [TestMethod]
        public void read_data_with_non_default_separator()
        {
            this.steps.given_there_is_properly_formatted_csv_with_header_and_tabs_as_separator();
            this.steps.when_csv_is_parsed(new CsvReaderOptions { Separator = '\t' });
            this.steps.expect_correct_count_of_items(useHeader: true);
            this.steps.expect_item_properties_to_correspond_to_headers();
            this.steps.expect_property_value_to_be_the_same_as_in_corresponding_cell();
        }
    }

    // ReSharper restore InconsistentNaming
}
