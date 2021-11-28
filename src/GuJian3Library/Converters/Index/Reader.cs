// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converters.Index
{
    using System;
    using System.Collections.Generic;
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat to IndexFile.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, IndexFile>
    {
        /// <summary>
        /// Reads a GuJian3 index file.
        /// </summary>
        /// <param name="source">The file in BinaryFormat.</param>
        /// <returns>The file.</returns>
        public IndexFile Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var result = new IndexFile();

            var reader = new TextDataReader(source.Stream);

            while (!source.Stream.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] split = line.Split('\t');

                if (!result.Dictionary.ContainsKey(split[0]))
                {
                    result.Dictionary[split[0]] = new List<string>();
                }

                result.Dictionary[split[0]].Add(split[1]);
            }

            return result;
        }
    }
}
