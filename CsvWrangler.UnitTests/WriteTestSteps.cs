// --------------------------------------------------------------------------------------------------------------------
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
    using System.Dynamic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;

    using ImpromptuInterface;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The steps used in CSV writer tests.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Test naming convention.")]
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

        private IEnumerable<dynamic> dynamicTestItems;

        private TestItemGenerator testItemGenerator;

        private List<IndexerTestItem> indexerTestItems;

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
            this.items = this.CreateItems().Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
        }

        public void given_there_is_a_list_of_same_type_items_with_some_null_values()
        {
            Console.WriteLine("Given there is a list of items of the same type and some values are null.");
            this.items = this.CreateItems(injectNulls: true).Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
        }

        public void given_there_is_a_list_with_read_counter()
        {
            Console.WriteLine("Given there is a list of items with read counter.");
            this.testItemGenerator = new TestItemGenerator();
            this.expectedHeaders = "Head1,Head2,Head3";
        }

        public void when_header_and_first_line_are_read()
        {
            Console.WriteLine("When header and first line are read.");

            // Length of the buffer the length of the header + new line character + length of one line.
            byte[] buffer = new byte[this.expectedHeaders.Length + 1 + TestItemGenerator.ExpectedLineLength];
            CsvWriter.ToCsv(this.testItemGenerator).Read(buffer, 0, buffer.Length);
            var value = Encoding.UTF8.GetString(buffer);
            Console.WriteLine(value);
        }

        public void expect_item_read_counter_to_be(int expectedCount)
        {
            Console.WriteLine("Expect item read counter to be {0}.", expectedCount);
            Assert.AreEqual(expectedCount, this.testItemGenerator.InvocationCount);
        }

        public void given_there_are_some_values_with_separator()
        {
            Console.WriteLine("Given there is a list of that contain separator.");
            var result = new List<dynamic>
                {
                    new { Head1 = "val11,val11", Head2 = "val12,val12", Head3 = "val13,val13" }
                };
            this.items = result.Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
            this.expectedLines = new[]
                                    {
                                        "\"val11,val11\",\"val12,val12\",\"val13,val13\""
                                    };
        }

        public void given_there_are_some_values_with_new_line_characters()
        {
            Console.WriteLine("Given there is a list of that contain new line characters.");
            var result = new List<dynamic>
                {
                    new { Head1 = "val11\nval11", Head2 = "val12\r\nval12", Head3 = "val13\rval13" }
                };
            this.items = result.Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
            this.expectedLines = new[]
                                    {
                                        "\"val11\nval11\",\"val12\r\nval12\",\"val13\rval13\""
                                    };
        }

        public void given_there_are_some_values_starting_with_new_line_characters()
        {
            Console.WriteLine("Given there are some values starting with new line.");
            var result = new List<dynamic>
                {
                    new { Head1 = "\nval11\nval11", Head2 = "val12\r\nval12", Head3 = "val13\rval13" }
                };
            this.items = result.Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
            this.expectedLines = new[]
                                    {
                                        "\"\nval11\nval11\",\"val12\r\nval12\",\"val13\rval13\""
                                    };
        }

        public void given_there_are_some_values_with_new_line_characters_and_quotes()
        {
            Console.WriteLine("Given there are some values containig new line and quotes.");
            var result = new List<dynamic>
                {
                    new { Head1 = "val11\nval\"11", Head2 = "val12\r\nval12", Head3 = "val13\rval13" }
                };
            this.items = result.Select(item => Impromptu.ActLike<ITestItemInterface>(item)).Cast<ITestItemInterface>().ToList();
            this.expectedLines = new[]
                                    {
                                        "\"val11\nval\"\"11\",\"val12\r\nval12\",\"val13\rval13\""
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

        public void given_there_is_a_list_of_dynamic_items()
        {
            Console.WriteLine("Given there is a list of dynamic items.");
            this.dynamicTestItems = this.CreateItems();
        }

        public void given_there_is_a_list_of_expando_items()
        {
            Console.WriteLine("Given there is a list of expando items.");
            var result = new List<dynamic>();
            this.expectedHeaders = "Head1,Head2,Head3";
            var headers = this.expectedHeaders.Split(',');
            this.expectedLines = new[]
                        {
                            "val11,val12,val13",
                            "val21,val22,val23"
                        };
            foreach (var line in this.expectedLines)
            {
                var expando = new ExpandoObject();
                var expandoDictionary = (IDictionary<string, object>)expando;
                var cells = line.Split(',');
                for (int i = 0; i < cells.Length; i++)
                {
                    expandoDictionary[headers[i]] = cells[i];
                }

                result.Add(expando);
            }

            this.dynamicTestItems = result;
        }

        public void given_there_is_a_list_of_dictionary_items()
        {
            Console.WriteLine("Given there is a list of dictionary items.");
            var result = new List<Dictionary<string, string>>();
            this.expectedHeaders = "Head1,Head2,Head3";
            var headers = this.expectedHeaders.Split(',');
            this.expectedLines = new[]
                        {
                            "val11,val12,val13",
                            "val21,val22,val23"
                        };
            foreach (var line in this.expectedLines)
            {
                var dictionary = new Dictionary<string, string>();
                var cells = line.Split(',');
                for (int i = 0; i < cells.Length; i++)
                {
                    dictionary[headers[i]] = cells[i];
                }

                result.Add(dictionary);
            }

            this.dynamicTestItems = result;
        }

        public void given_there_is_a_list_of_objects_with_indexer_properties()
        {
            Console.WriteLine("Given there is a list of objects with indexer properties.");
            this.indexerTestItems =
                new List<IndexerTestItem>
                    {
                        new IndexerTestItem { Head1 = "val11", Head2 = "val21" },
                        new IndexerTestItem { Head1 = "val12", Head2 = "val22" }
                    };
            this.indexerTestItems[0]["test"] = "test";
            this.expectedHeaders = "Head1,Head2";
            this.expectedLines = new[] { "val11,val21", "val12,val22" };
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
            else if (this.dynamicTestItems != null)
            {
                stream = CsvWriter.ToCsv(this.dynamicTestItems, options);
            }
            else if (this.indexerTestItems != null)
            {
                stream = CsvWriter.ToCsv(this.indexerTestItems, options);
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

        public void when_the_list_is_persisted_to_csv_file(CsvWriterOptions options = null)
        {
            Console.WriteLine("When the list is converted to CSV.");
            string csvFilePath = Path.GetTempFileName();
            if (this.items != null)
            {
                CsvWriter.ToCsvFile(this.items, csvFilePath, options);
            }
            else if (this.dateTimeItems != null)
            {
                CsvWriter.ToCsvFile(this.dateTimeItems, csvFilePath, options);
            }
            else if (this.doubleTestItems != null)
            {
                CsvWriter.ToCsvFile(this.doubleTestItems, csvFilePath, options);
            }
            else if (this.dynamicTestItems != null)
            {
                CsvWriter.ToCsvFile(this.dynamicTestItems, csvFilePath, options);
            }
            else if (this.indexerTestItems != null)
            {
                CsvWriter.ToCsv(this.indexerTestItems, options);
            }
            else
            {
                throw new InvalidOperationException("No items to serialized are specified.");
            }

            this.csv = File.ReadAllText(csvFilePath);
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
            using (TextReader reader = new StreamReader(this.csv.ToStream()))
            {
                var actualLine = CsvParser.ParseLine(reader, ',').ToList();
                if (useHeader)
                {
                    actualLine = CsvParser.ParseLine(reader, ',').ToList();
                }

                foreach (string currentLine in this.expectedLines)
                {
                    Assert.IsTrue(actualLine.Any());
                    var expectedLine = CsvParser.ParseLine(new StreamReader(currentLine.ToStream()), ',').ToList();
                    Assert.AreEqual(expectedLine.Count, actualLine.Count);
                    for (int j = 0; j < expectedLine.Count; j++)
                    {
                        Assert.AreEqual(expectedLine[j], actualLine[j]);
                    }

                    actualLine = CsvParser.ParseLine(reader, ',').ToList();
                }

                Assert.IsFalse(actualLine.Any());
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
            var line = this.GetFirstRow(useHeader).Replace("\"", string.Empty);
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

        private List<dynamic> CreateItems(bool injectNulls = false)
        {
            var result = new List<dynamic>
                {
                    new { Head1 = "val11", Head2 = "val12", Head3 = "val13" },
                    new { Head1 = "val21", Head2 = injectNulls ? null : "val22", Head3 = "val23" }
                };
            this.expectedHeaders = "Head1,Head2,Head3";
            this.expectedLines = new[]
                                    {
                                        "val11,val12,val13",
                                        injectNulls ? "val21,,val23" : "val21,val22,val23"
                                    };
            return result;
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

        private class IndexerTestItem
        {
            private readonly Dictionary<string, string> dictionary = new Dictionary<string, string>();

            public string Head1 { get; set; }

            public string Head2 { get; set; }

            public string this[string name]
            {
                get => this.dictionary[name];
                set => this.dictionary[name] = value;
            }
        }
    }

    // ReSharper restore InconsistentNaming
}