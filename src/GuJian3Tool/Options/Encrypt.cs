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

namespace GuJian3Tool.Options
{
    using CommandLine;

    /// <summary>
    /// GuJian 3 data decryption options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    [Verb("encrypt", HelpText = "Encrypts GuJian 3 .xxx files.")]
    internal class Encrypt
    {
        /// <summary>
        /// Gets or sets the output directory.
        /// </summary>
        [Value(0, MetaName = "decrypted_file", Required = true, HelpText = "Decrypted file.")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the archive path.
        /// </summary>
        [Value(1, MetaName = "encrypted_file", Required = true, HelpText = "Encrypted file.")]
        public string OutputFile { get; set; }

        /// <summary>
        /// Gets or sets the file encryption key.
        /// </summary>
        [Option('k', "key", HelpText = "Encryption key.")]
        public string Key { get; set; }
    }
}
