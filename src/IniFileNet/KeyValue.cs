namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A key and a value.
	/// </summary>
	public sealed class KeyValue
	{
		public KeyValue(string key, string value) : this(key, value, Array.Empty<string>()) { }
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
