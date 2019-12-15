// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace Gujian3Tool
{
    using System;
    using System.IO;
    using CommandLine;
    using CommandLine.Text;
    using Gujian3Tool.Options;

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
                .WithParsed<Options.Create>(Create)
                .WithNotParsed(x =>
                {
                    if (args.Length == 1)
                    {
                        if (File.Exists(args[0]))
                        {
                            var opts = new Options.Extract
                            {
                                ArchivePath = args[0],
                                OutputDirectory = string.Concat(args[0], ".unpack"),
                            };

                            Extract(opts);
                            return;
                        }

                        if (Directory.Exists(args[0]))
                        {
                            var opts = new Options.Create
                            {
                                InputDirectory = args[0],
                                ArchivePath =
                                    args[0].EndsWith(".unpack", StringComparison.InvariantCultureIgnoreCase)
                                        ? args[0].Substring(0, args[0].Length - 7)
                                        : string.Concat(args[0], ".par"),
                            };

                            Create(opts);
                            return;
                        }
                    }

                    HelpText helpText = HelpText.AutoBuild(
                        parserResult,
                        h =>
                        {
                            h.AutoHelp = false; // hide --help
                            h.AutoVersion = false; // hide --version
                            return HelpText.DefaultParsingErrorsHandler(parserResult, h);
                        },
                        e => e);

                    Console.WriteLine(helpText);
                });
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }
    }
}
