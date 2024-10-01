namespace IniFileNet.Test
{
	using System;
	using Xunit;

	public static class IniUtilTests
	{
		[Fact]
		public static void ResetAll()
		{
			IniValueAcceptorSingle s1 = new();
			IniValueAcceptorSingle s2 = new();
			IniValueAcceptorSingle s3 = new();
			foreach (var v in new IIniValueAcceptor[] { s1, s2, s3 })
			{
				v.Accept("", "key", "v");
			}

			IniUtil.ResetAll(s1, s2, s3);
			Assert.Null(s1.Value);
			Assert.Null(s2.Value);
			Assert.Null(s3.Value);
		}
		[Fact]
		public static void GetFullKey()
		{
			Assert.Equal("foo.bar", IniUtil.GetFullKey("foo", "bar", "."));
			Assert.Equal("bar", IniUtil.GetFullKey(default, "bar", "."));
			Assert.Throws<ArgumentException>(() => IniUtil.GetFullKey("foo", default, "."));
		}
	}
}