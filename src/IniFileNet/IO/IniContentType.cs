namespace IniFileNet.IO
{
	/// <summary>
	/// Denotes what type of content a block of text is.
	/// </summary>
	public enum IniContentType
	{
		/// <summary>
		/// End of the span; if more data is available, it should be read in and reading can continue.
		/// </summary>
		End,
		/// <summary>
		/// The opening [ of a section.
		/// Content is [
		/// </summary>
		StartSection,
		/// <summary>
		/// The entirety of a section name between [ and ].
		/// Content is the section name.
		/// </summary>
		Section,
		/// <summary>
		/// Part of a section name between [ and ], with at least one escape sequence.
		/// Content is the section name.
		/// </summary>
		SectionEscaped,
		/// <summary>
		/// The closing ] of a section.
		/// Content is ]
		/// </summary>
		EndSection,
		/// <summary>
		/// Encountered a key.
		/// Content is empty.
		/// </summary>
		StartKey,
		/// <summary>
		/// The entirety of a key name.
		/// Content is the portion of the key name that was read.
		/// </summary>
		Key,
		/// <summary>
		/// Part of a key name, with at least one escape sequence.
		/// Content is the portion of the key name that was read.
		/// </summary>
		KeyEscaped,
		/// <summary>
		/// Encountered an = or :, signifying the end of a key.
		/// Content is = or :.
		/// </summary>
		EndKey,
		/// <summary>
		/// Encountered a value.
		/// Content is empty.
		/// </summary>
		StartValue,
		/// <summary>
		/// Part of a value.
		/// Content is the portion of the value that was read.
		/// </summary>
		Value,
		/// <summary>
		/// Part of a value, with at least one escape sequence.
		/// Content is the portion of the value that was read.
		/// </summary>
		ValueEscaped,
		/// <summary>
		/// The end of a value.
		/// Content is the newline character encountered that marks the end of the value, or empty string if there was no newline.
		/// </summary>
		EndValue,
		/// <summary>
		/// Encountered a comment.
		/// Content is the character that signifies the beginning of the comment, which is either number sign (#) or semicolon (;).
		/// </summary>
		StartComment,
		/// <summary>
		/// Part of a comment.
		/// Content is the portion of the comment that was read.
		/// </summary>
		Comment,
		/// <summary>
		/// Part of a comment, with at least one escape sequence.
		/// Content is the portion of the comment that was read.
		/// </summary>
		CommentEscaped,
		/// <summary>
		/// The end of a comment.
		/// Content is the newline character encountered that marks the end of the value, or empty string if there was no newline.
		/// </summary>
		EndComment,
		/// <summary>
		/// Encountered an error.
		/// Content is the current block of text in which the error happened.
		/// </summary>
		Error,
	}
}
