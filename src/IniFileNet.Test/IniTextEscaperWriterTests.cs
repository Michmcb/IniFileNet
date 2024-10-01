namespace IniFileNet.Test
{
	using Xunit;
	using IniFileNet;
	using IniFileNet.IO;
	using System.IO;

	public static class IniTextEscaperWriterTests
	{
		[Fact]
		public static void EscapeVariants()
		{
			StringWriter sw = new();
			IniTextEscaperWriter e = new(new DefaultIniTextEscaper(false), sw, 2);
			Assert.True(e.Escape("t\next\n", IniTokenContext.Value, out string? errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\\next\\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			Assert.True(e.Escape("t\next\n", new char[3], IniTokenContext.Value, out errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\\next\\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			Assert.True(e.StackEscape("t\next\n", IniTokenContext.Value, out errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\\next\\n", sw.ToString());
		}
		[Fact]
		public static void UnescapeVariants()
		{
			StringWriter sw = new();
			IniTextEscaperWriter e = new(new DefaultIniTextEscaper(false), sw, 2);
			Assert.True(e.Unescape("t\\next\\n", IniTokenContext.Value, out string? errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\next\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			Assert.True(e.Unescape("t\\next\\n", new char[3], IniTokenContext.Value, out errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\next\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			Assert.True(e.StackUnescape("t\\next\\n", IniTokenContext.Value, out errMsg));
			Assert.Null(errMsg);
			Assert.Equal("t\next\n", sw.ToString());
		}
	}
}