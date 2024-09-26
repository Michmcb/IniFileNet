namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using Xunit;

	public static class ParseGoodComments
	{
		public const string CommentEndIni = ";Foo";
		public static readonly IniReaderOptions CommentEndOpt = new();
		[Fact]
		public static void CommentEndSpan()
		{
			IniSpanReaderChecker c = new(CommentEndIni, CommentEndOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Foo");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentEndStream()
		{
			var (c1, c2) = Checks.For(CommentEndIni, CommentEndOpt);
			await c1.Next(IniToken.Comment, "Foo");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], ["Foo"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentEndIni, CommentEndOpt, default, []);
		}
		public const string CommentEndLfIni = ";Foo\n";
		public static readonly IniReaderOptions CommentEndLfOpt = new();
		[Fact]
		public static void CommentEndLfSpan()
		{
			IniSpanReaderChecker c = new(CommentEndLfIni, CommentEndLfOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Foo");
			c.Next(IniContentType.EndComment, "\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentEndLfStream()
		{
			var (c1, c2) = Checks.For(CommentEndLfIni, CommentEndLfOpt);
			await c1.Next(IniToken.Comment, "Foo");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], ["Foo"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentEndLfIni, CommentEndLfOpt, default, []);
		}
		public const string CommentEndCrIni = ";Foo\r";
		public static readonly IniReaderOptions CommentEndCrOpt = new();
		[Fact]
		public static void CommentEndCrSpan()
		{
			IniSpanReaderChecker c = new(CommentEndCrIni, CommentEndCrOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Foo");
			c.Next(IniContentType.EndComment, "\r");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentEndCrStream()
		{
			var (c1, c2) = Checks.For(CommentEndCrIni, CommentEndCrOpt);
			await c1.Next(IniToken.Comment, "Foo");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], ["Foo"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentEndCrIni, CommentEndCrOpt, default, []);
		}
		public const string CommentEndCrLfIni = ";Foo\r\n";
		public static readonly IniReaderOptions CommentEndCrLfOpt = new();
		[Fact]
		public static void CommentEndCrLfSpan()
		{
			IniSpanReaderChecker c = new(CommentEndCrLfIni, CommentEndCrLfOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Foo");
			c.Next(IniContentType.EndComment, "\r\n");
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task CommentEndCrLfStream()
		{
			var (c1, c2) = Checks.For(CommentEndCrLfIni, CommentEndCrLfOpt);
			await c1.Next(IniToken.Comment, "Foo");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], ["Foo"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(CommentEndCrLfIni, CommentEndCrLfOpt, default, []);
		}
		public const string SlashEndCommentIni = ";Foo\\";
		public static readonly IniReaderOptions SlashEndCommentOpt = new(allowLineContinuations: false, ignoreCommentEscapes: true);
		[Fact]
		public static void SlashEndCommentSpan()
		{
			IniSpanReaderChecker c = new(SlashEndCommentIni, SlashEndCommentOpt);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "Foo\\");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static async Task SlashEndCommentStream()
		{
			var (c1, c2) = Checks.For(SlashEndCommentIni, SlashEndCommentOpt);
			await c1.Next(IniToken.Comment, "Foo\\");
			await c1.Next(IniToken.End, "");

			await c2.Next(new("", [], ["Foo\\"]));
			await c2.End();

			await Chk.CheckAllIniDictionaryReader(SlashEndCommentIni, SlashEndCommentOpt, default, []);
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
		}
		public const string CommentEscapeSequenceDifferentBlocksIni = "; Hello \\nworld!";
		[Fact]
		public static void CommentEscapeSequenceDifferentBlocks()
		{
			IniSpanReaderChecker c = new(CommentEscapeSequenceDifferentBlocksIni, 9, default, false);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello ");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(CommentEscapeSequenceDifferentBlocksIni, true, 8);
			c.Next(IniContentType.CommentEscaped, "\\nworld!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
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
		public const string CommentSeparateBlocksIni = "; Hello world!";
		[Fact]
		public static void CommentSeparateBlocks()
		{
			IniSpanReaderChecker c = new(CommentSeparateBlocksIni, 8, default, false);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello ");
			c.Next(IniContentType.End, default);
			c = c.NewBlock(CommentSeparateBlocksIni, true, 6);
			c.Next(IniContentType.Comment, "world!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);
		}
		[Fact]
		public static void LotsOfIgnoredComments()
		{
			// This used to throw a stack overflow exception
			IniSpanReaderChecker c1 = new(string.Join('\n', Enumerable.Repeat("; Hello!", 500)), new IniReaderOptions(ignoreComments: true));
			c1.Next(IniContentType.End, default);

			// Just make sure we pick up all the comments properly
			IniSpanReaderChecker c2 = new(string.Join('\n', Enumerable.Repeat("; Hello!", 500)), new IniReaderOptions(ignoreComments: false));
			for (int i = 0; i < 499; i++)
			{
				c2.Next(IniContentType.StartComment, ";");
				c2.Next(IniContentType.Comment, " Hello!");
				c2.Next(IniContentType.EndComment, "\n");
			}
			c2.Next(IniContentType.StartComment, ";");
			c2.Next(IniContentType.Comment, " Hello!");
			c2.Next(IniContentType.EndComment, default);
			c2.Next(IniContentType.End, default);
		}
	}
}