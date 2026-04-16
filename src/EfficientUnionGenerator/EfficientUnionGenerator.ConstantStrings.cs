using System;
using System.Collections.Generic;
using System.Text;

namespace EfficientUnionGenerator;

partial class EfficientUnionGenerator
{
    private enum UnmanagedFieldBitMaskMode
    {
        Auto = 0,
        Explicit = 1,
    }

    public const string AttributeNamespace = "EfficientUnion";

    public const string EfficientUnionAttributeName = "EfficientUnionAttribute";

    public const string EnumBitPatternAttributeName = "EnumBitPatternAttribute";

    public const string UnmanagedFieldBitMaskModeEnumName = nameof(UnmanagedFieldBitMaskMode);

    public static readonly string EfficientUnionAttributeSource = $$"""
using System;
namespace {{AttributeNamespace}};

/// <summary>
/// Indicates how to treat the fields corresponding to the set bits in the unmanaged field bit mask.
/// </summary>
internal enum {{UnmanagedFieldBitMaskModeEnumName}} : int
{
    /// <summary>
    /// The bit pattern will be specified by compiler.
    /// </summary>
    {{nameof(UnmanagedFieldBitMaskMode.Auto)}} = {{(int)UnmanagedFieldBitMaskMode.Auto}},

    /// <summary>
    /// The bit pattern is defined by user.
    /// </summary>
    {{nameof(UnmanagedFieldBitMaskMode.Explicit)}} = {{(int)UnmanagedFieldBitMaskMode.Explicit}},
}

/// <summary>
/// Generates an efficient union struct implementation for the attributed struct.
/// The struct must be a partial struct.
/// This generator regards a partial constructor with single parameter as a union type candidate.
/// </summary>
/// <remarks>
/// Specifying a bitmask means that instead of allocating an enumeration field for entity identification of unmanaged types,
/// a masked bit region is used.
/// </remarks>
[AttributeUsage(AttributeTargets.Struct, Inherited = false, AllowMultiple = false)]
internal sealed class {{EfficientUnionAttributeName}} : Attribute
{
    /// <summary>
    /// Gets the mode that determines how masked the part of the unmanaged field are handled.
    /// </summary>
    public {{UnmanagedFieldBitMaskModeEnumName}} UnmanagedFieldBitMaskMode { get; } = {{UnmanagedFieldBitMaskModeEnumName}}.{{UnmanagedFieldBitMaskMode.Auto}};

    /// <summary>
    /// Gets the unmanaged field mask.
    /// </summary>
    public ulong UnmanagedFieldMask { get; }

    /// <summary>
    /// Initializes a new instance of the `EfficientUnionAttribute` class which exports an enum field to determine unmanaged type.
    /// </summary>
    public {{EfficientUnionAttributeName}}()
        : this(default, 0)
    {
    }

    /// <summary>
    /// Initializes a new instance of the `EfficientUnionAttribute` with bit field specification.
    /// </summary>
    /// <param name="unmanagedFieldBitMaskMode">
    /// The mode that determines how masked the part of the unmanaged field are handled.
    /// </param>
    /// <param name="unmanagedFieldMask">
    /// The unmanaged field bit mask.
    /// </param>
    public {{EfficientUnionAttributeName}}(
        {{UnmanagedFieldBitMaskModeEnumName}} unmanagedFieldBitMaskMode = {{UnmanagedFieldBitMaskModeEnumName}}.{{UnmanagedFieldBitMaskMode.Auto}},
        ulong unmanagedFieldMask = 0)
    {
        UnmanagedFieldBitMaskMode = unmanagedFieldBitMaskMode;
        UnmanagedFieldMask = unmanagedFieldMask;
    }
}


[AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
internal sealed class {{EnumBitPatternAttributeName}} : Attribute
{
    public ulong Flag { get; }

    public {{EnumBitPatternAttributeName}}(ulong flag)
    {
    }
}
""";

    public static readonly string CompilerServicesUnionAttributeSource = """
        namespace System.Runtime.CompilerServices;

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
        internal sealed class UnionAttribute : Attribute;
        """;

    public static readonly string CompilerServicesIUnionSource = """
        namespace System.Runtime.CompilerServices;

        internal interface IUnion
        {
            object? Value { get; }
        }
        """;
}
