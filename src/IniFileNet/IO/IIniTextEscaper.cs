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
		/// <param name="text">The unescaped text to write.</param>
		/// <param name="validTextCallback">The callback invoked to provide a valid chunk of text.</param>
		/// <param name="charCallback">The callback invoked to provide an escape character.</param>
		/// <param name="token">The type of token being written.</param>
		void Escape(ReadOnlySpan<char> text, ValidTextCallback validTextCallback, EscapeCharCallback charCallback, IniWriteToken token);
	}
}
