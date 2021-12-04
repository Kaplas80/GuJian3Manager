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

namespace GuJian3Library.Converters.XXTEA
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// GuJian 3 file decrypter.
    /// </summary>
    public class Decrypt : IInitializer<string>, IConverter<BinaryFormat, BinaryFormat>
    {
        private uint[] _key;

        /// <summary>
        /// Initializes the decryption parameters.
        /// </summary>
        /// <param name="parameters">Decryptor configuration.</param>
        public void Initialize(string parameters)
        {
            byte[] bytes = Encoding.ASCII.GetBytes(parameters);
            _key = new uint[4];
            Buffer.BlockCopy(bytes, 0, _key, 0, 16);
        }

        /// <summary>
        /// Decrypts a XXTEA encrypted BinaryFormat.
        /// </summary>
        /// <param name="source">Encrypted format.</param>
        /// <returns>The decrypted binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (_key == null)
            {
                throw new FormatException("Uninitialized key.");
            }

            var result = new BinaryFormat();

            DataStream input = source.Stream;
            DataStream output = result.Stream;
            input.Position = 0;

            byte[] buffer = new byte[0x1000];
            while (!source.Stream.EndOfStream)
            {
                int size = (int)Math.Min(0x1000, input.Length - input.Position);
                int readCount = input.Read(buffer, 0, size);

                DecryptChunk(buffer, readCount, _key);

                output.Write(buffer, 0, readCount);
            }

            return result;
        }

        private static void DecryptChunk(byte[] buffer, int size, IReadOnlyList<uint> key)
        {
            int chunkCount = size / 256;

            for (int i = 0; i < chunkCount; i++)
            {
                uint[] data = new uint[64];
                Buffer.BlockCopy(buffer, i * 256, data, 0, 256);
                DecryptBlock(data, 64, key);
                Buffer.BlockCopy(data, 0, buffer, i * 256, 256);
            }

            // Process the last block
            int lastBlockLength = (size - (chunkCount * 256)) / 4;
            if (lastBlockLength > 1)
            {
                uint[] data = new uint[lastBlockLength];
                Buffer.BlockCopy(buffer, chunkCount * 256, data, 0, lastBlockLength * 4);
                DecryptBlock(data, lastBlockLength, key);
                Buffer.BlockCopy(data, 0, buffer, chunkCount * 256, lastBlockLength * 4);
            }

            // Process the last bytes
            int lastBytesCount = size - (chunkCount * 256) - (lastBlockLength * 4);
            if (lastBytesCount > 0)
            {
                for (int i = size - lastBytesCount; i < size; i++)
                {
                    buffer[i] = (byte)(buffer[i] ^ (size - i) ^ 0xB7);
                }
            }
        }

        // XXTEA algorithm
        // See: https://en.wikipedia.org/wiki/XXTEA
        private static void DecryptBlock(IList<uint> data, int blockLength, IReadOnlyList<uint> key)
        {
            const uint delta = 0x9E3779B9;
            uint rounds = (uint)(6 + (52 / blockLength));
            uint sum = rounds * delta;
            uint y = data[0];

            do
            {
                uint e = (sum >> 2) & 3;
                for (int p = blockLength - 1; p >= 0; p--)
                {
                    uint z = (p > 0) ? data[p - 1] : data[blockLength - 1];

                    uint value1 = (z >> 5) ^ (y << 2);
                    uint value2 = (y >> 3) ^ (z << 4);
                    uint value3 = sum ^ y;
                    uint value4 = key[(int)((p & 3) ^ e)] ^ z;

                    data[p] -= (value1 + value2) ^ (value3 + value4);

                    y = data[p];
                }

                sum -= delta;
                rounds--;
            }
            while (rounds > 0);
        }
    }
}
