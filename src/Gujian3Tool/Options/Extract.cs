// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace Gujian3Tool.Options
{
    using CommandLine;

    /// <summary>
    /// Gujian 3 data archive extract options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("extract", HelpText = "Extract contents from a Gujian 3 data archive.")]
    internal class Extract
    {
        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(0, MetaName = "archive", Required = true, HelpText = "Gujian 3 data archive path.")]
        public string ArchivePath { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Value(1, MetaName = "path_to_extract\\", Required = true, HelpText = "Output directory.")]
        public string OutputDirectory { get; set; }
    }
}
