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
    [Verb("info", HelpText = "Shows GuJian 3 data info.")]
    internal class Info
    {
        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(0, MetaName = "idx file", Required = true, HelpText = "GuJian 3 idx file path.")]
        public string IndexPath { get; set; }
    }
}
