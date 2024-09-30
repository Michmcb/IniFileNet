namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseBadKeys
	{
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
		public const string BadKeyEscapeSequenceIni = "F\\xoo=Bar";
		public static readonly IniReaderOptions BadKeyEscapeSequenceOpt = new(allowGlobalKeys: true);
		[Fact]
		public static async Task BadKeyEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadKeyEscapeSequenceIni, BadKeyEscapeSequenceOpt);
			await c1.Next(IniToken.Error, "Error unescaping at char 0 in stream, char 0 in block. Invalid escape sequence at index 1 of text:F\\xoo");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadKeyEscapeSequenceIni, BadKeyEscapeSequenceOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Error unescaping at char 0 in stream, char 0 in block. Invalid escape sequence at index 1 of text:F\\xoo"), []);
		}
		
		public const string TrailingSlashKeyIni = "Key\\";
		public static readonly IniReaderOptions TrailingSlashKeyOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void TrailingSlashKeySpan()
		{
			IniSpanReaderChecker c = new(TrailingSlashKeyIni, TrailingSlashKeyOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Key\\");
			c.Next(IniContentType.Error, "Key\\");
		}
		[Fact]
		public static async Task TrailingSlashKeyStream()
		{
			var (c1, c2) = Checks.For(TrailingSlashKeyIni, TrailingSlashKeyOpt);
			await c1.Next(IniToken.Error, "Key\\");
			await c1.Next(IniToken.Error, "Key\\");

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(TrailingSlashKeyIni, TrailingSlashKeyOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:Key\\"), []);
		}
		public const string EscapedEqualsIni = "Key\\=";
		public static readonly IniReaderOptions EscapedEqualsOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapedEqualsSpan()
		{
			IniSpanReaderChecker c = new(EscapedEqualsIni, EscapedEqualsOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Key\\=");
			c.Next(IniContentType.Error, "Key\\=");
		}
		[Fact]
		public static async Task EscapedEqualsStream()
		{
			var (c1, c2) = Checks.For(EscapedEqualsIni, EscapedEqualsOpt);
			await c1.Next(IniToken.Error, "Key\\=");
			await c1.Next(IniToken.Error, "Key\\=");

			await c2.Error(IniErrorCode.KeyDelimiterNotFound);

			await Chk.CheckAllIniDictionaryReader(EscapedEqualsIni, EscapedEqualsOpt, new IniError(IniErrorCode.KeyDelimiterNotFound, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:Key\\="), []);
		}
		public const string EscapedColonIni = "Key\\:";
		public static readonly IniReaderOptions EscapedColonOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapedColonSpan()
		{
			IniSpanReaderChecker c = new(EscapedColonIni, EscapedColonOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Key\\:");
			c.Next(IniContentType.Error, "Key\\:");
		}
		[Fact]
		public static async Task EscapedColonStream()
		{
			var (c1, c2) = Checks.For(EscapedColonIni, EscapedColonOpt);
			await c1.Next(IniToken.Error, "Key\\:");
			await c1.Next(IniToken.Error, "Key\\:");

			await c2.Error(IniErrorCode.KeyDelimiterNotFound);

			await Chk.CheckAllIniDictionaryReader(EscapedColonIni, EscapedColonOpt, new IniError(IniErrorCode.KeyDelimiterNotFound, "Error at char 0 in stream, char 0 in block. This is the block in which the error was encountered:Key\\:"), []);
		}
	}
}