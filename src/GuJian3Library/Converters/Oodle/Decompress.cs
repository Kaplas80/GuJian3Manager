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

namespace GuJian3Library.Converters.Oodle
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompress Oodle compressed files used in GuJian 3.
    /// </summary>
    public class Decompress : IConverter<OodleFile, BinaryFormat>
    {
        /// <summary>
        /// Decompress an Oodle compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The decompressed binary.</returns>
        public BinaryFormat Convert(OodleFile source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dest = new BinaryFormat();

            var writer = new DataWriter(dest.Stream);

            if (source.Compression == 0)
            {
                writer.Write(source.Chunks[0].Data);
            }
            else
            {
                for (int i = 0; i < source.Chunks.Count; i++)
                {
                    byte[] decompressedData = OodleWrapper.Decompress(source.Chunks[i].Data, source.Chunks[i].Size);
                    writer.Write(decompressedData);
                }
            }

            return dest;
        }
    }
}
