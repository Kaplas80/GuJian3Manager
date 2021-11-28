// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool
{
    using System;
    using CommandLine;

    /// <summary>
    /// Main program.
    /// </summary>
    internal static partial class Program
    {
        private static void Main(string[] args)
        {
            ParserResult<object> parserResult = Parser.Default.ParseArguments<Options.Extract, Options.ExtractSingle, Options.Info, Options.Decrypt, Options.Encrypt>(args);
            parserResult
                .WithParsed<Options.Extract>(Extract)
                .WithParsed<Options.ExtractSingle>(ExtractSingle)
                .WithParsed<Options.Info>(ShowInfo)
                .WithParsed<Options.Decrypt>(Decrypt)
                .WithParsed<Options.Encrypt>(Encrypt);
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }
    }
}
