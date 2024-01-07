namespace IniFileNet.IO
{
	using System;

	/// <summary>
	/// A snapshot the entire state of an <see cref="IniSpanReader"/>.
	/// </summary>
	public readonly struct IniSpanReaderState
	{
		public IniSpanReaderState(IniSpanReaderBlockState state, IniErrorCode error, IniReaderOptions options, int position)
		{
			State = state;
			Error = error;
			Options = options;
			Position = position;
		}
		/// <summary>
		/// The current state of the span reader.
		/// </summary>
		public IniSpanReaderBlockState State { get; }
		/// <summary>
		/// The current error code.
		/// </summary>
		public IniErrorCode Error { get; }
		/// <summary>
		/// The configured options.
		/// </summary>
		public IniReaderOptions Options { get; }
		/// <summary>
		/// The current position within the buffer.
		/// </summary>
		public int Position { get; }
		/// <summary>
		/// Creates a new <see cref="IniSpanReaderState"/> at position 0 of a new block. Use this when allocating a new buffer.
		/// </summary>
		/// <param name="remainingData">The remaining data in the old buffer that was not read, and should be copied into <paramref name="newBuffer"/>.</param>
		/// <param name="newBuffer">The new buffer.</param>
		/// <param name="charsCopied">The number of characters copied.</param>
		/// <returns></returns>
		public IniSpanReaderState NewBlock(ReadOnlySpan<char> remainingData, Span<char> newBuffer, out int charsCopied)
		{
			if (Position < remainingData.Length)
			{
				remainingData = remainingData.Slice(Position);
				remainingData.CopyTo(newBuffer);
				charsCopied = remainingData.Length;
			}
			else
			{
				charsCopied = 0;
			}
			return new IniSpanReaderState(State, Error, Options, 0);
		}
		/// <summary>
		/// Creates a new <see cref="IniSpanReaderState"/>. Use this when creating a new <see cref="IniSpanReader"/> for the first time.
		/// </summary>
		/// <param name="options"></param>
		/// <returns></returns>
		public static IniSpanReaderState Init(IniReaderOptions options)
		{
			return new IniSpanReaderState(IniSpanReaderBlockState.Global, IniErrorCode.None, options, 0);
		}
	}
}
