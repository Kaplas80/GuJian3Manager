// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converter
{
    using System;
    using System.Collections.Generic;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat to IndexFile.
    /// </summary>
    public class IndexFileReader : IConverter<BinaryFormat, IndexFile>
    {
        /// <inheritdoc/>
        public IndexFile Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var result = new IndexFile();

            var reader = new TextReader(source.Stream);

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
