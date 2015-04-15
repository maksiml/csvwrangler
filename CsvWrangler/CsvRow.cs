// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CsvRow.cs" company="CsvWrangler">
//   This file is a part of CsvWrangler and is licensed under the MS-PL.
//   http://www.opensource.org/licenses/ms-pl.html
// </copyright>
// <summary>
//   Represents row retrieved from CSV file.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace CsvWrangler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;

    /// <summary>
    /// Represents row retrieved from CSV file.
    /// </summary>
    public class CsvRow : DynamicObject, IEnumerable<string>, IDictionary<string, string>
    {
        /// <summary>
        /// The map of header names to values.
        /// </summary>
        private readonly Dictionary<string, string> values = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRow"/> class.
        /// </summary>
        public CsvRow()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvRow"/> class.
        /// </summary>
        /// <param name="headers">
        /// The headers.
        /// </param>
        /// <param name="values">
        /// The values.
        /// </param>
        /// <param name="options">
        /// The options.
        /// </param>
        public CsvRow(List<string> headers, List<string> values, CsvReaderOptions options)
        {
            if (options.StrictCellCount && headers.Count != values.Count)
            {
                throw new CsvInvalidCellCountException(headers.Count, values.Count);
            }

            for (int i = 0; i < headers.Count; i++)
            {
                string header = headers[i];
                this.values.Add(header, values.Count > i ? values[i] : string.Empty);
            }
        }

        /// <summary>
        /// Gets the number of elements contained in the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </summary>
        public int Count
        {
            get
            {
                return this.values.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <seealso cref="ICollection&lt;T&gt;"/> is read-only.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, string>)this.values).IsReadOnly;
            }
        }

        /// <summary>
        /// Gets an <seealso cref="ICollection&lt;T&gt;"/> containing the values in the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public ICollection<string> Values
        {
            get
            {
                return this.values.Values;
            }
        }

        /// <summary>
        /// Gets an <seealso cref="ICollection&lt;T&gt;"/> containing the keys of the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        public ICollection<string> Keys
        {
            get
            {
                return this.values.Keys;
            }
        }

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key of the element to get or set.
        /// </param>
        /// <returns>
        /// The element with the key.
        /// </returns>
        public string this[string key]
        {
            get
            {
                return this.values[key];
            }

            set
            {
                this.values[key] = value;
            }
        }

        /// <summary>
        /// Try to get cell value given header.
        /// </summary>
        /// <param name="binder">
        /// The binder.
        /// </param>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The cell value that corresponds to the header name provided in the <paramref name="binder"/>.
        /// </returns>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            result = null;
            string header = binder.Name;
            bool memeberExists = this.values.ContainsKey(header);
            if (memeberExists)
            {
                result = this.values[header];
            }

            return memeberExists;
        }

        /// <summary>
        /// Get list of 'properties' that the object supports, list consists of header names.
        /// </summary>
        /// <returns>
        /// List of header names.
        /// </returns>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return this.values.Keys;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <seealso cref="IEnumerator&lt;T&gt;"/> that can be used to iterate through the collection.
        /// </returns>
        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
        {
            return this.values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a values.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through a values.
        /// </returns>
        public IEnumerator<string> GetEnumerator()
        {
            return this.values.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a values.
        /// </summary>
        /// <returns>
        /// The enumerator that iterates through a values.
        /// </returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.values.Values.GetEnumerator();
        }

        /// <summary>
        /// Adds an item to the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="item">
        /// The object to add to the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </param>
        public void Add(KeyValuePair<string, string> item)
        {
            ((IDictionary<string, string>)this.values).Add(item);
        }

        /// <summary>
        /// Removes all items from the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </summary>
        public void Clear()
        {
            this.values.Clear();
        }

        /// <summary>
        /// Determines whether the <seealso cref="ICollection&lt;T&gt;"/> contains a specific value.
        /// </summary>
        /// <param name="item">
        /// The object to locate in the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </param>
        /// <returns>
        /// true if item is found in the <seealso cref="ICollection&lt;T&gt;"/>; otherwise, false.
        /// </returns>
        public bool Contains(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)this.values).Contains(item);
        }

        /// <summary>
        /// Copies the elements of the <seealso cref="ICollection&lt;T&gt;"/> to an <seealso cref="Array"/>, starting at a particular <seealso cref="Array"/> index.
        /// </summary>
        /// <param name="array">
        /// The one-dimensional <seealso cref="Array"/> that is the destination of the elements copied from <seealso cref="ICollection&lt;T&gt;"/>. The Array must have zero-based indexing.
        /// </param>
        /// <param name="arrayIndex">
        /// The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            ((IDictionary<string, string>)this.values).CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Removes the first occurrence of a specific object from the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </summary>
        /// <param name="item">
        /// The object to remove from the <seealso cref="ICollection&lt;T&gt;"/>.
        /// </param>
        /// <returns>
        /// true if item was successfully removed from the <seealso cref="ICollection&lt;T&gt;"/>; otherwise, false. 
        /// This method also returns false if item is not found in the original <seealso cref="ICollection&lt;T&gt;"/>.
        /// </returns>
        public bool Remove(KeyValuePair<string, string> item)
        {
            return ((IDictionary<string, string>)this.values).Remove(item);
        }

        /// <summary>
        /// Determines whether the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/> contains an element with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key to locate in the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </param>
        /// <returns>
        /// true if the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/> contains an element with the key; otherwise, false.
        /// </returns>
        public bool ContainsKey(string key)
        {
            return this.values.ContainsKey(key);
        }

        /// <summary>
        /// Adds an element with the provided key and value to the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        /// <param name="key">
        /// The object to use as the key of the element to add.
        /// </param>
        /// <param name="value">
        /// The object to use as the value of the element to add.
        /// </param>
        public void Add(string key, string value)
        {
            this.values.Add(key, value);
        }

        /// <summary>
        /// Removes the element with the specified key from the <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </summary>
        /// <param name="key">
        /// The key of the element to remove.
        /// </param>
        /// <returns>
        /// true if the element is successfully removed; otherwise, false. This method also returns false if key was not found in the original <seealso cref="IDictionary&lt;TKey, TValue&gt;"/>.
        /// </returns>
        public bool Remove(string key)
        {
            return this.values.Remove(key);
        }

        /// <summary>
        /// Gets the value associated with the specified key.
        /// </summary>
        /// <param name="key">
        /// The key whose value to get.
        /// </param>
        /// <param name="value">
        /// When this method returns, the value associated with the specified key, if the key is found; otherwise, 
        /// the default value for the type of the value parameter. This parameter is passed uninitialized.
        /// </param>
        /// <returns>
        /// true if the object that implements <seealso cref="IDictionary&lt;TKey, TValue&gt;"/> contains an element with the specified key; otherwise, false.
        /// </returns>
        public bool TryGetValue(string key, out string value)
        {
            return this.values.TryGetValue(key, out value);
        }
    }
}
