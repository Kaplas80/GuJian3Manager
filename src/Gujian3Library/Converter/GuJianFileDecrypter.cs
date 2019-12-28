// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// GuJian 3 file decrypter.
    /// </summary>
    public class GuJianFileDecrypter : IInitializer<string>, IConverter<BinaryFormat, BinaryFormat>
    {
        private readonly Dictionary<string, string> keys = new Dictionary<string, string>
        {
            { "cutscene\\1010\\cs_1010_1a.xxx", "84bf71f9a6a44fa3f3e1a266166dac7297f6018b" },
            { "cutscene\\1010\\cs_1010_1b.xxx", "3cdfdf73bef25ae40a40215025fcabaffca2c72a" },
            { "interface\\resource\\movie\\cg00100b.xxx", "25bcfb70b5cc7e0f2253ffe8a9e3d6e831aad2f5" },
            { "interface\\resource_en\\movie\\optxt.xxx", "73be7f337c0c248926630719cdbe927895866a41" },
            { "maps\\m01\\elems.xxx", "4a40a9712bfdc8e79c46b0ab735130d947dfe6f1" },
            { "sounds_console\\bgm.xxx", "5ff3f69e1d15a67f5d578dbc4d7ce263c1e83135" },
            { "sounds_console\\bgm_p1.xxx", "b388c0bf15d0b39c684a0af3c2c3b925c00598b0" },
            { "sounds_console\\voice.xxx", "7ba2771ca1907e9f7348f38a5b8b3881d44db8a2" },
            { "sounds_console\\voice_npc.xxx", "960513d4e71a2b003eda159891e102c43e344eed" },
            { "sounds_console\\voice_p1.xxx", "1b2466ac36ee2e9c2fce01f76dbb1e873df42c06" },
        };

        private string keyString;

        /// <inheritdoc/>
        public void Initialize(string parameters)
        {
            KeyValuePair<string, string> kvp = this.keys.FirstOrDefault(x => parameters.EndsWith(x.Key, StringComparison.InvariantCultureIgnoreCase));
            if (!string.IsNullOrEmpty(kvp.Value))
            {
                this.keyString = kvp.Value;
            }
        }

        /// <inheritdoc/>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ownserhip dispose transferred")]
        public BinaryFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (string.IsNullOrEmpty(this.keyString))
            {
                throw new FormatException("Uninitialized converter.");
            }

            var result = new BinaryFormat();

            DataStream input = source.Stream;
            DataStream output = result.Stream;
            input.Position = 0;

            uint[] key = GetKey(this.keyString);

            var buffer = new byte[0x1000];
            while (!source.Stream.EndOfStream)
            {
                int size = (int)Math.Min(0x1000, input.Length - input.Position);
                int readCount = input.Read(buffer, 0, size);

                DecryptChunk(buffer, readCount, key);

                output.Write(buffer, 0, readCount);
            }

            return result;
        }

        private static uint[] GetKey(string key)
        {
            var temp = new byte[16];
            byte[] hexKey = Encoding.ASCII.GetBytes(string.Concat(key, key.Substring(24, 8)));

            for (int i = 0; i < 16; i++)
            {
                temp[i] = (byte)(hexKey[i] ^ hexKey[i + 16] ^ hexKey[i + 32]);
            }

            var result = new uint[temp.Length / sizeof(uint)];
            Buffer.BlockCopy(temp, 0, result, 0, temp.Length);

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
            if (lastBlockLength > 0)
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

                    uint value1 = (next * 4) ^ (previous >> 5);
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
