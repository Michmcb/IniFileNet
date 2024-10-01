namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseGoodSections
	{
		private const string SingleSectionIni = "[Section]";
		private static readonly IniReaderOptions SingleSectionOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void SingleSectionSpan()
		{
			IniSpanReaderChecker c = new(SingleSectionIni, SingleSectionOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SingleSectionStream()
		{
			var (c1, c2) = Checks.For(SingleSectionIni, SingleSectionOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SingleSectionIni, SingleSectionOpt, default, []);
		}
		private const string SingleSectionTrailingNewlineIni = "[Section]\n";
		private static readonly IniReaderOptions SingleSectionTrailingNewlineOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void SingleSectionTrailingNewlineSpan()
		{
			IniSpanReaderChecker c = new(SingleSectionTrailingNewlineIni, SingleSectionTrailingNewlineOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SingleSectionTrailingNewlineStream()
		{
			var (c1, c2) = Checks.For(SingleSectionTrailingNewlineIni, SingleSectionTrailingNewlineOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SingleSectionTrailingNewlineIni, SingleSectionTrailingNewlineOpt, default, []);
		}
		private const string SectionEqualsIni = "[Sec=tion]";
		private static readonly IniReaderOptions SectionEqualsOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void SectionEqualsSpan()
		{
			IniSpanReaderChecker c = new(SectionEqualsIni, SectionEqualsOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Sec=tion");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionEqualsStream()
		{
			var (c1, c2) = Checks.For(SectionEqualsIni, SectionEqualsOpt);
			await c1.Next(IniToken.Section, "Sec=tion");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Sec=tion", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionEqualsIni, SectionEqualsOpt, default, []);
		}
		private const string SectionColonIni = "[Secti:on]";
		private static readonly IniReaderOptions SectionColonOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void SectionColonSpan()
		{
			IniSpanReaderChecker c = new(SectionColonIni, SectionColonOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Secti:on");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionColonStream()
		{
			var (c1, c2) = Checks.For(SectionColonIni, SectionColonOpt);
			await c1.Next(IniToken.Section, "Secti:on");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Secti:on", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionColonIni, SectionColonOpt, default, []);
		}

		private const string SectionExtraOpenBracketIni = "[Se[ction]";
		private static readonly IniReaderOptions SectionExtraOpenBracketOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void SectionExtraOpenBracketSpan()
		{
			IniSpanReaderChecker c = new(SectionExtraOpenBracketIni, SectionExtraOpenBracketOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Se[ction");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionExtraOpenBracketStream()
		{
			var (c1, c2) = Checks.For(SectionExtraOpenBracketIni, SectionExtraOpenBracketOpt);
			await c1.Next(IniToken.Section, "Se[ction");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Se[ction", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionExtraOpenBracketIni, SectionExtraOpenBracketOpt, default, []);
		}
		private const string TwoSectionsIni = "[Section1]\n[Section2]";
		private static readonly IniReaderOptions TwoSectionsOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void TwoSectionsSpan()
		{
			IniSpanReaderChecker c = new(TwoSectionsIni, TwoSectionsOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section1");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section2");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task TwoSectionsStream()
		{
			var (c1, c2) = Checks.For(TwoSectionsIni, TwoSectionsOpt);
			await c1.Next(IniToken.Section, "Section1");
			await c1.Next(IniToken.Section, "Section2");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section1", [], []));
			await c2.Next(new("Section2", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TwoSectionsIni, TwoSectionsOpt, default, []);
		}
		private const string TwoSectionsKeyValueIni = "[Section1]\nKey1=Value1\n[Section2]\nKey2=Value2";
		private static readonly IniReaderOptions TwoSectionsKeyValueOpt = new(allowGlobalKeys: false);
		[Fact]
		public static void TwoSectionsKeyValueSpan()
		{
			IniSpanReaderChecker c = new(TwoSectionsKeyValueIni, TwoSectionsKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section1");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value1");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section2");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key2");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value2");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task TwoSectionsKeyValueStream()
		{
			var (c1, c2) = Checks.For(TwoSectionsKeyValueIni, TwoSectionsKeyValueOpt);
			await c1.Next(IniToken.Section, "Section1");
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Value1");
			await c1.Next(IniToken.Section, "Section2");
			await c1.Next(IniToken.Key, "Key2");
			await c1.Next(IniToken.Value, "Value2");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section1", [new("Key1", "Value1")], []));
			await c2.Next(new("Section2", [new("Key2", "Value2")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TwoSectionsKeyValueIni, TwoSectionsKeyValueOpt, default, [
				x => Assert.Equal(new("Section1.Key1", "Value1"), x),
				x => Assert.Equal(new("Section2.Key2", "Value2"), x)
			]);
		}
		public const string SectionSeparateBlocksIni = "[Section]";
		[Fact]
		public static void SectionSeparateBlocks_SplitBegin()
		{
			IniSpanReaderChecker c = new(SectionSeparateBlocksIni, 1, default, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(SectionSeparateBlocksIni, true, 8);
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void SectionSeparateBlocks_SplitEnd()
		{
			IniSpanReaderChecker c = new(SectionSeparateBlocksIni, 8, default, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(SectionSeparateBlocksIni, true, 8);
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		public const string SectionSeparateBlocks2Ini = "[FooBar]";
		[Fact]
		public static void SectionSeparateBlocks_SplitMiddle()
		{
			IniSpanReaderChecker c = new(SectionSeparateBlocks2Ini, 4, default, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(SectionSeparateBlocks2Ini, true, 8);
			c.Next(IniContentType.Section, "FooBar");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		public const string TrimmedSectionIni = "[ Section ]";
		public static readonly IniReaderOptions TrimmedSectionOpt = new(trimSections: true);
		[Fact]
		public static void TrimmedSectionSpan()
		{
			IniSpanReaderChecker c = new(TrimmedSectionIni, TrimmedSectionOpt, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task TrimmedSectionStream()
		{
			var (c1, c2) = Checks.For(TrimmedSectionIni, TrimmedSectionOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TrimmedSectionIni, TrimmedSectionOpt, default, []);
		}
		public const string UntrimmedSectionIni = "[ Section ]";
		public static readonly IniReaderOptions UntrimmedSectionOpt = new(trimSections: false);
		[Fact]
		public static void UntrimmedSectionSpan()
		{
			IniSpanReaderChecker c = new(UntrimmedSectionIni, UntrimmedSectionOpt, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, " Section ");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task UntrimmedSectionStream()
		{
			var (c1, c2) = Checks.For(UntrimmedSectionIni, UntrimmedSectionOpt);
			await c1.Next(IniToken.Section, " Section ");
			await c1.Next(IniToken.End, "");

			await c2.Next(new(" Section ", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(UntrimmedSectionIni, UntrimmedSectionOpt, default, []);
		}
		private const string EscapedSectionIni = "[Se\\ncti\\]on]";
		private static readonly IniReaderOptions EscapedSectionOpt = default;
		[Fact]
		public static void EscapedSectionSpan()
		{
			IniSpanReaderChecker c = new(EscapedSectionIni, EscapedSectionOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.SectionEscaped, "Se\\ncti\\]on");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapedSectionStream()
		{
			var (c1, c2) = Checks.For(EscapedSectionIni, EscapedSectionOpt);
			await c1.Next(IniToken.Section, "Se\ncti]on");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Se\ncti]on", [], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EscapedSectionIni, EscapedSectionOpt, default, []);
		}
	}
}