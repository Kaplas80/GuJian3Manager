// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace Gujian3Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using Gujian3Library;
    using Gujian3Library.Converter;
    using Gujian3Library.Oodle;
    using Yarhl.FileSystem;

    /// <summary>
    /// Extract contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Extract(Options.Extract opts)
        {
            WriteHeader();

            if (!File.Exists(opts.ArchivePath))
            {
                Console.WriteLine($"ERROR: \"{opts.ArchivePath}\" not found!!!!");
                return;
            }

            if (Directory.Exists(opts.OutputDirectory))
            {
                Console.WriteLine("WARNING: Output directory already exists. Its contents may be overwritten.");
                Console.Write("Continue? (y/N) ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer) && answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            string idxPath = Path.Join(Path.GetDirectoryName(opts.ArchivePath), "_index", "303.idx");

            IDictionary<string, List<string>> fileNames;
            if (File.Exists(idxPath))
            {
                fileNames = LoadFileNames(idxPath);
            }
            else
            {
                Console.WriteLine($"WARNING: '{idxPath}' NOT FOUND. FILE HASHES WILL BE USED AS FILE NAMES.");
                fileNames = new Dictionary<string, List<string>>();
            }

            Directory.CreateDirectory(opts.OutputDirectory);

            Console.Write("Loading data file (this may take a while)...");
            using Node archive = NodeFactory.FromFile(opts.ArchivePath);
            archive.TransformWith<GujianArchiveReader>();
            Console.Write("DONE!");

            Extract(archive, opts.OutputDirectory, fileNames);
        }

        private static void Extract(Node root, string outputFolder, IDictionary<string, List<string>> fileNames)
        {
            int unknownFiles = 0;
            foreach (Node node in Navigator.IterateNodes(root))
            {
                List<string> files;
                if (fileNames.ContainsKey(node.Name))
                {
                    files = fileNames[node.Name];
                }
                else
                {
                    files = new List<string>
                    {
                        $"unknown_{unknownFiles:0000}",
                    };
                    unknownFiles++;
                }

                node.TransformWith<Decompressor>();

                foreach (string file in files)
                {
                    Console.Write($"Extracting {file}... ");

                    string outputPath = Path.Join(outputFolder, file);
                    Directory.CreateDirectory(Path.GetDirectoryName(outputPath));

                    node.Stream.WriteTo(outputPath);
                    Console.WriteLine("DONE!");
                }
            }
        }

        private static IDictionary<string, List<string>> LoadFileNames(string idxPath)
        {
            Console.Write("Loading index file...");
            using Node index = NodeFactory.FromFile(idxPath);
            index.TransformWith<Decompressor>().TransformWith<IndexFileReader>();
            IDictionary<string, List<string>> result = index.GetFormatAs<IndexFile>().Dictionary;
            Console.WriteLine("DONE!");

            return result;
        }
    }
}
