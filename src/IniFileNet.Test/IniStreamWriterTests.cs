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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Cr, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.CrLf, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Platform, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Lf, null, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Cr, null, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.CrLf, null, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Platform, null, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true))
				{
					writer.WriteComment("Line1\nLine2\nLine3\n", replaceLineBreaks: false);
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true))
				{
					writer.WriteComment("Line1\r\nLine2\r\nLine3\r\n", replaceLineBreaks: true);
				}
				Assert.Equal(expected, sw.ToString());
			}
			{
				StringWriter sw = new();
				using (IniStreamWriter writer = new(sw, NewLineStyle.CrLf, leaveOpen: true))
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
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true);
			Assert.Throws<ArgumentException>(() => writer.WriteSection(null!));
			Assert.Throws<ArgumentException>(() => writer.WriteSection(""));
		}
		[Fact]
		public static void IllegalKey()
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true);
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(null!, "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(" ", "Value"));
		}
		private static string WriteSectionName(string name, bool alwaysEscapeAll)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, escaper: new(new(), new DefaultIniTextEscaper(alwaysEscapeAll)), leaveOpen: true);
			writer.WriteSection(name);
			return sw.ToString();
		}
		[Fact]
		public static void EscapedSectionName()
		{
			Assert.Equal("[\\\\hey]\n", WriteSectionName("\\hey", true));
			Assert.Equal("[\\0hey]\n", WriteSectionName("\0hey", true));
			Assert.Equal("[\\ahey]\n", WriteSectionName("\ahey", true));
			Assert.Equal("[\\bhey]\n", WriteSectionName("\bhey", true));
			Assert.Equal("[\\[hey]\n", WriteSectionName("[hey", true));
			Assert.Equal("[\\]hey]\n", WriteSectionName("]hey", true));
			Assert.Equal("[\\;hey]\n", WriteSectionName(";hey", true));
			Assert.Equal("[\\:hey]\n", WriteSectionName(":hey", true));
			Assert.Equal("[\\=hey]\n", WriteSectionName("=hey", true));
			Assert.Equal("[\\#hey]\n", WriteSectionName("#hey", true));
			Assert.Equal("[he\\ny]\n", WriteSectionName("he\ny", true));
			Assert.Equal("[h\\rey]\n", WriteSectionName("h\rey", true));

			Assert.Equal("[\\\\hey]\n", WriteSectionName("\\hey", false));
			Assert.Equal("[\\0hey]\n", WriteSectionName("\0hey", false));
			Assert.Equal("[\\ahey]\n", WriteSectionName("\ahey", false));
			Assert.Equal("[\\bhey]\n", WriteSectionName("\bhey", false));
			Assert.Equal("[\\[hey]\n", WriteSectionName("[hey", false));
			Assert.Equal("[\\]hey]\n", WriteSectionName("]hey", false));
			Assert.Equal("[\\;hey]\n", WriteSectionName(";hey", false));
			Assert.Equal("[:hey]\n", WriteSectionName(":hey", false));
			Assert.Equal("[=hey]\n", WriteSectionName("=hey", false));
			Assert.Equal("[\\#hey]\n", WriteSectionName("#hey", false));
			Assert.Equal("[he\\ny]\n", WriteSectionName("he\ny", false));
			Assert.Equal("[h\\rey]\n", WriteSectionName("h\rey", false));
		}
		private static string WriteKeyValue(string key, string value, bool alwaysEscapeAll)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, escaper: new(new(), new DefaultIniTextEscaper(alwaysEscapeAll)), leaveOpen: true);
			writer.WriteKeyValue(key, value);
			return sw.ToString();
		}
		[Fact]
		public static void EscapedKeyName()
		{
			Assert.Equal("\\\\hey=value\n", WriteKeyValue("\\hey", "value", true));
			Assert.Equal("\\0hey=value\n", WriteKeyValue("\0hey", "value", true));
			Assert.Equal("\\ahey=value\n", WriteKeyValue("\ahey", "value", true));
			Assert.Equal("\\bhey=value\n", WriteKeyValue("\bhey", "value", true));
			Assert.Equal("\\=hey=value\n", WriteKeyValue("=hey", "value", true));
			Assert.Equal("\\:hey=value\n", WriteKeyValue(":hey", "value", true));
			Assert.Equal("\\;hey=value\n", WriteKeyValue(";hey", "value", true));
			Assert.Equal("\\[hey=value\n", WriteKeyValue("[hey", "value", true));
			Assert.Equal("\\]hey=value\n", WriteKeyValue("]hey", "value", true));
			Assert.Equal("\\#hey=value\n", WriteKeyValue("#hey", "value", true));
			Assert.Equal("he\\ny=value\n", WriteKeyValue("he\ny", "value", true));
			Assert.Equal("h\\rey=value\n", WriteKeyValue("h\rey", "value", true));

			Assert.Equal("\\\\hey=value\n", WriteKeyValue("\\hey", "value", false));
			Assert.Equal("\\0hey=value\n", WriteKeyValue("\0hey", "value", false));
			Assert.Equal("\\ahey=value\n", WriteKeyValue("\ahey", "value", false));
			Assert.Equal("\\bhey=value\n", WriteKeyValue("\bhey", "value", false));
			Assert.Equal("\\=hey=value\n", WriteKeyValue("=hey", "value", false));
			Assert.Equal("\\:hey=value\n", WriteKeyValue(":hey", "value", false));
			Assert.Equal("\\;hey=value\n", WriteKeyValue(";hey", "value", false));
			Assert.Equal("\\[hey=value\n", WriteKeyValue("[hey", "value", false));
			Assert.Equal("]hey=value\n", WriteKeyValue("]hey", "value", false));
			Assert.Equal("\\#hey=value\n", WriteKeyValue("#hey", "value", false));
			Assert.Equal("he\\ny=value\n", WriteKeyValue("he\ny", "value", false));
			Assert.Equal("h\\rey=value\n", WriteKeyValue("h\rey", "value", false));
		}
		[Fact]
		public static void EscapedValueName()
		{
			Assert.Equal("hey=\\\\value\n", WriteKeyValue("hey", "\\value", true));
			Assert.Equal("hey=\\0value\n", WriteKeyValue("hey", "\0value", true));
			Assert.Equal("hey=\\avalue\n", WriteKeyValue("hey", "\avalue", true));
			Assert.Equal("hey=\\bvalue\n", WriteKeyValue("hey", "\bvalue", true));
			Assert.Equal("hey=\\=value\n", WriteKeyValue("hey", "=value", true));
			Assert.Equal("hey=\\:value\n", WriteKeyValue("hey", ":value", true));
			Assert.Equal("hey=\\;value\n", WriteKeyValue("hey", ";value", true));
			Assert.Equal("hey=\\[value\n", WriteKeyValue("hey", "[value", true));
			Assert.Equal("hey=\\]value\n", WriteKeyValue("hey", "]value", true));
			Assert.Equal("hey=\\#value\n", WriteKeyValue("hey", "#value", true));
			Assert.Equal("hey=\\nvalue\n", WriteKeyValue("hey", "\nvalue", true));
			Assert.Equal("hey=\\rvalue\n", WriteKeyValue("hey", "\rvalue", true));

			Assert.Equal("hey=\\\\value\n", WriteKeyValue("hey", "\\value", false));
			Assert.Equal("hey=\\0value\n", WriteKeyValue("hey", "\0value", false));
			Assert.Equal("hey=\\avalue\n", WriteKeyValue("hey", "\avalue", false));
			Assert.Equal("hey=\\bvalue\n", WriteKeyValue("hey", "\bvalue", false));
			Assert.Equal("hey==value\n", WriteKeyValue("hey", "=value", false));
			Assert.Equal("hey=:value\n", WriteKeyValue("hey", ":value", false));
			Assert.Equal("hey=;value\n", WriteKeyValue("hey", ";value", false));
			Assert.Equal("hey=[value\n", WriteKeyValue("hey", "[value", false));
			Assert.Equal("hey=]value\n", WriteKeyValue("hey", "]value", false));
			Assert.Equal("hey=#value\n", WriteKeyValue("hey", "#value", false));
			Assert.Equal("hey=\\nvalue\n", WriteKeyValue("hey", "\nvalue", false));
			Assert.Equal("hey=\\rvalue\n", WriteKeyValue("hey", "\rvalue", false));
		}
		private static string WriteComment(string comment, bool alwaysEscapeAll)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, escaper: new(new(), new DefaultIniTextEscaper(alwaysEscapeAll)), leaveOpen: true);
			writer.WriteComment(comment);
			return sw.ToString();
		}
		[Fact]
		public static void EscapedComment()
		{
			Assert.Equal(";\\\\comment\n", WriteComment("\\comment", true));
			Assert.Equal(";\\0comment\n", WriteComment("\0comment", true));
			Assert.Equal(";\\acomment\n", WriteComment("\acomment", true));
			Assert.Equal(";\\bcomment\n", WriteComment("\bcomment", true));
			Assert.Equal(";\\[comment\n", WriteComment("[comment", true));
			Assert.Equal(";\\]comment\n", WriteComment("]comment", true));
			Assert.Equal(";\\;comment\n", WriteComment(";comment", true));
			Assert.Equal(";\\:comment\n", WriteComment(":comment", true));
			Assert.Equal(";\\=comment\n", WriteComment("=comment", true));
			Assert.Equal(";\\#comment\n", WriteComment("#comment", true));
			// newlines in comments get incorporated into the comment
			Assert.Equal(";\n;comment\n", WriteComment("\ncomment", true));
			Assert.Equal(";\n;comment\n", WriteComment("\rcomment", true));

			Assert.Equal(";\\\\comment\n", WriteComment("\\comment", false));
			Assert.Equal(";\\0comment\n", WriteComment("\0comment", false));
			Assert.Equal(";\\acomment\n", WriteComment("\acomment", false));
			Assert.Equal(";\\bcomment\n", WriteComment("\bcomment", false));
			Assert.Equal(";[comment\n", WriteComment("[comment", false));
			Assert.Equal(";]comment\n", WriteComment("]comment", false));
			Assert.Equal(";;comment\n", WriteComment(";comment", false));
			Assert.Equal(";:comment\n", WriteComment(":comment", false));
			Assert.Equal(";=comment\n", WriteComment("=comment", false));
			Assert.Equal(";#comment\n", WriteComment("#comment", false));
			Assert.Equal(";\n;comment\n", WriteComment("\ncomment", false));
			Assert.Equal(";\n;comment\n", WriteComment("\rcomment", false));
		}
	}
}