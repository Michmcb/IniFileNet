namespace IniFileNet
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// An acceptor that only accepts the last value. If made to accept another value when it already has a value, it ignores it.
	/// </summary>
	public sealed class IniValueAcceptorOnlyFirst : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key associated with the value that is to be accepted.</param>
		public IniValueAcceptorOnlyFirst(string key)
		{
			Key = key;
		}
		/// <inheritdoc/>
		public string Key { get; }
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
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/>.</returns>
		public IniError Accept(string value)
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
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or <paramref name="defaultValue"/> otherwise.
		/// </summary>
		/// <param name="defaultValue">The value to return when <see cref="HaveValue"/> is <see langword="false"/></param>
		/// <returns><see cref="Value"/> or <paramref name="defaultValue"/>.</returns>
		[return: NotNullIfNotNull(nameof(defaultValue))]
		public string? ValueOr(string? defaultValue)
		{
			return Value ?? defaultValue;
		}
		/// <summary>
		/// Returns an <see cref="IniResult{T}"/> with <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or an error of <see cref="IniErrorCode.ValueMissing"/> otherwise.
		/// </summary>
		/// <returns><see cref="Value"/> or <see cref="IniErrorCode.ValueMissing"/>.</returns>
		public IniResult<string> ValueOrError()
		{
			return Value != null ? new IniResult<string>(Value, default) : new(null!, IniError.KeyMissingValue(Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public string ValueOrException()
		{
			return Value ?? throw IniError.KeyMissingValue(Key).ToException();
		}
	}
	/// <summary>
	/// An acceptor that only accepts the last value. If made to accept another value when it already has a value, it ignores it.
	/// </summary>
	public sealed class IniValueAcceptorOnlyFirst<T> : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key associated with the value that is to be accepted.</param>
		/// <param name="parse">The parse function.</param>
		public IniValueAcceptorOnlyFirst(string key, Func<string, IniResult<T>> parse)
		{
			Key = key;
			Parse = parse;
		}
		/// <inheritdoc/>
		public string Key { get; }
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
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success (or already have a value), or an error code on failure.</returns>
		public IniError Accept(string value)
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
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or <paramref name="defaultValue"/> otherwise.
		/// </summary>
		/// <param name="defaultValue">The value to return when <see cref="HaveValue"/> is <see langword="false"/></param>
		/// <returns><see cref="Value"/> or <paramref name="defaultValue"/>.</returns>
		[return: NotNullIfNotNull(nameof(defaultValue))]
		public T? ValueOr(T? defaultValue)
		{
			return HaveValue ? Value : defaultValue;
		}
		/// <summary>
		/// Returns an <see cref="IniResult{T}"/> with <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or an error of <see cref="IniErrorCode.ValueMissing"/> otherwise.
		/// </summary>
		/// <returns><see cref="Value"/> or <see cref="IniErrorCode.ValueMissing"/>.</returns>
		public IniResult<T> ValueOrError()
		{
			return HaveValue ? new IniResult<T>(Value, default) : new(default!, IniError.KeyMissingValue(Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public T ValueOrException()
		{
			return HaveValue ? Value : throw IniError.KeyMissingValue(Key).ToException();
		}
	}
}
