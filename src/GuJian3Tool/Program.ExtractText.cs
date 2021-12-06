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
    using Newtonsoft.Json;
    using Yarhl.FileSystem;

    /// <summary>
    /// Extract contents functionality.
    /// </summary>
    internal static partial class Program
    {
        private static void ExtractText(Options.ExtractText opts)
        {
            WriteHeader();

            if (!File.Exists(opts.Path))
            {
                Console.WriteLine($"ERROR: \"{opts.Path}\" not found!!!!");
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

            Node n = NodeFactory.FromFile(opts.Path);
            n.TransformWith<GuJian3Library.Converters.ExeSection.ToJsonString>();

            JsonString format = n.GetFormatAs<JsonString>();

            string obj = JsonConvert.SerializeObject(format.Value);
            File.WriteAllText(opts.Path + ".json", obj);
        }
    }
}
