namespace IniFileNet.IO
{
	using IniFileNet;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	/// <summary>
	/// Reads a <see cref="IniStreamReader"/> into a dictionary.
	/// Ideal types for <typeparamref name="TValue"/> are <see cref="string"/>, <see cref="IniValue{T}"/>, <see cref="List{T}"/> of <see cref="string"/>.
	/// </summary>
	/// <typeparam name="TValue">The type of the values.</typeparam>
	public sealed class IniDictionaryReader<TValue>
	{
		/// <summary>
		/// Creates a new instance with an empty dictionary with the default string comparer.
		/// </summary>
		/// <param name="sectionKeyDelimiter">The delimiter to use.</param>
		public IniDictionaryReader(string sectionKeyDelimiter = ".") : this([], sectionKeyDelimiter) { }
		/// <summary>
		/// Creates a new instance with an empty dictionary, using the provided key comparer.
		/// </summary>
		/// <param name="stringComparer">The key comparer to use.</param>
		/// <param name="sectionKeyDelimiter">The delimiter to use.</param>
		public IniDictionaryReader(StringComparer stringComparer, string sectionKeyDelimiter = ".") : this(new Dictionary<string, TValue>(stringComparer), sectionKeyDelimiter) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="dictionary">The dictionary to add to.</param>
		/// <param name="sectionKeyDelimiter">The delimiter to use.</param>
		public IniDictionaryReader(Dictionary<string, TValue> dictionary, string sectionKeyDelimiter = ".")
		{
			SectionKeyDelimiter = sectionKeyDelimiter != null ? sectionKeyDelimiter.AsMemory() : ".".AsMemory();
			Dictionary = dictionary;
		}
		/// <summary>
		/// The delimiter to use when separating the section/key name. By convention, this is usually a full stop/period, which is the
		/// default if not specified otherwise.
		/// </summary>
		public ReadOnlyMemory<char> SectionKeyDelimiter { get; }
		/// <summary>
		/// The dictionary into which the items are loaded.
		/// </summary>
		public Dictionary<string, TValue> Dictionary { get; set; }
		/// <summary>
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. Overwrites any keys <see cref="Dictionary"/> if they already exist.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="addDictionaryValue">The function to use to add to the dictionary. You can use a static function on <see cref="IniDictionaryReader"/> for this.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public IniError Load(IniStreamReader reader, AddDictionaryValue<TValue> addDictionaryValue)
		{
			IniDictionaryReaderState<TValue> state = new(SectionKeyDelimiter, addDictionaryValue, Dictionary, reader.Options.IgnoreComments);
			while (reader.Error.Code == IniErrorCode.None)
			{
				ReadResult rr = reader.Read();
				if (rr.Token == IniToken.End) return reader.Error;
				var error = state.Handle(rr);
				if (error.Code != IniErrorCode.None) return error;
			}
			return reader.Error;
		}
		/// <summary>
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. Overwrites any keys <see cref="Dictionary"/> if they already exist.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <param name="addDictionaryValue">The function to use to add to the dictionary. You can use a static function on <see cref="IniDictionaryReader"/> for this.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public async Task<IniError> LoadAsync(IniStreamReader reader, AddDictionaryValue<TValue> addDictionaryValue)
		{
			IniDictionaryReaderState<TValue> state = new(SectionKeyDelimiter, addDictionaryValue, Dictionary, reader.Options.IgnoreComments);
			while (reader.Error.Code == IniErrorCode.None)
			{
				ReadResult rr = await reader.ReadAsync();
				if (rr.Token == IniToken.End) return reader.Error;
				var error = state.Handle(rr);
				if (error.Code != IniErrorCode.None) return error;
			}
			return reader.Error;
		}
	}
	/// <summary>
	/// Reads a <see cref="IniStreamReader"/> into a dictionary.
	/// </summary>
	public static class IniDictionaryReader
	{
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Overwrites existing keys. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
		/// <param name="value">The value.</param>
		/// <param name="sectionComments">Ignored.</param>
		/// <param name="keyValueComments">Ignored.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError StringOnlyLast(Dictionary<string, string> dict, string section, string key, ReadOnlyMemory<char> sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			string fullKey = Util.GetFullKey(section.AsSpan(), key.AsSpan(), sectionKeyDelimiter.Span);
			dict[fullKey] = value;
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
		/// <param name="value">The value.</param>
		/// <param name="sectionComments">Ignored.</param>
		/// <param name="keyValueComments">Ignored.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError StringOnlyFirst(Dictionary<string, string> dict, string section, string key, ReadOnlyMemory<char> sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			string fullKey = Util.GetFullKey(section.AsSpan(), key.AsSpan(), sectionKeyDelimiter.Span);
			if (!dict.ContainsKey(fullKey))
			{
				dict[fullKey] = value;
			}
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary.
		/// Returns success if the key doesn't exist in <paramref name="dict"/>, otherwise returns <see cref="IniErrorCode.ValueAlreadyPresent"/>.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
		/// <param name="value">The value.</param>
		/// <param name="sectionComments">Ignored.</param>
		/// <param name="keyValueComments">Ignored.</param>
		/// <returns>If the key does not exist, <see langword="default"/>. Otherwise, <see cref="IniErrorCode.ValueAlreadyPresent"/>.</returns>
		public static IniError StringSingle(Dictionary<string, string> dict, string section, string key, ReadOnlyMemory<char> sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			string fullKey = Util.GetFullKey(section.AsSpan(), key.AsSpan(), sectionKeyDelimiter.Span);
			if (dict.ContainsKey(fullKey))
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Section & key already present. Full Key: \"", fullKey, "\". Value is: \"", value, "\""));
			}
			dict[fullKey] = value;
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Always adds the value. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
		/// <param name="value">The value.</param>
		/// <param name="sectionComments">Ignored.</param>
		/// <param name="keyValueComments">Ignored.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError StringMany(Dictionary<string, List<string>> dict, string section, string key, ReadOnlyMemory<char> sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			string fullKey = Util.GetFullKey(section.AsSpan(), key.AsSpan(), sectionKeyDelimiter.Span);
			if (dict.TryGetValue(fullKey, out var values))
			{
				values.Add(value);
			}
			else
			{
				dict[fullKey] = [value];
			}
			return default;
		}
		/*
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Overwrites existing keys. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniValueOnlyLast(Dictionary<string, IniValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			dict[fullKey] = new IniValue<string>(value, comments);
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniValueOnlyFirst(Dictionary<string, IniValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (!dict.ContainsKey(fullKey))
			{
				dict[fullKey] = new IniValue<string>(value, comments);
			}
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary.
		/// Returns success if the key doesn't exist in <paramref name="dict"/>, otherwise returns <see cref="IniErrorCode.ValueAlreadyPresent"/>.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>If the key does not exist, <see langword="default"/>. Otherwise, <see cref="IniErrorCode.ValueAlreadyPresent"/>.</returns>
		public static IniError IniValueSingle(Dictionary<string, IniValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (dict.ContainsKey(fullKey))
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Section & key already present. Full Key: \"", fullKey, "\". Value is: \"", value, "\""));
			}
			dict[fullKey] = new IniValue<string>(value, comments);
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Always adds the value. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">Ignored.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniValueMany(Dictionary<string, IniValue<List<string>>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (dict.TryGetValue(fullKey, out var values))
			{
				values.Value.Add(value);
			}
			else
			{
				dict[fullKey] = new IniValue<List<string>>([value], comments);
			}
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Overwrites existing keys. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniKeyValueOnlyLast(Dictionary<string, IniKeyValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			dict[fullKey] = new IniKeyValue<string>(fullKey, value, comments);
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniKeyValueOnlyFirst(Dictionary<string, IniKeyValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (!dict.ContainsKey(fullKey))
			{
				dict[fullKey] = new IniKeyValue<string>(fullKey, value, comments);
			}
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary.
		/// Returns success if the key doesn't exist in <paramref name="dict"/>, otherwise returns <see cref="IniErrorCode.ValueAlreadyPresent"/>.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">The comments to add.</param>
		/// <returns>If the key does not exist, <see langword="default"/>. Otherwise, <see cref="IniErrorCode.ValueAlreadyPresent"/>.</returns>
		public static IniError IniKeyValueSingle(Dictionary<string, IniKeyValue<string>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (dict.ContainsKey(fullKey))
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Section & key already present. Full Key: \"", fullKey, "\". Value is: \"", value, "\""));
			}
			dict[fullKey] = new IniKeyValue<string>(fullKey, value, comments);
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Always adds the value. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
		/// <param name="comments">Ignored.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError IniKeyValueMany(Dictionary<string, IniKeyValue<List<string>>> dict, string section, string key, string sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments)
		{
			if (dict.TryGetValue(fullKey, out var values))
			{
				values.Value.Add(value);
			}
			else
			{
				dict[fullKey] = new IniKeyValue<List<string>>(fullKey, [value], comments);
			}
			return default;
		}*/
	}
}
