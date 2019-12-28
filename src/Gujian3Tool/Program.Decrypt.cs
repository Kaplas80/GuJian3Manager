// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool
{
    using System;
    using System.IO;
    using GuJian3Library.Converter;
    using Yarhl.FileSystem;

    /// <summary>
    /// Decrypt contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Decrypt(Options.Decrypt opts)
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
                if (!string.IsNullOrEmpty(answer) && answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            Console.Write("Decrypting...");
            using Node file = NodeFactory.FromFile(opts.InputFile);
            file.TransformWith<GuJianFileDecrypter, string>(opts.InputFile);
            file.Stream.WriteTo(opts.OutputFile);
            Console.WriteLine(" DONE!");

            file.Dispose();
        }
    }
}
