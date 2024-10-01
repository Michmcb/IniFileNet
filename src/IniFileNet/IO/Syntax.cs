namespace IniFileNet.IO
{
	using System;
#if NET8_0_OR_GREATER
	using System.Buffers;
#endif

	/// <summary>
	/// Syntax characters and syntax checking.
	/// </summary>
	public static class Syntax
	{
		/// <summary>
		/// Equals character.
		/// </summary>
		public static readonly ReadOnlyMemory<char> EqAsMemory = new char[] { '=' };
		/// <summary>
		/// Equals and Colon characters.
		/// </summary>
		public static readonly ReadOnlyMemory<char> EqColonAsMemory = new char[] { '=', ':' };
		/// <summary>
		/// The characters that may not appear in a key.
		/// </summary>
		public static readonly ReadOnlyMemory<char> IllegalKeyCharsAsMemory = new char[] { ';', '#', '=', ':', '\r', '\n' };
		/// <summary>
		/// The characters that may not appear in a section name.
		/// </summary>
		public static readonly ReadOnlyMemory<char> IllegalSectionNameCharsAsMemory = new char[] { ';', '#', '[', ']', '\r', '\n' };
		/// <summary>
		/// Characters that are considered a newline. (Line feed and Carriage return)
		/// </summary>
		public static readonly ReadOnlyMemory<char> NewLineCharsAsMemory = new char[] { '\r', '\n' };
		/// <summary>
		/// Characters that are considered whitespace. (Space, Tab, Carriage return, Line feed)
		/// </summary>
		public static readonly ReadOnlyMemory<char> WhitespaceAsMemory = new char[] { ' ', '\t', '\r', '\n' };
#if NET8_0_OR_GREATER
		/// <summary>
		/// Equals and Semicolon characters.
		/// </summary>
		public static readonly SearchValues<char> Eq = SearchValues.Create(EqAsMemory.Span);
		/// <summary>
		/// Equals, Colon, and Semicolon characters.
		/// </summary>
		public static readonly SearchValues<char> EqColon = SearchValues.Create(EqColonAsMemory.Span);
		/// <summary>
		/// The characters that may not appear in a key.
		/// </summary>
		public static readonly SearchValues<char> IllegalKeyChars = SearchValues.Create(IllegalKeyCharsAsMemory.Span);
		/// <summary>
		/// The characters that may not appear in a section name.
		/// </summary>
		public static readonly SearchValues<char> IllegalSectionNameChars = SearchValues.Create(IllegalSectionNameCharsAsMemory.Span);
		/// <summary>
		/// Characters that are considered a newline. (Line feed and Carriage return)
		/// </summary>
		public static readonly SearchValues<char> NewLineChars = SearchValues.Create(NewLineCharsAsMemory.Span);
		/// <summary>
		/// Characters that are considered whitespace. (Space, Tab, Carriage return, Line feed)
		/// </summary>
		public static readonly SearchValues<char> WhitespaceChars = SearchValues.Create(WhitespaceAsMemory.Span);
#endif
#if NET8_0_OR_GREATER
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="key"/> is not empty, whitespace, and does not contain any of <see cref="IllegalKeyChars"/>.
		/// </summary>
		/// <param name="key">The key to validate</param>
		public static bool IsLegalKey(ReadOnlySpan<char> key)
		{
			return !key.IsEmpty && !key.IsWhiteSpace() && !key.ContainsAny(IllegalKeyChars);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="name"/> is not empty and does not contain any of <see cref="IllegalSectionNameChars"/>.
		/// </summary>
		/// <param name="name">The name to validate</param>
		public static bool IsLegalSectionName(ReadOnlySpan<char> name)
		{
			return !name.IsEmpty && !name.ContainsAny(IllegalSectionNameChars);
		}
#else
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="key"/> is not empty, whitespace, and does not contain any of <see cref="IllegalKeyCharsAsMemory"/>.
		/// </summary>
		/// <param name="key">The key to validate</param>
		public static bool IsLegalKey(ReadOnlySpan<char> key)
		{
			return !key.IsEmpty && !key.IsWhiteSpace() && key.IndexOfAny(IllegalKeyCharsAsMemory.Span) == -1;
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="name"/> is not empty and does not contain any of <see cref="IllegalSectionNameCharsAsMemory"/>.
		/// </summary>
		/// <param name="name">The name to validate</param>
		public static bool IsLegalSectionName(ReadOnlySpan<char> name)
		{
			return !name.IsEmpty && name.IndexOfAny(IllegalSectionNameCharsAsMemory.Span) == -1;
		}
#endif
	}
}
