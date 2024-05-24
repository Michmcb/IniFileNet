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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Lf, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Cr, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.CrLf, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
				using (IniStreamWriter writer = new(sw, NewLineStyle.Platform, KeyDelimStyle.Colon, CommentStyle.NumberSign, leaveOpen: true))
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
			Assert.Throws<ArgumentException>(() => writer.WriteSection("[hey"));
			Assert.Throws<ArgumentException>(() => writer.WriteSection("]hey"));
			Assert.Throws<ArgumentException>(() => writer.WriteSection(";hey"));
			Assert.Throws<ArgumentException>(() => writer.WriteSection("#hey"));
			Assert.Throws<ArgumentException>(() => writer.WriteSection("he\ny"));
			Assert.Throws<ArgumentException>(() => writer.WriteSection("h\rey"));
		}
		[Fact]
		public static void IllegalKey()
		{
			StringWriter sw = new();
			using IniStreamWriter writer = new(sw, NewLineStyle.Lf, leaveOpen: true);
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(null!, "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(" ", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("\t", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("=hey", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(":hey", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue(";hey", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("#hey", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("he\ny", "Value"));
			Assert.Throws<ArgumentException>(() => writer.WriteKeyValue("h\rey", "Value"));
		}
	}
}