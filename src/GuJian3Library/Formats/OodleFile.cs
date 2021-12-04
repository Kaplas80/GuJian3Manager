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

namespace GuJian3Library.Formats
{
    using System;
    using System.Collections.Generic;
    using Nito.HashAlgorithms;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Representation of a Oodle compressed file.
    /// </summary>
    public class OodleFile : IFormat
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OodleFile"/> class.
        /// </summary>
        public OodleFile()
        {
            Chunks = new List<OodleChunk>();
        }

        /// <summary>
        /// Gets or sets the uncompressed size.
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// Gets or sets the compression date. (time_t 64).
        /// </summary>
        public long Date { get; set; }

        /// <summary>
        /// Gets or sets the compressed size.
        /// </summary>
        public long CompressedSize { get; set; }

        /// <summary>
        /// Gets or sets the seek chunk length used (needed for oodle algorithm).
        /// </summary>
        public int SeekChunkLength { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the compression format.
        /// If the file is compressed, the value is 4. If not, the value is 0.
        /// </summary>
        public ushort Compression { get; set; }

        /// <summary>
        /// Gets the header CRC-16 value.
        /// </summary>
        public ushort Crc16
        {
            get
            {
                byte[] data = new byte[30];
                using DataStream ds = DataStreamFactory.FromArray(data, 0, data.Length);
                DataWriter dw = new (ds)
                {
                    Endianness = EndiannessMode.LittleEndian,
                };

                dw.Write(Size);
                dw.Write(Date);
                dw.Write(CompressedSize);
                dw.Write(SeekChunkLength);
                dw.Write(Compression);

                CRC16 calculator = new ();
                byte[] result = calculator.ComputeHash(data, 0, data.Length);
                return BitConverter.ToUInt16(result, 0);
            }
        }

        /// <summary>
        /// Gets the list of compressed chunks.
        /// </summary>
        public List<OodleChunk> Chunks { get; }
    }
}
