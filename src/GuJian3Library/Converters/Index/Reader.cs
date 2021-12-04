// Copyright (c) 2021 Kaplas
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

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
