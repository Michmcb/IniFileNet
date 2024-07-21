namespace IniFileNet.IO
{
	using System;
	using System.IO;
	/// <summary>
	/// Writes an ini text stream.
	/// </summary>
	public sealed class IniStreamWriter : IDisposable
	{
		private static readonly char[] crlf = ['\r', '\n'];
		private static readonly char[] cr = ['\r'];
		private static readonly char[] lf = ['\n'];
		private readonly TextWriter writer;
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniStreamWriter(TextWriter writer, NewLineStyle newLine, IniTextEscaperBuffer? escaper = null, KeyDelimStyle keyDelimStyle = default, CommentStyle commentStyle = default, bool leaveOpen = false)
		{
			this.writer = writer;
			Escaper = escaper ?? new IniTextEscaperBuffer(new(), DefaultIniTextEscaper.Default);
			NewLine = newLine switch
			{
				NewLineStyle.Lf => lf,
				NewLineStyle.CrLf => crlf,
				NewLineStyle.Cr => cr,
				_ => Environment.NewLine.AsMemory(),
			};
			KeyDelim = keyDelimStyle switch
			{
				KeyDelimStyle.Colon => ':',
				_ => '=',
			};
			CommentStart = commentStyle switch
			{
				CommentStyle.NumberSign => '#',
				_ => ';',
			};
			LeaveOpen = leaveOpen;
		}
		/// <summary>
		/// The newline to use.
		/// </summary>
		public ReadOnlyMemory<char> NewLine { get; }
		/// <summary>
		/// The escaper to use when escaping text to write.
		/// </summary>
		public IniTextEscaperBuffer Escaper { get; }
		/// <summary>
		/// The key delimiter character to use.
		/// </summary>
		public char KeyDelim { get; }
		/// <summary>
		/// The comment start character to use.
		/// </summary>
		public char CommentStart { get; }
		/// <summary>
		/// <see langword="true"/> to leave <see cref="writer"/> open after this <see cref="IniStreamWriter"/> is disposed. Otherwise, false.
		/// </summary>
		public bool LeaveOpen { get; set; }
		/// <summary>
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.</exception>
		public void WriteSection(string name) => WriteSection(name.AsSpan());
		/// <summary>
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.</exception>
		public void WriteSection(ReadOnlySpan<char> name)
		{
			if (name.IsEmpty) throw new ArgumentException("Illegal section name", nameof(name));
			writer.Write('[');
			Escaper.Escape(name, IniWriteToken.Section);
			Escaper.WriteTo(writer);
			writer.Write(']');
			writer.Write(NewLine);
		}
		/// <summary>
		/// Writes a comment.
		/// </summary>
		/// <param name="comment">The comment text.</param>
		/// <param name="replaceLineBreaks">If true, line breaks within <paramref name="comment"/> will be replaced with <see cref="NewLine"/></param>
		public void WriteComment(string comment, bool replaceLineBreaks = true) => WriteComment(comment.AsSpan(), replaceLineBreaks);
		/// <summary>
		/// Writes a comment.
		/// </summary>
		/// <param name="comment">The comment text.</param>
		/// <param name="replaceLineBreaks">If true, line breaks within <paramref name="comment"/> will be replaced with <see cref="NewLine"/></param>
		public void WriteComment(ReadOnlySpan<char> comment, bool replaceLineBreaks = true)
		{
			writer.Write(CommentStart);

			int nl;
#if NET8_0_OR_GREATER
			while ((nl = comment.IndexOfAny(Syntax.NewLineChars)) != -1)
#else
			while ((nl = comment.IndexOfAny(Syntax.NewLineCharsAsMemory.Span)) != -1)
#endif
			{
				Escaper.Escape(comment.Slice(0, nl), IniWriteToken.Comment);
				Escaper.WriteTo(writer);
				// If we hit \r and the next character is \n, we skip 2
				// Otherwise, just skip 1
				int nlLength = comment[nl] == '\r' && comment.Length >= nl + 1 && comment[nl + 1] == '\n' ? 2 : 1;
				nl += nlLength;
				if (replaceLineBreaks)
				{
					// Just write the newline we've been configured with
					writer.Write(NewLine);
				}
				else
				{
					writer.Write(comment.Slice(nl - nlLength, nlLength));
				}
				writer.Write(CommentStart);

				comment = comment.Slice(nl);
			}
			Escaper.Escape(comment, IniWriteToken.Comment);
			Escaper.WriteTo(writer);
			writer.Write(NewLine);
		}
		/// <summary>
		/// Writes <paramref name="key"/>, then <see cref="KeyDelim"/>, then <paramref name="value"/>. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/> returns <see langword="false"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/>.</exception>
		public void WriteKeyValue(string key, string value) => WriteKeyValue(key.AsSpan(), value.AsSpan());
		/// <summary>
		/// Writes <paramref name="key"/>, then <see cref="KeyDelim"/>, then <paramref name="value"/>. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/> returns <see langword="false"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/>.</exception>
		public void WriteKeyValue(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
		{
			if (key.IsEmpty || key.IsWhiteSpace()) throw new ArgumentException("Illegal key name", nameof(key));
			Escaper.Escape(key, IniWriteToken.Key);
			Escaper.WriteTo(writer);
			writer.Write(KeyDelim);
			Escaper.Escape(value, IniWriteToken.Value);
			Escaper.WriteTo(writer);
			writer.Write(NewLine);
		}
		/// <summary>
		/// Writes a backslash followed by <paramref name="c"/>.
		/// </summary>
		/// <param name="c">The character representing the escape sequence.</param>
		public void WriteEscapeSequence(char c)
		{
			writer.Write('\\');
			writer.Write(c);
		}
		/// <summary>
		/// Writes a backslash followed by <see cref="NewLine"/> to <see cref="writer"/>.
		/// </summary>
		public void WriteLineContinuation()
		{
			writer.Write('\\');
			writer.Write(NewLine);
		}
		/// <summary>
		/// Writes <see cref="NewLine"/> to <see cref="writer"/>.
		/// </summary>
		public void WriteBlankLine()
		{
			writer.Write(NewLine);
		}
		/// <summary>
		/// Disposes of <see cref="writer"/> if <see cref="LeaveOpen"/> is <see langword="false"/>. Otherwise, does nothing.
		/// </summary>
		public void Dispose()
		{
			writer.Dispose();
		}
	}
}
