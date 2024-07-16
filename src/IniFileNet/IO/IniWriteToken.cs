namespace IniFileNet.IO
{
	/// <summary>
	/// The type of ini token to write
	/// </summary>
	public enum IniWriteToken
	{
		/// <summary>
		/// A section.
		/// </summary>
		Section,
		/// <summary>
		/// A comment.
		/// </summary>
		Comment,
		/// <summary>
		/// A key.
		/// </summary>
		Key,
		/// <summary>
		/// A value.
		/// </summary>
		Value,
	}
}
