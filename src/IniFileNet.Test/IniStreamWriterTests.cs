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
		private static string WriteSectionName(string name)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true);
			writer.WriteSection(name);
			return sw.ToString();
		}
		[Fact]
		public static void EscapedSectionName()
		{
			Assert.Equal("[\\[hey]\n", WriteSectionName("[hey"));
			Assert.Equal("[\\]hey]\n", WriteSectionName("]hey"));
			Assert.Equal("[\\;hey]\n", WriteSectionName(";hey"));
			Assert.Equal("[\\#hey]\n", WriteSectionName("#hey"));
			Assert.Equal("[he\\ny]\n", WriteSectionName("he\ny"));
			Assert.Equal("[h\\rey]\n", WriteSectionName("h\rey"));
		}
		private static string WriteKeyValue(string key, string value)
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true);
			writer.WriteKeyValue(key, value);
			return sw.ToString();
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
		[Fact]
		public static void EscapedKeyName()
		{
			Assert.Equal("\\=hey=x\n", WriteKeyValue("=hey", "x"));
			Assert.Equal("\\:hey=x\n", WriteKeyValue(":hey", "x"));
			Assert.Equal("\\;hey=x\n", WriteKeyValue(";hey", "x"));
			Assert.Equal("\\#hey=x\n", WriteKeyValue("#hey", "x"));
			Assert.Equal("he\\ny=x\n", WriteKeyValue("he\ny", "x"));
			Assert.Equal("h\\rey=x\n", WriteKeyValue("h\rey", "x"));
		}
	}
}