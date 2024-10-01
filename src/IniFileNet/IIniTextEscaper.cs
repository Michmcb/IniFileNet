namespace IniFileNet
{
	using System;
	using System.Buffers;

	/// <summary>
	/// Escapes ini text.
	/// </summary>
	public interface IIniTextEscaper
	{
		/// <summary>
		/// Escapes special characters in <paramref name="text"/>, and writes the result to <paramref name="destination"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="destination">The destination span.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="charsConsumed">The number of chars consumed from <paramref name="text"/>.</param>
		/// <param name="charsWritten">The number of chars written to <paramref name="destination"/>.</param>
		/// <param name="isFinalBlock"><see langword="true"/> if there is no further data to be encoded, else <see langword="false"/></param>
		OperationStatusMsg Escape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock);
		/// <summary>
		/// Unescapes characters that have been escaped in <paramref name="text"/>, and writes the result to <paramref name="destination"/>.
		/// </summary>
		/// <param name="text">The text to escape.</param>
		/// <param name="destination">The destination span.</param>
		/// <param name="tokenContext">The type of token being written.</param>
		/// <param name="charsConsumed">The number of chars consumed from <paramref name="text"/>.</param>
		/// <param name="charsWritten">The number of chars written to <paramref name="destination"/>.</param>
		/// <param name="isFinalBlock"><see langword="true"/> if there is no further data to be encoded, else <see langword="false"/></param>
		OperationStatusMsg Unescape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock);
	}
}
