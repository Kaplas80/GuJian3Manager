// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library
{
    using System.Collections.Generic;
    using Yarhl.FileFormat;

    /// <summary>
    /// GuJian 3 index file. Contains SHA-1 file hashes and names.
    /// </summary>
    public class IndexFile : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexFile"/> class.
        /// </summary>
        public IndexFile()
        {
            this.Dictionary = new Dictionary<string, List<string>>();
        }

        /// <summary>
        /// Gets the file hashes dictionary.
        /// </summary>
        public IDictionary<string, List<string>> Dictionary { get; }
    }
}
