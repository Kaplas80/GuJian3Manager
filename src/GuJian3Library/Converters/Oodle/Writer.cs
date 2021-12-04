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
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Writes Oodle compressed files used in GuJian 3.
    /// </summary>
    public class Writer : IConverter<OodleFile, BinaryFormat>
    {
        /// <summary>
        /// Writes an Oodle compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The binary.</returns>
        public BinaryFormat Convert(OodleFile source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var dest = new BinaryFormat();

            var writer = new DataWriter(dest.Stream);

            writer.Write(source.Size);
            writer.Write(source.Date);
            writer.Write(source.CompressedSize);
            writer.Write(source.SeekChunkLength);
            writer.Write(source.Compression);
            writer.Write(source.Crc16);

            if (source.Compression == 0)
            {
                writer.Write(source.Chunks[0].Data);
            }
            else
            {
                for (int i = 0; i < source.Chunks.Count; i++)
                {
                    writer.Write(source.Chunks[i].CompressedSize);
                }

                for (int i = 0; i < source.Chunks.Count; i++)
                {
                    writer.Write(source.Chunks[i].Size);
                    writer.Write(source.Chunks[i].Data);
                }
            }

            return dest;
        }
    }
}
