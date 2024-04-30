namespace IniFileNet.Test
{
	using System.Collections.Generic;
	using Xunit;
	public static class IniValueAcceptorTests
	{
		[Fact]
		public static void Single()
		{
			IniValueAcceptorSingle x = new();
			Assert.Equal(default, x.Accept("key", "value"));
			Assert.Equal("value", x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"key\". Value is: \"value\""), x.Accept("key", "value"));
			Assert.Equal("value", x.Value);
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "value"));
			Assert.Equal("value", x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void SingleParsed()
		{
			IniValueAcceptorSingle<int> x = new(Parse.Int32);
			Assert.Equal(default, x.Accept("key", "10"));
			Assert.Equal(10, x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"key\". Value is: \"15\""), x.Accept("key", "15"));
			Assert.True(x.HaveValue);
			Assert.Equal(10, x.Value);
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "20"));
			Assert.Equal(20, x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyFirst()
		{
			IniValueAcceptorOnlyFirst x = new();
			Assert.Equal(default, x.Accept("key", "value1"));
			Assert.Equal("value1", x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "value2"));
			Assert.Equal("value1", x.Value);
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "value1"));
			Assert.Equal("value1", x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyFirstParsed()
		{
			IniValueAcceptorOnlyFirst<int> x = new(Parse.Int32);
			Assert.Equal(default, x.Accept("key", "10"));
			Assert.Equal(10, x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "15"));
			Assert.True(x.HaveValue);
			Assert.Equal(10, x.Value);
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "20"));
			Assert.Equal(20, x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyLast()
		{
			IniValueAcceptorOnlyLast x = new();
			Assert.Equal(default, x.Accept("key", "value1"));
			Assert.Equal("value1", x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "value2"));
			Assert.Equal("value2", x.Value);
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "value1"));
			Assert.Equal("value1", x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyLastParsed()
		{
			IniValueAcceptorOnlyLast<int> x = new(Parse.Int32);
			Assert.Equal(default, x.Accept("key", "10"));
			Assert.Equal(10, x.Value);
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "15"));
			Assert.Equal(15, x.Value);
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("key", "20"));
			Assert.Equal(20, x.Value);
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void Many()
		{
			IniValueAcceptorMany xx = new(8);
			Assert.Equal(8, xx.Values.Capacity);

			IniValueAcceptorMany x = new();
			Assert.Equal(default, x.Accept("key", "value1"));
			Assert.Equal(default, x.Accept("key", "value2"));
			Assert.Equal(default, x.Accept("key", "value3"));
			Assert.True(x.HaveValue);

			var oldValues = x.Values;
			x.Reset();
			Assert.Empty(x.Values);
			Assert.False(x.HaveValue);

			Assert.Collection(oldValues,
				x => Assert.Equal("value1", x),
				x => Assert.Equal("value2", x),
				x => Assert.Equal("value3", x));

			Assert.Equal(default, x.Accept("key", "value4"));
			Assert.Equal(default, x.Accept("key", "value5"));
			Assert.True(x.HaveValue);
			Assert.Collection(x.Values,
				x => Assert.Equal("value4", x),
				x => Assert.Equal("value5", x));
		}
		[Fact]
		public static void ManyParsed()
		{
			IniValueAcceptorMany<int, List<int>> x = new(Parse.Int32);
			Assert.Equal(default, x.Accept("key", "1"));
			Assert.Equal(default, x.Accept("key", "2"));
			Assert.Equal(default, x.Accept("key", "3"));
			Assert.True(x.HaveValue);

			var oldValues = x.Values;
			x.Reset();
			Assert.Empty(x.Values);
			Assert.False(x.HaveValue);

			Assert.Collection(oldValues,
				x => Assert.Equal(1, x),
				x => Assert.Equal(2, x),
				x => Assert.Equal(3, x));

			Assert.Equal(default, x.Accept("key", "4"));
			Assert.Equal(default, x.Accept("key", "5"));
			Assert.True(x.HaveValue);
			Assert.Collection(x.Values,
				x => Assert.Equal(4, x),
				x => Assert.Equal(5, x));
		}
	}
}