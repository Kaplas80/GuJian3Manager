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
    using System.Diagnostics;
    using System.Text;
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat to IndexFile.
    /// </summary>
    public class ToJsonString : IConverter<BinaryFormat, JsonString>
    {
        /// <summary>
        /// Reads a GuJian3 index file.
        /// </summary>
        /// <param name="source">The file in BinaryFormat.</param>
        /// <returns>The file.</returns>
        public JsonString Convert(BinaryFormat source)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            source.Stream.Position = 0;

            var sb = new StringBuilder();

            var reader = new DataReader(source.Stream)
            {
                Endianness = EndiannessMode.LittleEndian,
            };

            uint magic = reader.ReadUInt32();

            if (magic != 0xF2060881 && magic != 0xD5B50981)
            {
                throw new FormatException("Bad magic Id.");
            }

            while (!reader.Stream.EndOfStream)
            {
                try
                {
                    string value = ReadData(reader, 0);
                    sb.Append(value);
                }
                catch (NotImplementedException)
                {
                    Debug.WriteLine(sb.ToString());
                    throw;
                }
            }

            var result = new JsonString();
            result.Value = sb.ToString();
            return result;
        }

        private string ReadData(DataReader reader, int indent)
        {
            byte typeId = reader.ReadByte();
            return ReadData(reader, typeId, indent);
        }

        private string ReadData(DataReader reader, byte typeId, int indent)
        {
            var result = new StringBuilder();
            string spacesPrev = new string(' ', indent * 2);
            string spaces = new string(' ', (indent + 1) * 2);

            switch (typeId)
            {
                case 0x00: // nil???
                    {
                        result.Append("nil");
                    }

                    break;

                case 0x01: // true
                    {
                        result.Append("true");
                    }

                    break;

                case 0x02: // false
                    {
                        result.Append("false");
                    }

                    break;

                case 0x03: // byte
                    {
                        byte value = reader.ReadByte();
                        result.Append(value);
                    }

                    break;

                case 0x04: // short
                    {
                        short value = reader.ReadInt16();
                        result.Append(value);
                    }

                    break;

                case 0x05: // int
                    {
                        int value = reader.ReadInt32();
                        result.Append(value);
                    }

                    break;

                case 0x06: // long
                    {
                        long value = reader.ReadInt64();
                        result.Append(value);
                    }

                    break;

                case 0x07: // double
                    {
                        double value = reader.ReadDouble();
                        result.Append(value);
                    }

                    break;

                case 0x08: // string (byte len)
                    {
                        int strLen = reader.ReadByte();
                        string str = reader.ReadString(strLen);
                        result.Append($"\"{str.Replace("\n", "\\n")}\"");
                    }

                    break;

                case 0x09: // string (ushort len)
                    {
                        int strLen = reader.ReadUInt16();
                        string str = reader.ReadString(strLen);
                        result.Append($"\"{str.Replace("\n", "\\n")}\"");
                    }

                    break;

                case 0x0B: // Object Map
                    {
                        int arrayLen = reader.ReadInt32();

                        if (arrayLen > 0)
                        {
                            result.Append("[ ");
                            for (int i = 0; i < arrayLen - 1; i++)
                            {
                                byte id2 = reader.ReadByte();
                                result.Append(ReadData(reader, id2, indent));
                                if (id2 != 0x0B)
                                {
                                    result.Append($", ");
                                }
                            }

                            result.Append(ReadData(reader, indent));
                            result.Append(" ]");
                        }

                        byte id = reader.ReadByte();
                        result.Append("{");
                        int count = 0;

                        while (id != 0)
                        {
                            result.Append($"\n{spaces}");
                            string key = ReadData(reader, id, indent + 1);
                            result.Append(key);
                            result.Append(" : ");
                            id = reader.ReadByte();
                            string value = ReadData(reader, id, indent + 1);
                            result.Append(value);

                            if (id != 0x0B)
                            {
                                result.Append($", ");
                            }

                            id = reader.ReadByte();
                            count++;
                        }

                        if (count > 0)
                        {
                            result.Append($"\n{spacesPrev}");
                        }

                        result.Append($"}}, ");
                    }

                    break;

                case 0x0D: // Function??
                    {
                        string name = ReadData(reader, indent);
                        result.Append(name);
                        result.Append("(");
                        string parameters = ReadData(reader, indent);
                        result.Append(parameters);
                        result.Append("),\n");
                    }

                    break;

                case 0x0F: // tref ushort
                    {
                        ushort value = reader.ReadUInt16();
                        result.Append(value);
                    }

                    break;

                case 0x10: // tref uint
                    {
                        uint value = reader.ReadUInt32();
                        result.Append(value);
                    }

                    break;

                default:
                    throw new NotImplementedException($"Unknown type {typeId} at 0x{reader.Stream.Position - 1:X8}");
            }

            return result.ToString();
        }
    }
}
