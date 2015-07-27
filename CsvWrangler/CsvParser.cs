// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvParser.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   The CSV parser.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The CSV parser.
    /// </summary>
    public static class CsvParser
    {
        /// <summary>
        /// States for parsers state machine.
        /// </summary>
        private enum ParserStates
        {
            /// <summary>
            /// State that indicates that the machine is at the start of
            /// some value.
            /// </summary>
            ValueStart,

            /// <summary>
            /// The state that indicates that the machine is at the end
            /// of current record.
            /// </summary>
            LineEnd,

            /// <summary>
            /// The state that indicates that the machine is processing 
            /// unquoted record value.
            /// </summary>
            Default,

            /// <summary>
            /// The state that indicates that the machine is at the start
            /// of the value enclosed in quotes.
            /// </summary>
            QuotedStart,

            /// <summary>
            /// Indicates that the machine is at the end of current value.
            /// </summary>
            ValueEnd,

            /// <summary>
            /// Indicates that the machine is processing quoted value.
            /// </summary>
            QuotedDefault,

            /// <summary>
            /// Indicates that the machine have finished processing quoted value.
            /// </summary>
            QuotedEnd
        }

        /// <summary>
        /// Parse single record from stream of CSV data.
        /// </summary>
        /// <param name="reader">
        /// The source of CSV data.
        /// </param>
        /// <param name="separator">
        /// The separator that splits record in to fields.
        /// </param>
        /// <returns>
        /// The enumeration of fields in the single record.
        /// </returns>
        public static IEnumerable<string> ParseLine(TextReader reader, char separator)
        {
            var state = ParserStates.ValueStart;
            var line = reader.ReadLine();
            if (line == null)
            {
                yield break;
            }

            int character = 0;
            int valueStart = 0;
            int valueEnd = 0;
            while (state != ParserStates.LineEnd)
            {
                switch (state)
                {
                    case ParserStates.ValueStart:
                        {
                            // it is an empty string at the end of the line.
                            if (line.Length == character)
                            {
                                yield return string.Empty;
                                state = ParserStates.LineEnd;
                                continue;
                            }

                            if (line[character] == '\"')
                            {
                                state = ParserStates.QuotedStart;
                                character++;
                                valueStart = character;
                            }
                            else
                            {
                                state = ParserStates.Default;
                                valueStart = character;
                            }
                        }

                        break;
                    case ParserStates.Default:
                        {
                            character = line.IndexOf(separator, character);
                            if (character < 0)
                            {
                                character = line.Length;
                            }

                            state = ParserStates.ValueEnd;
                        }

                        break;
                    case ParserStates.ValueEnd:
                        {
                            yield return line
                                            .Substring(valueStart, (valueEnd != 0 ? valueEnd : character) - valueStart)
                                            .Replace("\"\"", "\"");
                            valueEnd = 0;
                            if (character == line.Length)
                            {
                                state = ParserStates.LineEnd;
                            }
                            else
                            {
                                character++;
                                state = ParserStates.ValueStart;
                            }
                        }

                        break;
                    case ParserStates.QuotedStart:
                        {
                            // If this is escaped quote let's fall back to treating the
                            // value as unquoted.
                            if (character < line.Length && line[character] == '\"')
                            {
                                state = ParserStates.Default;
                                character++;
                            }
                            else
                            {
                                state = ParserStates.QuotedDefault;
                            }
                        }

                        break;
                    case ParserStates.QuotedDefault:
                        {
                            character = line.IndexOf('\"', character);
                            if (character < 0)
                            {
                                character = line.Length;
                                var newLine = reader.ReadLine();
                                if (newLine == null)
                                {
                                    state = ParserStates.ValueEnd;
                                }
                                else
                                {
                                    line = string.Format("{0}\n{1}", line, newLine);
                                }
                            }
                            else
                            {
                                state = ParserStates.QuotedEnd;
                            }
                        }

                        break;
                    case ParserStates.QuotedEnd:
                        {
                            character++;
                            if (character < line.Length && line[character] == '\"')
                            {
                                // Guard from the case of unterminated quoted value ending with escaped quotes.
                                if (character == line.Length - 1)
                                {
                                    state = ParserStates.ValueEnd;
                                    character = line.Length;
                                }
                                else
                                {
                                    character += 2;
                                    state = ParserStates.QuotedDefault;
                                }
                            }
                            else
                            {
                                valueEnd = character - 1;
                                while (character < line.Length && line[character] != separator)
                                {
                                    character++;
                                }

                                state = ParserStates.ValueEnd;
                            }
                        }

                        break;
                }
            }
        }
    }
}
