using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using EfficientUnion;

namespace EfficientUnionGenerator.SampleApp.ElfHeader;

internal class Sample : ISample
{
    public string Name => "ElfHeader";

    public void Run()
    {
        Console.WriteLine($"Struct size of `ElfHeader` union: {Unsafe.SizeOf<ElfHeader>()}");

        var elf32 = new ElfHeader(new Elf32Header());
        var elf64 = new ElfHeader(new Elf64Header());

    }

    private static void ShowValue(string name, ElfHeader x)
    {
#if NET11_OR_GREATER
        switch (x)
        {
        }
#else
#endif
    }
}


#pragma warning disable format
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Elf32Header()
{
    public readonly byte  EI_MAG0 = 0x7F;
    public readonly byte  EI_MAG1 = (byte)'E';
    public readonly byte  EI_MAG2 = (byte)'L';
    public readonly byte  EI_MAG3 = (byte)'F';
    public readonly byte  EI_CLASS = 1;
    public readonly byte  EI_DATA;
    public readonly byte  EI_VERSION;
    public readonly byte  EI_OSABI;
    public readonly ulong EI_RESERVED;
    public readonly ushort e_type;
    public readonly ushort e_machine;
    public readonly uint   e_version;
    public readonly ulong  e_entry;
    public readonly ulong  e_phoff;
    public readonly ulong  e_shoff;
    public readonly uint   e_flags;
    public readonly ushort e_ehsize;
    public readonly ushort e_phentsize;
    public readonly ushort e_phnum;
    public readonly ushort e_shentsize;
    public readonly ushort e_shnum;
    public readonly ushort e_shstrndx;
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public readonly struct Elf64Header()
{
    public readonly byte  EI_MAG0 = 0x7F;
    public readonly byte  EI_MAG1 = (byte)'E';
    public readonly byte  EI_MAG2 = (byte)'L';
    public readonly byte  EI_MAG3 = (byte)'F';
    public readonly byte  EI_CLASS = 2;
    public readonly byte  EI_DATA;
    public readonly byte  EI_VERSION;
    public readonly byte  EI_OSABI;
    public readonly ulong EI_RESERVED;
    public readonly ushort e_type;
    public readonly ushort e_machine;
    public readonly uint   e_version;
    public readonly uint   e_entry;
    public readonly uint   e_phoff;
    public readonly uint   e_shoff;
    public readonly uint   e_flags;
    public readonly ushort e_ehsize;
    public readonly ushort e_phentsize;
    public readonly ushort e_phnum;
    public readonly ushort e_shentsize;
    public readonly ushort e_shnum;
    public readonly ushort e_shstrndx;
}
#pragma warning restore format


[EfficientUnion(Mode, unmanagedFieldMask: 0x00_00_00_00_FF_00_00_00uL)]
public readonly partial struct ElfHeader
{
    private const TypeIdentifierValueMode Mode =
        TypeIdentifierValueMode.ExplicitAssign
        | TypeIdentifierValueMode.LeaveWhenCreate
        | TypeIdentifierValueMode.LeaveWhenGet;

    [EnumBitPattern(0x00_00_00_00_01_00_00_00uL)] public partial ElfHeader(Elf32Header x);
    [EnumBitPattern(0x00_00_00_00_02_00_00_00uL)] public partial ElfHeader(Elf64Header x);
}
