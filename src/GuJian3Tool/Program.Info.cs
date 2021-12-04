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

    /// <summary>
    /// Show content info functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void ShowInfo(Options.Info opts)
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

            IndexFile index = LoadFileNames(opts.IndexPath);

            BinaryFormat newIndex = (BinaryFormat)Yarhl.FileFormat.ConvertFormat.With<GuJian3Library.Converters.Index.Writer>(index);
            newIndex.Stream.WriteTo(opts.IndexPath + ".txt");

            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            string[] dataFiles = Directory.GetFiles(dataDirectory, "data???");

            foreach (string dataFile in dataFiles)
            {
                using Node archive = NodeFactory.FromFile(dataFile);
                archive.TransformWith<GuJian3Library.Converters.Data.Reader>();

                ShowInfo(archive, index);
            }
        }

        private static void ShowInfo(Node root, IndexFile index)
        {
            foreach (Node node in Navigator.IterateNodes(root))
            {
                if (!index.Hashes.ContainsKey(node.Name))
                {
                    continue;
                }

                List<string> files = index.Hashes[node.Name];

                long fileOffset = node.Stream.Offset;
                long fileSize = node.Stream.Length;

                foreach (string file in files)
                {
                    Console.WriteLine($"{file}\t{node.Name}\t{root.Name}\t0x{fileOffset:X8}\t{fileSize}");
                }

                node.Dispose();
            }
        }
    }
}
