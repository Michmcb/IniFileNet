namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Threading.Tasks;
	using Xunit;
	public static class ParseGoodGeneral
	{
		private const string SectionKeyEqualsValueIni = ";Comment1\n[Section]\n;Comment2\n]Key=Value";
		private static readonly IniReaderOptions SectionKeyEqualsValueOpt = new(ignoreSectionEscapes: true, ignoreKeyEscapes: true, ignoreValueEscapes: true, ignoreCommentEscapes: true);
		[Fact]
		public static void SectionKeyEqualsValueSpan()
		{
			IniSpanReaderChecker c = new(SectionKeyEqualsValueIni, SectionKeyEqualsValueOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Comment1");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Comment2");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "]Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionKeyEqualsValueStream()
		{
			var (c1, c2) = Checks.For(SectionKeyEqualsValueIni, SectionKeyEqualsValueOpt);
			await c1.Next(IniToken.Comment, "Comment1");
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Comment, "Comment2");
			await c1.Next(IniToken.Key, "]Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("]Key", "Value", ["Comment2"])], ["Comment1"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionKeyEqualsValueIni, default, default, [x => Assert.Equal(new("Section.]Key", "Value"), x)]);
		}
		public const string MultipleKeyValuesIni = "[Section]\rKey1=Value1\nKey2 = Value2 ";
		[Fact]
		public static void MultipleKeyValuesSpan()
		{
			IniSpanReaderChecker c = new(MultipleKeyValuesIni);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value1");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key2 ");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, " Value2 ");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task MultipleKeyValuesStream()
		{
			var (c1, c2) = Checks.For(MultipleKeyValuesIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Value1");
			await c1.Next(IniToken.Key, "Key2 ");
			await c1.Next(IniToken.Value, " Value2 ");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key1", "Value1"), new("Key2 ", " Value2 ")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(MultipleKeyValuesIni, default, default,
			[
				x => Assert.Equal(new("Section.Key1", "Value1"), x),
				x => Assert.Equal(new("Section.Key2 ", " Value2 "), x),
			]);
		}
		public const string SectionKeyValueIni = "[Section]\nKey1:Value1\nKey2 = Value2\n     ";
		public static readonly IniReaderOptions SectionKeyValueOpt = new(allowKeyDelimiterColon: true);
		[Fact]
		public static void SectionKeyValueSpan()
		{
			IniSpanReaderChecker c = new(SectionKeyValueIni, SectionKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, ":");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value1");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key2 ");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, " Value2");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionKeyValueStream()
		{
			var (c1, c2) = Checks.For(SectionKeyValueIni, SectionKeyValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Value1");
			await c1.Next(IniToken.Key, "Key2 ");
			await c1.Next(IniToken.Value, " Value2");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key1", "Value1"), new("Key2 ", " Value2")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionKeyValueIni, SectionKeyValueOpt, default,
			[
				x => Assert.Equal(new("Section.Key1", "Value1"), x),
				x => Assert.Equal(new("Section.Key2 ", " Value2"), x),
			]);
		}
		public const string EscapeSequencesIni = "[B\\\\ig \\0Lon\\bg \\rSe\\nction Name]\nBig Long\\= Key Name=Big Lo\\]ng Value\n;Comment\\a Stuff\n";
		public static readonly IniReaderOptions EscapeSequencesOpt = default;
		[Fact]
		public static void EscapeSequencesSpan()
		{
			IniSpanReaderChecker c = new(EscapeSequencesIni, EscapeSequencesOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.SectionEscaped, "B\\\\ig \\0Lon\\bg \\rSe\\nction Name");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Big Long\\= Key Name");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.ValueEscaped, "Big Lo\\]ng Value");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.CommentEscaped, "Comment\\a Stuff");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapeSequencesStream()
		{
			var (c1, c2) = Checks.For(EscapeSequencesIni, default);
			await c1.Next(IniToken.Section, "B\\ig \0Lon\bg \rSe\nction Name");
			await c1.Next(IniToken.Key, "Big Long= Key Name");
			await c1.Next(IniToken.Value, "Big Lo]ng Value");
			await c1.Next(IniToken.Comment, "Comment\a Stuff");
			await c1.Next(IniToken.End, "");
		}

	}
}