namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseBadComments
	{
		public const string BadCommentEscapeSequenceIni = ";F\\xoo";
		[Fact]
		public static async Task BadCommentEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadCommentEscapeSequenceIni, default);
			await c1.Next(IniToken.Error, "Error unescaping at char 0 in stream, char 1 in block. Invalid escape sequence at index 1 of text:F\\xoo");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadCommentEscapeSequenceIni, default, new IniError(IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:F\\xoo"), []);
		}
		public const string TrailingSlashCommentIni = ";Foo\\";
		public static readonly IniReaderOptions TrailingSlashCommentOpt = new(allowLineContinuations: false, ignoreCommentEscapes: false);
		[Fact]
		public static void TrailingSlashCommentSpan()
		{
			IniSpanReaderChecker c = new(TrailingSlashCommentIni, TrailingSlashCommentOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Error, ";Foo\\");
			c.Next(IniContentType.Error, ";Foo\\");
		}
		[Fact]
		public static async Task TrailingSlashCommentStream()
		{
			var (c1, c2) = Checks.For(TrailingSlashCommentIni, TrailingSlashCommentOpt);
			await c1.Next(IniToken.Error, "\\");
			await c1.Next(IniToken.Error, "\\");

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(TrailingSlashCommentIni, TrailingSlashCommentOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Error at char 5 in stream, char 0 in block. This is the block in which the error was encountered:\\"), []);
		}
	}
}