namespace IniFileNet.IO
{
	using System;
	using System.Buffers;

	/// <summary>
	/// Syntax characters and syntax checking.
	/// </summary>
	public static class Syntax
	{
		public static readonly SearchValues<char> EqSemicolon = SearchValues.Create(new char[] { '=', ';' });
		public static readonly SearchValues<char> EqColonSemicolon = SearchValues.Create(new char[] { '=', ':', ';' });
		/// <summary>
		/// The characters that may not appear in a key.
		/// </summary>
		public static readonly ReadOnlyMemory<char> IllegalKeyCharsAsMemory = new char[] { ';', '#', '=', ':', '\r', '\n' };
		/// <summary>
		/// The characters that may not appear in a key.
		/// </summary>
		public static readonly SearchValues<char> IllegalKeyChars = SearchValues.Create(IllegalKeyCharsAsMemory.Span);
		/// <summary>
		/// The characters that may not appear in a section name.
		/// </summary>
		public static readonly ReadOnlyMemory<char> IllegalSectionNameCharsAsMemory = new char[] { ';', '#', '[', ']', '\r', '\n' };
		/// <summary>
		/// The characters that may not appear in a section name.
		/// </summary>
		public static readonly SearchValues<char> IllegalSectionNameChars = SearchValues.Create(IllegalSectionNameCharsAsMemory.Span);
		/// <summary>
		/// Characters that are considered a newline. (Line feed and Carriage return)
		/// </summary>
		public static readonly ReadOnlyMemory<char> NewLineCharsAsMemory = new char[] { '\r', '\n' };
		/// <summary>
		/// Characters that are considered a newline. (Line feed and Carriage return)
		/// </summary>
		public static readonly SearchValues<char> NewLineChars = SearchValues.Create(NewLineCharsAsMemory.Span);
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="key"/> is not empty, whitespace, and does not contain any of <see cref="IllegalKeyChars"/>.
		/// </summary>
		/// <param name="key">The key to validate</param>
		public static bool IsLegalKey(ReadOnlySpan<char> key)
		{
			return !key.IsEmpty && !key.IsWhiteSpace() && !key.ContainsAny(IllegalKeyChars);
		}
		/// <summary>
		/// Returns <see langword="true"/> if <paramref name="name"/> is not empty and does not contain any of <see cref="IllegalKeyChars"/>.
		/// </summary>
		/// <param name="name">The name to validate</param>
		public static bool IsLegalSectionName(ReadOnlySpan<char> name)
		{
			return !name.IsEmpty && !name.ContainsAny(IllegalSectionNameChars);
		}
	}
}
