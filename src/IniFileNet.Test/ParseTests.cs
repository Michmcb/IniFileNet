namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;

	public static class SyntaxTests
	{
		[Fact]
		public static void IsLegalKey()
		{
			Assert.False(Syntax.IsLegalKey(""));
			Assert.False(Syntax.IsLegalKey("   "));
			Assert.False(Syntax.IsLegalKey(";"));
			Assert.False(Syntax.IsLegalKey("#"));
			Assert.False(Syntax.IsLegalKey("="));
			Assert.False(Syntax.IsLegalKey(":"));
			Assert.False(Syntax.IsLegalKey("\r"));
			Assert.False(Syntax.IsLegalKey("\n"));
			Assert.True(Syntax.IsLegalKey("key"));
		}
		[Fact]
		public static void IsLegalSectionName()
		{
			Assert.False(Syntax.IsLegalSectionName(""));
			Assert.False(Syntax.IsLegalSectionName(";"));
			Assert.False(Syntax.IsLegalSectionName("#"));
			Assert.False(Syntax.IsLegalSectionName("["));
			Assert.False(Syntax.IsLegalSectionName("]"));
			Assert.False(Syntax.IsLegalSectionName("\r"));
			Assert.False(Syntax.IsLegalSectionName("\n"));
			Assert.True(Syntax.IsLegalSectionName("section"));
		}
	}
	public static class ParseTests
	{
		[Fact]
		public static void BooleanGood()
		{
			Assert.Equal(new IniResult<bool>(true, default), Parse.Boolean("true"));
			Assert.Equal(new IniResult<bool>(false, default), Parse.Boolean("false"));
		}
		[Fact]
		public static void BooleanBad()
		{
			Assert.Equal(new IniResult<bool>(false, new(IniErrorCode.ValueInvalid, "Could not parse \"blah\" as boolean")), Parse.Boolean("blah"));
		}
		[Fact]
		public static void Boolean1Or0Good()
		{
			Assert.Equal(new IniResult<bool>(true, default), Parse.Boolean1Or0("1"));
			Assert.Equal(new IniResult<bool>(false, default), Parse.Boolean1Or0("0"));
		}
		[Fact]
		public static void Boolean1Or0Bad()
		{
			Assert.Equal(new IniResult<bool>(false, new(IniErrorCode.ValueInvalid, "The value \"blah\" was not 1 or 0")), Parse.Boolean1Or0("blah"));
			Assert.Equal(new IniResult<bool>(false, new(IniErrorCode.ValueInvalid, "The value \"2\" was not 1 or 0")), Parse.Boolean1Or0("2"));
		}
		[Fact]
		public static void ByteGood()
		{
			Assert.Equal(new IniResult<byte>(100, default), Parse.Byte("100"));
		}
		[Fact]
		public static void ByteBad()
		{
			Assert.Equal(new IniResult<byte>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"300\" as byte")), Parse.Byte("300"));
		}
		[Fact]
		public static void SByteGood()
		{
			Assert.Equal(new IniResult<sbyte>(100, default), Parse.SByte("100"));
		}
		[Fact]
		public static void SByteBad()
		{
			Assert.Equal(new IniResult<sbyte>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"300\" as sbyte")), Parse.SByte("300"));
		}
		[Fact]
		public static void Int16Good()
		{
			Assert.Equal(new IniResult<short>(100, default), Parse.Int16("100"));
		}
		[Fact]
		public static void Int16Bad()
		{
			Assert.Equal(new IniResult<short>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"99999\" as short")), Parse.Int16("99999"));
		}
		[Fact]
		public static void UInt16Good()
		{
			Assert.Equal(new IniResult<ushort>(100, default), Parse.UInt16("100"));
		}
		[Fact]
		public static void UInt16Bad()
		{
			Assert.Equal(new IniResult<ushort>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"99999\" as ushort")), Parse.UInt16("99999"));
		}
		[Fact]
		public static void Int32Good()
		{
			Assert.Equal(new IniResult<int>(100, default), Parse.Int32("100"));
		}
		[Fact]
		public static void Int32Bad()
		{
			Assert.Equal(new IniResult<int>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"163854687864671\" as int")), Parse.Int32("163854687864671"));
		}
		[Fact]
		public static void UInt32Good()
		{
			Assert.Equal(new IniResult<uint>(100, default), Parse.UInt32("100"));
		}
		[Fact]
		public static void UInt32Bad()
		{
			Assert.Equal(new IniResult<uint>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"163854687864671\" as uint")), Parse.UInt32("163854687864671"));
		}
		[Fact]
		public static void Int64Good()
		{
			Assert.Equal(new IniResult<long>(100, default), Parse.Int64("100"));
		}
		[Fact]
		public static void Int64Bad()
		{
			Assert.Equal(new IniResult<long>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"1638354254654687864671\" as long")), Parse.Int64("1638354254654687864671"));
		}
		[Fact]
		public static void UInt64Good()
		{
			Assert.Equal(new IniResult<ulong>(100, default), Parse.UInt64("100"));
		}
		[Fact]
		public static void UInt64Bad()
		{
			Assert.Equal(new IniResult<ulong>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"1638354254654687864671\" as ulong")), Parse.UInt64("1638354254654687864671"));
		}
		[Fact]
		public static void FloatGood()
		{
			Assert.Equal(new IniResult<float>((float)123.45, default), Parse.Single("123.45"));
		}
		[Fact]
		public static void FloatBad()
		{
			Assert.Equal(new IniResult<float>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"jgdsfopjgkd\" as float")), Parse.Single("jgdsfopjgkd"));
		}
		[Fact]
		public static void DoubleGood()
		{
			Assert.Equal(new IniResult<double>(123.45, default), Parse.Double("123.45"));
		}
		[Fact]
		public static void DoubleBad()
		{
			Assert.Equal(new IniResult<double>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"jgdsfopjgkd\" as double")), Parse.Double("jgdsfopjgkd"));
		}
		[Fact]
		public static void DecimalGood()
		{
			Assert.Equal(new IniResult<decimal>(123.45m, default), Parse.Decimal("123.45"));
		}
		[Fact]
		public static void DecimalBad()
		{
			Assert.Equal(new IniResult<decimal>(0, new(IniErrorCode.ValueInvalid, "Could not parse \"jgdsfopjgkd\" as decimal")), Parse.Decimal("jgdsfopjgkd"));
		}
		[Fact]
		public static void GuidGood()
		{
			Assert.Equal(new IniResult<Guid>(new Guid(0xd4cf2fd9, 0xc49a, 0x40bd, 0xbe, 0x28, 0x6c, 0x22, 0x2d, 0xb, 0x7a, 0x32), default), Parse.Guid("D4CF2FD9-C49A-40BD-BE28-6C222D0B7A32"));
		}
		[Fact]
		public static void GuidBad()
		{
			Assert.Equal(new IniResult<Guid>(default, new(IniErrorCode.ValueInvalid, "Could not parse \"jgdsfopjgkd\" as Guid")), Parse.Guid("jgdsfopjgkd"));
		}
	}
}