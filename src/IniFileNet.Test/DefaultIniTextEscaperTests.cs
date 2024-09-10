namespace IniFileNet.Test
{
	using System;
	using Xunit;
	using IniFileNet.IO;

	public static class DefaultIniTextEscaperTests
	{
		[Fact]
		public static void BadIniWriteContent()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new DefaultIniTextEscaper(false).Escape("text", TextCallback, CharCallback, (IniTokenContext)(-123)));
		}
		private static void TextCallback(ReadOnlySpan<char> text)
		{
			throw new NotImplementedException();
		}
		private static void CharCallback(char c)
		{
			throw new NotImplementedException();
		}
	}
}