using System.Diagnostics;
using System.Runtime.InteropServices;

Process _process;
IntPtr _hProcess;

// Import C++ functions
[DllImport("kernel32.dll")]
static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress, byte[] lpBuffer, int dwSize, out int lpNumberOfBytesRead);

const int PROCESS_ALL_ACCESS = 0x1F0FFF;
[DllImport("kernel32.dll")]
static extern IntPtr OpenProcess(int dwDesiredAccess, bool bInheritHandle, int dwProcessId);

// Find a string in the meory buffer
int FindString(byte[] buffer, string searchString)
{
    for (int i = 0; i < buffer.Length - searchString.Length; i++)
    {
        bool match = true;
        for (int j = 0; j < searchString.Length; j++)
        {
            if (buffer[i + j] != searchString[j])
            {
                match = false;
                break;
            }
        }
        if (match)
        {
            return i;
        }
    }
    return -1;
}

// The process name and the boats to search for
string processName = "vsf_ng";
List<string> boats = new(){"Adriatic", "HMAS_Anzac", "Goztepe", "Jim_Bickhoff"};
int processNum = 0;

// Get the process
Process[] processes = Process.GetProcessesByName(processName);

// Check if the process was found
if (processes.Length == 0)
{
    Console.WriteLine("Could not find process: " + processName);
    return;
}

// If there are multiple processes, find the one with a window
for(int i = 0;i<processes.Length;i++)
{
    if (processes[i].MainWindowTitle != "")
    {
        processNum = i;
    }
}

// Open the process
_process = processes[processNum];
_hProcess = OpenProcess(PROCESS_ALL_ACCESS, false, _process.Id);

// Check if the process was opened
if (_hProcess == IntPtr.Zero)
{
    Console.WriteLine("Could not open process: " + processName);
    return;
}

// Get the variables for each boat
foreach(string boat in boats)
{
    Console.WriteLine("Boat: " + boat);
    GetBoatVars($"A{boat}");
}

// Read a value at a memory address
void ReadValueAtAddress(IntPtr address, int size)
{
    byte[] buffer = new byte[size];
    int bytesRead;
    if (ReadProcessMemory(_hProcess, address, buffer, buffer.Length, out bytesRead))
    {
        Console.WriteLine("Value: 0x" + BitConverter.ToString(buffer).Replace("-", "") + ": " + BitConverter.ToSingle(buffer, 0));
    }
    else
    {
        Console.WriteLine("Could not read value.");
    }
}


// Get the variables for a boat
void GetBoatVars(string boat)
{
    byte[] buffer = new byte[4096];
    int bytesRead;
    foreach (ProcessModule module in _process.Modules)
    {
        IntPtr baseAddress = module.BaseAddress;
        IntPtr endAddress = IntPtr.Add(baseAddress, module.ModuleMemorySize);

        while (baseAddress.ToInt64() < endAddress.ToInt64())
        {
            if (ReadProcessMemory(_hProcess, baseAddress, buffer, buffer.Length, out bytesRead))
            {
                int index = FindString(buffer, boat);
                if (index >= 0)
                {
                    Console.WriteLine($"Found boat named {boat} at RAM address: 0x{(baseAddress.ToInt64() + index).ToString("X")}");

                    var startAddr = IntPtr.Add(baseAddress, index + 1 - 44);

                    for(int i = 0;i<11;i++)
                    {
                        ReadValueAtAddress(IntPtr.Add(baseAddress, index + 1 - 44 + i*4), 4);    
                    }
                    return;
                }
            }
            baseAddress = IntPtr.Add(baseAddress, buffer.Length);
        }
    }

    Console.WriteLine("String not found.");
}

