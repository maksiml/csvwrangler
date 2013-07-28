// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StringExtensions.cs" company="CsvWrangler">
//   This file is a part of CsvHelper and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The string extension methods.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.IO;

    /// <summary>
    /// The string extension methods.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Create stream from string.
        /// from: http://stackoverflow.com/questions/1879395/how-to-generate-a-stream-from-a-string
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="Stream"/>.
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "URL spelling.")]
        public static Stream ToStream(this string input)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(input);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }

        /// <summary>
        /// Convert string to title case.
        /// </summary>
        /// <param name="input">
        /// The source string.
        /// </param>
        /// <returns>
        /// The string with title case (wAr aND PeAce => War And Peace).
        /// </returns>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "String conversion sample.")]
        public static string ToTitleCase(this string input)
        {
            return CultureInfo.InvariantCulture.TextInfo.ToTitleCase(input);
        }
    }
}
