namespace IniFileNet.IO
{
	/// <summary>
	/// A token from an ini stream.
	/// </summary>
	public readonly struct ReadResult
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public ReadResult(IniToken token, string content)
		{
			Token = token;
			Content = content;
		}
		/// <summary>
		/// The type of the token.
		/// </summary>
		public IniToken Token { get; }
		/// <summary>
		/// The content of the token.
		/// </summary>
		public string Content { get; }
	}
}
