namespace IniFileNet.Test
{
	using System;
	using Xunit;
	using IniFileNet;
	using System.Buffers;
	using IniFileNet.IO;
	using System.IO;

	public static class DefaultIniTextEscaperTests
	{
		[Fact]
		public static void BadIniWriteContent()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultIniTextEscaper(false).Escape("text", new char[16], (IniTokenContext)(-123), out _, out _, false));
		}
		[Fact]
		public static void Escape_DestinationTooSmall_NoEscapes()
		{
			DefaultIniTextEscaper e = new(false);

			Span<char> output = new char[2];
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("text", output, IniTokenContext.Value, out int c, out int w, isFinalBlock: true));
			Assert.Equal(2, c);
			Assert.Equal(2, w);
			Assert.Equal("te", output);
			Chk.OperationStatusMsg(OperationStatus.Done, null, e.Escape("xt", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("xt", output);
			Assert.Equal(2, c);
			Assert.Equal(2, w);
		}
		[Fact]
		public static void Escape_DestinationTooSmall_Escapes()
		{
			DefaultIniTextEscaper e = new(false);
			Span<char> output = new char[2];
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("t\next\n", output, IniTokenContext.Value, out int c, out int w, isFinalBlock: true));
			Assert.Equal('t', output[0]);
			Assert.Equal(1, c);
			Assert.Equal(1, w);
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("\next\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("\\n", output);
			Assert.Equal(1, c);
			Assert.Equal(2, w);
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("ext\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("ex", output);
			Assert.Equal(2, c);
			Assert.Equal(2, w);
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("t\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal('t', output[0]);
			Assert.Equal(1, c);
			Assert.Equal(1, w);
			Chk.OperationStatusMsg(OperationStatus.Done, null, e.Escape("\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("\\n", output);
			Assert.Equal(1, c);
			Assert.Equal(2, w);
		}
		[Fact]
		public static void Escape_NeedMoreData()
		{
			DefaultIniTextEscaper e = new(false);
			Span<char> output = new char[2];
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Escape("f\n", output, IniTokenContext.Value, out int c, out int w, isFinalBlock: false));
			Assert.Equal('f', output[0]);
			Assert.Equal(1, c);
			Assert.Equal(1, w);
			Chk.OperationStatusMsg(OperationStatus.NeedMoreData, null, e.Escape("\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: false));
			Assert.Equal("\\n", output);
			Assert.Equal(1, c);
			Assert.Equal(2, w);
			Chk.OperationStatusMsg(OperationStatus.NeedMoreData, null, e.Escape("ok", output, IniTokenContext.Value, out c, out w, isFinalBlock: false));
			Assert.Equal("ok", output);
			Assert.Equal(2, c);
			Assert.Equal(2, w);
		}
		[Fact]
		public static void Unescape_DestinationTooSmall_NoEscapes()
		{
			DefaultIniTextEscaper e = new(false);

			Span<char> output = new char[2];
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Unescape("text", output, IniTokenContext.Value, out int c, out int w, isFinalBlock: true));
			Assert.Equal(2, c);
			Assert.Equal(2, w);
			Assert.Equal("te", output);
			Chk.OperationStatusMsg(OperationStatus.Done, null, e.Unescape("xt", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("xt", output);
			Assert.Equal(2, c);
			Assert.Equal(2, w);
		}
		[Fact]
		public static void Unescape_DestinationTooSmall_Escapes()
		{
			DefaultIniTextEscaper e = new(false);

			Span<char> output = new char[2];
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Unescape("t\\next\\n", output, IniTokenContext.Value, out int c, out int w, isFinalBlock: true));
			Assert.Equal("t\n", output);
			Assert.Equal(3, c);
			Assert.Equal(2, w);
			Chk.OperationStatusMsg(OperationStatus.DestinationTooSmall, null, e.Unescape("ext\\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("ex", output);
			Assert.Equal(2, c);
			Assert.Equal(2, w);
			Chk.OperationStatusMsg(OperationStatus.Done, null, e.Unescape("t\\n", output, IniTokenContext.Value, out c, out w, isFinalBlock: true));
			Assert.Equal("t\n", output);
			Assert.Equal(3, c);
			Assert.Equal(2, w);
		}
	}
}