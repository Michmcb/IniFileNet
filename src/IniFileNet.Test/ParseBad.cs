namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseBad
	{
		public const string EmptyKeyEqualsIni = "=";
		[Fact]
		public static void EmptyKeyEqualsSpan()
		{
			IniSpanReaderChecker c = new(EmptyKeyEqualsIni);
			c.Next(IniContentType.Error, "=");
			c.Next(IniContentType.Error, "=");
			c.Error(IniErrorCode.EmptyKeyName);
		}
		[Fact]
		public static async Task EmptyKeyEqualsStream()
		{
			var (c1, c2) = Checks.For(EmptyKeyEqualsIni, default);
			await c1.Next(IniToken.Error, "=");
			c1.Error(IniErrorCode.EmptyKeyName);

			await c2.Error(IniErrorCode.EmptyKeyName);

			await Chk.CheckAllIniDictionaryReader(EmptyKeyEqualsIni, default, new IniError(IniErrorCode.EmptyKeyName, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:="), []);
		}
		public const string EmptyKeyColonIni = ":";
		public static readonly IniReaderOptions EmptyKeyColonOpt = new(allowKeyDelimiterColon: true);
		[Fact]
		public static void EmptyKeyColonSpan()
		{
			IniSpanReaderChecker c = new(EmptyKeyColonIni, EmptyKeyColonOpt);
			c.Next(IniContentType.Error, ":");
			c.Next(IniContentType.Error, ":");
			c.Error(IniErrorCode.EmptyKeyName);
		}
		[Fact]
		public static async Task EmptyKeyColonStream()
		{
			var (c1, c2) = Checks.For(EmptyKeyColonIni, EmptyKeyColonOpt);
			await c1.Next(IniToken.Error, ":");
			c1.Error(IniErrorCode.EmptyKeyName);

			await c2.Error(IniErrorCode.EmptyKeyName);

			await Chk.CheckAllIniDictionaryReader(EmptyKeyColonIni, EmptyKeyColonOpt, new IniError(IniErrorCode.EmptyKeyName, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered::"), []);
		}
		public const string KeyNoEqualsIni = "[s]\nKey";
		[Fact]
		public static void KeyNoEqualsSpan()
		{
			IniSpanReaderChecker c = new(KeyNoEqualsIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "s");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "[s]\nKey");
			c.Next(IniContentType.Error, "[s]\nKey");
			c.Error(IniErrorCode.KeyDelimiterNotFound);
		}
		[Fact]
		public static async Task KeyNoEqualsStream()
		{
			var (c1, c2) = Checks.For(KeyNoEqualsIni, default);
			await c1.Next(IniToken.Section, "s");
			await c1.Next(IniToken.Error, "Key");
			await c1.Next(IniToken.Error, "Key");
			c1.Error(IniErrorCode.KeyDelimiterNotFound);

			await c2.Error(IniErrorCode.KeyDelimiterNotFound);

			await Chk.CheckAllIniDictionaryReader(KeyNoEqualsIni, default, new IniError(IniErrorCode.KeyDelimiterNotFound, "Error at char 4 in stream, char 0 in block. This is the block in which the error was encountered:Key"), []);
		}
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
		public const string GlobalKeysNotAllowedIni = "Key=Value";
		[Fact]
		public static void GlobalKeysNotAllowedSpan()
		{
			IniSpanReaderChecker c = new(GlobalKeysNotAllowedIni);
			c.Next(IniContentType.Error, "Key=Value");
			c.Next(IniContentType.Error, "Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static async Task GlobalKeysNotAllowedStream()
		{
			var (c1, c2) = Checks.For(GlobalKeysNotAllowedIni, default);
			await c1.Next(IniToken.Error, "Key=Value");
			await c1.Next(IniToken.Error, "Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			await c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:Key=Value"), []);
		}
		public const string GlobalKeysNotAllowedNumbersignIni = "#Key=Value";
		[Fact]
		public static void GlobalKeysNotAllowedNumbersignSpan()
		{
			IniSpanReaderChecker c = new(GlobalKeysNotAllowedNumbersignIni);
			c.Next(IniContentType.Error, "#Key=Value");
			c.Next(IniContentType.Error, "#Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static async Task GlobalKeysNotAllowedNumbersignStream()
		{
			var (c1, c2) = Checks.For(GlobalKeysNotAllowedNumbersignIni, default);
			await c1.Next(IniToken.Error, "#Key=Value");
			await c1.Next(IniToken.Error, "#Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			await c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedNumbersignIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:#Key=Value"), []);
		}
		public const string GlobalKeysNotAllowedColonIni = ":Key=Value";
		[Fact]
		public static void GlobalKeysNotAllowedColonSpan()
		{
			IniSpanReaderChecker c = new(GlobalKeysNotAllowedColonIni);
			c.Next(IniContentType.Error, ":Key=Value");
			c.Next(IniContentType.Error, ":Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static async Task GlobalKeysNotAllowedColonStream()
		{
			var (c1, c2) = Checks.For(GlobalKeysNotAllowedColonIni, default);
			await c1.Next(IniToken.Error, ":Key=Value");
			await c1.Next(IniToken.Error, ":Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			await c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedColonIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered::Key=Value"), []);
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
			await c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			await c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(SectionEndOfSpan2Ini, default, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at char 8 in stream, char 0 in block. This is the block in which the error was encountered:"), []);
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
		public const string BadKeyEscapeSequenceIni = "F\\xoo=Bar";
		public static readonly IniReaderOptions BadKeyEscapeSequenceOpt = new(allowGlobalKeys: true);
		[Fact]
		public static async Task BadKeyEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadKeyEscapeSequenceIni, BadKeyEscapeSequenceOpt);
			await c1.Next(IniToken.Error, "Error unescaping at char 0 in stream, char 0 in block. Invalid escape sequence at index 1 of text:F\\xoo");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadKeyEscapeSequenceIni, BadKeyEscapeSequenceOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:F\\xoo"), []);
		}
		public const string BadValueEscapeSequenceIni = "Foo=B\\xar";
		public static readonly IniReaderOptions BadValueEscapeSequenceOpt = new(allowGlobalKeys: true);
		[Fact]
		public static async Task BadValueEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadValueEscapeSequenceIni, BadValueEscapeSequenceOpt);
			await c1.Next(IniToken.Key, "Foo");
			await c1.Next(IniToken.Error, "Error unescaping at char 4 in stream, char 4 in block. Invalid escape sequence at index 1 of text:B\\xar");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadValueEscapeSequenceIni, BadValueEscapeSequenceOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:B\\xar"), []);
		}
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
	}
}