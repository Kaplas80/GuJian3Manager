// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
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

            IDictionary<string, List<string>> fileNames = LoadFileNames(opts.IndexPath);

            Directory.CreateDirectory(opts.OutputDirectory);

            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            foreach (string dataFile in Directory.GetFiles(dataDirectory, "data???"))
            {
                Console.Write($"Loading '{dataFile}' (this may take a while)... ");
                using Node archive = NodeFactory.FromFile(dataFile);
                archive.TransformWith<GuJian3Library.Converters.Data.Reader>();
                Console.WriteLine("DONE!");

                Extract(archive, opts.OutputDirectory, fileNames);
            }
        }

        private static void Extract(Node root, string outputFolder, IDictionary<string, List<string>> fileNames, bool extractAll = false)
        {
            foreach (Node node in Navigator.IterateNodes(root))
            {
                List<string> files = new ();

                if (fileNames == null)
                {
                    files.Add(node.Name);
                }
                else if (!fileNames.ContainsKey(node.Name))
                {
                    if (!extractAll)
                    {
                        continue;
                    }

                    files.Add(node.Name);
                }
                else
                {
                    files.AddRange(fileNames[node.Name]);
                }

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

        private static IDictionary<string, List<string>> LoadFileNames(string idxPath)
        {
            Console.Write("Loading index file... ");
            using Node index = NodeFactory.FromFile(idxPath);
            index.TransformWith<GuJian3Library.Converters.Oodle.Decompress>();
            index.TransformWith<GuJian3Library.Converters.Index.Reader>();
            IDictionary<string, List<string>> result = index.GetFormatAs<IndexFile>().Dictionary;
            Console.WriteLine("DONE!");

            return result;
        }
    }
}
