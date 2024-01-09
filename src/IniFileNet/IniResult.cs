namespace IniFileNet
{
	/// <summary>
	/// Holds a value and an <see cref="IniError"/>.
	/// If successful, then <see cref="Error"/> will have an <see cref="IniErrorCode"/> of <see cref="IniErrorCode.None"/>, and
	/// <see cref="Value"/> will hold some meaningful value.
	/// Otherwise, the value of <see cref="Value"/> is undefined, and <see cref="Error"/> tells you what went wrong.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public readonly struct IniResult<T>
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniResult(T value, IniError error)
		{
			Value = value;
			Error = error;
		}
		/// <summary>
		/// The value.
		/// </summary>
		public T Value { get; }
		/// <summary>
		/// The error.
		/// </summary>
		public IniError Error { get; }
	}
}
