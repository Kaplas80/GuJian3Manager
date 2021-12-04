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
    /// Compress files using Oodle.
    /// </summary>
    public class Compress : IConverter<BinaryFormat, OodleFile>, IInitializer<CompressorParameters>
    {
        private CompressorParameters _compressorParameters = new ();

        /// <summary>
        /// Initializes the compressor parameters.
        /// </summary>
        /// <param name="parameters">Compressor configuration.</param>
        public void Initialize(CompressorParameters parameters) => _compressorParameters = parameters;

        /// <summary>
        /// Compress a BinaryFormat using Oodle.
        /// </summary>
        /// <param name="source">Original format.</param>
        /// <returns>The compressed file.</returns>
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
            DateTime time = new DateTime(1970, 1, 1).ToLocalTime();

            dest.Size = source.Stream.Length;
            dest.Date = (long)(DateTime.Now - time).TotalSeconds;
            dest.CompressedSize = 0x20; // at this moment, only header size is known
            dest.SeekChunkLength = OodleWrapper.GetSeekChunkLen(source.Stream.Length);
            dest.Compression = (ushort)(_compressorParameters.CompressionLevel == OodleWrapper.OodleLZ_CompressionLevel.OodleLZ_CompressionLevel_None ? 0 : 4);

            if (dest.Compression == 0)
            {
                OodleChunk chunk = new ();
                chunk.Size = (int)dest.Size;
                chunk.CompressedSize += (int)dest.Size;
                chunk.Data = new byte[chunk.Size];
                source.Stream.Read(chunk.Data, 0, chunk.Size);

                dest.Chunks.Add(chunk);
            }
            else
            {
                int remaining = (int)source.Stream.Length;
                while (remaining > 0)
                {
                    OodleChunk chunk = new ();
                    chunk.Size = Math.Min(dest.SeekChunkLength, remaining);
                    byte[] data = reader.ReadBytes(chunk.Size);

                    chunk.Data = OodleWrapper.Compress(data, _compressorParameters.Compressor, _compressorParameters.CompressionLevel, dest.SeekChunkLength);
                    chunk.CompressedSize = chunk.Data.Length + 4;

                    remaining -= chunk.Size;
                    dest.CompressedSize += chunk.CompressedSize + 4;

                    dest.Chunks.Add(chunk);
                }
            }

            return dest;
        }
    }
}
