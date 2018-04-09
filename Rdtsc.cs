﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace testApp
{
    public static class Rdtsc
    {
        private static readonly byte[] Rdtscp = { 0x0F, 0x01, 0xF9, 0x48, 0xC1, 0xE2, 0x20, 0x48, 0x09, 0xD0, 0xC3 };
        //This function should not be inlined because it is replaced with asm code
        //Function should return const so native compile is already done when function is replaced by asm
        [MethodImpl(MethodImplOptions.NoInlining)]
        public static ulong TimestampP()
        {
            Stopwatch.GetTimestamp();
            Stopwatch.GetTimestamp();
            return 0;
        }

        public static unsafe void Init()
        {
            Func<ulong> func = TimestampP;
            var reference = __makeref(func);

            var ptrReference = (IntPtr*)*(IntPtr*)&reference;
            var ptrFunc = (IntPtr*)*ptrReference;
            var ptrTimestampP = (byte*)*(ptrFunc + 0x4);

            foreach (var b in Rdtscp)
                *(ptrTimestampP++) = b;
        }
    }
}