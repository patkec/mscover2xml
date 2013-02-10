using System;
using System.Collections;
using System.Collections.Generic;

namespace MSCover2Xml
{
    /// <summary>
    /// Represents a list of <see cref="FileSpec"/> objects.
    /// </summary>
    internal class FileSpecList: IEnumerable<FileSpec>
    {
        private readonly List<FileSpec> _list = new List<FileSpec>();

        /// <summary>
        /// Gets a file specification for the file or creates a new file specification if none exists.
        /// </summary>
        /// <param name="fileName">Name of the file for which to get the specification.</param>
        /// <returns><see cref="FileSpec"/> object for given file name.</returns>
        public FileSpec GetOrAdd(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) throw new ArgumentNullException("fileName");

            var file = _list.Find(x => x.FileName == fileName);
            if (file == null)
            {
                file = new FileSpec(fileName, _list.Count + 1);
                _list.Add(file);
            }
            return file;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<FileSpec> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}