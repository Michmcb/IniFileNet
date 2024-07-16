namespace IniFileNet.IO
{
	using System;
#if NET8_0_OR_GREATER
	using System.Buffers;
#endif
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Threading.Tasks;
	/// <summary>
	/// Escapes text and writes it to an <see cref="IniEscapedStreamWriter"/>.
	/// </summary>
	public interface IIniTextEscaper
	{
		/// <summary>
		/// Writes <paramref name="text"/> to <paramref name="writer"/>, escaping any special characters as necessary.
		/// </summary>
		/// <param name="text">The unescaped text to write.</param>
		/// <param name="writer">The <see cref="IniEscapedStreamWriter"/> to write to.</param>
		/// <param name="token">The type of token being written.</param>
		void WriteEscaped(ReadOnlySpan<char> text, IniEscapedStreamWriter writer, IniWriteToken token);
		/// <summary>
		/// Writes <paramref name="text"/> to <paramref name="writer"/>, escaping any special characters as necessary.
		/// </summary>
		/// <param name="text">The unescaped text to write.</param>
		/// <param name="writer">The <see cref="IniEscapedStreamWriter"/> to write to.</param>
		/// <param name="token">The type of token being written.</param>
		Task WriteEscapedAsync(ReadOnlySpan<char> text, IniEscapedStreamWriter writer, IniWriteToken token);
	}
	public sealed class DefaultIniTextEscaper : IIniTextEscaper
	{
#if NET8_0_OR_GREATER
		private static readonly SearchValues<char> valueEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n']);
		private static readonly SearchValues<char> keyEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', '=', ':']);
		private static readonly SearchValues<char> sectionEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ']']);
		private static readonly SearchValues<char> commentEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#']);
#endif
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static char GetEscapeChar(char c)
		{
			// The first thing we can do, which ties us to the IniEscapedStreamWriter, is write it directly. That allows checking that we have actually escaped the text properly.

			// The second thing we can do is have a way to request a buffer of a certain size, and write the entire escaped string to that buffer. That requires us to figure out how large the encoded string will be.
			// Might be able to do a stackalloc, but we basically need to find the first index of a character we need to escape, then multiply by the worst case expansion factor (2 in our case).

			// The third way is we have a callback that accepts ReadOnlySpan<char>, along with a boolean "is escape character". Which is kinda what we have now really with IniEscapedStreamWriter.
			// But what we can do is somehow change it so that the method calls accept the struct, and that struct's responsibility is to break the text up into chunks. The only tricky part is that ref structs can't impl interfaces
			return c switch
			{
				'\\' => '\\',
				'\0' => '0',
				'\a' => 'a',
				'\b' => 'b',
				'\r' => 'r',
				'\n' => 'n',
				'=' => '=',
				':' => ':',
				']' => ']',
				';' => ';',
				'#' => '#',
				_ => default,
			};
		}
		public void WriteEscaped(ReadOnlySpan<char> text, IniEscapedStreamWriter writer, IniWriteToken token)
		{
#if NET8_0_OR_GREATER
			ReadOnlySpan<char> str = text;
			int idx = text.IndexOfAny(valueEscapeChars);
			while (idx != -1)
			{
				writer.Write(str[..idx]);
				writer.WriteEscapeSequence(GetEscapeChar(str[idx]));
				str = str[(idx + 1)..];
			}
			writer.Write(str);
#endif
			// Basically we want to say, write raw. And if that works fine, yay! We also need to know, what characters are straight-up illegal?
			// Practically, there are only a few cases.
			// If we're writing a section, then ] is escaped as \]
			// If we're writing a key, then = and : are escaped as \= and \:
			// If we're writing a comment, then ; and # are escaped as \; and \#
			// Values have nothing else to escape.

			// We still want to be able to do checks for valid keys, sections, and comments inside of IniStreamWriter. Therefore the ideal design would be to have methods named like "WriteEscapedXXX". Those methods are where we do the actual check, and 
		}
		public Task WriteEscapedAsync(ReadOnlySpan<char> text, IniEscapedStreamWriter writer, IniWriteToken token)
		{
			throw new NotImplementedException();
		}
	}
	public readonly struct IniEscapedStreamWriter
	{
		private readonly TextWriter writer;
		private readonly IniWriteToken token;
		internal IniEscapedStreamWriter(TextWriter writer, IniWriteToken token)
		{
			this.writer = writer;
			this.token = token;
		}
		public void Write(ReadOnlySpan<char> text)
		{
			// TODO check the text conforms to what exactly it is we are writing

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
	}
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
		public IniStreamWriter(TextWriter writer, NewLineStyle newLine, IIniTextEscaper escaper, KeyDelimStyle keyDelimStyle = default, CommentStyle commentStyle = default, bool leaveOpen = false)
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
		public IIniTextEscaper Escaper { get; }
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
			if (!Syntax.IsLegalSectionName(name)) throw new ArgumentException("Illegal section name", nameof(name));
			writer.Write('[');
			Escaper.WriteValueEscaped(this, name);
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
				Escaper.WriteValueEscaped(this, comment.Slice(0, nl));
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
					Escaper.WriteValueEscaped(this, comment.Slice(nl - nlLength, nlLength));
				}
				writer.Write(CommentStart);

				comment = comment.Slice(nl);
			}
			Escaper.WriteValueEscaped(this, comment);
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
			if (!Syntax.IsLegalKey(key)) throw new ArgumentException("Illegal key", nameof(key));

			Escaper.WriteValueEscaped(this, key);
			writer.Write(KeyDelim);
			Escaper.WriteValueEscaped(this, value);
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
