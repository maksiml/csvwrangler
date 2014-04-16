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
    using System.Globalization;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Test writing to CSV file.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    public class TestWriteCsv
    {
        /// <summary>
        /// The steps used in tests.
        /// </summary>
        private readonly WriteTestSteps steps = new WriteTestSteps();

        [TestMethod]
        public void write_csv_with_header()
        {
            this.steps.given_there_is_a_list_of_same_type_items();
            this.steps.when_the_list_is_persisted_to_csv();
            this.steps.expect_csv_to_have_a_header_that_contains_expected_value();
            this.steps.expect_each_line_in_csv_to_correspond_to_the_respective_item(useHeader: true);
        }

        [TestMethod]
        public void dates_in_csv_default_to_invariant_culture()
        {
            this.steps.given_there_is_a_list_of_items_of_type_that_has_date_property();
            this.steps.when_the_list_is_persisted_to_csv();
            this.steps.expect_date_fields_to_be_persisted_using_invariant_culture(useHeader: true);
        }

        [TestMethod]
        public void dates_should_be_serialized_with_specified_date_format()
        {
            const string DateTimeFormat = "u";
            this.steps.given_there_is_a_list_of_items_of_type_that_has_date_property();
            this.steps.when_the_list_is_persisted_to_csv(new CsvWriterOptions { DateTimeFormat = DateTimeFormat });
            this.steps.expect_date_field_be_persited_using_provided_format(useHeader: true, dateTimeFormat: DateTimeFormat);
        }

        [TestMethod]
        public void culture_info_should_be_used_for_date_time_serialization_when_provided()
        {
            var cultureInfo = CultureInfo.GetCultureInfo("lt-LT");
            this.steps.given_there_is_a_list_of_items_of_type_that_has_date_property();
            this.steps.when_the_list_is_persisted_to_csv(new CsvWriterOptions { CultureInfo = cultureInfo });
            this.steps.expect_date_field_be_persited_using_provided_format(useHeader: true, cultureInfo: cultureInfo);
        }

        [TestMethod]
        public void date_time_string_format_takes_precedence_over_culture_info_in_options()
        {
            const string DateTimeFormat = "u";
            this.steps.given_there_is_a_list_of_items_of_type_that_has_date_property();
            this.steps.when_the_list_is_persisted_to_csv(new CsvWriterOptions
                                                             {
                                                                 DateTimeFormat = DateTimeFormat, 
                                                                 CultureInfo = CultureInfo.GetCultureInfo("lt-LT")
                                                             });
            this.steps.expect_date_field_be_persited_using_provided_format(useHeader: true, dateTimeFormat: DateTimeFormat);
        }

        [TestMethod]
        public void culture_info_is_taken_in_to_account_when_serializing_numbers()
        {
            this.steps.given_there_is_a_list_of_items_of_type_that_has_double_property();
            this.steps.when_the_list_is_persisted_to_csv(new CsvWriterOptions
                {
                    CultureInfo = CultureInfo.GetCultureInfo("lt-LT")
                });
            this.steps.expect_double_to_be_persisted_using_provided_format(useHeader: true, cultureInfo: CultureInfo.GetCultureInfo("lt-LT"));
        }
    }

    // ReSharper restore InconsistentNaming
}
