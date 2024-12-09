namespace IniFileNet
{
	/// <summary>
	/// An error code and error message.
	/// </summary>
	public readonly struct IniError
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniError(IniErrorCode error, string? msg)
		{
			Code = error;
			Msg = msg;
		}
		/// <summary>
		/// The error code. If no error happened, will be <see cref="IniErrorCode.None"/>.
		/// </summary>
		public IniErrorCode Code { get; }
		/// <summary>
		/// The error message.
		/// </summary>
		public string? Msg { get; }
		/// <summary>
		/// Creates a new <see cref="IniException"/> with <see cref="Code"/> and <see cref="Msg"/>.
		/// </summary>
		/// <returns>A new <see cref="IniException"/>.</returns>
		public IniException ToException()
		{
			return new IniException(Code, Msg ?? "");
		}
		/// <summary>
		/// If <see cref="Code"/> is not equal to <see cref="IniErrorCode.None"/>, throws a <see cref="IniException"/> with the result of <see cref="ToException"/>.
		/// Otherwise, does nothing.
		/// </summary>
		public void ThrowIfError()
		{
			if (Code != default)
			{
				throw ToException();
			}
		}
		/// <summary>
		/// Returns the string reperesentation of <see cref="Code"/> and <see cref="Msg"/>
		/// </summary>
		public override string ToString()
		{
			return string.Concat(Code.ToString(), ": ", Msg);
		}
		/// <summary>
		/// Returns an <see cref="IniError"/> with an <see cref="IniErrorCode"/> and error message representing a missing key with name <paramref name="key"/>.
		/// </summary>
		/// <param name="section">The section name that is missing.</param>
		/// <param name="key">The key name that is missing.</param>
		/// <returns>An <see cref="IniError"/>.</returns>
		public static IniError KeyMissingValue(string? section, string key)
		{
			return new(IniErrorCode.ValueMissing, string.Concat("The section \"", section, "\" key \"", key, "\" was missing a value"));
		}
	}
}
