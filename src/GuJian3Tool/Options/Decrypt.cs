// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJian3Tool.Options
{
    using CommandLine;

    /// <summary>
    /// GuJian 3 data decryption options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("decrypt", HelpText = "Decrypts GuJian 3 .xxx files.")]
    internal class Decrypt
    {
        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(0, MetaName = "encrypted_file", Required = true, HelpText = "GuJian 3 xxx file path.")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Value(1, MetaName = "decrypted_file", Required = true, HelpText = "Decrypted file.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the file decryption key.
        /// </summary>
        [Option('k', "key", HelpText = "Decryption key.")]
        public string Key { get; set; }
    }
}
