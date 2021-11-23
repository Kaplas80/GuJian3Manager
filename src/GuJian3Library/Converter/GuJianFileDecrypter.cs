// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// GuJian 3 file decrypter.
    /// </summary>
    public class GuJianFileDecrypter : IInitializer<string>, IConverter<BinaryFormat, BinaryFormat>
    {
        private uint[] key;

        /// <inheritdoc/>
        public void Initialize(string parameters)
        {
            var bytes = Encoding.ASCII.GetBytes(parameters);
            this.key = new uint[4];
            Buffer.BlockCopy(bytes, 0, this.key, 0, 16);
        }

        /// <inheritdoc/>
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (this.key == null)
            {
                throw new FormatException("Uninitialized key.");
            }

            var result = new BinaryFormat();

            DataStream input = source.Stream;
            DataStream output = result.Stream;
            input.Position = 0;

            var buffer = new byte[0x1000];
            while (!source.Stream.EndOfStream)
            {
                int size = (int)Math.Min(0x1000, input.Length - input.Position);
                int readCount = input.Read(buffer, 0, size);

                DecryptChunk(buffer, readCount, this.key);

                output.Write(buffer, 0, readCount);
            }

            return result;
        }

        private static void DecryptChunk(byte[] buffer, int size, IReadOnlyList<uint> key)
        {
            int chunkCount = size / 256;

            for (int i = 0; i < chunkCount; i++)
            {
                var data = new uint[64];
                Buffer.BlockCopy(buffer, i * 256, data, 0, 256);
                DecryptBlock(data, 64, key);
                Buffer.BlockCopy(data, 0, buffer, i * 256, 256);
            }

            // Process the last block
            int lastBlockLength = (size - (chunkCount * 256)) / 4;
            if (lastBlockLength > 1)
            {
                var data = new uint[lastBlockLength];
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

        // It's a XXTEA algorithm
        // See: https://en.wikipedia.org/wiki/XXTEA
        private static void DecryptBlock(IList<uint> data, int blockLength, IReadOnlyList<uint> key)
        {
            uint counter = (uint)(((0x34 / blockLength) + 6) * -0x61c88647);
            while (counter != 0)
            {
                uint keyBase = (counter >> 2) & 3;
                uint next = data[0];
                for (int i = blockLength - 1; i >= 0; i--)
                {
                    uint previous = (i - 1) >= 0 ? data[i - 1] : data[blockLength - 1];

                    uint value1 = (next << 2) ^ (previous >> 5);
                    uint value2 = (next >> 3) ^ (previous << 4);
                    uint value3 = next ^ counter;
                    uint value4 = previous ^ key[(int)((i & 3) ^ keyBase)];

                    data[i] = data[i] - (value1 + value2 ^ value3 + value4);

                    next = data[i];
                }

                counter += 0x61c88647;
            }
        }
    }
}
