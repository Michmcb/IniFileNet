namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A key and a value.
	/// </summary>
	public sealed class KeyValue
	{
		/// <summary>
		/// Creates a new instance, using <see cref="Array.Empty{T}"/> as the <see cref="Comments"/>.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		public KeyValue(string key, string value) : this(key, value, Array.Empty<string>()) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <param name="comments">The comments.</param>
		public KeyValue(string key, string value, IReadOnlyList<string> comments)
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
		public string Value { get; }
		/// <summary>
		/// The comments preceding this key/value pair.
		/// </summary>
		public IReadOnlyList<string> Comments { get; }
	}
}
