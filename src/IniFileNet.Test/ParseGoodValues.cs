namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseGoodValues
	{
		private const string TrailingNewlineIni = "Key=Value\n";
		private static readonly IniReaderOptions TrailingNewlineOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void TrailingNewlineSpan()
		{
			IniSpanReaderChecker c = new(TrailingNewlineIni, TrailingNewlineOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task TrailingNewlineStream()
		{
			var (c1, c2) = Checks.For(TrailingNewlineIni, TrailingNewlineOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TrailingNewlineIni, TrailingNewlineOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
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
			remaining.CopyTo(newBuf[copied..]);
			int size = copied + remaining.Length;
			var isr2 = new IniSpanReader(newBuf[..size], state, true);
			// Because the buffer gets filled with a backslash then newline, it technically counts as a blank string and then "world" on a new line
			// That doesn't affect the data in any way, it's just a harmless quirk
			Chk.IniContent(IniContentType.Value, default, isr2.Read());
			Chk.IniContent(IniContentType.Value, "world!", isr2.Read());
			Chk.IniContent(IniContentType.EndValue, default, isr2.Read());
			Chk.IniContent(IniContentType.End, default, isr2.Read());
		}
		public const string SlashEndNotEscapeSequenceIni = "Key1=Value\\";
		public static readonly IniReaderOptions SlashEndNotEscapeSequenceOpt = new(allowGlobalKeys: true, allowLineContinuations: false, ignoreValueEscapes: true);
		[Fact]
		public static void SlashEndNotEscapeSequenceSpan()
		{
			IniSpanReaderChecker c = new(SlashEndNotEscapeSequenceIni, SlashEndNotEscapeSequenceOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value\\");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SlashEndNotEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(SlashEndNotEscapeSequenceIni, SlashEndNotEscapeSequenceOpt);
			await c1.Next(IniToken.Key, "Key1");
			await c1.Next(IniToken.Value, "Value\\");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key1", "Value\\")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SlashEndNotEscapeSequenceIni, SlashEndNotEscapeSequenceOpt, default,
			[
				x => Assert.Equal(new("Key1", "Value\\"), x),
			]);
		}
		private const string EscapeSequenceIni = "[Section]\nKey=Va\\nlu\\\\e";
		private static readonly IniReaderOptions EscapeSequenceOpt = new();
		[Fact]
		public static void EscapeSequenceSpan()
		{
			IniSpanReaderChecker c = new(EscapeSequenceIni, EscapeSequenceOpt);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.ValueEscaped, "Va\\nlu\\\\e");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(EscapeSequenceIni, EscapeSequenceOpt);
			await c1.Next(IniToken.Section, "Section");
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Va\nlu\\e");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("Section", [new("Key", "Va\nlu\\e")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EscapeSequenceIni, EscapeSequenceOpt, default, [x => Assert.Equal(new("Section.Key", "Va\nlu\\e"), x)]);
		}
		private const string UntrimmedValueIni = "Key= Value ";
		private static readonly IniReaderOptions UntrimmedValueOpt = new(allowGlobalKeys: true, trimValues: false);
		[Fact]
		public static void UntrimmedValueSpan()
		{
			IniSpanReaderChecker c = new(UntrimmedValueIni, UntrimmedValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, " Value ");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task UntrimmedValueStream()
		{
			var (c1, c2) = Checks.For(UntrimmedValueIni, UntrimmedValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, " Value ");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", " Value ")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(UntrimmedValueIni, UntrimmedValueOpt, default, [x => Assert.Equal(new("Key", " Value "), x)]);
		}
		private const string TrimmedValueIni = "Key= Value ";
		private static readonly IniReaderOptions TrimmedValueOpt = new(allowGlobalKeys: true, trimValues: true);
		[Fact]
		public static void TrimmedValueSpan()
		{
			IniSpanReaderChecker c = new(TrimmedValueIni, TrimmedValueOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task TrimmedValueStream()
		{
			var (c1, c2) = Checks.For(TrimmedValueIni, TrimmedValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(TrimmedValueIni, TrimmedValueOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
		}
		private const string EndLineCrIni = "Key=Value\r";
		private static readonly IniReaderOptions EndLineCrOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EndLineCrSpan()
		{
			IniSpanReaderChecker c = new(EndLineCrIni, EndLineCrOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, "\r");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EndLineCrStream()
		{
			var (c1, c2) = Checks.For(EndLineCrIni, EndLineCrOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EndLineCrIni, EndLineCrOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
		}
		private const string EndLineCrLfIni = "Key=Value\r\n";
		private static readonly IniReaderOptions EndLineCrLfOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void EndLineCrLfSpan()
		{
			IniSpanReaderChecker c = new(EndLineCrLfIni, EndLineCrLfOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, "\r\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task EndLineCrLfStream()
		{
			var (c1, c2) = Checks.For(EndLineCrLfIni, EndLineCrLfOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Value");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Value")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(EndLineCrLfIni, EndLineCrLfOpt, default, [x => Assert.Equal(new("Key", "Value"), x)]);
		}
		private const string PartialValueIni = "Key=Foo Bar Baz";
		private static readonly IniReaderOptions PartialValueOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void PartialValueSpan()
		{
			IniSpanReaderChecker c = new(PartialValueIni, 5, PartialValueOpt, false);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "F");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(PartialValueIni, false, 5);
			c.Next(IniContentType.Value, "oo Ba");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(PartialValueIni, false, 5);
			c.Next(IniContentType.Value, "r Baz");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(PartialValueIni, true, 5);
			c.Next(IniContentType.Value, default);
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task PartialValueStream()
		{
			var (c1, c2) = Checks.For(PartialValueIni, 5, PartialValueOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Value, "Foo Bar Baz");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [new("Key", "Foo Bar Baz")], []));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(PartialValueIni, PartialValueOpt, default, [x => Assert.Equal(new("Key", "Foo Bar Baz"), x)]);
		}
	}
}