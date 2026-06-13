namespace EfficientUnionGenerator;

internal static class Constants
{
    public const string AttributeNamespace = "EfficientUnion";

    public const string EfficientUnionAttributeName = "EfficientUnionAttribute";

    public const string EfficientUnionAttributeFullName = $"{AttributeNamespace}.{EfficientUnionAttributeName}";

    public const string EnumBitPatternAttributeName = "EnumBitPatternAttribute";

    public const string TypeIdentifierValueModeEnumName = nameof(TypeIdentifierValueMode);

    public const string CompilerServicesUnionAttributeSource = """
        namespace System.Runtime.CompilerServices;

        [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
        internal sealed class UnionAttribute : Attribute;
        """;

    public const string CompilerServicesIUnionSource = """
        #nullable enable
        namespace System.Runtime.CompilerServices;

        internal interface IUnion
        {
            object? Value { get; }
        }
        """;

    public static readonly string EfficientUnionAttributeSource = $$"""
using System;
namespace {{AttributeNamespace}};


/// <summary>
/// Indicates how to treat the fields corresponding to the set bits in the unmanaged field bit mask.
/// </summary>
[Flags]
internal enum {{TypeIdentifierValueModeEnumName}} : int
{
    /// <summary>
    /// Every mode is set to default value. The default value is `AutoAssign | SetWhenCreate | ResetWhenGet`.
    /// </summary>
    Default = 0,

    /// <summary>
    /// Constant assignment policy: the bit pattern will be specified by compiler.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.AutoAssign)}} = {{(int)TypeIdentifierValueMode.AutoAssign}},

    /// <summary>
    /// Constant assignment policy: the bit pattern is defined by user.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.ExplicitAssign)}} = {{(int)TypeIdentifierValueMode.ExplicitAssign}},

    /// <summary>
    /// Creation policy: the bit pattern of the initialization argument will be set when creating the union instance.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.SetWhenCreate)}} = {{(int)TypeIdentifierValueMode.SetWhenCreate}},

    /// <summary>
    /// Creation policy: the bit pattern of the initialization argument will be kept when creating the union instance.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.LeaveWhenCreate)}} = {{(int)TypeIdentifierValueMode.LeaveWhenCreate}},

    /// <summary>
    /// Access policy: the bit pattern of the entity value will be reset when accessing the union instance.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.ResetWhenGet)}} = {{(int)TypeIdentifierValueMode.ResetWhenGet}},

    /// <summary>
    /// Access policy: the bit pattern of the entity value will be left unchanged when accessing the union instance.
    /// </summary>
    {{nameof(TypeIdentifierValueMode.LeaveWhenGet)}} = {{(int)TypeIdentifierValueMode.LeaveWhenGet}},
}


[AttributeUsage(AttributeTargets.Constructor, Inherited = false, AllowMultiple = false)]
internal sealed class {{EnumBitPatternAttributeName}} : Attribute
{
    public ulong Flag { get; }

    public {{EnumBitPatternAttributeName}}(ulong flag)
    {
    }
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
    public {{TypeIdentifierValueModeEnumName}} UnmanagedFieldBitMaskMode { get; } = {{TypeIdentifierValueModeEnumName}}.{{TypeIdentifierValueMode.AutoAssign}};

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
        {{TypeIdentifierValueModeEnumName}} unmanagedFieldBitMaskMode = {{TypeIdentifierValueModeEnumName}}.{{TypeIdentifierValueMode.AutoAssign}},
        ulong unmanagedFieldMask = 0)
    {
        UnmanagedFieldBitMaskMode = unmanagedFieldBitMaskMode;
        UnmanagedFieldMask = unmanagedFieldMask;
    }
}
""";
}
