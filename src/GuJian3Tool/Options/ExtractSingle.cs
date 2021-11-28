// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool.Options
{
    using CommandLine;

    /// <summary>
    /// GuJian 3 data archive extract options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("extract-single", HelpText = "Extract contents from a single GuJian 3 data file.")]
    internal class ExtractSingle
    {
        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(0, MetaName = "data file", Required = true, HelpText = "GuJian 3 data file path.")]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Value(1, MetaName = "path_to_extract\\", Required = true, HelpText = "Output directory.")]
        public string OutputDirectory { get; set; }

        /// <summary>
        /// Gets or sets the 303.idx path.
        /// </summary>
        [Option("index", Required = false, HelpText = "Path to 303.idx (optional)")]
        public string IndexPath { get; set; }
    }
}
