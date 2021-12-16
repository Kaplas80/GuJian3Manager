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
    using GuJian3Library.Formats;
    using Yarhl.FileFormat;
    using Yarhl.IO;

    /// <summary>
    /// Converter from BinaryFormat (exe section dump) to GameDataFormat.
    /// </summary>
    public class Reader : IConverter<BinaryFormat, GameDataFormat>, IInitializer<Dictionary<string, string>>
    {
        private readonly Dictionary<string, string> _strings = new ();

        private Dictionary<string, string> _newStrings = new ();

        /// <summary>
        /// Initialize the string replacements.
        /// </summary>
        /// <param name="parameters">The string replacements.</param>
        public void Initialize(Dictionary<string, string> parameters)
        {
            _newStrings = parameters;
        }

        /// <summary>
        /// Reads a GuJian3 game data dump.
        /// </summary>
        /// <param name="source">The file in BinaryFormat.</param>
        /// <returns>The game data structure.</returns>
        public GameDataFormat Convert(BinaryFormat source)
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

            return new GameDataFormat
            {
                Magic = magic,
                Data = (Dictionary<object, object>)ReadData(reader, string.Empty, false),
                Strings = _strings,
            };
        }

        private object ReadData(DataReader reader, string path, bool isKey)
        {
            byte typeId = reader.ReadByte();
            return ReadData(reader, typeId, path, isKey);
        }

        private object ReadData(DataReader reader, byte typeId, string path, bool isKey)
        {
            switch (typeId)
            {
                case 0x00: // nil???
                    return null;

                case 0x01: // true
                    return true;

                case 0x02: // false
                    return false;

                case 0x03: // byte
                    {
                        byte value = reader.ReadByte();
                        return $"b:{value}";
                    }

                case 0x04: // short
                    {
                        short value = reader.ReadInt16();
                        return $"s:{value}";
                    }

                case 0x05: // int
                    {
                        int value = reader.ReadInt32();
                        return $"i:{value}";
                    }

                case 0x06: // long
                    {
                        long value = reader.ReadInt64();
                        return $"l:{value}";
                    }

                case 0x07: // double
                    return reader.ReadDouble();

                case 0x08: // string (byte len)
                    {
                        int strLen = reader.ReadByte();
                        string value = reader.ReadString(strLen);

                        if (!isKey)
                        {
                            if (_strings.ContainsKey(path))
                            {
                                throw new FormatException($"Duplicated string path: {path}");
                            }

                            _strings[path] = value;

                            if (_newStrings.ContainsKey(path) && !string.IsNullOrEmpty(_newStrings[path]))
                            {
                                value = _newStrings[path];
                            }
                        }

                        return value;
                    }

                case 0x09: // string (ushort len)
                    {
                        int strLen = reader.ReadUInt16();
                        string value = reader.ReadString(strLen);

                        if (!isKey)
                        {
                            if (_strings.ContainsKey(path))
                            {
                                throw new FormatException($"Duplicated string path: {path}");
                            }

                            _strings[path] = value;

                            if (_newStrings.ContainsKey(path) && !string.IsNullOrEmpty(_newStrings[path]))
                            {
                                value = _newStrings[path];
                            }
                        }

                        return value;
                    }

                case 0x0B: // Object Map
                    {
                        Dictionary<object, object> result = new ();

                        int childCount = reader.ReadInt32();

                        object[] children = new object[childCount];
                        for (int i = 0; i < childCount; i++)
                        {
                            children[i] = ReadData(reader, string.Concat(path, "/", i), false);
                        }

                        result["children"] = children;

                        byte id = reader.ReadByte();
                        while (id != 0)
                        {
                            object key = ReadData(reader, id, path, true);
                            id = reader.ReadByte();
                            string newPath = string.Concat(path, "/", key);
                            object value = ReadData(reader, id, newPath, false);
                            result[key] = value;

                            id = reader.ReadByte();
                        }

                        return result;
                    }

                case 0x0D: // Function??
                    {
                        TupleGameData result = new ();
                        result.Value1 = ReadData(reader, path, false);
                        result.Value2 = ReadData(reader, path, false);

                        return result;
                    }

                case 0x0F: // tref ushort
                    {
                        TRefGameData result = new ();
                        result.Value = reader.ReadUInt16();

                        return result;
                    }

                case 0x10: // tref uint
                    {
                        TRefGameData result = new ();
                        result.Value = reader.ReadUInt32();

                        return result;
                    }

                default:
                    throw new NotImplementedException($"Unknown type {typeId} at 0x{reader.Stream.Position - 1:X8}");
            }
        }
    }
}
