namespace IniFileNet.IO
{
	/// <summary>
	/// An index, and whether or not a a slash (which marks an escape sequence) was found.
	/// </summary>
	public readonly ref struct IdxEsc
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="index">The index.</param>
		/// <param name="slashIndex">The index of the slash, or -1 if not found.</param>
		public IdxEsc(int index, int slashIndex)
		{
			Index = index;
			SlashIndex = slashIndex;
		}
		/// <summary>
		/// The index.
		/// </summary>
		public int Index { get; }
		/// <summary>
		/// The index of the slash, or -1 if not found.
		/// </summary>
		public int SlashIndex { get; }
	}
}
