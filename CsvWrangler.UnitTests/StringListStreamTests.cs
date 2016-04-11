// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringListStreamTests.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Tests for string list stream.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable InconsistentNaming
namespace CsvWrangler.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    /// <summary>
    /// Tests for string list stream.
    /// </summary>
    [TestClass]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1300:ElementMustBeginWithUpperCaseLetter", Justification = "Unit test naming convention.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Test naming convention.")]
    public class StringListStreamTests
    {
        private static readonly List<string> StringList = new List<string>
            {
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString()
            };

        private static StringListStream stream;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            stream = new StringListStream(StringList.GetEnumerator());
        }

        [TestMethod]
        public void the_string_list_stream_cannot_be_sought()
        {
            Assert.IsFalse(stream.CanSeek);
        }

        [TestMethod]
        public void the_string_list_stream_cannot_be_written_to()
        {
            Assert.IsFalse(stream.CanWrite);
        }

        [TestMethod]
        public void cannot_get_length_from_string_list_stream()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                Console.WriteLine(stream.Length);
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_position_cannot_be_set()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                stream.Position = 100;
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_position_cannot_be_read()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                Console.WriteLine(stream.Position);
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_cannot_be_written_to()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                var buffer = new byte[1024];
                stream.Write(buffer, 0, buffer.Length);
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_cannot_be_flushed()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                stream.Flush();
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_does_not_support_seek_operation()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void string_list_stream_does_not_support_setting_length()
        {
            bool notSupportedExceptionCaught = false;
            try
            {
                stream.SetLength(1024);
            }
            catch (NotSupportedException)
            {
                notSupportedExceptionCaught = true;
            }

            Assert.IsTrue(notSupportedExceptionCaught);
        }

        [TestMethod]
        public void buffer_cannot_be_null_when_reading_from_string_list_stream()
        {
            bool argumentExceptionCaught = false;
            try
            {
                // ReSharper disable once AssignNullToNotNullAttribute
                stream.Read(null, 0, 1024);
            }
            catch (ArgumentException)
            {
                argumentExceptionCaught = true;
            }

            Assert.IsTrue(argumentExceptionCaught);
        }

        [TestMethod]
        public void byte_count_cannot_be_negative_when_reading_from_string_list_stream()
        {
            bool argumentExceptionCaught = false;
            try
            {
                stream.Read(new byte[1024], 0, -10);
            }
            catch (ArgumentOutOfRangeException)
            {
                argumentExceptionCaught = true;
            }

            Assert.IsTrue(argumentExceptionCaught);
        }
    }
}
