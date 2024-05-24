namespace IniFileNet
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// An acceptor that only accepts a single value. If made to accept another value when it already has a value, returns an error.
	/// </summary>
	public sealed class IniValueAcceptorSingle : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key associated with the value that is to be accepted.</param>
		public IniValueAcceptorSingle(string key)
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
		/// Updates <see cref="Value"/> if <see cref="HaveValue"/> returns <see langword="false"/>.
		/// Otherwise, returns an error <see cref="IniErrorCode.ValueAlreadyPresent"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or <see cref="IniErrorCode.ValueAlreadyPresent"/> if a value has already been accepted.</returns>
		public IniError Accept(string value)
		{
			if (Value != null)
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Already accepted a value. Key: \"", Key, "\". Value is: \"", value, "\""));
			}
			else Value = value;
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
	/// An acceptor that only accepts a single value. If made to accept another value when it already has a value, returns an error.
	/// </summary>
	public sealed class IniValueAcceptorSingle<T> : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key associated with the value that is to be accepted.</param>
		/// <param name="parse">The parse function.</param>
		public IniValueAcceptorSingle(string key, Func<string, IniResult<T>> parse)
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
		/// If <see cref="HaveValue"/> is <see langword="true"/>, returns an error <see cref="IniErrorCode.ValueAlreadyPresent"/>.
		/// Otherwise, calls <see cref="Parse"/>. If that is successful, updates <see cref="Value"/>. Otherwise, returns an error.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or an error code on failure.</returns>
		public IniError Accept(string value)
		{
			if (HaveValue)
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Already accepted a value. Key: \"", Key, "\". Value is: \"", value, "\""));
			}
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
