// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Library.Converters.Data
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat to NodeContainer.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, NodeContainerFormat>
    {
        /// <summary>
        /// Max number of files in the archive.
        /// </summary>
        private const int MaxFiles = 0x1000;

        /// <summary>
        /// Ruads a GuJian3 data file.
        /// </summary>
        /// <param name="source">The GuJian3 data file.</param>
        /// <returns>The NodeContainerFormat.</returns>
        public NodeContainerFormat Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var result = new NodeContainerFormat();

            var reader = new DataReader(source.Stream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            string magicId = reader.ReadString(4);

            if (magicId != "ZFS\0")
            {
                throw new FormatException("ZFS: Bad magic Id.");
            }

            uint nextChunk;
            do
            {
                string chunkMagicId = reader.ReadString(4);

                if (chunkMagicId != "[IX]")
                {
                    throw new FormatException("ZFS: Bad magic chunk Id.");
                }

                nextChunk = reader.ReadUInt32();

                for (int i = 0; i < MaxFiles; i++)
                {
                    byte[] hash = reader.ReadBytes(20);
                    string hashString = ToHexString(hash);

                    uint fileOffset = reader.ReadUInt32();
                    uint fileCompressedSize = reader.ReadUInt32();

                    _ = reader.ReadUInt16(); // crc-16 of header (hash + offset + size)
                    _ = reader.ReadUInt16(); // unknown (always 0x0100)

                    if (fileOffset == 0)
                    {
                        continue;
                    }

                    var compressedFormat = new BinaryFormat(source.Stream, fileOffset, fileCompressedSize);

                    var node = new Node(hashString, compressedFormat);
                    result.Root.Add(node);
                }

                source.Stream.Seek(nextChunk, System.IO.SeekOrigin.Begin);
            }
            while (nextChunk != 0);

            return result;
        }

        private static string ToHexString(IReadOnlyCollection<byte> source)
        {
            // Merge all bytes into a string of bytes
            var builder = new StringBuilder(source.Count * 2);
            foreach (byte b in source)
            {
                builder.Append(b.ToString("x2", CultureInfo.InvariantCulture));
            }

            return builder.ToString();
        }
    }
}
