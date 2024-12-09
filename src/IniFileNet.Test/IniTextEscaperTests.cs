namespace IniFileNet.Test
{
	using IniFileNet;
	using IniFileNet.IO;
	using System.IO;
	using Xunit;

	public static class IniTextEscaperTests
	{
		//[Fact]
		//public static async Task SomeText()
		//{
		//	IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
		//	buf.WriteValidText("foo bar baz");
		//	Assert.Equal("foo bar baz", buf.ToString());
		//	Assert.Equal("foo bar baz", buf.Span);
		//	Assert.Equal("foo bar baz", buf.Memory.Span);
		//	buf.Clear();
		//	Assert.Empty(buf.ToString());
		//
		//	using (StringWriter sw = new())
		//	{
		//		buf.WriteValidText("hello world");
		//		buf.WriteTo(sw);
		//		Assert.Equal("hello world", sw.ToString());
		//		Assert.Empty(buf.ToString());
		//	}
		//	using (StringWriter sw = new())
		//	{
		//		buf.WriteValidText("hello again world");
		//		await buf.WriteToAsync(sw);
		//		Assert.Equal("hello again world", sw.ToString());
		//		Assert.Empty(buf.ToString());
		//	}
		//}
		private static void CheckEscape(string text, IniTokenContext context, string expected)
		{
			StringWriter writer = new();
			OperationStatusMsg op = IniTextEscaperWriter.Escape(text, 1024, DefaultIniTextEscaper.Default, context, writer);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal(expected, writer.ToString());
		}
		private static void CheckUnescape(string text, IniTokenContext context, string expected)
		{
			StringWriter writer = new();
			OperationStatusMsg op = IniTextEscaperWriter.Unescape(text, 1024, DefaultIniTextEscaper.Default, context, writer);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal(expected, writer.ToString());
		}
		private static void CheckBadUnescape(string text, IniTokenContext context, string? expectedMsg)
		{
			StringWriter writer = new();
			OperationStatusMsg op = IniTextEscaperWriter.Unescape(text, 1024, DefaultIniTextEscaper.Default, context, writer);
			Assert.NotEqual(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Equal(op.Msg, expectedMsg);
		}
		[Fact]
		public static void GoodEscapes()
		{
			CheckEscape("F\\oo", IniTokenContext.Value, "F\\\\oo");
			CheckEscape("F\roo", IniTokenContext.Value, "F\\roo");
			CheckEscape("Foo\n", IniTokenContext.Value, "Foo\\n");
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
		//	IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
		//	Chk.IniError(IniErrorCode.CannotEscapeCharacter, "Invalid escape sequence at index 1 of text:F\0oo", buf.Escape("F\0oo", IniTokenContext.Value));
		//}
		[Fact]
		public static void GoodUnescapes()
		{
			CheckUnescape("F\\\\oo", IniTokenContext.Value, "F\\oo");
			CheckUnescape("F\\roo", IniTokenContext.Value, "F\roo");
			CheckUnescape("Foo\\n", IniTokenContext.Value, "Foo\n");
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
			CheckBadUnescape("Foo\\", IniTokenContext.Value, "Invalid escape sequence at index 3 of text:Foo\\");
			CheckBadUnescape("F\\xoo", IniTokenContext.Value, "Invalid escape sequence at index 1 of text:F\\xoo");
		}
	}
}