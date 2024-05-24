namespace IniFileNet
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// An acceptor that accepts multiple values.
	/// </summary>
	public sealed class IniValueAcceptorMany : IIniValueAcceptor
	{
		/// <summary>
		/// Creates a new instance with a new empty list.
		/// </summary>
		/// <param name="key">The key associated with the values that are to be accepted.</param>
		public IniValueAcceptorMany(string key) : this(key, []) { }
		/// <summary>
		/// Creates a new instance with a new empty list, with a capacity of <paramref name="capacity"/>.
		/// </summary>
		/// <param name="key">The key associated with the values that are to be accepted.</param>
		/// <param name="capacity">The number of elements that the new list can initially store.</param>
		public IniValueAcceptorMany(string key, int capacity) : this(key, new List<string>(capacity)) { }
		/// <summary>
		/// Creates a new instance which will fill the provided list.
		/// </summary>
		/// <param name="key">The key associated with the values that are to be accepted.</param>
		/// <param name="values">The list of values to fill.</param>
		public IniValueAcceptorMany(string key, List<string> values)
		{
			Key = key;
			Value = values;
		}
		/// <inheritdoc/>
		public string Key { get; }
		/// <summary>
		/// The values accepted so far.
		/// </summary>
		public List<string> Value { get; set; }
		/// <summary>
		/// Returns true if <see cref="Value"/> has at least 1 item.
		/// </summary>
		public bool HaveValue => Value.Count > 0;
		/// <summary>
		/// Adds <paramref name="value"/> to <see cref="Value"/>. Always returns <see cref="IniErrorCode.None"/>.
		/// </summary>
		public IniError Accept(string value)
		{
			Value.Add(value);
			return default;
		}
		/// <summary>
		/// Replaces <see cref="Value"/> with a new list.
		/// Doesn't clear out the old list, so you don't need to take a defensive copy of it.
		/// </summary>
		public void Reset()
		{
			Value = [];
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or <paramref name="defaultValue"/> otherwise.
		/// </summary>
		/// <param name="defaultValue">The value to return when <see cref="HaveValue"/> is <see langword="false"/></param>
		/// <returns><see cref="Value"/> or <paramref name="defaultValue"/>.</returns>
		[return: NotNullIfNotNull(nameof(defaultValue))]
		public List<string> ValueOr(List<string> defaultValue)
		{
			return HaveValue ? Value : defaultValue;
		}
		/// <summary>
		/// Returns an <see cref="IniResult{T}"/> with <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or an error of <see cref="IniErrorCode.ValueMissing"/> otherwise.
		/// </summary>
		/// <returns><see cref="Value"/> or <see cref="IniErrorCode.ValueMissing"/>.</returns>
		public IniResult<List<string>> ValueOrError()
		{
			return HaveValue ? new IniResult<List<string>>(Value, default) : new(null!, IniError.KeyMissingValue(Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public List<string> ValueOrException()
		{
			return HaveValue ? Value : throw IniError.KeyMissingValue(Key).ToException();
		}
	}
	/// <summary>
	/// An acceptor that accepts multiple values.
	/// </summary>
	public sealed class IniValueAcceptorMany<T, C> : IIniValueAcceptor
		where C : ICollection<T>, new()
	{
		/// <summary>
		/// Creates a new instance with a new empty list.
		/// </summary>
		/// <param name="key">The key associated with the values that are to be accepted.</param>
		/// <param name="parse">The parse function.</param>
		public IniValueAcceptorMany(string key, Func<string, IniResult<T>> parse) : this(key, new(), parse) { }
		/// <summary>
		/// Creates a new instance which will fill the provided list.
		/// </summary>
		/// <param name="key">The key associated with the values that are to be accepted.</param>
		/// <param name="values">The list of values to fill.</param>
		/// <param name="parse">The parse function.</param>
		public IniValueAcceptorMany(string key, C values, Func<string, IniResult<T>> parse)
		{
			Key = key;
			Value = values;
			Parse = parse;
		}
		/// <inheritdoc/>
		public string Key { get; }
		/// <summary>
		/// The values accepted so far.
		/// </summary>
		public C Value { get; set; }
		/// <summary>
		/// Returns true if <see cref="Value"/> has at least 1 item.
		/// </summary>
		public bool HaveValue => Value.Count > 0;
		/// <summary>
		/// The parse function that attempts to parse the raw value.
		/// </summary>
		public Func<string, IniResult<T>> Parse { get; }
		/// <summary>
		/// Calls <see cref="Parse"/>. If that is successful, adds the value to <see cref="Value"/>. Otherwise, returns an error.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or an error code on failure.</returns>
		public IniError Accept(string value)
		{
			var p = Parse(value);
			if (p.Error.Code == default)
			{
				Value.Add(p.Value);
			}
			return p.Error;
		}
		/// <summary>
		/// Replaces <see cref="Value"/> with a new collection of type <typeparamref name="C"/>.
		/// Doesn't clear out the old collection, so you don't need to take a defensive copy of it.
		/// </summary>
		public void Reset()
		{
			Value = new();
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or <paramref name="defaultValue"/> otherwise.
		/// </summary>
		/// <param name="defaultValue">The value to return when <see cref="HaveValue"/> is <see langword="false"/></param>
		/// <returns><see cref="Value"/> or <paramref name="defaultValue"/>.</returns>
		[return: NotNullIfNotNull(nameof(defaultValue))]
		public C ValueOr(C defaultValue)
		{
			return HaveValue ? Value : defaultValue;
		}
		/// <summary>
		/// Returns an <see cref="IniResult{C}"/> with <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or an error of <see cref="IniErrorCode.ValueMissing"/> otherwise.
		/// </summary>
		/// <returns><see cref="Value"/> or <see cref="IniErrorCode.ValueMissing"/>.</returns>
		public IniResult<C> ValueOrError()
		{
			return HaveValue ? new IniResult<C>(Value, default) : new(default!, IniError.KeyMissingValue(Key));
		}
		/// <summary>
		/// Returns <see cref="Value"/> if <see cref="HaveValue"/> is <see langword="true"/>, or throws an <see cref="IniException"/>.
		/// </summary>
		/// <returns><see cref="Value"/> or throws <see cref="IniException"/>.</returns>
		public C ValueOrException()
		{
			return HaveValue ? Value : throw IniError.KeyMissingValue(Key).ToException();
		}
	}
}
