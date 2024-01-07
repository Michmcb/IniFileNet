namespace IniFileNet
{
	using System;

	/// <summary>
	/// An acceptor that only accepts the last value. If made to accept another value when it already has a value, it ignores it.
	/// </summary>
	public sealed class IniValueAcceptorOnlyFirst : IIniValueAcceptor
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
		/// Only updates <see cref="Value"/> if it is not null, and always returns <see cref="IniErrorCode.None"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/>.</returns>
		public IniError Accept(string key, string value)
		{
			Value ??= value;
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
	/// An acceptor that only accepts the last value. If made to accept another value when it already has a value, it ignores it.
	/// </summary>
	public sealed class IniValueAcceptorOnlyFirst<T> : IIniValueAcceptor
	{
		public IniValueAcceptorOnlyFirst(Func<string, IniResult<T>> parse)
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
		/// If <see cref="HaveValue"/> is <see langword="false"/>, calls <see cref="Parse"/>. If that is successful, updates <see cref="Value"/>. Otherwise, returns an error.
		/// If <see cref="HaveValue"/> is <see langword="true"/>, returns <see cref="IniErrorCode.None"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success (or already have a value), or an error code on failure.</returns>
		public IniError Accept(string key, string value)
		{
			if (!HaveValue)
			{
				var p = Parse(value);
				if (p.Error.Code == default)
				{
					HaveValue = true;
					Value = p.Value;
				}
				return p.Error;
			}
			return default;
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
