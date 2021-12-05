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

namespace GuJian3Library.Converters.Data
{
    using System;
    using System.Text;
    using Nito.HashAlgorithms;
    using Yarhl.FileFormat;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Converter from NodeContainer to BinaryFormat.
    /// </summary>
    public class Writer : IConverter<NodeContainerFormat, BinaryFormat>
    {
        /// <summary>
        /// Max number of files in the archive.
        /// </summary>
        private const int MaxFiles = 0x1000;

        /// <summary>
        /// Writes a GuJian3 data file.
        /// </summary>
        /// <param name="source">The GuJian3 data file.</param>
        /// <returns>The NodeContainerFormat.</returns>
        public BinaryFormat Convert(NodeContainerFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new BinaryFormat();

            var writer = new DataWriter(result.Stream)
            {
                DefaultEncoding = Encoding.ASCII,
                Endianness = EndiannessMode.LittleEndian,
            };

            writer.Write("ZFS");

            int remainingFiles = source.Root.Children.Count;
            int chunkCount = (int)Math.Ceiling(remainingFiles / (double)MaxFiles);
            int currentNode = 0;

            for (int i = 0; i < chunkCount; i++)
            {
                writer.Write("[IX]", false);

                long nextChunkValuePos = writer.Stream.Position;
                writer.Write(0); // the next chunk pos is unknown

                long chunkStartPos = writer.Stream.Position;
                writer.WriteTimes(0x00, 0x00020000);

                int fileOffset = (int)writer.Stream.Position;

                writer.Stream.Position = chunkStartPos;

                int fileCount = Math.Min(remainingFiles, MaxFiles);

                for (int j = 0; j < fileCount; j++)
                {
                    Node n = source.Root.Children[currentNode];

                    byte[] hash = ToByteArray(n.Name);
                    writer.Write(hash);

                    writer.Write(fileOffset);
                    writer.Write((uint)n.Stream.Length);
                    writer.Write(Crc16(hash, fileOffset, (uint)n.Stream.Length));
                    writer.Write((ushort)0x0100);

                    writer.Stream.PushToPosition(fileOffset);
                    n.Stream.WriteTo(writer.Stream);
                    fileOffset = (int)writer.Stream.Position;
                    writer.Stream.PopPosition();

                    currentNode++;
                }

                remainingFiles -= fileCount;
                writer.Stream.Position = nextChunkValuePos;

                if (remainingFiles != 0)
                {
                    writer.Write((uint)writer.Stream.Length);
                }

                writer.Stream.Seek(0, System.IO.SeekOrigin.End);
            }

            return result;
        }

        private static byte[] ToByteArray(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = System.Convert.ToByte(hex.Substring(i, 2), 16);
            }

            return bytes;
        }

        private static ushort Crc16(byte[] hash, int fileOffset, uint size)
        {
            byte[] data = new byte[28];
            using DataStream ds = DataStreamFactory.FromArray(data, 0, data.Length);
            DataWriter dw = new (ds)
            {
                Endianness = EndiannessMode.LittleEndian,
            };

            dw.Write(hash);
            dw.Write(fileOffset);
            dw.Write(size);

            CRC16 calculator = new ();
            byte[] result = calculator.ComputeHash(data, 0, data.Length);
            return BitConverter.ToUInt16(result, 0);
        }
    }
}
