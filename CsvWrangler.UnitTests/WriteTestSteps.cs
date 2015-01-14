﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WriteTestSteps.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The steps used in CSV writer tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;
    using System.Linq;

    using ImpromptuInterface;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The steps used in CSV writer tests.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    internal class WriteTestSteps
    {
        /// <summary>
        /// The expected double value.
        /// </summary>
        private const double ExpectedDoubleValue = 3.4;

        /// <summary>
        /// The expected date time value.
        /// </summary>
        private static DateTime expectedDateTime = DateTime.Now;

        /// <summary>
        /// List of items that will be converted to CSV.
        /// </summary>
        private IEnumerable<ITestItemInterface> items;

        /// <summary>
        /// The date time items.
        /// </summary>
        private IEnumerable<DateTimeTestItem> dateTimeItems;

        /// <summary>
        /// The double test items.
        /// </summary>
        private IEnumerable<DoubleTestItem> doubleTestItems; 

        /// <summary>
        /// The resulting CSV.
        /// </summary>
        private string csv;

        /// <summary>
        /// The expected headers in the CSV.
        /// </summary>
        private string expectedHeaders;

        /// <summary>
        /// The expected lines in the CSV.
        /// </summary>
        private string[] expectedLines;

        /// <summary>
        /// Given there is a list of items of the same type.
        /// </summary>
        public void given_there_is_a_list_of_same_type_items()
        {
            Console.WriteLine("Given there is a list of items of the same type.");
            var result = new List<dynamic>
                {
                    new { Head1 = "val11", Head2 = "val12", Head3 = "val13" },
                    new { Head1 = "val21", Head2 = "val22", Head3 = "val23" }
                };
            this.items = result.Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
            this.expectedHeaders = "Head1,Head2,Head3";
            this.expectedLines = new[]
                                    {
                                        "val11,val12,val13",
                                        "val21,val22,val23"
                                    };
        }

        /// <summary>
        /// Given there is a list of items of type that has DateTime field.
        /// </summary>
        public void given_there_is_a_list_of_items_of_type_that_has_date_property()
        {
            Console.WriteLine("Given there is a list of items of type that has DateTime field");
            this.dateTimeItems = new List<DateTimeTestItem> { new DateTimeTestItem { DateTime = expectedDateTime } };
        }

        /// <summary>
        /// Given there is a list of items of type that has 'double' property.
        /// </summary>
        public void given_there_is_a_list_of_items_of_type_that_has_double_property()
        {
            Console.WriteLine("Given there is a list of items of type that has 'double' property");
            this.doubleTestItems = new List<DoubleTestItem> { new DoubleTestItem { Double = ExpectedDoubleValue } };
        }

        /// <summary>
        /// When the list is converted to CSV.
        /// </summary>
        /// <param name="options">
        /// The options for serializations.
        /// </param>
        public void when_the_list_is_persisted_to_csv(CsvWriterOptions options = null)
        {
            Console.WriteLine("When the list is converted to CSV.");
            Stream stream;
            if (this.items != null)
            {
                stream = CsvWriter.ToCsv(this.items, options);
            } 
            else if (this.dateTimeItems != null)
            {
                stream = CsvWriter.ToCsv(this.dateTimeItems, options);
            }
            else if (this.doubleTestItems != null)
            {
                stream = CsvWriter.ToCsv(this.doubleTestItems, options);
            }
            else
            {
                throw new InvalidOperationException("No items to serialized are specified.");
            }

            using (var reader = new StreamReader(stream))
            {
                this.csv = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Expect the CSV to have a header that contains expected header values.
        /// </summary>
        public void expect_csv_to_have_a_header_that_contains_expected_value()
        {
            Console.WriteLine("Expect the CSV to have a header that contains expected header values.");
            string[] lines = this.csv.Split('\n');
            Assert.IsTrue(lines.Length > 0);
            Assert.AreEqual(this.expectedHeaders, lines[0]);
        }

        /// <summary>
        /// Expect that each item is converted to the corresponding line in the CSV.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV is expected to have a header.
        /// </param>
        public void expect_each_line_in_csv_to_correspond_to_the_respective_item(bool useHeader)
        {
            Console.WriteLine("Expect that each item is converted to the corresponding line in the CSV.");
            string[] lines = this.csv.Split('\n');
            Assert.IsTrue((useHeader ? lines.Length - 1 : lines.Length) == this.expectedLines.Length);
            for (int i = 0; i < this.expectedLines.Length; i++)
            {
                Assert.AreEqual(this.expectedLines[i], lines[useHeader ? i + 1 : i]);
            }
        }

        /// <summary>
        /// Expect the date field to be formatted using invariant culture.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV is expected to have a header.
        /// </param>
        public void expect_date_fields_to_be_persisted_using_invariant_culture(bool useHeader)
        {
            Console.WriteLine("Expect the date field to be formatted using invariant culture");
            this.expect_date_field_be_persited_using_correct_format(useHeader);
        }

        /// <summary>
        /// Expect the date field to be formatted using provided format.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV is expected to have a header.
        /// </param>
        /// <param name="dateTimeFormat">
        /// The date time format.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        public void expect_date_field_be_persited_using_provided_format(bool useHeader, string dateTimeFormat = null, CultureInfo cultureInfo = null)
        {
            Console.WriteLine("Expect the date field to be formatted using provided format");
            this.expect_date_field_be_persited_using_correct_format(useHeader, dateTimeFormat, cultureInfo);
        }

        /// <summary>
        /// Expect the double field to be serialized with provided format.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV is expected to have a header.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        public void expect_double_to_be_persisted_using_provided_format(bool useHeader, CultureInfo cultureInfo = null)
        {
            Console.WriteLine("Expect the double field to be serialized with provided format");
            var line = this.GetFirstRow(useHeader);
            string expectedValue = cultureInfo != null
                                       ? ExpectedDoubleValue.ToString(cultureInfo)
                                       : ExpectedDoubleValue.ToString(CultureInfo.InvariantCulture);
            Assert.AreEqual(expectedValue, line);
        }

        /// <summary>
        /// Verify that dates are persisted using specified format.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV is expected to have a header.
        /// </param>
        /// <param name="dateTimeFormat">
        /// The date time format.
        /// </param>
        /// <param name="cultureInfo">
        /// The culture info.
        /// </param>
        private void expect_date_field_be_persited_using_correct_format(bool useHeader, string dateTimeFormat = null, CultureInfo cultureInfo = null)
        {
            var line = this.GetFirstRow(useHeader);
            string expectedValue;
            if (dateTimeFormat != null)
            {
                expectedValue = expectedDateTime.ToString(dateTimeFormat);
            }
            else if (cultureInfo != null)
            {
                expectedValue = expectedDateTime.ToString(cultureInfo);
            }
            else
            {
                expectedValue = expectedDateTime.ToString(CultureInfo.InvariantCulture);
            }

            Assert.AreEqual(expectedValue, line);
        }

        /// <summary>
        /// Get first row from serialized CSV.
        /// </summary>
        /// <param name="useHeader">
        /// The use header.
        /// </param>
        /// <returns>
        /// The first actual row.
        /// </returns>
        private string GetFirstRow(bool useHeader)
        {
            string[] lines = this.csv.Split('\n');
            int actualLineCount = useHeader ? lines.Length - 1 : lines.Length;
            Assert.IsTrue(actualLineCount > 0);
            return lines[useHeader ? 1 : 0];
        }

        /// <summary>
        /// A class for the test with <seealso cref="DateTime"/> fields.
        /// </summary>
        private class DateTimeTestItem
        {
            /// <summary>
            /// Gets or sets the date time field.
            /// </summary>
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public DateTime DateTime { get; set; }
        }

        /// <summary>
        /// A class for the test with <seealso cref="double"/> fields.
        /// </summary>
        private class DoubleTestItem
        {
            /// <summary>
            /// Gets or sets the double property.
            /// </summary>
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public double Double { get; set; }
        }
    }
    
    // ReSharper restore InconsistentNaming
}