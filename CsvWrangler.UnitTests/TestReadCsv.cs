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

    using ImpromptuInterface;

    using NUnit.Framework;

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
            string head1 { get; set; }

            /// <summary>
            /// Gets or sets value for header 2.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Test class.")]
            string head2 { get; set; }

            /// <summary>
            /// Gets or sets value for header 3.
            /// </summary>
            [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Test class.")]
            string head3 { get; set; }
        }

        // ReSharper disable InconsistentNaming
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Unit test naming convention.")]
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        [TestCase]
        public void read_properly_formatted_csv_file()
        {
            Console.WriteLine("Given there is a properly formatted CSV with two rows and tree columns.");
            List<List<string>> testData = new List<List<string>>
                                              {
                                                  new List<string> { "head1", "head2", "head3" },
                                                  new List<string> { "val11", "val12", "val13" },
                                                  new List<string> { "val21", "val22", "val23" },
                                              };

            string csvContent = string.Join("\n", testData.Select(row => string.Join(",", row)));

            Console.WriteLine("When the CSV is parsed,");
            List<dynamic> result;
            using (var stream = csvContent.ToStream())
            {
                result = CsvReader.Parse(stream).ToList();
            }

            Console.WriteLine("Expect resulting enumeration to have 2 items,");
            Assert.AreEqual(testData.Count - 1, result.Count);

            Console.WriteLine("and each item to have 3 properties named according to the headers in the source data,");
            List<string> headerRow = testData[0];
            foreach (dynamic item in result)
            {
                var metaData = (IDynamicMetaObjectProvider)item;
                DynamicMetaObject metaObject = metaData.GetMetaObject(Expression.Constant(item));
                IEnumerable<string> memberNames = metaObject.GetDynamicMemberNames();
                
                List<string> properties = memberNames.ToList();
                
                Assert.AreEqual(headerRow.Count, properties.Count);
                
                for (int j = 0; j < headerRow.Count; j++)
                {
                    Assert.AreEqual(headerRow[j], properties[j]);
                }
            }

            Console.WriteLine("and each property to have value according to the corresponding cell.");
            for (int i = 0; i < result.Count; i++)
            {
                ITestItemInterface item = Impromptu.ActLike<ITestItemInterface>(result[i]);
                for (int j = 0; j < headerRow.Count; j++)
                {
                    string expected = testData[i + 1][j];
                    string actual = item.GetType().GetProperty(headerRow[j]).GetValue(item).ToString();
                    Assert.AreEqual(expected, actual);
                }
            }
        }
        // ReSharper restore InconsistentNaming
    }
}
