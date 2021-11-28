// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Yarhl.FileSystem;

    /// <summary>
    /// Show content info functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void ShowInfo(Options.Info opts)
        {
            WriteHeader();

            if (!File.Exists(opts.IndexPath))
            {
                Console.WriteLine($"ERROR: \"{opts.IndexPath}\" not found!!!!");
                return;
            }

            IDictionary<string, List<string>> fileNames = LoadFileNames(opts.IndexPath);

            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            string[] dataFiles = Directory.GetFiles(dataDirectory, "data???");

            foreach (string dataFile in dataFiles)
            {
                using Node archive = NodeFactory.FromFile(dataFile);
                archive.TransformWith<GuJian3Library.Converters.Data.Reader>();

                ShowInfo(archive, fileNames);
            }
        }

        private static void ShowInfo(Node root, IDictionary<string, List<string>> fileNames)
        {
            foreach (Node node in Navigator.IterateNodes(root))
            {
                if (!fileNames.ContainsKey(node.Name))
                {
                    continue;
                }

                List<string> files = fileNames[node.Name];

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
