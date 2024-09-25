namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;
	public static class ParseGoodKeys
	{
		private const string LocalKeyValueIni = "[Section]\nKey=Value";
		private static readonly IniReaderOptions LocalKeyValueOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void LocalKeyValueSpan()
		{
			IniSpanReaderChecker c = new(LocalKeyValueIni, LocalKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LocalKeyValueStream()
		{
			var (c1, c2) = Checks.For(LocalKeyValueIni, LocalKeyValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LocalKeyValueIni, LocalKeyValueOpt, default, [x => Assert.Equal(new("Section.Key", "Value"), x)]);
		}
		private const string LocalKeyColonValueIni = "[Section]\nKey:Value";
		private static readonly IniReaderOptions LocalKeyColonValueOpt = new(allowGlobalKeys: false, allowKeyDelimiterColon: true);
		[Fact]
		public static void LocalKeyColonValueSpan()
		{
			IniSpanReaderChecker c = new(LocalKeyColonValueIni, LocalKeyColonValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, ":");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LocalKeyColonValueStream()
		{
			var (c1, c2) = Checks.For(LocalKeyColonValueIni, LocalKeyColonValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LocalKeyColonValueIni, LocalKeyColonValueOpt, default, [x => Assert.Equal(new("Section.Key", "Value"), x)]);
		}
		private const string LocalKeyWithEndBracketValueIni = "[Section]\n]Key=Value";
		private static readonly IniReaderOptions LocalKeyWithEndBracketValueOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void LocalKeyWithEndBracketValueSpan()
		{
			IniSpanReaderChecker c = new(LocalKeyWithEndBracketValueIni, LocalKeyWithEndBracketValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "]Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LocalKeyWithEndBracketValueStream()
		{
			var (c1, c2) = Checks.For(LocalKeyWithEndBracketValueIni, LocalKeyWithEndBracketValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "]Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("]Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LocalKeyWithEndBracketValueIni, LocalKeyWithEndBracketValueOpt, default, [x => Assert.Equal(new("Section.]Key", "Value"), x)]);
		}
		private const string GlobalKeyValueIni = "Key=Value";
		private static readonly IniReaderOptions GlobalKeyValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void GlobalKeyValueSpan()
		{
			IniSpanReaderChecker c = new(GlobalKeyValueIni, GlobalKeyValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task GlobalKeyValueStream()
		{
			var (c1, c2) = Checks.For(GlobalKeyValueIni, GlobalKeyValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(GlobalKeyValueIni, GlobalKeyValueOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
		}
		private const string EscapedKeyValueIni = "Ke\\ny=Value";
		private static readonly IniReaderOptions EscapedKeyValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapedKeyValueSpan()
		{
			IniSpanReaderChecker c = new(EscapedKeyValueIni, EscapedKeyValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Ke\\ny");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapedKeyValueStream()
		{
			var (c1, c2) = Checks.For(EscapedKeyValueIni, EscapedKeyValueOpt);
			await c1.Next(IniToken.Key, "Ke\ny");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Ke\ny", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EscapedKeyValueIni, EscapedKeyValueOpt, default, [x => Assert.Equal(new("Ke\ny", "Value"), x)]);
		}
		private const string EscapedEqKeyValueIni = "Ke\\=y=Value";
		private static readonly IniReaderOptions EscapedEqKeyValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapedEqKeyValueSpan()
		{
			IniSpanReaderChecker c = new(EscapedEqKeyValueIni, EscapedEqKeyValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Ke\\=y");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapedEqKeyValueStream()
		{
			var (c1, c2) = Checks.For(EscapedEqKeyValueIni, EscapedEqKeyValueOpt);
			await c1.Next(IniToken.Key, "Ke=y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Ke=y", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EscapedEqKeyValueIni, EscapedEqKeyValueOpt, default, [x => Assert.Equal(new("Ke=y", "Value"), x)]);
		}
		private const string EscapedColonKeyValueIni = "Ke\\:y:Value";
		private static readonly IniReaderOptions EscapedColonKeyValueOpt = new(allowGlobalKeys: true, allowKeyDelimiterColon: true);
		[Fact]
		public static void EscapedColonKeyValueSpan()
		{
			IniSpanReaderChecker c = new(EscapedColonKeyValueIni, EscapedColonKeyValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Ke\\:y");
			c.Next(IniContentType.EndKey, ":");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapedColonKeyValueStream()
		{
			var (c1, c2) = Checks.For(EscapedColonKeyValueIni, EscapedColonKeyValueOpt);
			await c1.Next(IniToken.Key, "Ke:y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Ke:y", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EscapedColonKeyValueIni, EscapedColonKeyValueOpt, default, [x => Assert.Equal(new("Ke:y", "Value"), x)]);
		}

		public const string EscapedKeyEndIni = "Key\\==";
		public static readonly IniReaderOptions EscapedKeyEndOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapedKeyEndSpan()
		{
			IniSpanReaderChecker c = new(EscapedKeyEndIni, EscapedKeyEndOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Key\\=");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapedKeyEndStream()
		{
			var (c1, c2) = Checks.For(EscapedKeyEndIni, EscapedKeyEndOpt);
			await c1.Next(IniToken.Key, "Key=");
			await c1.Next(IniToken.Value, "");
			await c1.Next(IniToken.End, "");
		}
		private const string KeyContainingColonValueIni = "Ke:y=Value";
		private static readonly IniReaderOptions KeyContainingColonValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void KeyContainingColonValueSpan()
		{
			IniSpanReaderChecker c = new(KeyContainingColonValueIni, KeyContainingColonValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Ke:y");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyContainingColonValueStream()
		{
			var (c1, c2) = Checks.For(KeyContainingColonValueIni, KeyContainingColonValueOpt);
			await c1.Next(IniToken.Key, "Ke:y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Ke:y", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(KeyContainingColonValueIni, KeyContainingColonValueOpt, default, [x => Assert.Equal(new("Ke:y", "Value"), x)]);
		}
		public const string KeyContainingSemicolonIni = "[Section]\nKe;y=Value";
		[Fact]
		public static void KeyContainingSemicolonSpan()
		{
			IniSpanReaderChecker c = new(KeyContainingSemicolonIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Ke;y");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyContainingSemicolonStream()
		{
			var (c1, c2) = Checks.For(KeyContainingSemicolonIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Ke;y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await Chk.CheckAllIniDictionaryReader(KeyContainingSemicolonIni, default, default,
			[
				x => Assert.Equal(new("Section.Ke;y", "Value"), x),
			]);
		}
		public const string KeyContainingNumbersignIni = "[Section]\nKe#y=Value";
		public static readonly IniReaderOptions KeyContainingNumbersignOpt = new(allowCommentsNumberSign: true);
		[Fact]
		public static void KeyContainingNumbersignSpan()
		{
			IniSpanReaderChecker c = new(KeyContainingNumbersignIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Ke#y");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyContainingNumbersignStream()
		{
			var (c1, c2) = Checks.For(KeyContainingNumbersignIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Ke#y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await Chk.CheckAllIniDictionaryReader(KeyContainingNumbersignIni, default, default,
			[
				x => Assert.Equal(new("Section.Ke#y", "Value"), x),
			]);
		}
		private const string TwoKeysValueIni = "[Section]\nKey=Value1\nKey=Value2";
		private static readonly IniReaderOptions TwoKeysValueOpt = new();
		[Fact]
		public static void TwoKeysValueSpan()
		{
			IniSpanReaderChecker c = new(TwoKeysValueIni, TwoKeysValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			Check(ref c, "Value1");
			c.Next(IniContentType.EndValue, "\n");
			Check(ref c, "Value2");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			static void Check(ref IniSpanReaderChecker c, string value)
			{
				c.Next(IniContentType.StartKey, default);
				c.Next(IniContentType.Key, "Key");
				c.Next(IniContentType.EndKey, "=");
				c.Next(IniContentType.StartValue, default);
				c.Next(IniContentType.Value, value);
			}
		}
		[Fact]
		public static async Task TwoKeysValueStream()
		{
			var (c1, c2) = Checks.For(TwoKeysValueIni, TwoKeysValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value1");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value2");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value1"), new("Key", "Value2")], []));
			await c2.End();

			IniDictionaryReader<string> last = new(StringComparer.OrdinalIgnoreCase);
			last.Load(new IniStreamReader(new StringReader(TwoKeysValueIni), null, TwoKeysValueOpt), IniDictionaryReader.StringOnlyLast).ThrowIfError();
			Assert.Collection(last.Dictionary, [x => Assert.Equal(new("Section.Key", "Value2"), x)]);
			IniDictionaryReader<string> first = new(StringComparer.OrdinalIgnoreCase);
			first.Load(new IniStreamReader(new StringReader(TwoKeysValueIni), null, TwoKeysValueOpt), IniDictionaryReader.StringOnlyFirst).ThrowIfError();
			Assert.Collection(first.Dictionary, [x => Assert.Equal(new("Section.Key", "Value1"), x)]);

			IniDictionaryReader<string> single = new(StringComparer.OrdinalIgnoreCase);
			Chk.IniError(IniErrorCode.ValueAlreadyPresent, "Section & key already present. Full Key: \"Section.Key\". Value is: \"Value2\"", single.Load(new IniStreamReader(new StringReader(TwoKeysValueIni), null, TwoKeysValueOpt), IniDictionaryReader.StringSingle));
			// Even though we got an error the dictionary should still be partially filled
			Assert.Collection(single.Dictionary, [x => Assert.Equal(new("Section.Key", "Value1"), x)]);
		}
		public const string KeyEmptyValueIni = "Key=";
		public static readonly IniReaderOptions KeyEmptyValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void KeyEmptyValueSpan()
		{
			IniSpanReaderChecker c = new(KeyEmptyValueIni, KeyEmptyValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyEmptyValueStream()
		{
			var (c1, c2) = Checks.For(KeyEmptyValueIni, KeyEmptyValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(KeyEmptyValueIni, KeyEmptyValueOpt, default, [x => Assert.Equal(new("Key", ""), x)]);
		}
		public const string KeyValueSeparateBlocksIni = "Key=Value";
		public static readonly IniReaderOptions KeyValueSeparateBlocksOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void KeyValueSeparateBlocks_SplitBegin()
		{
			IniSpanReaderChecker c = new(KeyValueSeparateBlocksIni, 1, KeyValueSeparateBlocksOpt, false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.End, default);
			c = c.NewBlock(KeyValueSeparateBlocksIni, true, 9);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void KeyValueSeparateBlocks_SplitEnd()
		{
			IniSpanReaderChecker c = new(KeyValueSeparateBlocksIni,4, KeyValueSeparateBlocksOpt, false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.End, default);
			c = c.NewBlock(KeyValueSeparateBlocksIni, true, 5);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void KeyValueSeparateBlocks_SplitMiddle()
		{
			IniSpanReaderChecker c = new(KeyValueSeparateBlocksIni, 3, KeyValueSeparateBlocksOpt, false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.End, default);
			c = c.NewBlock(KeyValueSeparateBlocksIni, true, 9);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		// Even though we have a leading space, that is insignificant because we skip all whitespace to get to the key in the first place
		private const string LocalUntrimmedKeyValueIni = "[Section]\n Key =Value";
		private static readonly IniReaderOptions LocalUntrimmedKeyValueOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void LocalUntrimmedKeyValueSpan()
		{
			IniSpanReaderChecker c = new(LocalUntrimmedKeyValueIni, LocalUntrimmedKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key ");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LocalUntrimmedKeyValueStream()
		{
			var (c1, c2) = Checks.For(LocalUntrimmedKeyValueIni, LocalUntrimmedKeyValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key ");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key ", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LocalUntrimmedKeyValueIni, LocalUntrimmedKeyValueOpt, default, [x => Assert.Equal(new("Section.Key ", "Value"), x)]);
		}
		private const string LocalTrimmedKeyValueIni = "[Section]\n Key =Value";
		private static readonly IniReaderOptions LocalTrimmedKeyValueOpt = new(allowGlobalKeys: false, trimKeys: true);
		[Fact]
		public static void LocalTrimmedKeyValueSpan()
		{
			IniSpanReaderChecker c = new(LocalTrimmedKeyValueIni, LocalTrimmedKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LocalTrimmedKeyValueStream()
		{
			var (c1, c2) = Checks.For(LocalTrimmedKeyValueIni, LocalTrimmedKeyValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LocalTrimmedKeyValueIni, LocalTrimmedKeyValueOpt, default, [x => Assert.Equal(new("Section.Key", "Value"), x)]);
		}
	}
}