// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestItemGenerator.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Generates test items with random names.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
// ReSharper disable ConvertPropertyToExpressionBody
namespace CsvWrangler.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using ImpromptuInterface;

    /// <summary>
    /// Generates test items with random names.
    /// </summary>
    public class TestItemGenerator : IEnumerable<ITestItemInterface>
    {
        /// <summary>
        /// Max times the generator can be called. Prevents endless tests.
        /// </summary>
        private const int MaxInvocationCount = 1000;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestItemGenerator"/> class.
        /// </summary>
        public TestItemGenerator()
        {
            this.InvocationCount = 0;
        }

        /// <summary>
        /// Gets value indicating expected length of string containing values.
        /// </summary>
        public static int ExpectedLineLength
        {
            get
            {
                return (Guid.Empty.ToString().Length * 3) + 2;
            }
        }

        /// <summary>
        /// Gets count of times the enumerator was invoked.
        /// </summary>
        public int InvocationCount { get; private set; }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// An enumerator that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ITestItemInterface> GetEnumerator()
        {
            for (int i = 0; i < MaxInvocationCount; i++)
            {
                this.InvocationCount++;
                yield return
                    new
                        {
                            Head1 = Guid.NewGuid().ToString(),
                            Head2 = Guid.NewGuid().ToString(),
                            Head3 = Guid.NewGuid().ToString()
                        }.ActLike<ITestItemInterface>();
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
