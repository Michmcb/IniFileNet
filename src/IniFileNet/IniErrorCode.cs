namespace IniFileNet
{
	/// <summary>
	/// An error code describing a specific type of error when parsing an ini file.
	/// </summary>
	public enum IniErrorCode
	{
		/// <summary>
		/// No error.
		/// </summary>
		None,
		/// <summary>
		/// A key was found, but no delimiter after the key (such as = or :) was found.
		/// </summary>
		KeyDelimiterNotFound,
		/// <summary>
		/// A key delimiter was found but no key name preceded it.
		/// </summary>
		EmptyKeyName,
		/// <summary>
		/// A section name was opened, but it was not closed.
		/// </summary>
		SectionCloseBracketNotFound,
		/// <summary>
		/// A semicolon was found in a key name.
		/// </summary>
		SemicolonInKeyName,
		/// <summary>
		/// A section was found with something else on the same line.
		/// </summary>
		SectionIsNotOnlyThingOnLine,
		/// <summary>
		/// A duplicate key was found.
		/// </summary>
		DuplicateKey,
		/// <summary>
		/// A value was already present.
		/// </summary>
		ValueAlreadyPresent,
		/// <summary>
		/// A value was invalid.
		/// </summary>
		ValueInvalid,
		/// <summary>
		/// A value was missing.
		/// </summary>
		ValueMissing,
		/// <summary>
		/// A section's name was empty.
		/// </summary>
		EmptySectionName,
		/// <summary>
		/// A key was found before any sections were found.
		/// </summary>
		GlobalKeyNotAllowed,
		/// <summary>
		/// An escape sequence (backslash followed by a character) was invalid.
		/// </summary>
		InvalidEscapeSequence,
		/// <summary>
		/// Unable to escape a particular character.
		/// </summary>
		CannotEscapeCharacter,
	}
}
