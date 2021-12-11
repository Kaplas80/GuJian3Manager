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
    /// Extract strings functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void ExtractText(Options.ExtractText opts)
        {
            WriteHeader();

            if (!File.Exists(opts.InputFile))
            {
                Console.WriteLine($"ERROR: \"{opts.InputFile}\" not found!!!!");
                return;
            }

            if (Directory.Exists(opts.OutputDirectory))
            {
                Console.WriteLine("WARNING: Output directory already exists. Its contents may be overwritten.");
                Console.Write("Continue? (y/N) ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer) && !string.Equals(answer, "Y", StringComparison.InvariantCultureIgnoreCase))
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            Console.Write("Reading dump...");
            using Node n = NodeFactory.FromFile(opts.InputFile);
            n.TransformWith<GuJian3Library.Converters.ExeSection.Reader>();
            Console.WriteLine(" DONE!");

            GameDataFormat format = n.GetFormatAs<GameDataFormat>();

            Console.Write("Writing POs...");
            PoHeader header = new ("GuJian 3", "none@dummy.com", "und");

            Dictionary<string, Po> poFiles = new ();

            foreach (KeyValuePair<object, object> kvp in format.Data)
            {
                if ((string)kvp.Key == "children")
                {
                    continue;
                }

                poFiles[(string)kvp.Key] = new Po(header);
            }

            foreach (KeyValuePair<string, string> kvp in format.Strings)
            {
                if (!kvp.Key.Contains("/EN"))
                {
                    continue;
                }

                if (string.IsNullOrWhiteSpace(kvp.Value))
                {
                    continue;
                }

                string[] parts = kvp.Key.Split('/');

                PoEntry poEntry = new ();
                poEntry.Context = kvp.Key;

                poEntry.Original = kvp.Value.Replace("\\", "\\\\");
                poEntry.Translated = kvp.Value.Replace("\\", "\\\\");

                poFiles[parts[1]].Add(poEntry);
            }

            foreach (KeyValuePair<string, Po> kvp in poFiles)
            {
                if (kvp.Value.Entries.Count > 0)
                {
                    using BinaryFormat bin = (BinaryFormat)Yarhl.FileFormat.ConvertFormat.With<Po2Binary>(kvp.Value);
                    bin.Stream.WriteTo(Path.Combine(opts.OutputDirectory, $"{kvp.Key}.po"));
                }
            }

            Console.WriteLine(" DONE!");
        }
    }
}
