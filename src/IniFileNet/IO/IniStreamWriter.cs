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
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniStreamWriter(TextWriter writer, NewLineStyle newLine, KeyDelimStyle keyDelimStyle = default, CommentStyle commentStyle = default, bool leaveOpen = false)
		{
			Writer = writer;
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
		/// The <see cref="TextWriter"/> to write to.
		/// </summary>
		public TextWriter Writer { get; }
		/// <summary>
		/// The newline to use.
		/// </summary>
		public ReadOnlyMemory<char> NewLine { get; }
		/// <summary>
		/// The key delimiter character to use.
		/// </summary>
		public char KeyDelim { get; }
		/// <summary>
		/// The comment start character to use.
		/// </summary>
		public char CommentStart { get; }
		/// <summary>
		/// <see langword="true"/> to leave <see cref="Writer"/> open after this <see cref="IniStreamWriter"/> is disposed. Otherwise, false.
		/// </summary>
		public bool LeaveOpen { get; set; }
		/// <summary>
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalSectionName(ReadOnlySpan{char})"/> returns <see langword="false"/>.</exception>
		public void WriteSection(ReadOnlySpan<char> name)
		{
			if (!Syntax.IsLegalSectionName(name)) throw new ArgumentException("Illegal section name", nameof(name));
			Writer.Write('[');
			Writer.Write(name);
			Writer.Write(']');
			Writer.Write(NewLine);
		}
		/// <summary>
		/// Writes a comment.
		/// </summary>
		/// <param name="comment">The comment text.</param>
		/// <param name="replaceLineBreaks">If true, line breaks within <paramref name="comment"/> will be replaced with <see cref="NewLine"/></param>
		public void WriteComment(ReadOnlySpan<char> comment, bool replaceLineBreaks = true)
		{
			Writer.Write(CommentStart);

			int nl;
			while ((nl = comment.IndexOfAny(Syntax.NewLineChars)) != -1)
			{
				Writer.Write(comment.Slice(0, nl));
				// If we hit \r and the next character is \n, we skip 2
				// Otherwise, just skip 1
				int nlLength = comment[nl] == '\r' && comment.Length >= nl + 1 && comment[nl + 1] == '\n' ? 2 : 1;
				nl += nlLength;
				if (replaceLineBreaks)
				{
					// Just write the newline we've been configured with
					Writer.Write(NewLine);
				}
				else
				{
					Writer.Write(comment.Slice(nl - nlLength, nlLength));
				}
				Writer.Write(CommentStart);
				
				comment = comment.Slice(nl);
			}
			Writer.Write(comment);
			Writer.Write(NewLine);
		}
		/// <summary>
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if <see cref="Syntax.IsLegalKey(ReadOnlySpan{char})"/>.</exception>
		public void WriteKeyValue(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
		{
			if (!Syntax.IsLegalKey(key)) throw new ArgumentException("Illegal key", nameof(key));

			Writer.Write(key);
			Writer.Write(KeyDelim);
			Writer.Write(value);
			Writer.Write(NewLine);
		}
		/// <summary>
		/// Writes <see cref="NewLine"/> to <see cref="Writer"/>.
		/// </summary>
		public void WriteBlankLine()
		{
			Writer.Write(NewLine);
		}
		/// <summary>
		/// Disposes of <see cref="Writer"/> if <see cref="LeaveOpen"/> is <see langword="false"/>. Otherwise, does nothing.
		/// </summary>
		public void Dispose()
		{
			Writer.Dispose();
		}
	}
}
