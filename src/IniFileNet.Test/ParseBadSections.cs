namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;
	public static class ParseBadSections
	{
		public const string SectionNotOnOwnLineIni = "[Section]Key=Value";
		[Fact]
		public static void SectionNotOnOwnLineSpan()
		{
			IniSpanReaderChecker c = new(SectionNotOnOwnLineIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.Error, "[Section]Key=Value");
			c.Next(IniContentType.Error, "[Section]Key=Value");
			c.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);
		}
		[Fact]
		public static async Task SectionNotOnOwnLineStream()
		{
			var (c1, c2) = Checks.For(SectionNotOnOwnLineIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Error, "[Section]Key=Value");
			await c1.Next(IniToken.Error, "[Section]Key=Value");
			c1.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			await c2.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			await Chk.CheckAllIniDictionaryReader(SectionNotOnOwnLineIni, default, new IniError(IniErrorCode.SectionIsNotOnlyThingOnLine, "Error at char 9 in stream, char 9 in block. This is the block in which the error was encountered:[Section]Key=Value"), []);
		}
		public const string SectionEndOfSpan1Ini = "[";
		[Fact]
		public static void SectionEndOfSpan1Span()
		{
			IniSpanReaderChecker c = new(SectionEndOfSpan1Ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "[");
			c.Next(IniContentType.Error, "[");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
		[Fact]
		public static async Task SectionEndOfSpan1Stream()
		{
			var (c1, c2) = Checks.For(SectionEndOfSpan1Ini, default);
			await c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			await c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(SectionEndOfSpan1Ini, default, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at char 1 in stream, char 0 in block. This is the block in which the error was encountered:"), []);
		}
		public const string SectionEndOfSpan2Ini = "[Section";
		[Fact]
		public static void SectionEndOfSpan2Span()
		{
			IniSpanReaderChecker c = new(SectionEndOfSpan2Ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "[Section");
			c.Next(IniContentType.Error, "[Section");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
		[Fact]
		public static async Task SectionEndOfSpan2Stream()
		{
			var (c1, c2) = Checks.For(SectionEndOfSpan2Ini, default);
			await c1.Next(IniToken.Error, "Section");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			await c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(SectionEndOfSpan2Ini, default, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at char 1 in stream, char 0 in block. This is the block in which the error was encountered:Section"), []);
		}
		public const string BadSectionEscapeSequence = "[F\\xoo]";
		[Fact]
		public static async Task BadSectionEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadSectionEscapeSequence, default);
			await c1.Next(IniToken.Error, "Error unescaping at char 0 in stream, char 1 in block. Invalid escape sequence at index 1 of text:F\\xoo");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadSectionEscapeSequence, default, new IniError(IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:F\\xoo"), []);
		}
		public const string TrailingSlashSectionIni = "[Section\\";
		public static readonly IniReaderOptions TrailingSlashSectionOpt = new();
		[Fact]
		public static void TrailingSlashSectionSpan()
		{
			IniSpanReaderChecker c = new(TrailingSlashSectionIni, TrailingSlashSectionOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "[Section\\");
			c.Next(IniContentType.Error, "[Section\\");
		}
		[Fact]
		public static async Task TrailingSlashSectionStream()
		{
			var (c1, c2) = Checks.For(TrailingSlashSectionIni, TrailingSlashSectionOpt);
			await c1.Next(IniToken.Error, "Section\\");
			await c1.Next(IniToken.Error, "Section\\");

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(TrailingSlashSectionIni, TrailingSlashSectionOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Error at char 1 in stream, char 0 in block. This is the block in which the error was encountered:Section\\"), []);
		}
		public const string EscapedEndBracketSectionIni = "[Section\\]";
		public static readonly IniReaderOptions EscapedEndBracketSectionOpt = new();
		[Fact]
		public static void EscapedEndBracketSectionSpan()
		{
			IniSpanReaderChecker c = new(EscapedEndBracketSectionIni, EscapedEndBracketSectionOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "[Section\\]");
			c.Next(IniContentType.Error, "[Section\\]");
		}
		[Fact]
		public static async Task EscapedEndBracketSectionStream()
		{
			var (c1, c2) = Checks.For(EscapedEndBracketSectionIni, EscapedEndBracketSectionOpt);
			await c1.Next(IniToken.Error, "Section\\]");
			await c1.Next(IniToken.Error, "Section\\]");

			await c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(EscapedEndBracketSectionIni, EscapedEndBracketSectionOpt, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at char 1 in stream, char 0 in block. This is the block in which the error was encountered:Section\\]"), []);
		}
	}
}