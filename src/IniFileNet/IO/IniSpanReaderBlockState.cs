namespace IniFileNet.IO
{
	/// <summary>
	/// The current state of a <see cref="IniSpanReader"/> within a block of text.
	/// </summary>
	public enum IniSpanReaderBlockState
	{
		/// <summary>
		/// In the global keys area
		/// </summary>
		Global = 0,
		/// <summary>
		/// Ready to read a key/section/comment
		/// </summary>
		Any = 1,
		/// <summary>
		/// Reading a comment
		/// </summary>
		Comment = 2,
		/// <summary>
		/// Reading a comment
		/// </summary>
		CommentGlobal = 3,
		/// <summary>
		/// Finished reading a comment
		/// </summary>
		CommentEnded = 4,
		/// <summary>
		/// Finished reading a comment
		/// </summary>
		CommentEndedGlobal = 5,
		/// <summary>
		/// Reading a section name
		/// </summary>
		Section = 6,
		/// <summary>
		/// Finished reading a section name
		/// </summary>
		SectionEnded = 7,
		/// <summary>
		/// Finished reading a section name, and we have to verify that the section is the only thing that appears on this line
		/// </summary>
		SectionEndedVerifyOnlyThingOnLine = 8,
		/// <summary>
		/// Reading a key
		/// </summary>
		Key = 9,
		/// <summary>
		/// Finished reading a key name
		/// </summary>
		KeyEnded = 10,
		/// <summary>
		/// Finished reading a key name, and about to start reading a value
		/// </summary>
		PreValue = 11,
		/// <summary>
		/// Reading the first part of a value
		/// </summary>
		ValueFirstPart = 12,
		/// <summary>
		/// Reading a value
		/// </summary>
		Value = 13,
		/// <summary>
		/// Finished reading a value
		/// </summary>
		ValueEnded = 14,
		/// <summary>
		/// In an error state
		/// </summary>
		Error = 15,
	}
}
