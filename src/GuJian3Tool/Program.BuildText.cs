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

namespace GuJian3Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GuJian3Library.Formats;
    using Yarhl.FileSystem;
    using Yarhl.IO;
    using Yarhl.Media.Text;

    /// <summary>
    /// Rebuild strings functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void BuildText(Options.BuildText opts)
        {
            WriteHeader();

            if (!File.Exists(opts.InputFile))
            {
                Console.WriteLine($"ERROR: \"{opts.InputFile}\" not found!!!!");
                return;
            }

            if (!Directory.Exists(opts.InputDirectory))
            {
                Console.WriteLine($"ERROR: \"{opts.InputDirectory}\" not found!!!!");
                return;
            }

            if (File.Exists(opts.OutputFile))
            {
                Console.WriteLine("WARNING: Output file already exists. It will be overwritten.");
                Console.Write("Continue? (y/N) ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer) && !string.Equals(answer, "Y", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            Dictionary<string, string> newStrings = new Dictionary<string, string>();

            Console.Write("Reading PO files...");
            using Node poNodes = NodeFactory.FromDirectory(opts.InputDirectory, "*.po", FileOpenMode.Read);
            foreach (Node poNode in poNodes.Children)
            {
                poNode.TransformWith<Binary2Po>();
                Po po = poNode.GetFormatAs<Po>();

                foreach (PoEntry poEntry in po.Entries)
                {
                    newStrings.Add(poEntry.Context, poEntry.Translated.Replace("\\\\", "\\"));
                }
            }

            Console.WriteLine(" DONE!");

            Console.Write("Reading original dump and replacing strings...");
            using Node n = NodeFactory.FromFile(opts.InputFile);
            n.TransformWith<GuJian3Library.Converters.ExeSection.Reader, Dictionary<string, string>>(newStrings);
            GameDataFormat format = n.GetFormatAs<GameDataFormat>();
            Console.WriteLine(" DONE!");

            Console.Write("Writing...");
            using BinaryFormat bin = (BinaryFormat)Yarhl.FileFormat.ConvertFormat.With<GuJian3Library.Converters.ExeSection.Writer>(format);
            bin.Stream.WriteTo(opts.OutputFile);
            Console.WriteLine(" DONE!");
        }
    }
}
