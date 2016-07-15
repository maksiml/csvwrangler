// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ParserTest.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   //   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Test for CSVParser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// ReSharper disable InconsistentNaming
// ReSharper disable RedundantArgumentNameForLiteralExpression
namespace CsvWrangler.UnitTests
{
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Test class.")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Reviewed. Unit test naming convention.")]
    public class ParserTest
    {
        [TestMethod]
        public void quotes_are_removed_from_quoted_values()
        {
            List<string> line = new List<string> { "val11", "\"val12\"", "val13" };
            string input = string.Join(",", line);
            string expected = input.Replace("\"", string.Empty).Replace(",", ";");
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void quoted_value_at_the_end_of_record_is_treated_correctly()
        {
            string input = "val11,\"val12\"";
            string expected = "val11;val12";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void separator_is_allowed_in_quotes()
        {
            List<string> line = new List<string> { "val11", "\"val121,val122\"", "val13" };
            string input = string.Join(",", line);
            string expected = string.Join(";", line).Replace("\"", string.Empty);
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void quotes_can_be_escaped()
        {
            string input = "val11,\"\"val12\"\",val13";
            string expected = "val11;\"val12\";val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void quotes_can_be_escaped_inside_quoted_value()
        {
            string input = "val11,\"val121\"\"val122\",val13";
            string expected = "val11;val121\"val122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void escaped_double_quote_outside_of_quoted_string_will_be_treated_as_quote()
        {
            // This behavior is not required by standard and Excel will use quoted values in this
            // cases, but it seems to make sense.
            string input = "val11,val121\"\"val122,val13";
            string expected = "val11;val121\"val122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void escaped_double_quote_at_the_start_will_be_treated_as_quote()
        {
            // This behavior is not required by standard and Excel will use quoted values in this
            // cases, but it seems to make sense.
            string input = "val11,\"\"val122,val13";
            string expected = "val11;\"val122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void multiple_escaped_double_quotes_in_quoted_value_will_be_treated_as_quote()
        {
            string input = "val11,\"val121\"\"val122\"\"val123\",val13";
            string expected = "val11;val121\"val122\"val123;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void escapde_double_quotes_at_the_end_of_quoted_value_will_be_treated_as_quote()
        {
            string input = "val11,\"val121\"\"\",val13";
            string expected = "val11;val121\";val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void unterminated_double_quoted_values_will_be_treated_as_quoted()
        {
            // This behavior is breaking the standard but we will treat it in a way that makes most sense
            // unless otherwise requested.
            string input = "val11,\"val121\"\"val122,val13";
            string expected = "val11;val121\"val122,val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void unterminated_quoted_value_ending_with_quote_at_the_end_of_the_record_is_treated_correctly()
        {
            string input = "val11,\"val12\"\"";
            string expected = "val11;val12\"";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void quoted_value_may_contain_CRLF()
        {
            string input = "val11,\"val121\r\nval122\",val13";
            string expected = "val11;val121\nval122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void quoted_value_may_contain_LF()
        {
            string input = "val11,\"val121\nval122\",val13";
            string expected = "val11;val121\nval122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void LF_at_the_begging_of_the_quoted_value_treated_correctly()
        {
            // This behavior is not required by standard and Excel will use quoted values in this
            // cases, but it seems to make sense.
            string input = "val11,\"\nval122\",val13";
            string expected = "val11;\nval122;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void unquoted_part_of_the_value_is_ignored_when_trailing_quoted_value()
        {
            // This behavior is breaking the standard but we will treat it in a way that makes most sense
            // unless otherwise requested.
            string input = "val11,\"val121\"val122,val13";
            string expected = "val11;val121;val13";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void values_are_not_trimmed()
        {
            string input = " val11 ,\" val121\nval122 \", val13 ";
            string expected = " val11 ; val121\nval122 ; val13 ";
            var output = CsvReader.Parse(input.ToStream(), hasHeader: false);
            string actual = string.Join(";", output.First());
            Assert.AreEqual(expected, actual);
        }
    }
}
