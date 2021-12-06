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
                    string value = ReadData(reader);
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

        private string ReadData(DataReader reader)
        {
            byte typeId = reader.ReadByte();
            return ReadData(reader, typeId, false);
        }

        private string ReadData(DataReader reader, byte typeId, bool isKey)
        {
            var result = new StringBuilder();

            switch (typeId)
            {
                case 0x00: // nil???
                    {
                        result.Append("{}");
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
                        if (isKey)
                        {
                            result.Append('\"').Append(value).Append('\"');
                        }
                        else
                        {
                            result.Append(value);
                        }
                    }

                    break;

                case 0x04: // short
                    {
                        short value = reader.ReadInt16();
                        if (isKey)
                        {
                            result.Append('\"').Append(value).Append('\"');
                        }
                        else
                        {
                            result.Append(value);
                        }
                    }

                    break;

                case 0x05: // int
                    {
                        int value = reader.ReadInt32();
                        if (isKey)
                        {
                            result.Append('\"').Append(value).Append('\"');
                        }
                        else
                        {
                            result.Append(value);
                        }
                    }

                    break;

                case 0x06: // long
                    {
                        long value = reader.ReadInt64();
                        if (isKey)
                        {
                            result.Append('\"').Append(value).Append('\"');
                        }
                        else
                        {
                            result.Append(value);
                        }
                    }

                    break;

                case 0x07: // double
                    {
                        double value = reader.ReadDouble();
                        if (isKey)
                        {
                            result.Append('\"').Append(value.ToString(CultureInfo.InvariantCulture)).Append('\"');
                        }
                        else
                        {
                            result.Append(value.ToString(CultureInfo.InvariantCulture));
                        }
                    }

                    break;

                case 0x08: // string (byte len)
                    {
                        int strLen = reader.ReadByte();
                        string str = reader.ReadString(strLen);
                        result.Append('\"').Append(Escape(str)).Append('\"');
                    }

                    break;

                case 0x09: // string (ushort len)
                    {
                        int strLen = reader.ReadUInt16();
                        string str = reader.ReadString(strLen);
                        result.Append('\"').Append(Escape(str)).Append('\"');
                    }

                    break;

                case 0x0B: // Object Map
                    {
                        int refsLen = reader.ReadInt32();

                        var refs = new StringBuilder();
                        refs.Append("\"refs\": [");
                        if (refsLen > 0)
                        {
                            refs.Append(ReadData(reader));
                            for (int i = 1; i < refsLen; i++)
                            {
                                refs.Append(", ").Append(ReadData(reader));
                            }
                        }

                        refs.Append(']');

                        byte id = reader.ReadByte();
                        result.Append('{');
                        result.Append(refs);

                        while (id != 0)
                        {
                            string key = ReadData(reader, id, true);
                            id = reader.ReadByte();
                            string value = ReadData(reader, id, false);

                            result.Append(',');
                            result.Append(key);
                            result.Append(": ");
                            result.Append(value);

                            id = reader.ReadByte();
                        }

                        result.Append('}');
                    }

                    break;

                case 0x0D: // Function??
                    {
                        string name = ReadData(reader);
                        string parameters = ReadData(reader);

                        result.Append('{');
                        result.Append("\"name\": ");
                        result.Append(name);
                        result.Append(',');
                        result.Append("\"parameters\": ");
                        result.Append(parameters);
                        result.Append('}');
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

        private static string Escape(string original)
        {
            string result = original;
            result = result.Replace("\\", "\\\\");
            result = result.Replace("\n", "\\n");
            result = result.Replace("\"", "\\\"");
            return result;
        }
    }
}
