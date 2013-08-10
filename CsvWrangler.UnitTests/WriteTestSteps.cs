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
    using System.IO;
    using System.Linq;

    using ImpromptuInterface;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    // ReSharper disable InconsistentNaming

    /// <summary>
    /// The steps used in CSV writer tests.
    /// </summary>
    internal class WriteTestSteps
    {
        /// <summary>
        /// List of items that will be converted to CSV.
        /// </summary>
        private IEnumerable<ITestItemInterface> items = null;

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
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
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
            this.expectedLines = new string[]
                                    {
                                        "val11,val12,val13",
                                        "val21,val22,val23"
                                    };
        }

        /// <summary>
        /// When the list is converted to CSV.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
        public void when_the_list_is_persisted_to_csv()
        {
            Console.WriteLine("When the list is converted to CSV.");
            using (var reader = new StreamReader(CsvWriter.ToCsv(this.items)))
            {
                this.csv = reader.ReadToEnd();
            }
        }

        /// <summary>
        /// Expect the CSV to have a header that contains expected header values.
        /// </summary>
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
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
        [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
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
    }
    
    // ReSharper restore InconsistentNaming
}