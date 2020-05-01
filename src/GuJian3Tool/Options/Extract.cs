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
    [Verb("extract", HelpText = "Extract contents from GuJian 3 data directory.")]
    internal class Extract
    {
        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(0, MetaName = "idx file", Required = true, HelpText = "GuJian 3 idx file path.")]
        public string IndexPath { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Value(1, MetaName = "path_to_extract\\", Required = true, HelpText = "Output directory.")]
        public string OutputDirectory { get; set; }
    }
}
