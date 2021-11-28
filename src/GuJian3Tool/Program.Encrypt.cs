// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using GuJian3Library.Converters.XXTEA;
    using Yarhl.FileSystem;

    /// <summary>
    /// Encrypt contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void Encrypt(Options.Encrypt opts)
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
            }

            string encryptionKey = string.Empty;
            if (!string.IsNullOrEmpty(opts.Key))
            {
                encryptionKey = opts.Key;
            }
            else
            {
                KeyValuePair<string, string> kvp = Keys.FirstOrDefault(x => opts.InputFile.EndsWith(x.Key, StringComparison.InvariantCultureIgnoreCase));
                if (!kvp.Equals(default(KeyValuePair<string, string>)))
                {
                    encryptionKey = kvp.Value;
                }
            }

            if (string.IsNullOrEmpty(encryptionKey))
            {
                Console.WriteLine("ERROR: Encryption key not found!!");
                return;
            }

            Console.WriteLine("Encrypting...");

            using Node file = NodeFactory.FromFile(opts.InputFile);
            file.TransformWith<Encrypt, string>(encryptionKey);
            file.Stream.WriteTo(opts.OutputFile);
            Console.WriteLine(" DONE!");
        }
    }
}
