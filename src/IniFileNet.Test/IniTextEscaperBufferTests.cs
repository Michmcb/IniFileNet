namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;
	using static System.Net.Mime.MediaTypeNames;

	public static class IniTextEscaperBufferTests
	{
		[Fact]
		public static async Task SomeText()
		{
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
			buf.WriteValidText("foo bar baz");
			Assert.Equal("foo bar baz", buf.ToString());
			Assert.Equal("foo bar baz", buf.Span);
			Assert.Equal("foo bar baz", buf.Memory.Span);
			buf.Clear();
			Assert.Empty(buf.ToString());

			using (StringWriter sw = new())
			{
				buf.WriteValidText("hello world");
				buf.WriteTo(sw);
				Assert.Equal("hello world", sw.ToString());
				Assert.Empty(buf.ToString());
			}
			using (StringWriter sw = new())
			{
				buf.WriteValidText("hello again world");
				await buf.WriteToAsync(sw);
				Assert.Equal("hello again world", sw.ToString());
				Assert.Empty(buf.ToString());
			}
		}
		private static void CheckEscape(string text, IniTokenContext context, string expected)
		{
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
			buf.Escape(text, context).ThrowIfError();
			Assert.Equal(expected, buf.ToString());
		}
		private static void CheckUnescape(string text, IniTokenContext context, string expected)
		{
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
			buf.Unescape(text, context).ThrowIfError();
			Assert.Equal(expected, buf.ToString());
		}
		private static void CheckBadUnescape(string text, IniTokenContext context, IniErrorCode expectedCode, string? expectedMsg)
		{
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
			Chk.IniError(expectedCode, expectedMsg, buf.Unescape(text, context));
		}
		[Fact]
		public static void GoodEscapes()
		{
			CheckEscape("F\\oo", IniTokenContext.Value, "F\\\\oo");
			CheckEscape("F\0oo", IniTokenContext.Value, "F\\0oo");
			CheckEscape("F\aoo", IniTokenContext.Value, "F\\aoo");
			CheckEscape("F\boo", IniTokenContext.Value, "F\\boo");
			CheckEscape("F\roo", IniTokenContext.Value, "F\\roo");
			CheckEscape("F\noo", IniTokenContext.Value, "F\\noo");
			CheckEscape("F=oo", IniTokenContext.Value, "F\\=oo");
			CheckEscape("F:oo", IniTokenContext.Value, "F\\:oo");
			CheckEscape("F]oo", IniTokenContext.Value, "F\\]oo");
			CheckEscape("F[oo", IniTokenContext.Value, "F\\[oo");
			CheckEscape("F;oo", IniTokenContext.Value, "F\\;oo");
			CheckEscape("F#oo", IniTokenContext.Value, "F\\#oo");
		}
		//[Fact]
		//public static void BadEscapes()
		//{
		//	 // TODO Bad escape sequences
		//}
		[Fact]
		public static void GoodUnescapes()
		{
			CheckUnescape("F\\\\oo", IniTokenContext.Value, "F\\oo");
			CheckUnescape("F\\0oo", IniTokenContext.Value, "F\0oo");
			CheckUnescape("F\\aoo", IniTokenContext.Value, "F\aoo");
			CheckUnescape("F\\boo", IniTokenContext.Value, "F\boo");
			CheckUnescape("F\\roo", IniTokenContext.Value, "F\roo");
			CheckUnescape("F\\noo", IniTokenContext.Value, "F\noo");
			CheckUnescape("F\\=oo", IniTokenContext.Value, "F=oo");
			CheckUnescape("F\\:oo", IniTokenContext.Value, "F:oo");
			CheckUnescape("F\\]oo", IniTokenContext.Value, "F]oo");
			CheckUnescape("F\\[oo", IniTokenContext.Value, "F[oo");
			CheckUnescape("F\\;oo", IniTokenContext.Value, "F;oo");
			CheckUnescape("F\\#oo", IniTokenContext.Value, "F#oo");
		}
		[Fact]
		public static void BadUnescapes()
		{
			CheckBadUnescape("Foo\\", IniTokenContext.Value, IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 3 of text:Foo\\");
			CheckBadUnescape("F\\xoo", IniTokenContext.Value, IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:F\\xoo");
		}
	}
}