namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;
	public static class ParseGood
	{
		private const string KeyEqualsValueIni = "Key=Value";
		private static readonly IniReaderOptions KeyEqualsValueOpt = new(allowGlobalKeys: true, ignoreSectionEscapes: true, ignoreKeyEscapes: true, ignoreValueEscapes: true, ignoreCommentEscapes: true);
		[Fact]
		public static void KeyEqualsValueSpan()
		{
			IniSpanReaderChecker c = new(KeyEqualsValueIni, KeyEqualsValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyEqualsValueStream()
		{
			var (c1, c2) = Checks.For(KeyEqualsValueIni, KeyEqualsValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(KeyEqualsValueIni, KeyEqualsValueOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
			//await Chk.CheckAllIniDictionaryReader(KeyEqualsValueIni, KeyEqualsValueOpt, default, [x => Chk.IniValueKvp("Key", "Value", [], x)]);
		}
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
			//await Chk.CheckAllIniDictionaryReader(SectionKeyEqualsValueIni, default, default, [x => Chk.IniValueKvp("Section.Key", "Value", ["Comment2"], x)]);
		}
		private const string DoubleKeyEqualsValueIni = "[Section]\nKey=Value1\nKey=Value2";
		private static readonly IniReaderOptions DoubleKeyEqualsValueOpt = new();
		[Fact]
		public static void DoubleKeyEqualsValueSpan()
		{
			IniSpanReaderChecker c = new(DoubleKeyEqualsValueIni, DoubleKeyEqualsValueOpt);
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
		public static async Task DoubleKeyEqualsValueStream()
		{
			var (c1, c2) = Checks.For(DoubleKeyEqualsValueIni, DoubleKeyEqualsValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value1");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value2");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value1"), new("Key", "Value2")], []));
			await c2.End();

			IniDictionaryReader<string> last = new(StringComparer.OrdinalIgnoreCase);
			last.Load(new IniStreamReader(new StringReader(DoubleKeyEqualsValueIni), null, DoubleKeyEqualsValueOpt), IniDictionaryReader.StringOnlyLast).ThrowIfError();
			Assert.Collection(last.Dictionary, [x => Assert.Equal(new("Section.Key", "Value2"), x)]);
			IniDictionaryReader<string> first = new(StringComparer.OrdinalIgnoreCase);
			first.Load(new IniStreamReader(new StringReader(DoubleKeyEqualsValueIni), null, DoubleKeyEqualsValueOpt), IniDictionaryReader.StringOnlyFirst).ThrowIfError();
			Assert.Collection(first.Dictionary, [x => Assert.Equal(new("Section.Key", "Value1"), x)]);

			IniDictionaryReader<string> single = new(StringComparer.OrdinalIgnoreCase);
			Chk.IniError(IniErrorCode.ValueAlreadyPresent, "Section & key already present. Full Key: \"Section.Key\". Value is: \"Value2\"", single.Load(new IniStreamReader(new StringReader(DoubleKeyEqualsValueIni), null, DoubleKeyEqualsValueOpt), IniDictionaryReader.StringSingle));
			// Even though we got an error the dictionary should still be partially filled
			Assert.Collection(single.Dictionary, [x => Assert.Equal(new("Section.Key", "Value1"), x)]);
		}
		public const string KeyColonValueIni = "Key:Value";
		public static readonly IniReaderOptions KeyColonValueOpt = new(allowGlobalKeys: true, allowKeyDelimiterColon: true);
		[Fact]
		public static void KeyColonValueSpan()
		{
			IniSpanReaderChecker c = new(KeyColonValueIni, KeyColonValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, ":");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task KeyColonValueStream()
		{
			var (c1, c2) = Checks.For(KeyColonValueIni, KeyColonValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(KeyColonValueIni, KeyColonValueOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
			//await Chk.CheckAllIniDictionaryReader(KeyColonValueIni, KeyColonValueOpt, default, [x => Chk.IniValueKvp("Key", "Value", [], x)]);
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
			//await Chk.CheckAllIniDictionaryReader(KeyEmptyValueIni, KeyEmptyValueOpt, default, [x => Chk.IniValueKvp("Key", "", [], x)]);
		}
		[Fact]
		public static void KeyValueSeparateBlocks1()
		{
			IniSpanReaderChecker c = new("Key=", new(allowGlobalKeys: true), false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.End, default);
			c = c.NewBlock("Value", true);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void KeyValueSeparateBlocks2()
		{
			IniSpanReaderChecker c = new("Key", new(allowGlobalKeys: true), false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.End, default);
			c = c.NewBlock("Key=Value", true);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void SectionSeparateBlocks1()
		{
			IniSpanReaderChecker c = new("[", default, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.End, default);
			c = c.NewBlock("Section]", true);
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void SectionSeparateBlocks2()
		{
			IniSpanReaderChecker c = new("[Section", default, false);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.End, default);
			c = c.NewBlock("]", true);
			c.Next(IniContentType.Section, "");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.End, default);
		}
		public const string TrimmableIni = "[ Section ]\n Key = Value ";
		[Fact]
		public static async Task TrimmableIniStream()
		{
			var (c1, c2) = Checks.For(TrimmableIni, new(trimSections: true, trimKeys: true, trimValues: true));
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TrimmableIni, new(trimSections: true, trimKeys: true, trimValues: true), default,
			[
				x => Assert.Equal(new("Section.Key", "Value"), x),
			]);
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
			//await Chk.CheckAllIniDictionaryReader(MultipleKeyValuesIni, default, default,
			//[
			//	x => Chk.IniValueKvp("Section.Key1", "Value1", [], x),
			//	x => Chk.IniValueKvp("Section.Key2 ", " Value2", [], x),
			//]);
		}
		public const string SectionKeyValueLineContinutationIni = "[Section]\nKey1=Hello\\\nworld!";
		public static readonly IniReaderOptions SectionKeyValueLineContinutationOpt = new(allowLineContinuations: true);
		[Fact]
		public static void SectionKeyValueLineContinutationSpan()
		{
			IniSpanReaderChecker c = new(SectionKeyValueLineContinutationIni, SectionKeyValueLineContinutationOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Hello");
			c.Next(IniContentType.Value, "world!");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionKeyValueLineContinutationStream()
		{
			var (c1, c2) = Checks.For(SectionKeyValueLineContinutationIni, SectionKeyValueLineContinutationOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Helloworld!");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key1", "Helloworld!")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionKeyValueLineContinutationIni, SectionKeyValueLineContinutationOpt, default,
			[
				x => Assert.Equal(new("Section.Key1", "Helloworld!"), x),
			]);
			//await Chk.CheckAllIniDictionaryReader(SectionKeyValueLineContinutationIni, SectionKeyValueLineContinutationOpt, default,
			//[
			//	x => Chk.IniValueKvp("Section.Key1", "Helloworld!", [], x),
			//]);
		}
		public const string LineContinutationEndIni = "Key1=Value\\";
		public static readonly IniReaderOptions LineContinutationEndOpt = new(allowGlobalKeys: true, allowLineContinuations: true, ignoreValueEscapes: true);
		[Fact]
		public static void LineContinutationEndSpan()
		{
			IniSpanReaderChecker c = new(LineContinutationEndIni, LineContinutationEndOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task LineContinutationEndStream()
		{
			var (c1, c2) = Checks.For(LineContinutationEndIni, LineContinutationEndOpt);
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key1", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(LineContinutationEndIni, LineContinutationEndOpt, default,
			[
				x => Assert.Equal(new("Key1", "Value"), x),
			]);
			//await Chk.CheckAllIniDictionaryReader(LineContinutationEndIni, LineContinutationEndOpt, default,
			//[
			//	x => Chk.IniValueKvp("Key1", "Value", [], x),
			//]);
		}
		[Fact]
		public static void LineContinutationSeparateBlock()
		{
			string fullIni = "[Section]\nKey1=Hello\\\nworld!";
			IniSpanReader isr = new(fullIni.AsSpan(0, 21), IniSpanReaderState.Init(new IniReaderOptions(allowLineContinuations: true)), false);
			Chk.IniContent(IniContentType.StartSection, "[", isr.Read());
			Chk.IniContent(IniContentType.Section, "Section", isr.Read());
			Chk.IniContent(IniContentType.EndSection, "]", isr.Read());
			Chk.IniContent(IniContentType.StartKey, default, isr.Read());
			Chk.IniContent(IniContentType.Key, "Key1", isr.Read());
			Chk.IniContent(IniContentType.EndKey, "=", isr.Read());
			Chk.IniContent(IniContentType.StartValue, default, isr.Read());
			Chk.IniContent(IniContentType.Value, "Hello", isr.Read());
			Chk.IniContent(IniContentType.End, default, isr.Read());

			Span<char> newBuf = stackalloc char[32];
			var state = isr.GetState().NewBlock(isr.Block, newBuf, out int copied);
			var remaining = fullIni.AsSpan(isr.Position + 1);
			remaining.CopyTo(newBuf.Slice(copied));
			int size = copied + remaining.Length;
			var isr2 = new IniSpanReader(newBuf.Slice(0, size), state, true);
			// Because the buffer gets filled with a backslash then newline, it technically counts as a blank string and then "world" on a new line
			// That doesn't affect the data in any way, it's just a harmless quirk
			Chk.IniContent(IniContentType.Value, default, isr2.Read());
			Chk.IniContent(IniContentType.Value, "world!", isr2.Read());
			Chk.IniContent(IniContentType.EndValue, default, isr2.Read());
			Chk.IniContent(IniContentType.End, default, isr2.Read());
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
			//await Chk.CheckAllIniDictionaryReader(SectionKeyValueIni, SectionKeyValueOpt, default,
			//[
			//	x => Chk.IniValueKvp("Section.Key1", "Value1", [], x),
			//	x => Chk.IniValueKvp("Section.Key2 ", " Value2", [], x),
			//]);
		}
		public const string SectionCommentKeyValueIni = "[Section]\n# Hello world!\nKey=Value";
		public static readonly IniReaderOptions SectionCommentKeyValueOpt = new(allowCommentsNumberSign: true, allowGlobalKeys: true);
		[Fact]
		public static void SectionCommentKeyValueSpan()
		{
			IniSpanReaderChecker c = new(SectionCommentKeyValueIni, SectionCommentKeyValueOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartComment, "#");
			c.Next(IniContentType.Comment, " Hello world!");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SectionCommentKeyValueStream()
		{
			var (c1, c2) = Checks.For(SectionCommentKeyValueIni, SectionCommentKeyValueOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Comment, " Hello world!");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], []));
			await c2.Next(new("Section", [new("Key", "Value", [" Hello world!"])], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SectionCommentKeyValueIni, SectionCommentKeyValueOpt, default,
			[
				x => Assert.Equal(new("Section.Key", "Value"), x),
			]);
			//await Chk.CheckAllIniDictionaryReader(SectionCommentKeyValueIni, SectionCommentKeyValueOpt, default,
			//[
			//	x => Chk.IniValueKvp("Section.Key", "Value", [" Hello world!"], x),
			//]);
		}
		public const string EscapeSequencesEndOfBlockIni = "Key\\==";
		public static readonly IniReaderOptions EscapeSequencesEndOfBlockOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EscapeSequencesEndOfBlockSpan()
		{
			IniSpanReaderChecker c = new(EscapeSequencesEndOfBlockIni, EscapeSequencesEndOfBlockOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.KeyEscaped, "Key\\=");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapeSequencesEndOfBlockStream()
		{
			var (c1, c2) = Checks.For(EscapeSequencesEndOfBlockIni, EscapeSequencesEndOfBlockOpt);
			await c1.Next(IniToken.Key, "Key=");
			await c1.Next(IniToken.Value, "");
			await c1.Next(IniToken.End, "");
		}

		public const string EscapeSequencesIni = "[B\\\\ig \\0Lon\\bg \\rSe\\nction Name]\nBig Long\\= Key Name=Big Lo\\]ng Value\n;Comment\\a Stuff\n";
		[Fact]
		public static void EscapeSequencesSpan()
		{
			IniSpanReaderChecker c = new(EscapeSequencesIni, default);
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
		public const string ValueEndingWithSlashIni = "Key=Value\\";
		[Fact]
		public static void ValueEndingWithSlash_BadEscapeSequence()
		{
			IniSpanReaderChecker c = new(ValueEndingWithSlashIni, new(allowGlobalKeys: true), false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(ValueEndingWithSlashIni.Substring(c.Position), true);
			c.Next(IniContentType.Error, "\\");
		}
		[Fact]
		public static void ValueEndingWithSlash_LineContinuation()
		{
			IniSpanReaderChecker c = new(ValueEndingWithSlashIni, new(allowGlobalKeys: true, allowLineContinuations: true, ignoreValueEscapes: true), false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void CommentEscapeSequenceDifferentBlocks()
		{
			IniSpanReaderChecker c = new("; Hello \\", default, false);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello ");
			c.Next(IniContentType.End, default);
			c = c.NewBlock("\\nworld!", true);
			c.Next(IniContentType.CommentEscaped, "\\nworld!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
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
			c.Next(IniContentType.Key, "Ke;y");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SemicolonInKeyNameStream()
		{
			var (c1, c2) = Checks.For(SemicolonInKeyNameIni, default);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Ke;y");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await Chk.CheckAllIniDictionaryReader(SemicolonInKeyNameIni, default, default,
			[
				x => Assert.Equal(new("Section.Ke;y", "Value"), x),
			]);
		}
		public const string CommentKeyValueIni = "# Hello world!\nKey=Value";
		public static readonly IniReaderOptions CommentKeyValueOpt = new(allowCommentsNumberSign: true, allowGlobalKeys: true);
		[Fact]
		public static void CommentKeyValueSpan()
		{
			IniSpanReaderChecker c = new(CommentKeyValueIni, CommentKeyValueOpt);
			c.Next(IniContentType.StartComment, "#");
			c.Next(IniContentType.Comment, " Hello world!");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentKeyValueStream()
		{
			var (c1, c2) = Checks.For(CommentKeyValueIni, CommentKeyValueOpt);
			await c1.Next(IniToken.Comment, " Hello world!");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], [" Hello world!"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentKeyValueIni, CommentKeyValueOpt, default,
			[
				x => Assert.Equal(new("Key", "Value"), x),
			]);
			//await Chk.CheckAllIniDictionaryReader(CommentKeyValueIni, CommentKeyValueOpt, default,
			//[
			//	x => Chk.IniValueKvp("Key", "Value", [" Hello world!"], x),
			//]);
		}
		public const string CommentIgnoredIni = "# Hello world!\nKey=Value\n;Goodbye world!";
		public static readonly IniReaderOptions CommentIgnoredOpt = new(allowCommentsNumberSign: true, allowGlobalKeys: true, ignoreComments: true);
		[Fact]
		public static void CommentIgnoredSpan()
		{
			IniSpanReaderChecker c = new(CommentIgnoredIni, CommentIgnoredOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentIgnoredStream()
		{
			var (c1, c2) = Checks.For(CommentIgnoredIni, CommentIgnoredOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentIgnoredIni, CommentIgnoredOpt, default,
			[
				x => Assert.Equal(new("Key", "Value"), x),
			]);
			//await Chk.CheckAllIniDictionaryReader(CommentIgnoredIni, CommentIgnoredOpt, default,
			//[
			//	x => Chk.IniValueKvp("Key", "Value", [], x),
			//]);
		}
		public const string EmptyCommentEndOfSpanIni = ";";
		[Fact]
		public static void EmptyCommentEndOfContentSpan()
		{
			IniSpanReaderChecker c = new(EmptyCommentEndOfSpanIni);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EmptyCommentEndOfContentStream()
		{
			var (c1, c2) = Checks.For(EmptyCommentEndOfSpanIni, default);
			await c1.Next(IniToken.Comment, "");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], [""]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EmptyCommentEndOfSpanIni, default, default, []);
		}
		public const string CommentEndOfSpanIni = "; Hello world!";
		[Fact]
		public static void CommentEndOfContentSpan()
		{
			IniSpanReaderChecker c = new(CommentEndOfSpanIni);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello world!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentEndOfContentStream()
		{
			var (c1, c2) = Checks.For(CommentEndOfSpanIni, default);
			await c1.Next(IniToken.Comment, " Hello world!");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], [" Hello world!"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentEndOfSpanIni, default, default, []);
		}
		[Fact]
		public static void CommentSeparateBlocks()
		{
			IniSpanReaderChecker c = new("; Hello ", default, false);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello ");
			c.Next(IniContentType.End, default);
			c = c.NewBlock("world!", true);
			c.Next(IniContentType.Comment, "world!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void LotsOfIgnoredComments()
		{
			// This used to throw a stack overflow exception
			IniSpanReaderChecker c1 = new(string.Join('\n', Enumerable.Repeat("; Hello!", 500)), new(ignoreComments: true), false);
			c1.Next(IniContentType.End, default);

			// Just make sure we pick up all the comments properly
			IniSpanReaderChecker c2 = new(string.Join('\n', Enumerable.Repeat("; Hello!", 500)), new(ignoreComments: false), false);
			for (int i = 0; i < 499; i++)
			{
				c2.Next(IniContentType.StartComment, ";");
				c2.Next(IniContentType.Comment, " Hello!");
				c2.Next(IniContentType.EndComment, "\n");
			}
			c2.Next(IniContentType.StartComment, ";");
			c2.Next(IniContentType.Comment, " Hello!");
			c2.Next(IniContentType.End, default);
		}
	}
}