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
		/// <summary>
		/// Returns <see langword="true"/> if <see cref="Error"/> has an error code of <see cref="IniErrorCode.None"/>. Otherwise, returns <see langword="false"/>.
		/// </summary>
		/// <param name="value">Set to the value of <see cref="Value"/>.</param>
		/// <param name="error">Set to the value of <see cref="Error"/>.</param>
		/// <returns><see langword="true"/> if <see cref="Error"/> has an error code of <see cref="IniErrorCode.None"/>. Otherwise, <see langword="false"/>.</returns>
		public bool Ok(out T value, out IniError error)
		{
			value = Value;
			error= Error;
			return Error.Code == IniErrorCode.None;
		}
	}
}
