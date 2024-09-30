namespace IniFileNet.Test
{
	using IniFileNet;
	using IniFileNet.IO;
	using System;
	using System.IO;
	using Xunit;
	public static class IniStreamWriterTests
	{
		[Fact]
		public static void BasicFile()
		{
			string expected = ";Comment\n\n[Section]\nKey=Value\n";

			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Cr, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace('\n', '\r'), sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.CrLf, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace("\n", "\r\n"), sw.ToString());
			}
			{
				StringWriter sw = new();
				string newline;
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Platform, leaveOpen: true))
				{
					newline = writer.NewLine.Span.ToString();
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace("\n", newline), sw.ToString());
			}
		}
		[Fact]
		public static void BasicFileDifferentChars()
		{
			string expected = "#Comment\n\n[Section]\nKey:Value\n";

			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Cr, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace('\n', '\r'), sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.CrLf, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
				{
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace("\n", "\r\n"), sw.ToString());
			}
			{
				StringWriter sw = new();
				string newline;
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Platform, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
				{
					newline = writer.NewLine.Span.ToString();
					writer.WriteComment("Comment");
					writer.WriteBlankLine();
					writer.WriteSection("Section");
					writer.WriteKeyValue("Key", "Value");
				}
				Assert.Equal(expected.Replace("\n", newline), sw.ToString());
			}
		}
		[Fact]
		public static void MultilineComment()
		{
			string expected = ";Line1\n;Line2\n;Line3\n;\n";

			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, leaveOpen: true))
				{
					writer.WriteComment("Line1\nLine2\nLine3\n", replaceLineBreaks: false);
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, leaveOpen: true))
				{
					writer.WriteComment("Line1\r\nLine2\r\nLine3\r\n", replaceLineBreaks: true);
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, null, NewLineStyle.CrLf, leaveOpen: true))
				{
					writer.WriteComment("Line1\nLine2\nLine3\n", replaceLineBreaks: true);
				}
				Assert.Equal(expected.Replace("\n", "\r\n"), sw.ToString());
			}
		}
		[Fact]
		public static void IllegalSectionName()
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, leaveOpen: true);
			Assert.Throws<ArgumentException>(() => writer.WriteSection(null!));
			Assert.Throws<ArgumentException>(() => writer.WriteSection(""));
		}
		[Fact]
		public static void IllegalKey()
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, null, NewLineStyle.Lf, leaveOpen: true);
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(null!, "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(" ", "Value"));
		}
		private static string WriteSectionName(string name, IIniTextEscaper? escaper)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, escaper: escaper, newLine: NewLineStyle.Lf, leaveOpen: true);
			writer.WriteSection(name);
			return sw.ToString();
		}
		private static string WriteKeyValue(string key, string value, IIniTextEscaper? escaper)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, escaper: escaper, newLine: NewLineStyle.Lf, leaveOpen: true);
			writer.WriteKeyValue(key, value);
			return sw.ToString();
		}
		private static string WriteComment(string comment, IIniTextEscaper? escaper)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, escaper: escaper, newLine: NewLineStyle.Lf, leaveOpen: true);
			writer.WriteComment(comment);
			return sw.ToString();
		}
		[Fact]
		public static void EscapedSectionName()
		{
			DefaultIniTextEscaper escaper1 = new(true);
			Assert.Equal("[\\\\hey]\n", WriteSectionName("\\hey", escaper1));
			Assert.Equal("[\\[hey]\n", WriteSectionName("[hey", escaper1));
			Assert.Equal("[\\]hey]\n", WriteSectionName("]hey", escaper1));
			Assert.Equal("[\\;hey]\n", WriteSectionName(";hey", escaper1));
			Assert.Equal("[\\:hey]\n", WriteSectionName(":hey", escaper1));
			Assert.Equal("[\\=hey]\n", WriteSectionName("=hey", escaper1));
			Assert.Equal("[\\#hey]\n", WriteSectionName("#hey", escaper1));
			Assert.Equal("[he\\ny]\n", WriteSectionName("he\ny", escaper1));
			Assert.Equal("[h\\rey]\n", WriteSectionName("h\rey", escaper1));

			DefaultIniTextEscaper escaper2 = new(false);
			Assert.Equal("[\\\\hey]\n", WriteSectionName("\\hey", escaper2));
			Assert.Equal("[\\[hey]\n", WriteSectionName("[hey", escaper2));
			Assert.Equal("[\\]hey]\n", WriteSectionName("]hey", escaper2));
			Assert.Equal("[\\;hey]\n", WriteSectionName(";hey", escaper2));
			Assert.Equal("[:hey]\n", WriteSectionName(":hey", escaper2));
			Assert.Equal("[=hey]\n", WriteSectionName("=hey", escaper2));
			Assert.Equal("[\\#hey]\n", WriteSectionName("#hey", escaper2));
			Assert.Equal("[he\\ny]\n", WriteSectionName("he\ny", escaper2));
			Assert.Equal("[h\\rey]\n", WriteSectionName("h\rey", escaper2));
		}
		[Fact]
		public static void EscapedKeyName()
		{
			DefaultIniTextEscaper escaper1 = new(true);
			Assert.Equal("\\\\hey=value\n", WriteKeyValue("\\hey", "value", escaper1));
			Assert.Equal("\\=hey=value\n", WriteKeyValue("=hey", "value", escaper1));
			Assert.Equal("\\:hey=value\n", WriteKeyValue(":hey", "value", escaper1));
			Assert.Equal("\\;hey=value\n", WriteKeyValue(";hey", "value", escaper1));
			Assert.Equal("\\[hey=value\n", WriteKeyValue("[hey", "value", escaper1));
			Assert.Equal("\\]hey=value\n", WriteKeyValue("]hey", "value", escaper1));
			Assert.Equal("\\#hey=value\n", WriteKeyValue("#hey", "value", escaper1));
			Assert.Equal("he\\ny=value\n", WriteKeyValue("he\ny", "value", escaper1));
			Assert.Equal("h\\rey=value\n", WriteKeyValue("h\rey", "value", escaper1));

			DefaultIniTextEscaper escaper2 = new(false);
			Assert.Equal("\\\\hey=value\n", WriteKeyValue("\\hey", "value", escaper2));
			Assert.Equal("\\=hey=value\n", WriteKeyValue("=hey", "value", escaper2));
			Assert.Equal("\\:hey=value\n", WriteKeyValue(":hey", "value", escaper2));
			Assert.Equal("\\;hey=value\n", WriteKeyValue(";hey", "value", escaper2));
			Assert.Equal("\\[hey=value\n", WriteKeyValue("[hey", "value", escaper2));
			Assert.Equal("]hey=value\n", WriteKeyValue("]hey", "value", escaper2));
			Assert.Equal("\\#hey=value\n", WriteKeyValue("#hey", "value", escaper2));
			Assert.Equal("he\\ny=value\n", WriteKeyValue("he\ny", "value", escaper2));
			Assert.Equal("h\\rey=value\n", WriteKeyValue("h\rey", "value", escaper2));
		}
		[Fact]
		public static void EscapedValueName()
		{
			DefaultIniTextEscaper escaper1 = new(true);
			Assert.Equal("hey=\\\\value\n", WriteKeyValue("hey", "\\value", escaper1));
			Assert.Equal("hey=\\=value\n", WriteKeyValue("hey", "=value", escaper1));
			Assert.Equal("hey=\\:value\n", WriteKeyValue("hey", ":value", escaper1));
			Assert.Equal("hey=\\;value\n", WriteKeyValue("hey", ";value", escaper1));
			Assert.Equal("hey=\\[value\n", WriteKeyValue("hey", "[value", escaper1));
			Assert.Equal("hey=\\]value\n", WriteKeyValue("hey", "]value", escaper1));
			Assert.Equal("hey=\\#value\n", WriteKeyValue("hey", "#value", escaper1));
			Assert.Equal("hey=\\nvalue\n", WriteKeyValue("hey", "\nvalue", escaper1));
			Assert.Equal("hey=\\rvalue\n", WriteKeyValue("hey", "\rvalue", escaper1));

			DefaultIniTextEscaper escaper2 = new(false);
			Assert.Equal("hey=\\\\value\n", WriteKeyValue("hey", "\\value", escaper2));
			Assert.Equal("hey==value\n", WriteKeyValue("hey", "=value", escaper2));
			Assert.Equal("hey=:value\n", WriteKeyValue("hey", ":value", escaper2));
			Assert.Equal("hey=;value\n", WriteKeyValue("hey", ";value", escaper2));
			Assert.Equal("hey=[value\n", WriteKeyValue("hey", "[value", escaper2));
			Assert.Equal("hey=]value\n", WriteKeyValue("hey", "]value", escaper2));
			Assert.Equal("hey=#value\n", WriteKeyValue("hey", "#value", escaper2));
			Assert.Equal("hey=\\nvalue\n", WriteKeyValue("hey", "\nvalue", escaper2));
			Assert.Equal("hey=\\rvalue\n", WriteKeyValue("hey", "\rvalue", escaper2));
		}
		[Fact]
		public static void EscapedComment()
		{
			DefaultIniTextEscaper escaper1 = new(true);
			Assert.Equal(";\\\\comment\n", WriteComment("\\comment", escaper1));
			Assert.Equal(";\\[comment\n", WriteComment("[comment", escaper1));
			Assert.Equal(";\\]comment\n", WriteComment("]comment", escaper1));
			Assert.Equal(";\\;comment\n", WriteComment(";comment", escaper1));
			Assert.Equal(";\\:comment\n", WriteComment(":comment", escaper1));
			Assert.Equal(";\\=comment\n", WriteComment("=comment", escaper1));
			Assert.Equal(";\\#comment\n", WriteComment("#comment", escaper1));
			// newlines in comments get incorporated into the comment
			Assert.Equal(";\n;comment\n", WriteComment("\ncomment", escaper1));
			Assert.Equal(";\n;comment\n", WriteComment("\rcomment", escaper1));

			DefaultIniTextEscaper escaper2 = new(false);
			Assert.Equal(";\\\\comment\n", WriteComment("\\comment", escaper2));
			Assert.Equal(";[comment\n", WriteComment("[comment", escaper2));
			Assert.Equal(";]comment\n", WriteComment("]comment", escaper2));
			Assert.Equal(";;comment\n", WriteComment(";comment", escaper2));
			Assert.Equal(";:comment\n", WriteComment(":comment", escaper2));
			Assert.Equal(";=comment\n", WriteComment("=comment", escaper2));
			Assert.Equal(";#comment\n", WriteComment("#comment", escaper2));
			Assert.Equal(";\n;comment\n", WriteComment("\ncomment", escaper2));
			Assert.Equal(";\n;comment\n", WriteComment("\rcomment", escaper2));
		}
		[Fact]
		public static void BadEscapedThings()
		{
			StubEscaper badEscaper = new((IniTokenContext)(-1), "Failure message");
			StubEscaper keyEscaper = new(IniTokenContext.Key, "Failure message");
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because Failure message:Se\nction", Assert.Throws<IniException>(() => WriteSectionName("Se\nction", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because Failure message:ke=y", Assert.Throws<IniException>(() => WriteKeyValue("ke=y", "value", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because Failure message:valu#e", Assert.Throws<IniException>(() => WriteKeyValue("key", "valu#e", keyEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because Failure message:Comm\0ent", Assert.Throws<IniException>(() => WriteComment("Comm\0ent", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because Failure message:Co\0m", Assert.Throws<IniException>(() => WriteComment("Co\0m\nment", badEscaper)));

			badEscaper = new((IniTokenContext)(-1), null);
			keyEscaper = new(IniTokenContext.Key, null);
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because (no error message received):Se\nction", Assert.Throws<IniException>(() => WriteSectionName("Se\nction", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because (no error message received):ke=y", Assert.Throws<IniException>(() => WriteKeyValue("ke=y", "value", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because (no error message received):valu#e", Assert.Throws<IniException>(() => WriteKeyValue("key", "valu#e", keyEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because (no error message received):Comm\0ent", Assert.Throws<IniException>(() => WriteComment("Comm\0ent", badEscaper)));
			Chk.IniException(IniErrorCode.InvalidEscapeSequence, "Cannot escape text because (no error message received):Co\0m", Assert.Throws<IniException>(() => WriteComment("Co\0m\nment", badEscaper)));
		}
	}
}