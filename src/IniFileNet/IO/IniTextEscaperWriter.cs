namespace IniFileNet.IO
{
	using IniFileNet;
	using System;
	using System.Buffers;
	using System.IO;

	/// <summary>
	/// Escapes text using an <see cref="IIniTextEscaper"/> and writes it to a destination <see cref="TextWriter"/>.
	/// </summary>
	public readonly struct IniTextEscaperWriter
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniTextEscaperWriter(IIniTextEscaper escaper, TextWriter writer, int bufferSize = 1024)
		{
			Escaper = escaper;
			Writer = writer;
			BufferSize = bufferSize;
		}
		/// <summary>
		/// The escaper used to escape text.
		/// </summary>
		public IIniTextEscaper Escaper { get; }
		/// <summary>
		/// The writer to output data to.
		/// </summary>
		public TextWriter Writer { get; }
		/// <summary>
		/// The size of the buffer to allocate.
		/// </summary>
		public int BufferSize { get; }
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <see name="Escaper"/> when escaping text.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool Escape(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IniTokenContext tokenContext, out string? errMsg)
		{
			return EscapeTo(text, buffer, tokenContext, Escaper, Writer, out errMsg);
		}
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// Allocates the buffer as a new array.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool Escape(scoped ReadOnlySpan<char> text, IniTokenContext tokenContext, out string? errMsg)
		{
			return EscapeTo(text, new char[BufferSize], tokenContext, Escaper, Writer, out errMsg);
		}
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// Allocates the buffer on the stack.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool StackEscape(scoped ReadOnlySpan<char> text, IniTokenContext tokenContext, out string? errMsg)
		{
			return EscapeTo(text, stackalloc char[BufferSize], tokenContext, Escaper, Writer, out errMsg);
		}
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="writer">The destination.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <paramref name="escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public static bool EscapeTo(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IniTokenContext tokenContext, IIniTextEscaper escaper, TextWriter writer, out string? errMsg)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Escape(text, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				writer.Write(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.Done:
						errMsg = null;
						return true;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
					case OperationStatus.InvalidData:
						errMsg = op.Msg;
						return false;
				}
			}
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// </summary>
		/// <param name="text">The text to unescape.</param>
		/// <param name="buffer">The buffer to provide <see name="Escaper"/> when escaping text.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool Unescape(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IniTokenContext tokenContext, out string? errMsg)
		{
			return UnescapeTo(text, buffer, tokenContext, Escaper, Writer, out errMsg);
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// Allocates the buffer as a new array.
		/// </summary>
		/// <param name="text">The text to unescape.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool Unescape(scoped ReadOnlySpan<char> text, IniTokenContext tokenContext, out string? errMsg)
		{
			return UnescapeTo(text, new char[BufferSize], tokenContext, Escaper, Writer, out errMsg); ;
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <see name="Writer"/>.
		/// Allocates the buffer on the stack.
		/// </summary>
		/// <param name="text">The text to unescape.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <see name="Escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public bool StackUnescape(scoped ReadOnlySpan<char> text, IniTokenContext tokenContext, out string? errMsg)
		{
			return UnescapeTo(text, stackalloc char[BufferSize], tokenContext, Escaper, Writer, out errMsg);
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to unescape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="escaper">The unescaper to use.</param>
		/// <param name="writer">The destination.</param>
		/// <param name="errMsg">The error message if <see langword="false"/>, otherwise null.</param>
		/// <returns><see langword="true"/> on success, or <see langword="false"/> if <paramref name="escaper"/> returns <see cref="OperationStatus.InvalidData"/>.</returns>
		public static bool UnescapeTo(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IniTokenContext tokenContext, IIniTextEscaper escaper, TextWriter writer, out string? errMsg)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Unescape(text, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				writer.Write(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.Done:
						errMsg = null;
						return true;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
					case OperationStatus.InvalidData:
						errMsg = op.Msg;
						return false;
				}
			}
		}
	}
}
