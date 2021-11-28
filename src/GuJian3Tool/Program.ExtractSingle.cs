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
        private static void ExtractSingle(Options.ExtractSingle opts)
        {
            WriteHeader();

            if (!File.Exists(opts.Path))
            {
                Console.WriteLine($"ERROR: \"{opts.Path}\" not found!!!!");
                return;
            }

            if (!string.IsNullOrEmpty(opts.IndexPath) && !File.Exists(opts.IndexPath))
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

            IDictionary<string, List<string>> fileNames = !string.IsNullOrEmpty(opts.IndexPath) ? LoadFileNames(opts.IndexPath) : null;

            Directory.CreateDirectory(opts.OutputDirectory);

            Console.Write($"Loading '{opts.Path}' (this may take a while)... ");
            using Node archive = NodeFactory.FromFile(opts.Path);
            archive.TransformWith<GuJian3Library.Converters.Data.Reader>();
            Console.WriteLine("DONE!");

            Extract(archive, opts.OutputDirectory, fileNames, true);
        }
    }
}
