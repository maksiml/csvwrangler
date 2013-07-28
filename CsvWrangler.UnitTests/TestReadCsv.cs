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
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;

    using CsvWrangler;

    using ImpromptuInterface;

    using NUnit.Framework;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Test reading CSV files.
    /// </summary>
    [TestFixture]
    public class TestReadCsv
    {
        /// <summary>
        /// The interface for the item that is expected from CSV.
        /// </summary>
        public interface ITestItemInterface
        {
            /// <summary>
            /// Gets or sets value for header 1.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Test class.")]
            string Head1 { get; set; }

            /// <summary>
            /// Gets or sets value for header 2.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Test class.")]
            string Head2 { get; set; }

            /// <summary>
            /// Gets or sets value for header 3.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Test class.")]
            string Head3 { get; set; }
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        [TestCase]
        public void read_properly_formatted_csv_file()
        {
            string csvContent;
            List<string> expectedHeaders = null;

            var testData = given_there_is_properly_formatted_csv_with_header(out csvContent, out expectedHeaders);
            var result   = when_csv_is_parsed(csvContent);
                           expect_correct_count_of_items(testData, result, useHeader: true);
                           expect_item_properties_to_correspond_to_headers(result, expectedHeaders);
                           expect_property_value_to_be_the_same_as_in_corresponding_cell(testData, result);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        [TestCase]
        public void space_removed_from_header()
        {
            string csvContent;
            List<string> expectedHeaders = null;

                           given_there_is_a_csv_with_header_with_space(out csvContent, out expectedHeaders);
            var result   = when_csv_is_parsed(csvContent);
                           expect_item_properties_to_correspond_to_headers(result, expectedHeaders);
        }

        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        [TestCase]
        public void replace_headers_not_macthing_rules_with_generic_name()
        {
            string csvContent;
            List<string> expectedHeaders = null;

                         given_there_is_a_csv_with_header_not_matching_identifier_rules(out csvContent, out expectedHeaders);
            var result = when_csv_is_parsed(csvContent);
                         expect_item_properties_to_correspond_to_headers(result, expectedHeaders);
        }

        /// <summary>
        /// Create properly formatted CSV with header.
        /// </summary>
        /// <param name="csvContent">
        /// The output parameter that contains CSV content.
        /// </param>
        /// <param name="expectedHeaders">
        /// The expected headers.
        /// </param>
        /// <returns>
        /// The matrix that contains source data.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static List<List<string>> given_there_is_properly_formatted_csv_with_header(
            out string csvContent, 
            out List<string> expectedHeaders)
        {
            Console.WriteLine("Given there is a properly formatted CSV file with header.");
            var testData = new List<List<string>>
                                {
                                    new List<string> { "head1", "head2", "head3" },
                                    new List<string> { "val11", "val12", "val13" },
                                    new List<string> { "val21", "val22", "val23" },
                                };
            expectedHeaders = new List<string>
                                  {
                                      "Head1", "Head2", "Head3"
                                  };
            csvContent = string.Join("\n", testData.Select(row => string.Join(",", row)));
            return testData;
        }

        /// <summary>
        /// Given there is a properly formatted CSV file with headers that have spaces.
        /// </summary>
        /// <param name="csvContent">
        /// The CSV content.
        /// </param>
        /// <param name="expectedHeaders">
        /// The expected headers.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static List<List<string>> given_there_is_a_csv_with_header_with_space(
            out string csvContent, 
            out List<string> expectedHeaders)
        {
            Console.WriteLine("Given there is a properly formatted CSV file with headers that have spaces.");
            var testData = new List<List<string>>
                                {
                                    new List<string> { "head1 head", "head2", "head3" },
                                    new List<string> { "val11", "val12", "val13" },
                                };
            expectedHeaders = new List<string>
                                  {
                                      "Head1Head", "Head2", "Head3"
                                  };
            csvContent = string.Join("\n", testData.Select(row => string.Join(",", row)));
            return testData;
        }

        /// <summary>
        /// Given there is a properly formatted CSV file with headers that contain C# keywords.
        /// </summary>
        /// <param name="csvContent">
        /// The CSV content.
        /// </param>
        /// <param name="expectedHeaders">
        /// The expected headers.
        /// </param>
        /// <returns>
        /// The <see cref="List"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static List<List<string>> given_there_is_a_csv_with_header_not_matching_identifier_rules(
            out string csvContent,
            out List<string> expectedHeaders)
        {
            Console.WriteLine("Given there is a properly formatted CSV file with headers that have spaces.");
            var testData = new List<List<string>>
                                {
                                    new List<string> { "1header", "head3", "два" },
                                    new List<string> { "val11", "val12", "val13" },
                                };
            expectedHeaders = new List<string>
                                  {
                                      "Column0", "Head3", "Column2"
                                  };
            csvContent = string.Join("\n", testData.Select(row => string.Join(",", row)));
            return testData;
        }

        /// <summary>
        /// Parse CSV.
        /// </summary>
        /// <param name="csvContent">
        /// The CSV content.
        /// </param>
        /// <returns>
        /// List of object retrieved from the CSV.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static List<dynamic> when_csv_is_parsed(string csvContent)
        {
            Console.WriteLine("When the CSV is parsed,");
            List<dynamic> result;
            using (var stream = csvContent.ToStream())
            {
                result = CsvReader.Parse(stream).ToList();
            }

            return result;
        }

        /// <summary>
        /// Expect property values to be the same as in corresponding cells.
        /// </summary>
        /// <param name="testData">
        /// The test data.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static void expect_property_value_to_be_the_same_as_in_corresponding_cell(List<List<string>> testData, List<dynamic> result)
        {
            List<string> headerRow = testData[0];
            Console.WriteLine("Expect property values to be the same as in corresponding cells.");
            for (int i = 0; i < result.Count; i++)
            {
                ITestItemInterface item = Impromptu.ActLike<ITestItemInterface>(result[i]);
                for (int j = 0; j < headerRow.Count; j++)
                {
                    string expected = testData[i + 1][j];
                    string actual = item.GetType().GetProperty(headerRow[j].ToTitleCase()).GetValue(item).ToString();
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        /// Expect each item to have a property per header with corresponding name.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="expectedHeaders">
        /// Expected headers.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static void expect_item_properties_to_correspond_to_headers(List<dynamic> result, List<string> expectedHeaders)
        {
            Console.WriteLine("Expect each item to have a property per header with corresponding name.");
            foreach (dynamic item in result)
            {
                var metaData = (IDynamicMetaObjectProvider)item;
                DynamicMetaObject metaObject = metaData.GetMetaObject(Expression.Constant(item));
                IEnumerable<string> memberNames = metaObject.GetDynamicMemberNames();

                List<string> properties = memberNames.ToList();

                Assert.AreEqual(expectedHeaders.Count, properties.Count);

                for (int j = 0; j < expectedHeaders.Count; j++)
                {
                    Assert.AreEqual(expectedHeaders[j], properties[j]);
                }
            }
        }

        /// <summary>
        /// Check that there is a correct number of items created.
        /// </summary>
        /// <param name="testData">
        /// The test data.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <param name="useHeader">
        /// Indicates if the CSV has header or not.
        /// </param>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        private static void expect_correct_count_of_items(
            List<List<string>> testData, 
            List<dynamic> result, 
            bool useHeader)
        {
            Console.WriteLine("Expect resulting enumeration to have rows minus one items,");
            Assert.AreEqual(useHeader ? testData.Count - 1 : testData.Count, result.Count);
        }
    }
    // ReSharper restore InconsistentNaming
}
