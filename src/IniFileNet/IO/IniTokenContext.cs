namespace IniFileNet.IO
{
	/// <summary>
	/// The type of ini token is currently being read or written.
	/// </summary>
	public enum IniTokenContext
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
