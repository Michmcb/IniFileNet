namespace IniFileNet.Test
{
	using Xunit;
	using IniFileNet;
	using IniFileNet.IO;
	using System.IO;
	using System;
	using System.Threading.Tasks;

	public static class IniTextEscaperWriterTests
	{
		[Fact]
		public static async Task EscapeVariants()
		{
			DefaultIniTextEscaper escaper = new DefaultIniTextEscaper(false);
			StringWriter sw = new();
			var op = IniTextEscaperWriter.Escape("t\next\n", 2, escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\\next\\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = IniTextEscaperWriter.Escape("t\next\n", new char[3], escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\\next\\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = await IniTextEscaperWriter.EscapeAsync("t\next\n".AsMemory(), new char[3], escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\\next\\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = await IniTextEscaperWriter.EscapeAsync("t\next\n".AsMemory(), new char[3].AsMemory(), escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\\next\\n", sw.ToString());
		}
		[Fact]
		public static async Task UnescapeVariants()
		{
			DefaultIniTextEscaper escaper = new (false);
			StringWriter sw = new();
			var op = IniTextEscaperWriter.Unescape("t\\next\\n", 2, escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\next\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = IniTextEscaperWriter.Unescape("t\\next\\n", new char[3], escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\next\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = await IniTextEscaperWriter.UnescapeAsync("t\\next\\n".AsMemory(), new char[3], escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\next\n", sw.ToString());
			sw.GetStringBuilder().Clear();

			op = await IniTextEscaperWriter.UnescapeAsync("t\\next\\n".AsMemory(), new char[3].AsMemory(), escaper, IniTokenContext.Value, sw);
			Assert.Equal(System.Buffers.OperationStatus.Done, op.Status);
			Assert.Null(op.Msg);
			Assert.Equal("t\next\n", sw.ToString());
		}
	}
}