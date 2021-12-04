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

namespace GuJian3Library.Converters.Oodle
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Oodle library wrapper.
    /// Code from https://github.com/JKAnderson/SoulsFormats/blob/master/SoulsFormats/Util/Oodle26.cs.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1602:Enumeration items should be documented", Justification = "Not my code.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Minor Code Smell", "S101:Types should be named in PascalCase", Justification = "Not my code.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S1144:Unused private types or members should be removed", Justification = "Not my code.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Not my code.")]
    public static class OodleWrapper
    {
        /// <summary>
        /// Oodle compression level.
        /// </summary>
        public enum OodleLZ_CompressionLevel
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            OodleLZ_CompressionLevel_None = 0,
            OodleLZ_CompressionLevel_SuperFast = 1,
            OodleLZ_CompressionLevel_VeryFast = 2,
            OodleLZ_CompressionLevel_Fast = 3,
            OodleLZ_CompressionLevel_Normal = 4,

            OodleLZ_CompressionLevel_Optimal1 = 5,
            OodleLZ_CompressionLevel_Optimal2 = 6,
            OodleLZ_CompressionLevel_Optimal3 = 7,
            OodleLZ_CompressionLevel_Optimal4 = 8,
            OodleLZ_CompressionLevel_Optimal5 = 9,

            OodleLZ_CompressionLevel_HyperFast1 = -1,
            OodleLZ_CompressionLevel_HyperFast2 = -2,
            OodleLZ_CompressionLevel_HyperFast3 = -3,
            OodleLZ_CompressionLevel_HyperFast4 = -4,

            OodleLZ_CompressionLevel_HyperFast = OodleLZ_CompressionLevel_HyperFast1,
            OodleLZ_CompressionLevel_Optimal = OodleLZ_CompressionLevel_Optimal2,
            OodleLZ_CompressionLevel_Max = OodleLZ_CompressionLevel_Optimal5,
            OodleLZ_CompressionLevel_Min = OodleLZ_CompressionLevel_HyperFast4,

            OodleLZ_CompressionLevel_Force32 = 0x40000000,
            OodleLZ_CompressionLevel_Invalid = OodleLZ_CompressionLevel_Force32,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        /// <summary>
        /// Oodle compressors.
        /// </summary>
        public enum OodleLZ_Compressor
        {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
            OodleLZ_Compressor_Invalid = -1,
            OodleLZ_Compressor_None = 3,

            OodleLZ_Compressor_Kraken = 8,
            OodleLZ_Compressor_Leviathan = 13,
            OodleLZ_Compressor_Mermaid = 9,
            OodleLZ_Compressor_Selkie = 11,
            OodleLZ_Compressor_Hydra = 12,

            OodleLZ_Compressor_BitKnit = 10,
            OodleLZ_Compressor_LZB16 = 4,
            OodleLZ_Compressor_LZNA = 7,
            OodleLZ_Compressor_LZH = 0,
            OodleLZ_Compressor_LZHLW = 1,
            OodleLZ_Compressor_LZNIB = 2,
            OodleLZ_Compressor_LZBLW = 5,
            OodleLZ_Compressor_LZA = 6,

            OodleLZ_Compressor_Count = 14,
            OodleLZ_Compressor_Force32 = 0x40000000,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
        }

        private enum OodleLZ_CheckCRC
        {
            OodleLZ_CheckCRC_No = 0,
            OodleLZ_CheckCRC_Yes = 1,
            OodleLZ_CheckCRC_Force32 = 0x40000000,
        }

        private enum OodleLZ_Decode_ThreadPhase
        {
            OodleLZ_Decode_ThreadPhase1 = 1,
            OodleLZ_Decode_ThreadPhase2 = 2,
            OodleLZ_Decode_ThreadPhaseAll = 3,
            OodleLZ_Decode_Unthreaded = OodleLZ_Decode_ThreadPhaseAll,
        }

        private enum OodleLZ_FuzzSafe
        {
            OodleLZ_FuzzSafe_No = 0,
            OodleLZ_FuzzSafe_Yes = 1,
        }

        private enum OodleLZ_Profile
        {
            OodleLZ_Profile_Main = 0,
            OodleLZ_Profile_Reduced = 1,
            OodleLZ_Profile_Force32 = 0x40000000,
        }

        private enum OodleLZ_Verbosity
        {
            OodleLZ_Verbosity_None = 0,
            OodleLZ_Verbosity_Minimal = 1,
            OodleLZ_Verbosity_Some = 2,
            OodleLZ_Verbosity_Lots = 3,
            OodleLZ_Verbosity_Force32 = 0x40000000,
        }

        /// <summary>
        /// Compress the source array.
        /// </summary>
        /// <param name="source">Source data.</param>
        /// <param name="compressor">Compressor to use.</param>
        /// <param name="level">Level of compression.</param>
        /// <param name="seekChunkLength">Seek chunk length.</param>
        /// <returns>The compressed bytes.</returns>
        public static byte[] Compress(byte[] source, OodleLZ_Compressor compressor, OodleLZ_CompressionLevel level, int seekChunkLength)
        {
            IntPtr pOptions = OodleLZ_CompressOptions_GetDefault(compressor, level);
            OodleLZ_CompressOptions options = Marshal.PtrToStructure<OodleLZ_CompressOptions>(pOptions);

            options.seekChunkReset = true;
            options.seekChunkLen = seekChunkLength;

            pOptions = Marshal.AllocHGlobal(Marshal.SizeOf<OodleLZ_CompressOptions>());

            try
            {
                Marshal.StructureToPtr(options, pOptions, false);
                long compressedBufferSizeNeeded = OodleLZ_GetCompressedBufferSizeNeeded(source.LongLength);
                byte[] compBuf = new byte[compressedBufferSizeNeeded];
                long compLen = OodleLZ_Compress(compressor, source, source.LongLength, compBuf, level, pOptions, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero, 0);
                Array.Resize(ref compBuf, (int)compLen);

                return compBuf;
            }
            finally
            {
                Marshal.FreeHGlobal(pOptions);
            }
        }

        /// <summary>
        /// Decompress a byte array.
        /// </summary>
        /// <param name="source">Source data.</param>
        /// <param name="uncompressedSize">Real size.</param>
        /// <returns>The decompressed bytes.</returns>
        public static byte[] Decompress(byte[] source, long uncompressedSize)
        {
            long decodeBufferSize = OodleLZ_GetDecodeBufferSize(uncompressedSize, true);
            byte[] rawBuf = new byte[decodeBufferSize];
            long rawLen = OodleLZ_Decompress(source, source.LongLength, rawBuf, uncompressedSize);
            Array.Resize(ref rawBuf, (int)rawLen);
            return rawBuf;
        }

        /// <summary>
        /// Gets the seek chunk length based on the uncompressed size.
        /// </summary>
        /// <param name="rawSize">Uncompressed size.</param>
        /// <returns>The seek chunk length.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Major Code Smell", "S4200:Native methods should be wrapped", Justification = "No checks needed.")]
        public static int GetSeekChunkLen(long rawSize)
        {
            return OodleLZ_MakeSeekChunkLen(rawSize, 16);
        }

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_Compress(
            OodleLZ_Compressor compressor,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] rawBuf,
            long rawLen,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] compBuf,
            OodleLZ_CompressionLevel level,
            IntPtr pOptions,
            IntPtr dictionaryBase,
            IntPtr lrm,
            IntPtr scratchMem,
            long scratchSize);

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern IntPtr OodleLZ_CompressOptions_GetDefault(
            OodleLZ_Compressor compressor,
            OodleLZ_CompressionLevel lzLevel);

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_Decompress(
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] compBuf,
            long compBufSize,
            [MarshalAs(UnmanagedType.LPArray)]
            byte[] rawBuf,
            long rawLen,
            OodleLZ_FuzzSafe fuzzSafe,
            OodleLZ_CheckCRC checkCRC,
            OodleLZ_Verbosity verbosity,
            IntPtr decBufBase,
            long decBufSize,
            IntPtr fpCallback,
            IntPtr callbackUserData,
            IntPtr decoderMemory,
            long decoderMemorySize,
            OodleLZ_Decode_ThreadPhase threadPhase);

        private static long OodleLZ_Decompress(byte[] compBuf, long compBufSize, byte[] rawBuf, long rawLen)
            => OodleLZ_Decompress(
                compBuf,
                compBufSize,
                rawBuf,
                rawLen,
                OodleLZ_FuzzSafe.OodleLZ_FuzzSafe_Yes,
                OodleLZ_CheckCRC.OodleLZ_CheckCRC_No,
                OodleLZ_Verbosity.OodleLZ_Verbosity_None,
                IntPtr.Zero,
                0,
                IntPtr.Zero,
                IntPtr.Zero,
                IntPtr.Zero,
                0,
                OodleLZ_Decode_ThreadPhase.OodleLZ_Decode_Unthreaded);

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_GetCompressedBufferSizeNeeded(
               long rawSize);

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern long OodleLZ_GetDecodeBufferSize(
            long rawSize,
            [MarshalAs(UnmanagedType.Bool)]
            bool corruptionPossible);

        [DllImport("oo2core_6_win64.dll", CallingConvention = CallingConvention.StdCall)]
        private static extern int OodleLZ_MakeSeekChunkLen(
            long rawSize,
            int desiredSeekPointCount);

        [StructLayout(LayoutKind.Sequential)]
        private struct OodleLZ_CompressOptions
        {
            public uint verbosity;
            public int minMatchLen;
            [MarshalAs(UnmanagedType.Bool)]
            public bool seekChunkReset;
            public int seekChunkLen;
            public OodleLZ_Profile profile;
            public int dictionarySize;
            public int spaceSpeedTradeoffBytes;
            public int maxHuffmansPerChunk;
            [MarshalAs(UnmanagedType.Bool)]
            public bool sendQuantumCRCs;
            public int maxLocalDictionarySize;
            public int makeLongRangeMatcher;
            public int matchTableSizeLog2;
        }
    }
}
