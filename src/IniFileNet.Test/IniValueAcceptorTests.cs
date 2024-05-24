﻿namespace IniFileNet.Test
{
	using System.Collections.Generic;
	using Xunit;
	public static class IniValueAcceptorTests
	{
		[Fact]
		public static void Single()
		{
			IniValueAcceptorSingle x = new("key");
			Assert.Equal("key", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("value"));
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"key\". Value is: \"value\""), x.Accept("value"));
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("value"));
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void SingleParsed()
		{
			IniValueAcceptorSingle<int> x = new("key", Parse.Int32);
			Assert.Equal("key", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("10"));
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"key\". Value is: \"15\""), x.Accept("15"));
			Assert.True(x.HaveValue);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("20"));
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyFirst()
		{
			IniValueAcceptorOnlyFirst x = new("key");
			Assert.Equal("key", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("value1"));
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("value2"));
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("value1"));
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyFirstParsed()
		{
			IniValueAcceptorOnlyFirst<int> x = new("key", Parse.Int32);
			Assert.Equal("key", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("10"));
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("15"));
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("20"));
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyLast()
		{
			IniValueAcceptorOnlyLast x = new("key");
			Assert.Equal("key", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("value1"));
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("value2"));
			Assert.Equal("value2", x.Value);
			Assert.Equal("value2", x.ValueOr("foo"));
			Chk.IniResult("value2", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value2", x.ValueOrException());
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Null(x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("value1"));
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void OnlyLastParsed()
		{
			IniValueAcceptorOnlyLast<int> x = new("key", Parse.Int32);
			Assert.Equal("key", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("10"));
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HaveValue);
			Assert.Equal(default, x.Accept("15"));
			Assert.Equal(15, x.Value);
			Assert.Equal(15, x.ValueOr(5));
			Chk.IniResult(15, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(15, x.ValueOrException());
			Assert.True(x.HaveValue);
			x.Reset();
			Assert.Equal(0, x.Value);
			Assert.False(x.HaveValue);
			Assert.Equal(default, x.Accept("20"));
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HaveValue);
		}
		[Fact]
		public static void Many()
		{
			IniValueAcceptorMany xx = new("key", 8);
			Assert.Equal("key", xx.Key);
			Assert.Collection(xx.ValueOr(["1", "2", "3"]), y => Assert.Equal("1", y), y => Assert.Equal("2", y), y => Assert.Equal("3", y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, xx.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(xx.ValueOrException).IniErrorCode);
			Assert.Equal(8, xx.Value.Capacity);

			IniValueAcceptorMany x = new("key");
			Assert.Equal("key", x.Key);
			Assert.Collection(x.ValueOr(["1", "2", "3"]), y => Assert.Equal("1", y), y => Assert.Equal("2", y), y => Assert.Equal("3", y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("value1"));
			Assert.Equal(default, x.Accept("value2"));
			Assert.Equal(default, x.Accept("value3"));
			Assert.True(x.HaveValue);

			List<string> oldValues = x.Value;
			Assert.Same(x.Value, x.ValueOr(["1", "2", "3"]));
			IniResult<List<string>> ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			x.Reset();
			Assert.Empty(x.Value);
			Assert.False(x.HaveValue);

			Assert.Collection(oldValues,
				x => Assert.Equal("value1", x),
				x => Assert.Equal("value2", x),
				x => Assert.Equal("value3", x));

			Assert.Equal(default, x.Accept("value4"));
			Assert.Equal(default, x.Accept("value5"));
			Assert.True(x.HaveValue);
			Assert.Same(x.Value, x.ValueOr(["1", "2", "3"]));
			ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			Assert.Collection(x.Value,
				x => Assert.Equal("value4", x),
				x => Assert.Equal("value5", x));
		}
		[Fact]
		public static void ManyParsed()
		{
			IniValueAcceptorMany<int, List<int>> x = new("key", Parse.Int32);
			Assert.Equal("key", x.Key);
			Assert.Collection(x.ValueOr([1, 2, 3]), y => Assert.Equal(1, y), y => Assert.Equal(2, y), y => Assert.Equal(3, y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("1"));
			Assert.Equal(default, x.Accept("2"));
			Assert.Equal(default, x.Accept("3"));
			Assert.True(x.HaveValue);

			var oldValues = x.Value;
			Assert.Same(x.Value, x.ValueOr([1, 2, 3]));
			IniResult<List<int>> ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			x.Reset();
			Assert.Empty(x.Value);
			Assert.False(x.HaveValue);

			Assert.Collection(oldValues,
				x => Assert.Equal(1, x),
				x => Assert.Equal(2, x),
				x => Assert.Equal(3, x));

			Assert.Equal(default, x.Accept("4"));
			Assert.Equal(default, x.Accept("5"));
			Assert.True(x.HaveValue);
			Assert.Same(x.Value, x.ValueOr([1, 2, 3]));
			ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			Assert.Collection(x.Value,
				x => Assert.Equal(4, x),
				x => Assert.Equal(5, x));
		}
	}
}