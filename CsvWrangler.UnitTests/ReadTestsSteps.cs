// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ReadTestsSteps.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Step definitions for CSV Wrangler tests.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Dynamic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    using ImpromptuInterface;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// Step definitions for CSV Wrangler tests.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Descriptive names.")]
    internal class ReadTestsSteps
    {
        /// <summary>
        /// The new column default value.
        /// </summary>
        private const string NewColumnDefaultValue = "Default Value";

        /// <summary>
        /// The new column name.
        /// </summary>
        private const string NewColumnName = "NewColumn";

        /// <summary>
        /// Gets or sets the test data.
        /// </summary>
        public List<List<string>> TestData { get; set; }

        /// <summary>
        /// Gets or sets the CSV content.
        /// </summary>
        public string CsvContent { get; set; }

        /// <summary>
        /// Gets or sets the expected headers.
        /// </summary>
        public List<string> ExpectedHeaders { get; set; }

        /// <summary>
        /// Gets or sets the read result.
        /// </summary>
        public List<dynamic> ReadResult { get; set; }

        public Exception Exception { get; set; }

        /// <summary>
        /// Reset steps.
        /// </summary>
        public void Reset()
        {
            this.TestData = null;
            this.CsvContent = null;
            this.ExpectedHeaders = null;
            this.ReadResult = null;
            this.Exception = null;
        }

        /// <summary>
        /// Create properly formatted CSV with header.
        /// </summary>
        public void given_there_is_properly_formatted_csv_with_header()
        {
            Console.WriteLine("Given there is a properly formatted CSV file with header.");
            this.TestData = new List<List<string>>
                                {
                                    new List<string> { "head1", "head2", "head3" },
                                    new List<string> { "val11", "val12", "val13" },
                                    new List<string> { "val21", "val22", "val23" },
                                };
            this.ExpectedHeaders = new List<string>
                                  {
                                      "Head1", "Head2", "Head3"
                                  };
            this.CsvContent = string.Join("\n", this.TestData.Select(row => string.Join(",", row)));
        }

        /// <summary>
        /// Given there is a properly formatted CSV file with headers that have spaces.
        /// </summary>
        public void given_there_is_a_csv_with_header_with_space()
        {
            Console.WriteLine("Given there is a properly formatted CSV file with headers that have spaces.");
            this.TestData = new List<List<string>>
                                {
                                    new List<string> { "head1 head", "head2", "head3" },
                                    new List<string> { "val11", "val12", "val13" },
                                };
            this.ExpectedHeaders = new List<string>
                                  {
                                      "Head1Head", "Head2", "Head3"
                                  };
            this.CsvContent = string.Join("\n", this.TestData.Select(row => string.Join(",", row)));
        }

        /// <summary>
        /// Given there is a properly formatted CSV file with headers that contain C# keywords.
        /// </summary>
        public void given_there_is_a_csv_with_header_not_matching_identifier_rules()
        {
            Console.WriteLine("Given there is a properly formatted CSV file with headers that do not match rules.");
            this.TestData = new List<List<string>>
                                {
                                    new List<string> { "1header", "head3", "два" },
                                    new List<string> { "val11", "val12", "val13" },
                                };
            this.ExpectedHeaders = new List<string>
                                  {
                                      "Column0", "Head3", "Column2"
                                  };
            this.CsvContent = string.Join("\n", this.TestData.Select(row => string.Join(",", row)));
        }

        /// <summary>
        /// Given there is a CSV without header.
        /// </summary>
        public void given_there_is_a_csv_without_header()
        {
            Console.WriteLine("Given there is a CSV without header.");
            this.TestData = new List<List<string>>
                                {
                                    new List<string> { "val11", "val12", "val13" },
                                    new List<string> { "val21", "val22", "val23" },
                                };
            this.CsvContent = string.Join("\n", this.TestData.Select(row => string.Join(",", row)));
            this.ExpectedHeaders = new List<string>
                                  {
                                      "Column0", "Column1", "Column2"
                                  };
        }

        /// <summary>
        /// Given there is an empty source CSV.
        /// </summary>
        public void given_there_is_empty_csv()
        {
            Console.WriteLine("Given there is an empty source CSV.");
            this.CsvContent = string.Empty;
            this.TestData = new List<List<string>>();
        }

        public void given_there_is_a_csv_with_header_and_mismatched_row()
        {
            Console.WriteLine("Given there is a properly formatted CSV file with header and some row misses cells.");
            this.TestData = new List<List<string>>
                                {
                                    new List<string> { "head1", "head2", "head3" },
                                    new List<string> { "val11", "val12", "val13" },
                                    new List<string> { "val21", "val22" },
                                };
            this.ExpectedHeaders = new List<string>
                                  {
                                      "Head1", "Head2", "Head3"
                                  };
            this.CsvContent = string.Join("\n", this.TestData.Select(row => string.Join(",", row)));
        }

        /// <summary>
        /// Parse CSV.
        /// </summary>
        /// <param name="options">
        /// Reader options.
        /// </param>
        public void when_csv_is_parsed(CsvReaderOptions options = null)
        {
            Console.WriteLine("When the CSV is parsed.");
            try
            {
                using (var stream = this.CsvContent.ToStream())
                {
                    this.ReadResult = CsvReader.Parse(stream, options: options).ToList();
                }
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        /// <summary>
        /// When the CSV without header is parsed.
        /// </summary>
        public void when_csv_without_header_is_parsed()
        {
            Console.WriteLine("When the CSV without header is parsed.");
            using (var stream = this.CsvContent.ToStream())
            {
                this.ReadResult = CsvReader.Parse(stream, hasHeader: false).ToList();
            }
        }

        /// <summary>
        /// When new column is added to the read result.
        /// </summary>
        public void when_new_column_is_added_to_read_result()
        {
            Console.WriteLine("When new column is added to the read result.");
            foreach (IDictionary<string, string> item in this.ReadResult)
            {
                item.Add(NewColumnName, NewColumnDefaultValue);
            }
        }

        /// <summary>
        /// When column is removed from the read results.
        /// </summary>
        public void when_column_is_removed_from_read_result()
        {
            Console.WriteLine("When column is removed from the read results.");
            foreach (IDictionary<string, string> item in this.ReadResult)
            {
                item.Remove(item.First().Key);
            }
        }

        public void when_first_line_is_read()
        {
            Console.WriteLine("When the first line from the CSV is parsed.");
            try
            {
                using (var stream = this.CsvContent.ToStream())
                {
                    this.ReadResult = new List<dynamic> { CsvReader.Parse(stream, options: new CsvReaderOptions { StrictCellCount = true }).First() };
                }
            }
            catch (Exception exception)
            {
                this.Exception = exception;
            }
        }

        /// <summary>
        /// Expect property values to be the same as in corresponding cells.
        /// </summary>
        public void expect_property_value_to_be_the_same_as_in_corresponding_cell()
        {
            List<string> headerRow = this.TestData[0];
            Console.WriteLine("Expect property values to be the same as in corresponding cells.");
            for (int i = 0; i < this.ReadResult.Count; i++)
            {
                ITestItemInterface item = Impromptu.ActLike<ITestItemInterface>(this.ReadResult[i]);
                for (int j = 0; j < headerRow.Count; j++)
                {
                    string expected = this.TestData[i + 1][j];
                    string actual = item.GetType().GetProperty(headerRow[j].ToTitleCase()).GetValue(item).ToString();
                    Assert.AreEqual(expected, actual);
                }
            }
        }

        /// <summary>
        /// Expect enumeration of the items that result from parsing of CSV to yield cell values.
        /// </summary>
        /// <param name="hasHeader">
        /// Indicates if source CSV has a header.
        /// </param>
        public void expect_enumeration_of_item_to_yield_cells(bool hasHeader)
        {
            Console.WriteLine("Expect enumeration of the items that result from parsing of CSV to yield cell values.");
            for (int i = 0; i < this.ReadResult.Count; i++)
            {
                int index = hasHeader ? i + 1 : i;
                List<string> expectedRow = this.TestData[index];
                dynamic row = this.ReadResult[i];
                List<string> actualRow = ((IEnumerable<string>)row).ToList();
                for (int j = 0; j < expectedRow.Count; j++)
                {
                    Assert.AreEqual(expectedRow[j], actualRow[j]);
                }
            }
        }

        /// <summary>
        /// Expect that each item can be explicitly cast to dictionary.
        /// </summary>
        public void expect_that_each_item_can_be_cast_to_dictionary()
        {
            Console.WriteLine("Expect that each item can be explicitly cast to dictionary.");
            for (int i = 0; i < this.ReadResult.Count; i++)
            {
                int index = this.ExpectedHeaders != null ? i + 1 : i;
                List<string> expectedRow = this.TestData[index];
                dynamic row = this.ReadResult[i];
                var actualRow = (IDictionary<string, string>)row;
                var values = actualRow.Values.ToList();
                for (int j = 0; j < values.Count; j++)
                {
                    Assert.AreEqual(expectedRow[j], values[j]);
                }

                if (this.ExpectedHeaders != null)
                {
                    var keys = actualRow.Keys.ToList();
                    for (int j = 0; j < keys.Count; j++)
                    {
                        Assert.AreEqual(this.ExpectedHeaders[j], keys[j]);
                    }                    
                }
            }
        }

        /// <summary>
        /// Expect that each item behaves as collection.
        /// </summary>
        public void expect_that_each_item_behaves_as_collection()
        {
            Console.WriteLine("Expect that each item behaves as collection.");
            foreach (ICollection<KeyValuePair<string, string>> item in this.ReadResult)
            {
                var newColumn = new KeyValuePair<string, string>(NewColumnName, NewColumnDefaultValue);
                item.Add(newColumn);
                Assert.IsTrue(item.Contains(newColumn));
                Assert.IsTrue(item.Remove(newColumn));
                Assert.IsFalse(item.Contains(newColumn));

                item.Clear();
                item.Add(newColumn);
                var destination = new KeyValuePair<string, string>[1];
                item.CopyTo(destination, 0);
                Assert.IsTrue(destination.Contains(newColumn));

                Assert.AreEqual(1, item.Count);
                Assert.IsFalse(item.IsReadOnly);
            }
        }

        /// <summary>
        /// Expect each item to have a property per header with corresponding name.
        /// </summary>
        public void expect_item_properties_to_correspond_to_headers()
        {
            Console.WriteLine("Expect each item to have a property per header with corresponding name.");
            foreach (dynamic item in this.ReadResult)
            {
                var metaData = (IDynamicMetaObjectProvider)item;
                DynamicMetaObject metaObject = metaData.GetMetaObject(Expression.Constant(item));
                IEnumerable<string> memberNames = metaObject.GetDynamicMemberNames();

                List<string> properties = memberNames.ToList();

                Assert.AreEqual(this.ExpectedHeaders.Count, properties.Count);

                for (int j = 0; j < this.ExpectedHeaders.Count; j++)
                {
                    Assert.AreEqual(this.ExpectedHeaders[j], properties[j]);
                }
            }
        }

        /// <summary>
        /// Check that there is a correct number of items created.
        /// </summary>
        /// <param name="useHeader">
        /// Indicates if the CSV has header or not.
        /// </param>
        public void expect_correct_count_of_items(bool useHeader)
        {
            Console.WriteLine("Expect resulting enumeration to count all rows except header.");
            Assert.AreEqual(useHeader ? this.TestData.Count - 1 : this.TestData.Count, this.ReadResult.Count);
        }

        /// <summary>
        /// Expect each row to be a list of string values.
        /// </summary>
        public void expect_each_row_to_be_a_list_of_string_values()
        {
            Console.WriteLine("Expect each row to be a list of string values.");
            this.ReadResult.ForEach(row => Assert.IsNotNull(row as IEnumerable<string>));
        }

        /// <summary>
        /// The expect_zero_items_in_the_result.
        /// </summary>
        public void expect_zero_items_in_the_result()
        {
            Console.WriteLine("Expect 0 items to be in the result.");
            Assert.IsNotNull(this.ReadResult);
            Assert.AreEqual(0, this.ReadResult.Count);
        }

        /// <summary>
        /// Expect values that for new column to be retained in items..
        /// </summary>
        public void expect_values_set_for_new_column_to_be_retained()
        {
            Console.WriteLine("Expect values that for new column to be retained in items.");
            foreach (IDictionary<string, string> item in this.ReadResult)
            {
                Assert.IsTrue(item.ContainsKey(NewColumnName));
                Assert.AreEqual(NewColumnDefaultValue, item[NewColumnName]);
            }
        }

        /// <summary>
        /// Expect none of the items to have the removed column.
        /// </summary>
        public void expect_none_of_the_items_to_have_removed_column()
        {
            Console.WriteLine("Expect none of the items to have the removed column.");
            Console.WriteLine("Expect values that for new column to be retained in items.");
            foreach (IDictionary<string, string> item in this.ReadResult)
            {
                string value;
                Assert.IsFalse(item.TryGetValue(this.TestData[0][0].ToTitleCase(), out value));
            }
        }

        public void expect_invalid_cell_count_exception()
        {
            Console.WriteLine("Expect invalid cell count exception.");
            Assert.IsInstanceOfType(this.Exception, typeof(CsvInvalidCellCountException));
        }

        public void expect_there_are_no_exceptions()
        {
            Console.WriteLine("Expect there are no exceptions.");
            Assert.IsNull(this.Exception);
        }

        public void expect_cell_data_can_be_updated()
        {
            ((IDictionary<string, string>)this.ReadResult[0])[this.ExpectedHeaders[0]] = "Test";
        }

        public void expect_cell_values_can_be_enumerated()
        {
            var expectedValues = this.TestData[1];
            var actualValues = (IEnumerable)this.ReadResult[0];
            int expectedCount = 0;
            foreach (string actualValue in actualValues)
            {
                Assert.AreEqual(expectedValues[expectedCount], actualValue);
                expectedCount++;
            }
        }
    }
    // ReSharper restore InconsistentNaming
}
