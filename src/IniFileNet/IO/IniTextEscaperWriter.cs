namespace IniFileNet.IO
{
	using IniFileNet;
	using System;
	using System.Buffers;
	using System.IO;
	using System.Threading.Tasks;

	/// <summary>
	/// Convenience methods to escape or unescape text using an <see cref="IIniTextEscaper"/>,
	/// and writes it to a destination <see cref="TextWriter"/>.
	/// </summary>
	public static class IniTextEscaperWriter
	{
		// All we need to do is accept a buffer, the text, a token context, and a destination writer.
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses a stack-allocated buffer of size <paramref name="bufferSize"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="bufferSize">The size of the stack-allocated buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static OperationStatusMsg Escape(scoped ReadOnlySpan<char> text, int bufferSize, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			return Escape(text, stackalloc char[bufferSize], escaper, tokenContext, writer);
		}
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static OperationStatusMsg Escape(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Escape(text, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				writer.Write(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static async Task<OperationStatusMsg> EscapeAsync(ReadOnlyMemory<char> text, char[] buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Escape(text.Span, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				await writer.WriteAsync(buffer, 0, w);
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
#if NET6_0_OR_GREATER
		/// <summary>
		/// Escapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for escaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when escaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static async Task<OperationStatusMsg> EscapeAsync(ReadOnlyMemory<char> text, Memory<char> buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Escape(text.Span, buffer.Span, tokenContext, out int c, out int w, isFinalBlock: true);
				await writer.WriteAsync(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
#endif
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses a stack-allocated buffer of size <paramref name="bufferSize"/> as the intermediate destination for unescaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="bufferSize">The size of the stack-allocated buffer to provide <paramref name="escaper"/> when unescaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static OperationStatusMsg Unescape(scoped ReadOnlySpan<char> text, int bufferSize, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			return Unescape(text, stackalloc char[bufferSize], escaper, tokenContext, writer);
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for unescaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when unescaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static OperationStatusMsg Unescape(scoped ReadOnlySpan<char> text, scoped Span<char> buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Unescape(text, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				writer.Write(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for unescaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when unescaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static async Task<OperationStatusMsg> UnescapeAsync(ReadOnlyMemory<char> text, char[] buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Unescape(text.Span, buffer, tokenContext, out int c, out int w, isFinalBlock: true);
				await writer.WriteAsync(buffer, 0, w);
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
#if NET6_0_OR_GREATER
		/// <summary>
		/// Unescapes all of the text in <paramref name="text"/>, and writes it to <paramref name="writer"/>.
		/// Uses <paramref name="buffer"/> as the intermediate destination for unescaping text using <paramref name="escaper"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="buffer">The buffer to provide <paramref name="escaper"/> when unescaping text.</param>
		/// <param name="escaper">The escaper to use.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="writer">The destination.</param>
		/// <returns>An <see cref="OperationStatusMsg"/>, which holds the <see cref="OperationStatus"/>.</returns>
		public static async Task<OperationStatusMsg> UnescapeAsync(ReadOnlyMemory<char> text, Memory<char> buffer, IIniTextEscaper escaper, IniTokenContext tokenContext, TextWriter writer)
		{
			while (true)
			{
				OperationStatusMsg op = escaper.Unescape(text.Span, buffer.Span, tokenContext, out int c, out int w, isFinalBlock: true);
				await writer.WriteAsync(buffer.Slice(0, w));
				switch (op.Status)
				{
					case OperationStatus.InvalidData:
					case OperationStatus.Done:
						return op;
					case OperationStatus.DestinationTooSmall:
					case OperationStatus.NeedMoreData:
						text = text.Slice(c);
						break;
				}
			}
		}
#endif
	}
}
