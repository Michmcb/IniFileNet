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
		/// Part of a section name between [ and ].
		/// Content is the section name.
		/// </summary>
		Section,
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
		/// Part of a key name.
		/// Content is the portion of the key name that was read.
		/// </summary>
		Key,
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
		/// The end of a comment.
		/// Content is the newline character encountered that marks the end of the value, or empty string if there was no newline.
		/// </summary>
		EndComment,
		/// <summary>
		/// Encountered an error.
		/// Content is a chunk of text around where the error happened.
		/// </summary>
		Error,
	}
}
