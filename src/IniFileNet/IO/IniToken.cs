namespace IniFileNet.IO
{
	/// <summary>
	/// A token read from an <see cref="IniStreamReader"/>.
	/// </summary>
	public enum IniToken
	{
		/// <summary>
		/// The end of the stream.
		/// </summary>
		End,
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
		/// <summary>
		/// An error.
		/// </summary>
		Error,
	}
}
