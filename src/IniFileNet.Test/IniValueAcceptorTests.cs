﻿namespace IniFileNet.Test
{
	using System.Collections.Generic;
	using Xunit;
	public static class IniValueAcceptorTests
	{
		[Fact]
		public static void Single()
		{
			IniValueAcceptorSingle x = new("k");
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "value"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. This Section: \"s2\" This Key: \"k2\". Last Section: \"s1\" Expected Key: \"k\". Value is: \"value\""), x.Accept("s2", "k2", "value"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HasValue);
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Null(x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "value"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value", x.Value);
			Assert.Equal("value", x.ValueOr("foo"));
			Chk.IniResult("value", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value", x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void SingleParsed()
		{
			IniValueAcceptorSingle<int> x = new("k", IniParse.Int32);
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "10"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"k2\". Value is: \"15\""), x.Accept("s2", "k2", "15"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.True(x.HasValue);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(0, x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "20"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void OnlyFirst()
		{
			IniValueAcceptorOnlyFirst x = new("k");
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "value1"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(default, x.Accept("s2", "k2", "value2"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HasValue);
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Null(x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "value1"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void OnlyFirstParsed()
		{
			IniValueAcceptorOnlyFirst<int> x = new("k", IniParse.Int32);
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "10"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(default, x.Accept("s2", "k2", "15"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HasValue);
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(0, x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "20"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void OnlyLast()
		{
			IniValueAcceptorOnlyLast x = new("k");
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("foo", x.ValueOr("foo"));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "value1"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(default, x.Accept("s2", "k2", "value2"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value2", x.Value);
			Assert.Equal("value2", x.ValueOr("foo"));
			Chk.IniResult("value2", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value2", x.ValueOrException());
			Assert.True(x.HasValue);
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Null(x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "value1"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal("value1", x.Value);
			Assert.Equal("value1", x.ValueOr("foo"));
			Chk.IniResult("value1", IniErrorCode.None, null, x.ValueOrError());
			Assert.Equal("value1", x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void OnlyLastParsed()
		{
			IniValueAcceptorOnlyLast<int> x = new("kk", IniParse.Int32);
			Assert.Equal("", x.Section);
			Assert.Equal("kk", x.Key);
			Assert.Equal(5, x.ValueOr(5));
			Chk.IniResult(0, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(() => x.ValueOrException()).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "10"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("kk", x.Key);
			Assert.Equal(10, x.Value);
			Assert.Equal(10, x.ValueOr(5));
			Chk.IniResult(10, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(10, x.ValueOrException());
			Assert.True(x.HasValue);
			Assert.Equal(default, x.Accept("s2", "k2", "15"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("kk", x.Key);
			Assert.Equal(15, x.Value);
			Assert.Equal(15, x.ValueOr(5));
			Chk.IniResult(15, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(15, x.ValueOrException());
			Assert.True(x.HasValue);
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("kk", x.Key);
			Assert.Equal(0, x.Value);
			Assert.False(x.HasValue);
			Assert.Equal(default, x.Accept("s3", "k3", "20"));
			Assert.Equal("s3", x.Section);
			Assert.Equal("kk", x.Key);
			Assert.Equal(20, x.Value);
			Assert.Equal(20, x.ValueOr(5));
			Chk.IniResult(20, IniErrorCode.None, x.ValueOrError());
			Assert.Equal(20, x.ValueOrException());
			Assert.True(x.HasValue);
		}
		[Fact]
		public static void Many()
		{
			IniValueAcceptorMany xx = new("k", 8);
			Assert.Equal("", xx.Section);
			Assert.Equal("k", xx.Key);
			Assert.Collection(xx.ValueOr(["1", "2", "3"]), y => Assert.Equal("1", y), y => Assert.Equal("2", y), y => Assert.Equal("3", y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, xx.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(xx.ValueOrException).IniErrorCode);
			Assert.Equal(8, xx.Value.Capacity);

			IniValueAcceptorMany x = new("k");
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Collection(x.ValueOr(["1", "2", "3"]), y => Assert.Equal("1", y), y => Assert.Equal("2", y), y => Assert.Equal("3", y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "value1"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s1", "k1", "value2"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s1", "k1", "value3"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.True(x.HasValue);

			List<string> oldValues = x.Value;
			Assert.Same(x.Value, x.ValueOr(["1", "2", "3"]));
			IniResult<List<string>> ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Empty(x.Value);
			Assert.False(x.HasValue);

			Assert.Collection(oldValues,
				x => Assert.Equal("value1", x),
				x => Assert.Equal("value2", x),
				x => Assert.Equal("value3", x));

			Assert.Equal(default, x.Accept("s2", "k2", "value4"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s2", "k2", "value5"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("k", x.Key);
			Assert.True(x.HasValue);
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
			IniValueAcceptorMany<int, List<int>> x = new("k", IniParse.Int32);
			Assert.Equal("", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Collection(x.ValueOr([1, 2, 3]), y => Assert.Equal(1, y), y => Assert.Equal(2, y), y => Assert.Equal(3, y));
			Chk.IniResult(null, IniErrorCode.ValueMissing, x.ValueOrError());
			Assert.Equal(IniErrorCode.ValueMissing, Assert.ThrowsAny<IniException>(x.ValueOrException).IniErrorCode);
			Assert.Equal(default, x.Accept("s1", "k1", "1"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s1", "k1", "2"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s1", "k1", "3"));
			Assert.Equal("s1", x.Section);
			Assert.Equal("k", x.Key);
			Assert.True(x.HasValue);

			var oldValues = x.Value;
			Assert.Same(x.Value, x.ValueOr([1, 2, 3]));
			IniResult<List<int>> ir = x.ValueOrError();
			Assert.Same(x.Value, ir.Value);
			Chk.IniError(IniErrorCode.None, null, ir.Error);
			Assert.Same(x.Value, x.ValueOrException());
			x.Reset();
			Assert.Empty(x.Section);
			Assert.Equal("k", x.Key);
			Assert.Empty(x.Value);
			Assert.False(x.HasValue);

			Assert.Collection(oldValues,
				x => Assert.Equal(1, x),
				x => Assert.Equal(2, x),
				x => Assert.Equal(3, x));

			Assert.Equal(default, x.Accept("s2", "k2", "4"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("k", x.Key);
			Assert.Equal(default, x.Accept("s2", "k2", "5"));
			Assert.Equal("s2", x.Section);
			Assert.Equal("k", x.Key);
			Assert.True(x.HasValue);
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