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
    using System.Diagnostics;
    using System.Dynamic;
    using System.Globalization;
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

            var reader = new DataReader(source.Stream)
            {
                Endianness = EndiannessMode.LittleEndian,
            };

            uint magic = reader.ReadUInt32();

            if (magic != 0xF2060881 && magic != 0xD5B50981)
            {
                throw new FormatException("Bad magic Id.");
            }

            var result = new JsonString();
            result.Value = ReadData(reader);

            return result;
        }

        private dynamic ReadData(DataReader reader)
        {
            byte typeId = reader.ReadByte();
            return ReadData(reader, typeId);
        }

        private dynamic ReadData(DataReader reader, byte typeId)
        {
            dynamic result = new ExpandoObject();

            result.type = typeId;

            switch (typeId)
            {
                case 0x00: // nil???
                    break;

                case 0x01: // true
                    {
                        result.data = true;
                    }

                    break;

                case 0x02: // false
                    {
                        result.data = false;
                    }

                    break;

                case 0x03: // byte
                    {
                        result.data = reader.ReadByte();
                    }

                    break;

                case 0x04: // short
                    {
                        result.data = reader.ReadInt16();
                    }

                    break;

                case 0x05: // int
                    {
                        result.data = reader.ReadInt32();
                    }

                    break;

                case 0x06: // long
                    {
                        result.data = reader.ReadInt64();
                    }

                    break;

                case 0x07: // double
                    {
                        result.data = reader.ReadDouble();
                    }

                    break;

                case 0x08: // string (byte len)
                    {
                        int strLen = reader.ReadByte();
                        result.data = reader.ReadString(strLen);
                    }

                    break;

                case 0x09: // string (ushort len)
                    {
                        int strLen = reader.ReadUInt16();
                        result.data = reader.ReadString(strLen);
                    }

                    break;

                case 0x0B: // Object Map
                    {
                        int refsLen = reader.ReadInt32();

                        var refs = new dynamic[refsLen];
                        if (refsLen > 0)
                        {
                            for (int i = 0; i < refsLen; i++)
                            {
                                refs[i] = ReadData(reader);
                            }
                        }

                        result.refs = refs;
                        result.data = new List<dynamic>();

                        byte id = reader.ReadByte();
                        while (id != 0)
                        {
                            dynamic pair = new ExpandoObject();

                            pair.key = ReadData(reader, id);

                            id = reader.ReadByte();
                            pair.value = ReadData(reader, id);

                            result.data.Add(pair);

                            id = reader.ReadByte();
                        }
                    }

                    break;

                case 0x0D: // Function??
                    {
                        result.data = new ExpandoObject();

                        result.data.name = ReadData(reader);
                        result.data.parameters = ReadData(reader);
                    }

                    break;

                case 0x0F: // tref ushort
                    {
                        result.data = reader.ReadUInt16();
                    }

                    break;

                case 0x10: // tref uint
                    {
                        result.data = reader.ReadUInt32();
                    }

                    break;

                default:
                    throw new NotImplementedException($"Unknown type {typeId} at 0x{reader.Stream.Position - 1:X8}");
            }

            return result;
        }
    }
}
