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
    using System.IO;
    using GuJian3Library.Formats;
    using Newtonsoft.Json;
    using Yarhl.FileSystem;

    /// <summary>
    /// Extract contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void ExtractJson(Options.ExtractJson opts)
        {
            WriteHeader();

            if (!File.Exists(opts.InputFile))
            {
                Console.WriteLine($"ERROR: \"{opts.InputFile}\" not found!!!!");
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

                File.Delete(opts.OutputFile);
            }

            Console.Write("Reading dump...");
            using Node n = NodeFactory.FromFile(opts.InputFile);
            n.TransformWith<GuJian3Library.Converters.ExeSection.Reader>();
            Console.WriteLine(" DONE!");

            GameDataFormat format = n.GetFormatAs<GameDataFormat>();
            JsonSerializer serializer = new ();
            serializer.Formatting = Formatting.Indented;
            serializer.TypeNameHandling = TypeNameHandling.Auto;

            Console.Write("Writing JSON...");
            using StreamWriter file = File.CreateText(opts.OutputFile);
            serializer.Serialize(file, format);
            Console.WriteLine(" DONE!");
        }
    }
}
