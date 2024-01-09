namespace IniFileNet
{
	using System;

	/// <summary>
	/// An acceptor that only accepts the last value. Effectively, it always accepts whatever value you give it, overwriting whatever value it has.
	/// </summary>
	public sealed class IniValueAcceptorOnlyLast : IIniValueAcceptor
	{
		/// <summary>
		/// The current value.
		/// </summary>
		public string? Value { get; set; }
		/// <summary>
		/// Returns <see langword="true"/> if <see cref="Value"/> is not <see langword="null"/>.
		/// </summary>
		public bool HaveValue => Value != null;
		/// <summary>
		/// Always updates <see cref="Value"/> and always returns <see cref="IniErrorCode.None"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/>.</returns>
		public IniError Accept(string key, string value)
		{
			Value = value;
			return default;
		}
		/// <summary>
		/// Resets <see cref="Value"/> to <see langword="null"/>.
		/// </summary>
		public void Reset()
		{
			Value = null;
		}
	}
	/// <summary>
	/// An acceptor that only accepts the last value. Effectively, it always accepts whatever value you give it, overwriting whatever value it has.
	/// </summary>
	public sealed class IniValueAcceptorOnlyLast<T> : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="parse">The parse function.</param>
		public IniValueAcceptorOnlyLast(Func<string, IniResult<T>> parse)
		{
			Parse = parse;
		}
		/// <summary>
		/// The current value.
		/// </summary>
		public T? Value { get; set; }
		/// <summary>
		/// Whether or not this acceptor has a value.
		/// </summary>
		public bool HaveValue { get; set; }
		/// <summary>
		/// The parse function that attempts to parse the raw value.
		/// </summary>
		public Func<string, IniResult<T>> Parse { get; }
		/// <summary>
		/// Calls <see cref="Parse"/>. If that is successful, updates <see cref="Value"/>. Otherwise, returns an error.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or an error code on failure.</returns>
		public IniError Accept(string key, string value)
		{
			var p = Parse(value);
			if (p.Error.Code == default)
			{
				HaveValue = true;
				Value = p.Value;
			}
			return p.Error;
		}
		/// <summary>
		/// Resets <see cref="Value"/> to <see langword="null"/> and <see cref="HaveValue"/> to <see langword="false"/>.
		/// </summary>
		public void Reset()
		{
			Value = default;
			HaveValue = false;
		}
	}
}
