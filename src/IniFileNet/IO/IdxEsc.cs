namespace IniFileNet.IO
{
	/// <summary>
	/// An index, and whether or not an escape sequence was found.
	/// </summary>
	public readonly ref struct IdxEsc
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="escape">Whether or not an escape sequence was found.</param>
		public IdxEsc(int index, bool escape)
		{
			Index = index;
			Escape = escape;
		}
		/// <summary>
		/// The index.
		/// </summary>
		public int Index { get; }
		/// <summary>
		/// Whether or not an escape sequence was found.
		/// </summary>
		public bool Escape { get; }
	}
}
