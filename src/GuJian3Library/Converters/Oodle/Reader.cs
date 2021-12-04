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
    using System.Text;
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Reads Oodle compressed files used in GuJian 3.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, OodleFile>
    {
        /// <summary>
        /// Reads an Oodle compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The mormat.</returns>
        public OodleFile Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            var dest = new OodleFile();

            // Header
            dest.Size = reader.ReadInt64();
            dest.Date = reader.ReadInt64(); // (time_t 64 bits)
            dest.CompressedSize = reader.ReadInt64(); // compressed size
            dest.SeekChunkLength = reader.ReadInt32();
            dest.Compression = reader.ReadUInt16();

            ushort crc16 = reader.ReadUInt16();

            if (crc16 != dest.Crc16)
            {
                throw new FormatException("Invalid CRC16");
            }

            if (dest.Compression == 0)
            {
                OodleChunk chunk = new ();
                chunk.Size = (int)dest.Size;
                chunk.CompressedSize = (int)dest.Size;
                chunk.Data = new byte[chunk.Size];
                source.Stream.Read(chunk.Data, 0, chunk.Size);

                dest.Chunks.Add(chunk);
            }
            else
            {
                int chunkCount = (int)Math.Ceiling(dest.Size / (double)dest.SeekChunkLength);
                long dataOffset = source.Stream.Position + (chunkCount * 4);

                for (int i = 0; i < chunkCount; i++)
                {
                    OodleChunk chunk = new ();
                    chunk.CompressedSize = reader.ReadInt32();
                    chunk.Data = new byte[chunk.CompressedSize - 4];

                    source.Stream.PushToPosition(dataOffset, System.IO.SeekOrigin.Begin);

                    chunk.Size = reader.ReadInt32();
                    source.Stream.Read(chunk.Data, 0, chunk.CompressedSize - 4);

                    dataOffset += chunk.CompressedSize;

                    source.Stream.PopPosition();

                    dest.Chunks.Add(chunk);
                }
            }

            return dest;
        }
    }
}
