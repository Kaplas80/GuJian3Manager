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

    /// <summary>
    /// Extract contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Extract(Options.Extract opts)
        {
            WriteHeader();

            if (!HasOodleDll())
            {
                Console.WriteLine("ERROR: \"oo2core_6_win64.dll\" not found!!!!");
                return;
            }

            if (!IsWindows())
            {
                Console.WriteLine("ERROR: This option only works in Windows");
                return;
            }

            if (!File.Exists(opts.IndexPath))
            {
                Console.WriteLine($"ERROR: \"{opts.IndexPath}\" not found!!!!");
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

            IndexFile index = LoadFileNames(opts.IndexPath);

            Directory.CreateDirectory(opts.OutputDirectory);

            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            foreach (string dataFile in Directory.GetFiles(dataDirectory, "data???"))
            {
                Console.Write($"Loading '{dataFile}' (this may take a while)... ");
                using Node archive = NodeFactory.FromFile(dataFile);
                archive.TransformWith<GuJian3Library.Converters.Data.Reader>();
                Console.WriteLine("DONE!");

                Extract(archive, opts.OutputDirectory, index);
            }
        }

        private static void Extract(Node root, string outputFolder, IndexFile index, bool extractAll = false)
        {
            foreach (Node node in Navigator.IterateNodes(root))
            {
                List<string> files = new ();

                if (index == null)
                {
                    files.Add(node.Name);
                }
                else if (!index.Hashes.ContainsKey(node.Name))
                {
                    if (!extractAll)
                    {
                        continue;
                    }

                    files.Add(node.Name);
                }
                else
                {
                    files.AddRange(index.Hashes[node.Name]);
                }

                node.TransformWith<GuJian3Library.Converters.Oodle.Reader>();
                node.TransformWith<GuJian3Library.Converters.Oodle.Decompress>();

                foreach (string file in files)
                {
                    Console.Write($"Extracting {file}... ");
                    string outputPath = Path.Join(outputFolder, file);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    node.Stream.WriteTo(outputPath);
                    Console.WriteLine("DONE!");
                }

                node.Dispose();
            }
        }
    }
}
