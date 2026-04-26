using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using EfficientUnion;

namespace EfficientUnionGenerator.Test;

[EfficientUnion]
public partial struct TestUnion_Empty
{
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestEmptyUnion()
    {
        var u = new TestUnion_Empty();
        Assert.False(u.HasValue);
        Assert.Null(u.Value);
    }
}


[EfficientUnion]
public partial struct TestUnion_Unmanaged
{
    public partial TestUnion_Unmanaged(int x);
    public partial TestUnion_Unmanaged(double y);
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestUnmanagedUnion()
    {
        {
            var u = new TestUnion_Unmanaged(42);

            Assert.True(u.HasValue);
            Assert.IsType<int>(u.Value);
            Assert.Equal(42, u.Value);

            Assert.True(u.TryGetValue(out int intValue));
            Assert.False(u.TryGetValue(out double _));
            Assert.Equal(42, intValue);
        }
        {
            var u = new TestUnion_Unmanaged(3.14);

            Assert.True(u.HasValue);
            Assert.IsType<double>(u.Value);
            Assert.Equal(3.14, u.Value);

            Assert.True(u.TryGetValue(out double doubleValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.Equal(3.14, doubleValue);
        }
    }
}


[EfficientUnion]
public partial struct TestUnion_Managed
{
    public partial TestUnion_Managed(string s);
    public partial TestUnion_Managed(List<int> list);
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestManagedUnion()
    {
        {
            var u = new TestUnion_Managed("Hello");

            Assert.True(u.HasValue);
            Assert.IsType<string>(u.Value);
            Assert.Equal("Hello", u.Value);

            Assert.True(u.TryGetValue(out string? stringValue));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal("Hello", stringValue);
        }
        {
            var list = new List<int> { 1, 2, 3 };

            var u = new TestUnion_Managed(list);
            Assert.True(u.HasValue);
            Assert.IsType<List<int>>(u.Value);
            Assert.Equal(list, u.Value);

            Assert.True(u.TryGetValue(out List<int>? listValue));
            Assert.False(u.TryGetValue(out string? _));
            Assert.Equal(list, listValue);
        }
    }
}


[EfficientUnion]
public partial struct TestUnion_Mixture
{
    public partial TestUnion_Mixture(int x);
    public partial TestUnion_Mixture(double y);
    public partial TestUnion_Mixture(string s);
    public partial TestUnion_Mixture(List<int> list);
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestMixtureUnion()
    {
        {
            var u = new TestUnion_Mixture(42);

            Assert.True(u.HasValue);
            Assert.IsType<int>(u.Value);
            Assert.Equal(42, u.Value);

            Assert.True(u.TryGetValue(out int intValue));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal(42, intValue);
        }
        {
            var u = new TestUnion_Mixture(3.14);

            Assert.True(u.HasValue);
            Assert.IsType<double>(u.Value);
            Assert.Equal(3.14, u.Value);

            Assert.True(u.TryGetValue(out double doubleValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal(3.14, doubleValue);
        }
        {
            var u = new TestUnion_Mixture("Hello");

            Assert.True(u.HasValue);
            Assert.IsType<string>(u.Value);
            Assert.Equal("Hello", u.Value);

            Assert.True(u.TryGetValue(out string? stringValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal("Hello", stringValue);
        }
        {
            var list = new List<int> { 1, 2, 3 };

            var u = new TestUnion_Mixture(list);
            Assert.True(u.HasValue);
            Assert.IsType<List<int>>(u.Value);
            Assert.Equal(list, u.Value);

            Assert.True(u.TryGetValue(out List<int>? listValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.Equal(list, listValue);
        }
    }
}


public partial class Nest1
{
    public partial struct Nest2
    {
        public partial interface INest3
        {
            public partial record Nest4
            {
                public partial record class Nest5
                {
                    public partial record struct Nest6
                    {
                        [EfficientUnion]
                        public partial struct TestUnion_Nested
                        {
                            public partial TestUnion_Nested(int x);
                            public partial TestUnion_Nested(double y);
                            public partial TestUnion_Nested(string s);
                            public partial TestUnion_Nested(List<int> list);
                        }
                    }
                }
            }
        }
    }
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestNestedUnion()
    {
        {
            var u = new Nest1.Nest2.INest3.Nest4.Nest5.Nest6.TestUnion_Nested(42);

            Assert.True(u.HasValue);
            Assert.IsType<int>(u.Value);
            Assert.Equal(42, u.Value);

            Assert.True(u.TryGetValue(out int intValue));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal(42, intValue);
        }
        {
            var u = new Nest1.Nest2.INest3.Nest4.Nest5.Nest6.TestUnion_Nested(3.14);

            Assert.True(u.HasValue);
            Assert.IsType<double>(u.Value);
            Assert.Equal(3.14, u.Value);

            Assert.True(u.TryGetValue(out double doubleValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal(3.14, doubleValue);
        }
        {
            var u = new Nest1.Nest2.INest3.Nest4.Nest5.Nest6.TestUnion_Nested("Hello");

            Assert.True(u.HasValue);
            Assert.IsType<string>(u.Value);
            Assert.Equal("Hello", u.Value);

            Assert.True(u.TryGetValue(out string? stringValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out List<int>? _));
            Assert.Equal("Hello", stringValue);

        }
        {
            var list = new List<int> { 1, 2, 3 };
            var u = new Nest1.Nest2.INest3.Nest4.Nest5.Nest6.TestUnion_Nested(list);

            Assert.True(u.HasValue);
            Assert.IsType<List<int>>(u.Value);
            Assert.Equal(list, u.Value);

            Assert.True(u.TryGetValue(out List<int>? listValue));
            Assert.False(u.TryGetValue(out int _));
            Assert.False(u.TryGetValue(out double _));
            Assert.False(u.TryGetValue(out string? _));
            Assert.Equal(list, listValue);
        }
    }
}


public readonly struct BitFlagUnionContent1(ulong a)
{
    public readonly ulong A = a;
}

public readonly struct BitFlagUnionContent2(ulong b)
{
    public readonly ulong B = b;
}

public readonly struct BitFlagUnionContent3(ulong c)
{
    public readonly ulong C = c;
}

public readonly struct BitFlagUnionContent4(ulong d)
{
    public readonly ulong D = d;
}

[EfficientUnion(EfficientUnion.TypeIdentifierValueMode.Default, 0xC000_0000_0000_0000uL)]
public partial struct TestUnion_BitFlag_Default
{
    public partial TestUnion_BitFlag_Default(BitFlagUnionContent1 content);
    public partial TestUnion_BitFlag_Default(BitFlagUnionContent2 content);
    public partial TestUnion_BitFlag_Default(BitFlagUnionContent3 content);
    public partial TestUnion_BitFlag_Default(BitFlagUnionContent4 content);
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestBitFlagUnion_Default()
    {
        Assert.Equal(8, Unsafe.SizeOf<TestUnion_BitFlag_Default>());
        {
            var content = new BitFlagUnionContent1(42);
            var u = new TestUnion_BitFlag_Default(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent1>(u.Value);
            Assert.Equal(42uL, ((BitFlagUnionContent1)u.Value).A);

            Assert.True(u.TryGetValue(out BitFlagUnionContent1 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(42uL, contentValue.A);
        }
        {
            var content = new BitFlagUnionContent2(84);
            var u = new TestUnion_BitFlag_Default(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent2>(u.Value);
            Assert.Equal(84uL, ((BitFlagUnionContent2)u.Value).B);

            Assert.True(u.TryGetValue(out BitFlagUnionContent2 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(84uL, contentValue.B);
        }
        {
            var content = new BitFlagUnionContent3(126);
            var u = new TestUnion_BitFlag_Default(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent3>(u.Value);
            Assert.Equal(126uL, ((BitFlagUnionContent3)u.Value).C);

            Assert.True(u.TryGetValue(out BitFlagUnionContent3 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(126uL, contentValue.C);
        }
        {
            var content = new BitFlagUnionContent4(168);
            var u = new TestUnion_BitFlag_Default(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent4>(u.Value);
            Assert.Equal(168uL, ((BitFlagUnionContent4)u.Value).D);

            Assert.True(u.TryGetValue(out BitFlagUnionContent4 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.Equal(168uL, contentValue.D);
        }
    }
}



[EfficientUnion(
    EfficientUnion.TypeIdentifierValueMode.ExplicitAssign
    | EfficientUnion.TypeIdentifierValueMode.LeaveWhenCreate
    | EfficientUnion.TypeIdentifierValueMode.LeaveWhenGet, 0xF000_0000_0000_0000uL)]
public partial struct TestUnion_BitFlag_MagicNumber
{
    [EnumBitPattern(1uL << 60)] public partial TestUnion_BitFlag_MagicNumber(BitFlagUnionContent1 content);
    [EnumBitPattern(1uL << 61)] public partial TestUnion_BitFlag_MagicNumber(BitFlagUnionContent2 content);
    [EnumBitPattern(1uL << 62)] public partial TestUnion_BitFlag_MagicNumber(BitFlagUnionContent3 content);
    [EnumBitPattern(1uL << 63)] public partial TestUnion_BitFlag_MagicNumber(BitFlagUnionContent4 content);
}

public partial class EfficientUnionGeneratorTest
{
    [Fact]
    public void TestBitFlagUnion_MagicNumber()
    {
        Assert.Equal(8, Unsafe.SizeOf<TestUnion_BitFlag_MagicNumber>());
        {
            var value = 42uL | (1uL << 60);
            var content = new BitFlagUnionContent1(value);
            var u = new TestUnion_BitFlag_MagicNumber(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent1>(u.Value);
            Assert.Equal(value, ((BitFlagUnionContent1)u.Value).A);

            Assert.True(u.TryGetValue(out BitFlagUnionContent1 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(value, contentValue.A);
        }
        {
            var value = 84uL | (1uL << 61);
            var content = new BitFlagUnionContent2(value);
            var u = new TestUnion_BitFlag_MagicNumber(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent2>(u.Value);
            Assert.Equal(value, ((BitFlagUnionContent2)u.Value).B);

            Assert.True(u.TryGetValue(out BitFlagUnionContent2 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(value, contentValue.B);
        }
        {
            var value = 126uL | (1uL << 62);
            var content = new BitFlagUnionContent3(value);
            var u = new TestUnion_BitFlag_MagicNumber(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent3>(u.Value);
            Assert.Equal(value, ((BitFlagUnionContent3)u.Value).C);

            Assert.True(u.TryGetValue(out BitFlagUnionContent3 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent4 _));
            Assert.Equal(value, contentValue.C);
        }
        {
            var value = 168uL | (1uL << 63);
            var content = new BitFlagUnionContent4(value);
            var u = new TestUnion_BitFlag_MagicNumber(content);

            Assert.True(u.HasValue);
            Assert.IsType<BitFlagUnionContent4>(u.Value);
            Assert.Equal(value, ((BitFlagUnionContent4)u.Value).D);

            Assert.True(u.TryGetValue(out BitFlagUnionContent4 contentValue));
            Assert.False(u.TryGetValue(out BitFlagUnionContent1 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent2 _));
            Assert.False(u.TryGetValue(out BitFlagUnionContent3 _));
            Assert.Equal(value, contentValue.D);
        }
    }



}

