namespace IniFileNet.IO
{
	using System;
	/// <summary>
	/// Escapes ini text.
	/// </summary>
	public interface IIniTextEscaper
	{
		/// <summary>
		/// Escapes special characters in <paramref name="text"/>, passing valid chunks that don't need escaping to <paramref name="validTextCallback"/> and the
		/// escape characters to <paramref name="charCallback"/>.
		/// </summary>
		/// <param name="text">The unescaped text to escape.</param>
		/// <param name="validTextCallback">The callback invoked to provide a valid chunk of text.</param>
		/// <param name="charCallback">The callback invoked to provide the significant character of an escape sequence. For example, the 'n' of "\n", to represent a newline.</param>
		/// <param name="token">The type of token being written.</param>
		IniError Escape(ReadOnlySpan<char> text, TextCallback validTextCallback, CharCallback charCallback, IniTokenContext token);
		/// <summary>
		/// Unescapes characters that have been escaped in <paramref name="text"/>. Unescaped text and characters are provided to <paramref name="textCallback"/> and <paramref name="charCallback"/>.
		/// </summary>
		/// <param name="text">The escaped text to unescape.</param>
		/// <param name="textCallback">The callback invoked to provide a chunk of text.</param>
		/// <param name="charCallback">The callback invoked for single unescaped characters</param>
		/// <param name="token">The type of token being read.</param>
		IniError Unescape(ReadOnlySpan<char> text, TextCallback textCallback, CharCallback charCallback, IniTokenContext token);
	}
}
