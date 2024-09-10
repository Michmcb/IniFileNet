namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;

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
		[Fact]
		public static void GoodEscapes()
		{
			throw new NotImplementedException();
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
		}
		[Fact]
		public static void BadEscapes()
		{
			throw new NotImplementedException();
			IniTextEscaperBuffer buf = new(new(), DefaultIniTextEscaper.Default);
		}
	}
}