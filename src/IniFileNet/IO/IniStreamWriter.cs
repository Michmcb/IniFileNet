namespace IniFileNet.IO
{
	using IniFileNet;
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
		public IniStreamWriter(TextWriter writer, IIniTextEscaper? escaper, NewLineStyle newLine, KeyDelimStyle keyDelimStyle = default, CommentStyle commentStyle = default, bool leaveOpen = false)
		{
			this.writer = writer;
			Escaper = escaper;
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
		public IIniTextEscaper? Escaper { get; }
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
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <paramref name="name"/> is empty.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
		public void WriteSection(string name) => WriteSection(name.AsSpan());
		/// <summary>
		/// Writes a section name. Throws <see cref="ArgumentException"/> if <paramref name="name"/> is empty.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is empty.</exception>
		public void WriteSection(ReadOnlySpan<char> name)
		{
			if (name.IsEmpty) throw new ArgumentException("Illegal section name", nameof(name));
			writer.Write('[');
			if (Escaper != null)
			{
				if (!IniTextEscaperWriter.EscapeTo(name, stackalloc char[1024], IniTokenContext.Section, Escaper, writer, out string? errMsg))
				{
					throw CannotEscapeTextException(name, errMsg);
				}
			}
			else
			{
				writer.Write(name);
			}
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

			IniTextEscaperWriter w = Escaper != null ? new(Escaper, writer) : default;

			int nl;
#if NET8_0_OR_GREATER
			while ((nl = comment.IndexOfAny(Syntax.NewLineChars)) != -1)
#else
			while ((nl = comment.IndexOfAny(Syntax.NewLineCharsAsMemory.Span)) != -1)
#endif
			{
				if (Escaper != null)
				{
					if (!w.StackEscape(comment.Slice(0, nl), IniTokenContext.Comment, out string? errMsg))
					{
						throw CannotEscapeTextException(comment.Slice(0, nl), errMsg);
					}
				}
				else
				{
					writer.Write(comment.Slice(0, nl));
				}
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
			if (Escaper != null)
			{
				if (!w.StackEscape(comment, IniTokenContext.Comment, out string? errMsg))
				{
					throw CannotEscapeTextException(comment, errMsg);
				}
			}
			else
			{
				writer.Write(comment);
			}
			writer.Write(NewLine);
		}
		/// <summary>
		/// Writes <paramref name="key"/>, then <see cref="KeyDelim"/>, then <paramref name="value"/>. Throws <see cref="ArgumentException"/> if <paramref name="key"/> is empty or entire whitespace.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty or entire whitespace.</exception>
		public void WriteKeyValue(string key, string value) => WriteKeyValue(key.AsSpan(), value.AsSpan());
		/// <summary>
		/// Writes <paramref name="key"/>, then <see cref="KeyDelim"/>, then <paramref name="value"/>. Throws <see cref="ArgumentException"/> if <paramref name="key"/> is empty or entire whitespace.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <exception cref="ArgumentException">Thrown if <paramref name="key"/> is empty or entire whitespace.</exception>
		public void WriteKeyValue(ReadOnlySpan<char> key, ReadOnlySpan<char> value)
		{
			if (key.IsEmpty || key.IsWhiteSpace()) throw new ArgumentException("Illegal key name", nameof(key));
			if (Escaper != null)
			{
				Span<char> buf = stackalloc char[1024];
				if (!IniTextEscaperWriter.EscapeTo(key, buf, IniTokenContext.Key, Escaper, writer, out string? errMsg))
				{
					throw CannotEscapeTextException(key, errMsg);
				}
				writer.Write(KeyDelim);
				if (!IniTextEscaperWriter.EscapeTo(value, buf, IniTokenContext.Value, Escaper, writer, out errMsg))
				{
					throw CannotEscapeTextException(value, errMsg);
				}
			}
			else
			{
				writer.Write(key);
				writer.Write(KeyDelim);
				writer.Write(value);
			}
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
		private static IniException CannotEscapeTextException(ReadOnlySpan<char> text, string? errMsg)
		{

			return new IniException(IniErrorCode.InvalidEscapeSequence, string.Concat("Cannot escape text because ", errMsg ?? "(no error message received)", ":",
#if NETSTANDARD2_0
				text.ToString()
#else
				text
#endif
			));
		}
	}
}
