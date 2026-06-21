using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

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
