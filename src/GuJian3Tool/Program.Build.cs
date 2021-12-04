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
    using System.Security.Cryptography;
    using System.Text;
    using GuJian3Library.Converters.Oodle;
    using GuJian3Library.Formats;
    using Yarhl.FileSystem;
    using Yarhl.IO;

    /// <summary>
    /// Build contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Build(Options.Build opts)
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

            if (!Directory.Exists(opts.InputDirectory))
            {
                Console.WriteLine($"ERROR: \"{opts.InputDirectory}\" not found!!!!");
                return;
            }

            if (!File.Exists(opts.IndexPath))
            {
                Console.WriteLine($"ERROR: \"{opts.IndexPath}\" not found!!!!");
                return;
            }

            if (!File.Exists(opts.IndexPath + ".backup"))
            {
                Console.Write($"Making backup of '{opts.IndexPath}'... ");
                File.Copy(opts.IndexPath, opts.IndexPath + ".backup");
                Console.WriteLine("DONE!");
            }
            else
            {
                Console.Write($"Backup of '{opts.IndexPath}' already exists. Skipping. ");
            }

            IndexFile index = LoadFileNames(opts.IndexPath);

            string inputDirectory = Path.GetFullPath(opts.InputDirectory);
            string dataDirectory = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(opts.IndexPath), ".."));
            string[] dataFiles = Directory.GetFiles(dataDirectory, "data???");

            Array.Sort(dataFiles);

            string newDataFile = Path.Combine(dataDirectory, GetNewDataFile(dataFiles));

            using Node container = NodeFactory.CreateContainer("new_files");
            string[] newFiles = Directory.GetFiles(inputDirectory, "*.*", SearchOption.AllDirectories);

            foreach (string file in newFiles)
            {
                Console.WriteLine($"Processing '{file}'...");
                string relativePath = file.Substring(inputDirectory.Length + 1).Replace('\\', '/');

                DataStream stream = DataStreamFactory.FromFile(file, FileOpenMode.Read);

                // 1. Calculate SHA-1
                Console.Write("\tCalculating SHA-1... ");
                (byte[] sha1, string sha1String) = CalculateSHA1(stream);
                Console.WriteLine("DONE!");

                Node n = NodeFactory.FromSubstream(sha1String, stream, 0, stream.Length);

                // 2. Search and update index
                if (index.Names.ContainsKey(relativePath))
                {
                    // The file is in the index
                    // There are 2 options:
                    //  - The hash is the same => Skip the file
                    //  - The hash has changed => Invalidate the old hash and add the file to the new dataXXX
                    Console.Write("\tFile found in index: ");

                    string oldHash = index.Names[relativePath];
                    if (oldHash == sha1String)
                    {
                        Console.WriteLine("Skipping");
                        continue;
                    }

                    index.Names[relativePath] = sha1String;
                    index.Hashes[oldHash].Remove(relativePath);

                    container.Add(n);
                    Console.WriteLine("Updated!");
                }
                else
                {
                    // The file is not in the index
                    // There are 2 options:
                    //  - The hash exists for another file => Add the file to the index, but not to the dataXXX
                    //  - The hash is unique => Add the file to the index and to the dataXXX
                    Console.Write("\tNew file: ");
                    if (!index.Hashes.ContainsKey(sha1String))
                    {
                        index.Hashes[sha1String] = new ();
                        container.Add(n);
                    }

                    index.Names[relativePath] = sha1String;
                    index.Hashes[sha1String].Add(relativePath);

                    Console.WriteLine("Added!");
                }
            }

            CompressorParameters parameters = new ();
            parameters.Compressor = OodleWrapper.OodleLZ_Compressor.OodleLZ_Compressor_Kraken;
            parameters.CompressionLevel = OodleWrapper.OodleLZ_CompressionLevel.OodleLZ_CompressionLevel_Optimal3;

            // 3. Write the data file
            if (container.Children.Count > 0)
            {
                Console.Write($"Compressing files... ");
                foreach (Node n in container.Children)
                {
                    n.TransformWith<GuJian3Library.Converters.Oodle.Compress, GuJian3Library.Converters.Oodle.CompressorParameters>(parameters);
                    n.TransformWith<GuJian3Library.Converters.Oodle.Writer>();
                }

                Console.Write($"Creating '{newDataFile}'... ");
                container.TransformWith<GuJian3Library.Converters.Data.Writer>();
                container.Stream.WriteTo(newDataFile);
                Console.WriteLine("DONE!");
            }

            // 4. Update the index
            Console.Write($"Updating '{opts.IndexPath}'... ");
            BinaryFormat newIndex = (BinaryFormat)Yarhl.FileFormat.ConvertFormat.With<GuJian3Library.Converters.Index.Writer>(index);
            OodleFile compressedIndex = (OodleFile)Yarhl.FileFormat.ConvertFormat.With<GuJian3Library.Converters.Oodle.Compress, GuJian3Library.Converters.Oodle.CompressorParameters>(parameters, newIndex);
            BinaryFormat finalIndex = (BinaryFormat)Yarhl.FileFormat.ConvertFormat.With<GuJian3Library.Converters.Oodle.Writer>(compressedIndex);
            File.Delete(opts.IndexPath);
            finalIndex.Stream.WriteTo(opts.IndexPath);
            Console.WriteLine("DONE!");
        }

        private static string GetNewDataFile(string[] dataFiles)
        {
            string lastDataFile = dataFiles[dataFiles.Length - 1];

            int index = int.Parse(Path.GetFileNameWithoutExtension(lastDataFile).Substring(4));

            return $"data{index + 1:000}";
        }

        private static (byte[], string) CalculateSHA1(Stream stream)
        {
            stream.Position = 0;

            HashAlgorithm hasher = SHA1.Create();
            hasher.Initialize();

            byte[] result = hasher.ComputeHash(stream);

            hasher.Dispose();

            // Merge all bytes into a string of bytes
            StringBuilder builder = new ();
            for (int i = 0; i < result.Length; i++)
            {
                builder.Append(result[i].ToString("x2"));
            }

            return (result, builder.ToString());
        }
    }
}
