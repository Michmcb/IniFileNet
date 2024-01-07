namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;
	public static class ParseGood
	{
		[Fact]
		public static void KeyEqualsValue()
		{
			string ini = "Key=Value";
			IniSpanReaderChecker c = new(ini, new(allowGlobalKeys: true));
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key", "Value")], []));
			c2.End();
		}
		[Fact]
		public static void KeyColonValue()
		{
			string ini = "Key:Value";
			IniSpanReaderChecker c = new(ini, new(allowGlobalKeys: true, allowKeyDelimiterColon: true));
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, ":");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key", "Value")], []));
			c2.End();
		}
		[Fact]
		public static void KeyEmptyValue()
		{
			string ini = "Key=";
			IniSpanReaderChecker c = new(ini, new(allowGlobalKeys: true));
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key", "")], []));
			c2.End();
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
		[Fact]
		public static void MultipleKeyValues()
		{
			string ini = "[Section]\rKey1=Value1\nKey2 = Value2";
			IniSpanReaderChecker c = new(ini);
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
			c.Next(IniContentType.Value, " Value2");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Key, "Key1");
			c1.Next(IniToken.Value, "Value1");
			c1.Next(IniToken.Key, "Key2 ");
			c1.Next(IniToken.Value, " Value2");
			c1.Next(IniToken.End, "");

			c2.Next(new("Section", [new("Key1", "Value1"), new("Key2 ", " Value2")], []));
			c2.End();
		}
		[Fact]
		public static void SectionKeyValueLineContinutation()
		{
			// TODO I don't actually know if this is correct - Does a line continuation eat the newline, or not?
			string ini = "[Section]\nKey1=Hello\\\nworld!";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowLineContinuations: true));
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

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Key, "Key1");
			c1.Next(IniToken.Value, "Helloworld!");
			c1.Next(IniToken.End, "");	

			c2.Next(new("Section", [new("Key1", "Helloworld!")], []));
			c2.End();
		}
		[Fact]
		public static void LineContinutationEnd()
		{
			string ini = "Key1=Value\\";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowGlobalKeys: true, allowLineContinuations: true));
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key1");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Key, "Key1");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key1", "Value")], []));
			c2.End();
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
		[Fact]
		public static void SectionKeyValue()
		{
			string ini = "[Section]\nKey1:Value1\nKey2 = Value2\n     ";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowKeyDelimiterColon: true));
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

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Key, "Key1");
			c1.Next(IniToken.Value, "Value1");
			c1.Next(IniToken.Key, "Key2 ");
			c1.Next(IniToken.Value, " Value2");
			c1.Next(IniToken.End, "");

			c2.Next(new("Section", [new("Key1", "Value1"), new("Key2 ", " Value2")], []));
			c2.End();
		}
		[Fact]
		public static void SectionCommentKeyValue()
		{
			string ini = "[Section]\n# Hello world!\nKey=Value";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowCommentsNumberSign: true, allowGlobalKeys: true));
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

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Comment, " Hello world!");
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [], []));
			c2.Next(new("Section", [new("Key", "Value", [" Hello world!"])], []));
			c2.End();
		}
		[Fact]
		public static void CommentKeyValue()
		{
			string ini = "# Hello world!\nKey=Value";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowCommentsNumberSign: true, allowGlobalKeys: true));
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

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Comment, " Hello world!");
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key", "Value", [" Hello world!"])]));
			c2.End();
		}
		[Fact]
		public static void CommentIgnored()
		{
			string ini = "# Hello world!\nKey=Value\n;Goodbye world!";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowCommentsNumberSign: true, allowGlobalKeys: true, ignoreComments: true));
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Value, "Value");
			c.Next(IniContentType.EndValue, "\n");
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Key, "Key");
			c1.Next(IniToken.Value, "Value");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [new("Key", "Value")], []));
			c2.End();
		}
		[Fact]
		public static void EmptyCommentEndOfSpan()
		{
			string ini = ";";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, "");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Comment, "");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [], [""]));
			c2.End();
		}
		[Fact]
		public static void CommentEndOfSpan()
		{
			string ini = "; Hello world!";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartComment, ";");
			c.Next(IniContentType.Comment, " Hello world!");
			c.Next(IniContentType.EndComment, default);
			c.Next(IniContentType.End, default);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Comment, " Hello world!");
			c1.Next(IniToken.End, "");

			c2.Next(new("", [], [" Hello world!"]));
			c2.End();
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
	}
}