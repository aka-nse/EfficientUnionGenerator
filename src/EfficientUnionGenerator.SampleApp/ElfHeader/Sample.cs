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

        var elf32 = new ElfHeader(new Elf32Header(EI_DATA.ELFDATA2LSB));
        var elf64 = new ElfHeader(new Elf64Header(EI_DATA.ELFDATA2LSB));
        ShowValue(nameof(elf32), elf32);
        ShowValue(nameof(elf64), elf64);
    }

    private static void ShowValue(string name, ElfHeader x)
    {
#if NET11_OR_GREATER
        switch (x)
        {
        case ElfHeader(var header32):
            Console.WriteLine($"{name} is a valid Elf32Header");
            break;
        case ElfHeader(var header64):
            Console.WriteLine($"{name} is a valid Elf64Header");
            break;
        }
#else
        if(x.TryGetValue(out Elf32Header header32))
        {
            Console.WriteLine($"{name} is a valid Elf32Header");
        }
        else if (x.TryGetValue(out Elf64Header header64))
        {
            Console.WriteLine($"{name} is a valid Elf64Header");
        }
#endif
    }
}


public enum EI_CLASS : byte
{
    ELFCLASSNONE = 0,
    ELFCLASS32 = 1,
    ELFCLASS64 = 2,
}

public enum EI_DATA : byte
{
    ELFDATANONE = 0,
    ELFDATA2LSB = 1,
    ELFDATA2MSB = 2,
}

public enum EI_VERSION : byte
{
    EV_NONE = 0,
    EV_CURRENT = 1,
}

public enum E_TYPE : ushort
{
    ET_NONE = 0,
    ET_REL = 1,
    ET_EXEC = 2,
    ET_DYN = 3,
    ET_CORE = 4,
}

public enum E_MACHINE : ushort
{
    EM_NONE = 0,
    EM_M32 = 1,
    EM_SPARC = 2,
    EM_386 = 3,
    EM_68K = 4,
    EM_88K = 5,
    EM_860 = 7,
    EM_MIPS = 8,
    EM_ARM = 40,
    EM_X86_64 = 62,
}

public enum E_VERSION : uint
{
    EV_NONE = 0,
    EV_CURRENT = 1,
}

public enum E_SHSTRNDX : ushort
{
    SHN_UNDEF = 0,
    SHN_LORESERVE = 0xFF00,
    SHN_LOPROC = 0xFF00,
    SHN_HIPROC = 0xFF1F,
    SHN_ABS = 0xFFF1,
    SHN_COMMON = 0xFFF2,
    SHN_HIRESERVE = 0xFFFF,
}

