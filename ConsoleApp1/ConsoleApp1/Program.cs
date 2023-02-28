using System;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace NoRecoilTest
{
    unsafe class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool ReadProcessMemory(
            IntPtr hProcess,
            IntPtr lpBaseAddress,
            int* lpBuffer,
            int dwSize,
            out IntPtr lpNumberOfBytesRead);


        [DllImport("kernel32.dll")]
        static extern bool WriteProcessMemory(
            IntPtr hProcess,
            int* lpBaseAddress,
            byte[] lpBuffer,
            Int32 nSize,
            out IntPtr lpNumberOfBytesWritten
        );

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr OpenProcess(
            uint processAccess,
            bool bInheritHandle,
            uint processId
        );

        [Flags]
        public enum ProcessAccessFlags : uint
        {
            All = 0x001F0FFF,
            Terminate = 0x00000001,
            CreateThread = 0x00000002,
            VirtualMemoryOperation = 0x00000008,
            VirtualMemoryRead = 0x00000010,
            VirtualMemoryWrite = 0x00000020,
            DuplicateHandle = 0x00000040,
            CreateProcess = 0x000000080,
            SetQuota = 0x00000100,
            SetInformation = 0x00000200,
            QueryInformation = 0x00000400,
            QueryLimitedInformation = 0x00001000,
            Synchronize = 0x00100000
        }

        static unsafe void Main(string[] args)
        {
            Process[] processses = Process.GetProcessesByName("rust");
            if (processses.Length < 1)
            {
                Console.WriteLine("Nothing found");
                Console.ReadKey();
                return;
            }

            Process rust_maybe = processses[0];
            var h_process = OpenProcess((uint)ProcessAccessFlags.All, false, (uint)rust_maybe.Id);
            if (h_process == IntPtr.Zero)
            {
                Console.WriteLine("Open with Administarator");
                Console.ReadKey();
                return;
            }

            ProcessModule monoModule = null;
            foreach (ProcessModule module in rust_maybe.Modules)
            {
                if (module.ModuleName == "mono.dll")
                {
                    monoModule = module;
                }
            }

            if (monoModule == null)
            {
                Console.WriteLine("Wait For Game to Load");
                Console.ReadKey();
                return;
            }
            IntPtr base_addr = monoModule.BaseAddress;
            Console.WriteLine("Mono Base Is " + base_addr.ToString("X8"));
            int* readMem = (int*)Marshal.AllocHGlobal(1024);

            // this is where you start "mono.dll"+001F20AC
            IntPtr startAddr = base_addr + 0x001F20AC;

            int[] offsets = new int[3];
            offsets[0] = 0x8EC;
            offsets[1] = 0x0;
            offsets[2] = 0x334;

            int* tempAddress = stackalloc int[1];
            bool readOK = ReadProcessMemory(h_process, startAddr, tempAddress,
                sizeof(int), out var shouldbe4);
            if (!readOK || shouldbe4 != (IntPtr)sizeof(int))
            {
                Console.WriteLine("Reinstall the game and restart your pc");
                Console.WriteLine("Try Again");
                Console.ReadLine();
                return;
            }

            for (int i = 0; i < offsets.Length; i++)
            {
                IntPtr oldMem = new IntPtr(*tempAddress + offsets[i]);
                bool readOffset = ReadProcessMemory(h_process, oldMem, tempAddress,
                    sizeof(int), out var readbytes);
                if (readOffset && readbytes == (IntPtr)sizeof(int))
                {
                    if (i + 1 < offsets.Length)
                    {
                        Console.WriteLine($"{i+1}. Offset Is {*tempAddress:X8}");
                    }
                    else
                    {
                        // the last offset is the place in memory you were looking for, it no longer refers to where you put it
                        // there is the value you want to change, it should be 257 otherwise it gives you an error
                        Console.WriteLine($"Value At {oldMem:X8} Is {*tempAddress:X8} / {*tempAddress:D}");
                        if (*tempAddress == 257)
                        {
                            var zeroBytes = new byte[4];
                            bool written = WriteProcessMemory(h_process, (int*)oldMem.ToPointer(), zeroBytes,
                                zeroBytes.Length, out var bytesWritten);
                            if (written && bytesWritten == new IntPtr(zeroBytes.Length))
                            {
                                Console.WriteLine("Written " + bytesWritten);
                                // Recoil Off
                            }
                            else
                            {
                                Console.WriteLine("Cannot be Overwritten");
                                Console.ReadKey();
                                return;
                            }
                        }
                    }

                }
                else
                {
                    Console.WriteLine($"Read {i+1}. Offset Failed");
                    Console.ReadKey();
                }
            }
            Console.ReadLine();
        }
    }
}