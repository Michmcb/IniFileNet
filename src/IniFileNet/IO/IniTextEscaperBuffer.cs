namespace IniFileNet.IO
{
	using System;
	using System.IO;
	using System.Threading;
	using System.Threading.Tasks;

	/// <summary>
	/// Uses a <see cref="IIniTextEscaper"/>, to escape text into a <see cref="BufferWriter{T}"/>, which can then be written
	/// to a <see cref="TextWriter"/>, or gotten as a string.
	/// </summary>
	public sealed class IniTextEscaperBuffer
	{
		private readonly BufferWriter<char> buffer;
		private readonly IIniTextEscaper escaper;
		/// <summary>
		/// Creates a new instance, using <paramref name="escaper"/> to write to <paramref name="buffer"/>.
		/// </summary>
		/// <param name="buffer">The buffer to write to.</param>
		/// <param name="escaper">The escaper to use.</param>
		public IniTextEscaperBuffer(BufferWriter<char> buffer, IIniTextEscaper escaper)
		{
			this.buffer = buffer;
			this.escaper = escaper;
		}
		/// <summary>
		/// Clears the buffer.
		/// </summary>
		public void Clear()
		{
			buffer.Clear();
		}
		/// <summary>
		/// Uses the provided <see cref="IIniTextEscaper"/> to escape <paramref name="text"/>, adding it to the buffer.
		/// </summary>
		/// <param name="text"></param>
		/// <param name="token"></param>
		public void Escape(ReadOnlySpan<char> text, IniWriteToken token)
		{
			escaper.Escape(text, WriteValidText, WriteEscapeChar, token);
		}
		/// <summary>
		/// Writes a chunk of valid text to the buffer.
		/// </summary>
		/// <param name="text">The chunk of valid text.</param>
		public void WriteValidText(ReadOnlySpan<char> text)
		{
			Span<char> span = buffer.GetSpan(text.Length);
			text.CopyTo(span);
			buffer.Advance(text.Length);
		}
		/// <summary>
		/// Writes a backslash followed by <paramref name="c"/> to the buffer.
		/// </summary>
		/// <param name="c">The second character of the escape sequence.</param>
		public void WriteEscapeChar(char c)
		{
			Span<char> span = buffer.GetSpan(2);
			span[0] = '\\';
			span[1] = c;
			buffer.Advance(2);
		}
		/// <summary>
		/// Writes the stored text to <paramref name="writer"/>, and resets the position of the stored buffer.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write the data to.</param>
		public void WriteTo(TextWriter writer)
		{
			writer.Write(buffer.Span);
			buffer.SetWritten(0);
		}
		/// <summary>
		/// Writes the stored text to <paramref name="writer"/>, and resets the position of the stored buffer.
		/// </summary>
		/// <param name="writer">The <see cref="TextWriter"/> to write the data to.</param>
		/// <param name="ct">The token to monitor for cancellation requests. Note that <see cref="TextWriter"/> in .netstandard2.0 does not
		/// support accepting <see cref="CancellationToken"/>, so cancellation may be unreliable in this case.</param>
		/// <returns></returns>
		public async Task WriteToAsync(TextWriter writer, CancellationToken ct = default)
		{
#if NETSTANDARD2_0
			// so naughty, but saves an array copy
			ct.ThrowIfCancellationRequested();
			await writer.WriteAsync(buffer.buf, 0, buffer.Written);
#else
			await writer.WriteAsync(buffer.Memory, ct);
#endif
			buffer.SetWritten(0);
		}
		/// <summary>
		/// Creates a string from the text stored in 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return new string(buffer.buf);
		}
	}
}
