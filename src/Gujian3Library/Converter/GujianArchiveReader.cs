// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace Gujian3Library.Converter
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Globalization;
    using System.Text;
    using Gujian3Library.Oodle;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat to GujianArchive.
    /// </summary>
    public class GujianArchiveReader : IConverter<BinaryFormat, NodeContainerFormat>
    {
        /// <inheritdoc/>
        [SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Ownserhip dispose transferred")]
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

            int nextChunk;
            do
            {
                string chunkMagicId = reader.ReadString(4);

                if (chunkMagicId != "[IX]")
                {
                    throw new FormatException("ZFS: Bad magic chunk Id.");
                }

                nextChunk = reader.ReadInt32();

                for (int i = 0; i < 0x1000; i++)
                {
                    byte[] hash = reader.ReadBytes(20);
                    string hashString = ToHexString(hash);

                    int fileOffset = reader.ReadInt32();
                    int fileCompressedSize = reader.ReadInt32();
                    reader.ReadInt32(); // file time??

                    if (fileOffset == 0)
                    {
                        continue;
                    }

                    var compressedFormat = new BinaryFormat(source.Stream, fileOffset, fileCompressedSize);

                    var node = new Node(hashString, compressedFormat);
                    result.Root.Add(node);
                }

                source.Stream.Seek(nextChunk, SeekMode.Start);
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
