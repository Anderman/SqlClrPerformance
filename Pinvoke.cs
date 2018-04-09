using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;

namespace testApp
{
    public static class Pinvoke
    {
        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.StdCall)]
        public delegate ulong FuncUInt64();

        private const uint PAGE_EXECUTE = 0x10;
        private const uint PAGE_EXECUTE_READWRITE = 0x40;
        private const uint MEM_COMMIT = 0x1000;
        private const uint MEM_RELEASE = 0x8000;

        public static readonly FuncUInt64 Empty;
        private static readonly byte[] ReturnOnlyAsm = {  0xC3 };

        static Pinvoke()
        {
            var buf = IntPtr.Zero;
            try
            {
                // We pad the functions to 64 bytes (the length of a cacheline on the Intel processors)
                var rdtscpLength = (ReturnOnlyAsm.Length & 63) != 0 ? (ReturnOnlyAsm.Length | 63) + 1 : ReturnOnlyAsm.Length;

                buf = VirtualAlloc(IntPtr.Zero, (IntPtr) rdtscpLength, MEM_COMMIT, PAGE_EXECUTE_READWRITE);
                if (buf == IntPtr.Zero) throw new Win32Exception();


                Marshal.Copy(ReturnOnlyAsm, 0, buf, ReturnOnlyAsm.Length);
                for (var i = ReturnOnlyAsm.Length; i < rdtscpLength; i++) Marshal.WriteByte(buf, i, 0x90); // nop

                // Change the access of the allocated memory from R/W to Execute
                var result = VirtualProtect(buf, (IntPtr) rdtscpLength, PAGE_EXECUTE, out var oldProtection);
                if (!result) throw new Win32Exception();

                // Create a delegate to the "function"
                Empty = (FuncUInt64) Marshal.GetDelegateForFunctionPointer(buf, typeof(FuncUInt64));

                buf = IntPtr.Zero;
            }
            finally
            {
                if (buf != IntPtr.Zero)
                    VirtualFree(buf, IntPtr.Zero, MEM_RELEASE);
            }
        }

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern IntPtr VirtualAlloc(IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualProtect(IntPtr lpAddress, IntPtr dwSize, uint flAllocationType, out uint lpflOldProtect);

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool VirtualFree(IntPtr lpAddress, IntPtr dwSize, uint dwFreeType);
    }
}