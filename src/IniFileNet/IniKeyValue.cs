namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A key and a value.
	/// </summary>
	/// <typeparam name="TValue">The type of the values.</typeparam>
	public sealed class IniKeyValue<TValue>
	{
		/// <summary>
		/// Creates a new instance, using <see cref="Array.Empty{T}"/> as the <see cref="Comments"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public IniKeyValue(string key, TValue value) : this(key, value, Array.Empty<string>()) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="comments">The comments.</param>
		public IniKeyValue(string key, TValue value, IReadOnlyList<string> comments)
		{
			Key = key;
			Value = value;
			Comments = comments;
		}
		/// <summary>
		/// The key.
		/// </summary>
		public string Key { get; }
		/// <summary>
		/// The value.
		/// </summary>
		public TValue Value { get; }
		/// <summary>
		/// The comments preceding this key/value pair.
		/// </summary>
		public IReadOnlyList<string> Comments { get; }
	}
}
