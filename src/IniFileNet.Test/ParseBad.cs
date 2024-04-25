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

			c2.Error(IniErrorCode.EmptyKeyName);

			await Chk.CheckAllIniDictionaryReader(EmptyKeyEqualsIni, default, new IniError(IniErrorCode.EmptyKeyName, "Error at character 0 in stream. This is the data where the error was encountered: ="), []);
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

			c2.Error(IniErrorCode.EmptyKeyName);

			await Chk.CheckAllIniDictionaryReader(EmptyKeyColonIni, EmptyKeyColonOpt, new IniError(IniErrorCode.EmptyKeyName, "Error at character 0 in stream. This is the data where the error was encountered: :"), []);
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
			c.Next(IniContentType.Error, "Key");
			c.Next(IniContentType.Error, "Key");
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

			c2.Error(IniErrorCode.KeyDelimiterNotFound);

			await Chk.CheckAllIniDictionaryReader(KeyNoEqualsIni, default, new IniError(IniErrorCode.KeyDelimiterNotFound, "Error at character 3 in stream. This is the data where the error was encountered: Key"), []);
		}
		public const string SectionNotOnOwnLineIni = "[Section]Key=Value";
		[Fact]
		public static void SectionNotOnOwnLineSpan()
		{
			IniSpanReaderChecker c = new(SectionNotOnOwnLineIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.Error, "Key=Value");
			c.Next(IniContentType.Error, "Key=Value");
			c.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);
		}
		[Fact]
		public static async Task SectionNotOnOwnLineStream()
		{
			var (c1, c2) = Checks.For(SectionNotOnOwnLineIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Error, "Key=Value");
			await c1.Next(IniToken.Error, "Key=Value");
			c1.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			c2.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			await Chk.CheckAllIniDictionaryReader(SectionNotOnOwnLineIni, default, new IniError(IniErrorCode.SectionIsNotOnlyThingOnLine, "Error at character 9 in stream. This is the data where the error was encountered: Key=Value"), []);
		}
		public const string SemicolonInKeyNameIni = "[Section]\nKe;y=Value";
		[Fact]
		public static void SemicolonInKeyNameSpan()
		{
			IniSpanReaderChecker c = new(SemicolonInKeyNameIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Ke;y=Value");
			c.Next(IniContentType.Error, "Ke;y=Value");
			c.Error(IniErrorCode.SemicolonInKeyName);
		}
		[Fact]
		public static async Task SemicolonInKeyNameStream()
		{
			var (c1, c2) = Checks.For(SemicolonInKeyNameIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Error, "Ke;y=Value");
			await c1.Next(IniToken.Error, "Ke;y=Value");
			c1.Error(IniErrorCode.SemicolonInKeyName);

			c2.Error(IniErrorCode.SemicolonInKeyName);

			await Chk.CheckAllIniDictionaryReader(SemicolonInKeyNameIni, default, new IniError(IniErrorCode.SemicolonInKeyName, "Error at character 10 in stream. This is the data where the error was encountered: Ke;y=Value"), []);
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

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at character 0 in stream. This is the data where the error was encountered: Key=Value"), []);
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

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedNumbersignIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at character 0 in stream. This is the data where the error was encountered: #Key=Value"), []);
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

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);

			await Chk.CheckAllIniDictionaryReader(GlobalKeysNotAllowedColonIni, default, new IniError(IniErrorCode.GlobalKeyNotAllowed, "Error at character 0 in stream. This is the data where the error was encountered: :Key=Value"), []);
		}
		public const string SectionEndOfSpan1Ini = "[";
		[Fact]
		public static void SectionEndOfSpan1Span()
		{
			IniSpanReaderChecker c = new(SectionEndOfSpan1Ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "");
			c.Next(IniContentType.Error, "");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
		[Fact]
		public static async Task SectionEndOfSpan1Stream()
		{
			var (c1, c2) = Checks.For(SectionEndOfSpan1Ini, default);
			await c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(SectionEndOfSpan1Ini, default, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at character 0 in stream. This is the data where the error was encountered: "), []);
		}
		public const string SectionEndOfSpan2Ini = "[Section";
		[Fact]
		public static void SectionEndOfSpan2Span()
		{
			IniSpanReaderChecker c = new(SectionEndOfSpan2Ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "Section");
			c.Next(IniContentType.Error, "Section");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
		[Fact]
		public static async Task SectionEndOfSpan2Stream()
		{
			var (c1, c2) = Checks.For(SectionEndOfSpan2Ini, default);
			await c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			c2.Error(IniErrorCode.SectionCloseBracketNotFound);

			await Chk.CheckAllIniDictionaryReader(SectionEndOfSpan2Ini, default, new IniError(IniErrorCode.SectionCloseBracketNotFound, "Error at character 0 in stream. This is the data where the error was encountered: "), []);
		}
	}
}