#pragma warning disable format
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Elf32Header
{
    private fixed byte e_ident[16];
    public E_TYPE      e_type;
    public E_MACHINE   e_machine;
    public E_VERSION   e_version;
    public uint        e_entry;
    public uint        e_phoff;
    public uint        e_shoff;
    public uint        e_flags;
    public ushort      e_ehsize;
    public ushort      e_phentsize;
    public ushort      e_phnum;
    public ushort      e_shentsize;
    public ushort      e_shnum;
    public E_SHSTRNDX  e_shstrndx;

    public byte E_Ident(int offset) => e_ident[offset];
    public byte       EI_MAG0       { get =>             e_ident[0]; init => e_ident[0] =       value; }
    public byte       EI_MAG1       { get =>             e_ident[1]; init => e_ident[1] =       value; }
    public byte       EI_MAG2       { get =>             e_ident[2]; init => e_ident[2] =       value; }
    public byte       EI_MAG3       { get =>             e_ident[3]; init => e_ident[3] =       value; }
    public EI_CLASS   EI_CLASS      { get => (EI_CLASS)  e_ident[4]; init => e_ident[4] = (byte)value; }
    public EI_DATA    EI_DATA       { get => (EI_DATA)   e_ident[5]; init => e_ident[5] = (byte)value; }
    public EI_VERSION EI_VERSION    { get => (EI_VERSION)e_ident[6]; init => e_ident[6] = (byte)value; }
    public byte       EI_OSABI      { get =>             e_ident[7]; init => e_ident[7] =       value; }
    public byte       EI_ABIVERSION { get =>             e_ident[8]; init => e_ident[8] =       value; }

    public bool IsValid =>
        EI_MAG0 == 0x7F &&
        EI_MAG1 == (byte)'E' &&
        EI_MAG2 == (byte)'L' &&
        EI_MAG3 == (byte)'F' &&
        EI_CLASS == EI_CLASS.ELFCLASS32 &&
        EI_VERSION == EI_VERSION.EV_CURRENT &&
        e_version == E_VERSION.EV_CURRENT &&
        e_ehsize == 52;

    public Elf32Header(EI_DATA ei_data)
    {
        EI_MAG0 = 0x7F;
        EI_MAG1 = (byte)'E';
        EI_MAG2 = (byte)'L';
        EI_MAG3 = (byte)'F';
        EI_CLASS = EI_CLASS.ELFCLASS32;
        EI_DATA = ei_data;
        EI_VERSION = EI_VERSION.EV_CURRENT;
        e_version = E_VERSION.EV_CURRENT;
        e_ehsize = 52;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public unsafe struct Elf64Header
{
    private fixed byte e_ident[16];
    public E_TYPE     e_type;
    public E_MACHINE  e_machine;
    public E_VERSION  e_version;
    public ulong      e_entry;
    public ulong      e_phoff;
    public ulong      e_shoff;
    public uint       e_flags;
    public ushort     e_ehsize;
    public ushort     e_phentsize;
    public ushort     e_phnum;
    public ushort     e_shentsize;
    public ushort     e_shnum;
    public E_SHSTRNDX e_shstrndx;
    
    public byte E_Ident(int offset) => e_ident[offset];
    public byte       EI_MAG0       { get =>             e_ident[0]; init => e_ident[0] =       value; }
    public byte       EI_MAG1       { get =>             e_ident[1]; init => e_ident[1] =       value; }
    public byte       EI_MAG2       { get =>             e_ident[2]; init => e_ident[2] =       value; }
    public byte       EI_MAG3       { get =>             e_ident[3]; init => e_ident[3] =       value; }
    public EI_CLASS   EI_CLASS      { get => (EI_CLASS)  e_ident[4]; init => e_ident[4] = (byte)value; }
    public EI_DATA    EI_DATA       { get => (EI_DATA)   e_ident[5]; init => e_ident[5] = (byte)value; }
    public EI_VERSION EI_VERSION    { get => (EI_VERSION)e_ident[6]; init => e_ident[6] = (byte)value; }
    public byte       EI_OSABI      { get =>             e_ident[7]; init => e_ident[7] =       value; }
    public byte       EI_ABIVERSION { get =>             e_ident[8]; init => e_ident[8] =       value; }

    public bool IsValid =>
        EI_MAG0 == 0x7F &&
        EI_MAG1 == (byte)'E' &&
        EI_MAG2 == (byte)'L' &&
        EI_MAG3 == (byte)'F' &&
        EI_CLASS == EI_CLASS.ELFCLASS64 &&
        EI_VERSION == EI_VERSION.EV_CURRENT &&
        e_version == E_VERSION.EV_CURRENT &&
        e_ehsize == 64;

    public Elf64Header(EI_DATA ei_data)
    {
        EI_MAG0 = 0x7F;
        EI_MAG1 = (byte)'E';
        EI_MAG2 = (byte)'L';
        EI_MAG3 = (byte)'F';
        EI_CLASS = EI_CLASS.ELFCLASS64;
        EI_DATA = ei_data;
        EI_VERSION = EI_VERSION.EV_CURRENT;
        e_version = E_VERSION.EV_CURRENT;
        e_ehsize = 64;
    }
}
#pragma warning restore format

//                                           7  6  5  4  3  2  1  0 e_ident
[EfficientUnion(Mode, unmanagedFieldMask: 0x00_00_00_FF_00_00_00_00uL)]
public readonly partial struct ElfHeader
{
    private const TypeIdentifierValueMode Mode =
        TypeIdentifierValueMode.ExplicitAssign
        | TypeIdentifierValueMode.LeaveWhenCreate
        | TypeIdentifierValueMode.LeaveWhenGet;

    //                 7  6  5  4  3  2  1  0 e_ident
    [EnumBitPattern(0x00_00_00_01_00_00_00_00uL)] public partial ElfHeader(Elf32Header x);
    //                 7  6  5  4  3  2  1  0 e_ident
    [EnumBitPattern(0x00_00_00_02_00_00_00_00uL)] public partial ElfHeader(Elf64Header x);
}
