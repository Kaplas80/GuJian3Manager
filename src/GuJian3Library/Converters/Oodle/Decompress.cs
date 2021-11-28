// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converters.Oodle
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Decompress Oodle compressed files used in GuJian 3.
    /// </summary>
    public class Decompress : IConverter<BinaryFormat, BinaryFormat>
    {
        /// <summary>
        /// Decompress an Oodle compressed BinaryFormat.
        /// </summary>
        /// <param name="source">Compressed format.</param>
        /// <returns>The decompressed binary.</returns>
        public BinaryFormat Convert(BinaryFormat source)
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

            var dest = new BinaryFormat();

            var writer = new DataWriter(dest.Stream);

            // Header
            ulong decompressedSize = reader.ReadUInt64();
            _ = reader.ReadUInt64(); // date (time_t 64 bits)
            _ = reader.ReadUInt64(); // compressed size
            uint chunkDecompressedSize = reader.ReadUInt32();
            ushort compression = reader.ReadUInt16();

            _ = reader.ReadUInt16(); // crc-16 of header

            if (compression == 0x0000)
            {
                byte[] data = new byte[decompressedSize];
                source.Stream.Read(data, 0, (int)decompressedSize);
                writer.Write(data);
            }
            else
            {
                uint chunkCount = (uint)Math.Ceiling(decompressedSize / (double)chunkDecompressedSize);
                long dataOffset = source.Stream.Position + (chunkCount * 4);

                for (int i = 0; i < chunkCount; i++)
                {
                    int chunkCompressedSize = reader.ReadInt32();
                    source.Stream.PushToPosition(dataOffset, System.IO.SeekOrigin.Begin);

                    int currentChunkSize = reader.ReadInt32();
                    byte[] compressedData = new byte[chunkCompressedSize - 4];
                    source.Stream.Read(compressedData, 0, chunkCompressedSize - 4);

                    byte[] decompressedData = DecompressChunk(compressedData, currentChunkSize);
                    writer.Write(decompressedData);

                    dataOffset += chunkCompressedSize;

                    source.Stream.PopPosition();
                }
            }

            return dest;
        }

        private static byte[] DecompressChunk(byte[] source, int decompressedSize)
        {
            byte[] dest = new byte[decompressedSize];
            _ = OodleLZ_Decompress(
                source,
                (ulong)source.LongLength,
                dest,
                (ulong)decompressedSize,
                0,
                0,
                0,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                0);
            return dest;
        }

        [DllImport("oo2core_6_win64.dll")]
        private static extern uint OodleLZ_Decompress(byte[] src_buf, ulong src_len, byte[] dst_buf, ulong dst_size, int fuzz, int crc, int verbose, IntPtr dst_base, ulong e, IntPtr cb, IntPtr cb_ctx, IntPtr scratch, ulong scratch_size, int threadPhase);
    }
}
