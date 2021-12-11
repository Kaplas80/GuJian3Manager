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

namespace GuJian3Library.Converters.ExeSection
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter from GameDataFormat to BinaryFormat.
    /// </summary>
    public class Writer : IConverter<GameDataFormat, BinaryFormat>
    {
        /// <summary>
        /// Write a GuJian 3 game data dump.
        /// </summary>
        /// <param name="source">The game data.</param>
        /// <returns>The binary file.</returns>
        public BinaryFormat Convert(GameDataFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            var result = new BinaryFormat();

            var writer = new DataWriter(result.Stream)
            {
                DefaultEncoding = Encoding.UTF8,
                Endianness = EndiannessMode.LittleEndian,
            };

            writer.Write(source.Magic);
            WriteData(writer, source.Data);

            return result;
        }

        private void WriteData(DataWriter writer, object data)
        {
            if (data is bool boolData)
            {
                writer.Write(boolData ? (byte)0x01 : (byte)0x02);
            }
            else if (data is double doubleData)
            {
                writer.Write((byte)0x07);
                writer.Write(doubleData);
            }
            else if (data is string stringData)
            {
                string[] split = stringData.Split(':');
                if (split.Length == 2)
                {
                    switch (split[0])
                    {
                        case "b":
                            {
                                writer.Write((byte)0x03);
                                writer.Write(byte.Parse(split[1]));
                                return;
                            }

                        case "s":
                            {
                                writer.Write((byte)0x04);
                                writer.Write(short.Parse(split[1]));
                                return;
                            }

                        case "i":
                            {
                                writer.Write((byte)0x05);
                                writer.Write(int.Parse(split[1]));
                                return;
                            }

                        case "l":
                            {
                                writer.Write((byte)0x06);
                                writer.Write(long.Parse(split[1]));
                                return;
                            }
                    }
                }

                int strLen = Encoding.UTF8.GetByteCount(stringData);

                if (strLen < 256)
                {
                    writer.Write((byte)0x08);
                    writer.Write((byte)strLen);
                }
                else
                {
                    writer.Write((byte)0x09);
                    writer.Write((ushort)strLen);
                }

                writer.Write(stringData, false);
            }
            else if (data is Dictionary<object, object> tableData)
            {
                writer.Write((byte)0x0B);

                object[] children = (object[])tableData["children"];
                writer.Write(children.Length);

                for (int i = 0; i < children.Length; i++)
                {
                    WriteData(writer, children[i]);
                }

                foreach (KeyValuePair<object, object> kvp in tableData)
                {
                    if ((string)kvp.Key == "children")
                    {
                        continue;
                    }

                    WriteData(writer, kvp.Key);
                    WriteData(writer, kvp.Value);
                }

                writer.Write((byte)0x00);
            }
            else if (data is TupleGameData tupleData)
            {
                writer.Write((byte)0x0D);
                WriteData(writer, tupleData.Value1);
                WriteData(writer, tupleData.Value2);
            }
            else if (data is TRefGameData trefData)
            {
                if (trefData.Value <= ushort.MaxValue)
                {
                    writer.Write((byte)0x0F);
                    writer.Write((ushort)trefData.Value);
                }
                else
                {
                    writer.Write((byte)0x10);
                    writer.Write(trefData.Value);
                }
            }
            else
            {
                writer.Write((byte)0x00);
            }
        }
    }
}
