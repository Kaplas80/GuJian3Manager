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
            using var parser = new Parser(with => with.HelpWriter = null);
            ParserResult<object> parserResult = parser.ParseArguments<Options.Extract, Options.Create, Options.Info, Options.Decrypt>(args);
            parserResult
                .WithParsed<Options.Extract>(Extract)
                .WithParsed<Options.Create>(Create)
                .WithParsed<Options.Info>(ShowInfo)
                .WithParsed<Options.Decrypt>(Decrypt);
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }
    }
}
