// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace Gujian3Tool
{
    using System;
    using CommandLine;
    using CommandLine.Text;

    /// <summary>
    /// Main program.
    /// </summary>
    internal static partial class Program
    {
        private static void Main(string[] args)
        {
            using var parser = new Parser(with => with.HelpWriter = null);
            ParserResult<object> parserResult = parser.ParseArguments<Options.Extract, Options.Create>(args);
            parserResult
                .WithParsed<Options.Extract>(Extract)
                .WithParsed<Options.Create>(Create);
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }
    }
}
