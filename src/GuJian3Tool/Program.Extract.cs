// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using GuJian3Library;
    using GuJian3Library.Converter;
    using GuJian3Library.Oodle;
    using Yarhl.FileSystem;

    /// <summary>
    /// Extract contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Extract(Options.Extract opts)
        {
            WriteHeader();

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
                if (!string.IsNullOrEmpty(answer) && answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            IDictionary<string, List<string>> fileNames = LoadFileNames(opts.IndexPath);

            Directory.CreateDirectory(opts.OutputDirectory);

            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            string[] dataFiles = Directory.GetFiles(dataDirectory, "data???");

            foreach (string dataFile in dataFiles)
            {
                Console.Write($"Loading '{dataFile}' (this may take a while)... ");
                using Node archive = NodeFactory.FromFile(dataFile);
                archive.TransformWith<GuJianArchiveReader>();
                Console.WriteLine("DONE!");

                Extract(archive, opts.OutputDirectory, fileNames);

                archive.Dispose();
            }
        }

        private static void Extract(Node root, string outputFolder, IDictionary<string, List<string>> fileNames)
        {
            foreach (Node node in Navigator.IterateNodes(root))
            {
                if (!fileNames.ContainsKey(node.Name))
                {
                    continue;
                }

                List<string> files = fileNames[node.Name];

                node.TransformWith<Decompressor>();

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

        private static IDictionary<string, List<string>> LoadFileNames(string idxPath)
        {
            Console.Write("Loading index file... ");
            using Node index = NodeFactory.FromFile(idxPath);
            index.TransformWith<Decompressor>().TransformWith<IndexFileReader>();
            IDictionary<string, List<string>> result = index.GetFormatAs<IndexFile>().Dictionary;
            Console.WriteLine("DONE!");

            return result;
        }
    }
}
