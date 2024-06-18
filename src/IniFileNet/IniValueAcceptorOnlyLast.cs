namespace IniFileNet
{
	using System;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// An acceptor that only accepts the last value. Effectively, it always accepts whatever value you give it, overwriting whatever value it has.
	/// </summary>
	public sealed class IniValueAcceptorOnlyLast : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniValueAcceptorOnlyLast()
		{
			Section = string.Empty;
			Key = string.Empty;
		}
		/// <inheritdoc/>
		public string Section { get; set; }
		/// <inheritdoc/>
		public string Key { get; set; }
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
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/>.</returns>
		public IniError Accept(string section, string key, string value)
		{
			Section = section;
			Key = key;
			Value = value;
			return default;
		}
		/// <summary>
		/// Resets <see cref="Value"/> to <see langword="null"/>, and <see cref="Section"/> and <see cref="Key"/> to <see cref="string.Empty"/>.
		/// </summary>
		public void Reset()
		{
			Section = string.Empty;
			Key = string.Empty;
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
			return Value != null ? new IniResult<string>(Value, default) : new(null!, IniError.KeyMissingValue(Section, Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public string ValueOrException()
		{
			return Value ?? throw IniError.KeyMissingValue(Section, Key).ToException();
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
			Section = string.Empty;
			Key = string.Empty;
			Parse = parse;
		}
		/// <inheritdoc/>
		public string Section { get; set; }
		/// <inheritdoc/>
		public string Key { get; set; }
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
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or an error code on failure.</returns>
		public IniError Accept(string section, string key, string value)
		{
			var p = Parse(value);
			if (p.Error.Code == default)
			{
				Section = section;
				Key = key;
				HaveValue = true;
				Value = p.Value;
			}
			return p.Error;
		}
		/// <summary>
		/// Resets <see cref="Value"/> to <see langword="default"/> and <see cref="HaveValue"/> to <see langword="false"/>, and <see cref="Section"/> and <see cref="Key"/> to <see cref="string.Empty"/>.
		/// </summary>
		public void Reset()
		{
			Section = string.Empty;
			Key = string.Empty;
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
			return HaveValue ? new IniResult<T>(Value, default) : new(default!, IniError.KeyMissingValue(Section,	Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public T ValueOrException()
		{
			return HaveValue ? Value : throw IniError.KeyMissingValue(Section, Key).ToException();
		}
	}
}
