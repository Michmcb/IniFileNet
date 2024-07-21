namespace IniFileNet.IO
{
	using System;
	/// <summary>
	/// Content of an .ini text stream that was that was read.
	/// </summary>
	public readonly ref struct IniContent
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniContent(IniContentType type, ReadOnlySpan<char> content)
		{
			Type = type;
			Content = content;
		}
		/// <summary>
		/// The type of content that this is.
		/// </summary>
		public IniContentType Type { get; }
		/// <summary>
		/// The text of the content.
		/// </summary>
		public ReadOnlySpan<char> Content { get; }
	}
}
