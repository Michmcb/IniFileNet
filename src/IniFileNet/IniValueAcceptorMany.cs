namespace IniFileNet
{
	using System;
	using System.Collections.Generic;
	/// <summary>
	/// An acceptor that accepts multiple values.
	/// </summary>
	public sealed class IniValueAcceptorMany : IIniValueAcceptor
	{
		public IniValueAcceptorMany() : this([]) { }
		public IniValueAcceptorMany(int capacity) : this(new List<string>(capacity)) { }
		public IniValueAcceptorMany(List<string> values)
		{
			Values = values;
		}
		/// <summary>
		/// The values accepted so far.
		/// </summary>
		public List<string> Values { get; set; }
		/// <summary>
		/// Returns true if <see cref="Values"/> has at least 1 item.
		/// </summary>
		public bool HaveValue => Values.Count > 0;
		/// <summary>
		/// Adds <paramref name="value"/> to <see cref="Values"/>. Always returns <see cref="IniErrorCode.None"/>.
		/// </summary>
		public IniError Accept(string key, string value)
		{
			Values.Add(value);
			return default;
		}
		/// <summary>
		/// Replaces <see cref="Values"/> with a new list.
		/// Doesn't clear out the old list, so you don't need to take a defensive copy of it.
		/// </summary>
		public void Reset()
		{
			Values = [];
		}
	}
	/// <summary>
	/// An acceptor that accepts multiple values.
	/// </summary>
	public sealed class IniValueAcceptorMany<T, C> : IIniValueAcceptor
		where C : ICollection<T>, new()
	{
		public IniValueAcceptorMany(Func<string, IniResult<T>> parse) : this(new(), parse) { }
		public IniValueAcceptorMany(C values, Func<string, IniResult<T>> parse)
		{
			Values = values;
			Parse = parse;
		}
		/// <summary>
		/// The values accepted so far.
		/// </summary>
		public C Values { get; set; }
		/// <summary>
		/// Returns true if <see cref="Values"/> has at least 1 item.
		/// </summary>
		public bool HaveValue => Values.Count > 0;
		/// <summary>
		/// The parse function that attempts to parse the raw value.
		/// </summary>
		public Func<string, IniResult<T>> Parse { get; }
		/// <summary>
		/// Calls <see cref="Parse"/>. If that is successful, adds the value to <see cref="Values"/>. Otherwise, returns an error.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns><see cref="IniErrorCode.None"/> on success, or an error code on failure.</returns>
		public IniError Accept(string key, string value)
		{
			var p = Parse(value);
			if (p.Error.Code == default)
			{
				Values.Add(p.Value);
			}
			return p.Error;
		}
		/// <summary>
		/// Replaces <see cref="Values"/> with a new collection of type <typeparamref name="C"/>.
		/// Doesn't clear out the old collection, so you don't need to take a defensive copy of it.
		/// </summary>
		public void Reset()
		{
			Values = new();
		}
	}
}
