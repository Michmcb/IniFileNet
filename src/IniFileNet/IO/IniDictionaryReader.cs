namespace IniFileNet.IO
{
	using IniFileNet;
	using System;
	using System.Collections.Generic;
	using System.Threading.Tasks;
	/// <summary>
	/// Reads a <see cref="IniStreamReader"/> into a dictionary.
	/// </summary>
	public sealed class IniDictionaryReader
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
		public IniDictionaryReader(StringComparer stringComparer, string sectionKeyDelimiter = ".") : this(new Dictionary<string, CommentedValue>(stringComparer), sectionKeyDelimiter) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="dictionary">The dictionary to add to.</param>
		/// <param name="sectionKeyDelimiter">The delimiter to use.</param>
		public IniDictionaryReader(Dictionary<string, CommentedValue> dictionary, string sectionKeyDelimiter = ".")
		{
			SectionKeyDelimiter = sectionKeyDelimiter;
			Dictionary = dictionary;
		}
		/// <summary>
		/// The delimiter to use when separating the section/key name. By convention, this is usually a full stop/period, which is the
		/// default if not specified otherwise.
		/// </summary>
		public string SectionKeyDelimiter { get; }
		/// <summary>
		/// The dictionary into which the items are loaded.
		/// </summary>
		public Dictionary<string, CommentedValue> Dictionary { get; set; }
		/// <summary>
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. Overwrites any keys <see cref="Dictionary"/> if they already exist.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public IniError LoadOnlyLastValues(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, OnlyLast, Dictionary, reader.Options.IgnoreComments);
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
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public async Task<IniError> LoadOnlyLastValuesAsync(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, OnlyLast, Dictionary, reader.Options.IgnoreComments);
			while (reader.Error.Code == IniErrorCode.None)
			{
				ReadResult rr = await reader.ReadAsync();
				if (rr.Token == IniToken.End) return reader.Error;
				var error = state.Handle(rr);
				if (error.Code != IniErrorCode.None) return error;
			}
			return reader.Error;
		}
		/// <summary>
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. If any keys already exist in <see cref="Dictionary"/>, then they are skipped.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public IniError LoadOnlyFirstValues(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, OnlyFirst, Dictionary, reader.Options.IgnoreComments);
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
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. If any keys already exist in <see cref="Dictionary"/>, then they are skipped.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public async Task<IniError> LoadOnlyFirstValuesAsync(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, OnlyFirst, Dictionary, reader.Options.IgnoreComments);
			while (reader.Error.Code == IniErrorCode.None)
			{
				ReadResult rr = await reader.ReadAsync();
				if (rr.Token == IniToken.End) return reader.Error;
				var error = state.Handle(rr);
				if (error.Code != IniErrorCode.None) return error;
			}
			return reader.Error;
		}
		/// <summary>
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. If any keys already exist in <see cref="Dictionary"/>, then an error is returned.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public IniError LoadSingleValues(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, Single, Dictionary, reader.Options.IgnoreComments);
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
		/// Loads all values from <paramref name="reader"/> into <see cref="Dictionary"/>. If any keys already exist in <see cref="Dictionary"/>, then an error is returned.
		/// </summary>
		/// <param name="reader">The reader to read from.</param>
		/// <returns>Any error when handling the <see cref="ReadResult"/> or the error of <paramref name="reader"/>, otherwise <see langword="default"/>.</returns>
		public async Task<IniError> LoadSingleValuesAsync(IniStreamReader reader)
		{
			IniDictionaryReaderState state = new(SectionKeyDelimiter, Single, Dictionary, reader.Options.IgnoreComments);
			while (reader.Error.Code == IniErrorCode.None)
			{
				ReadResult rr = await reader.ReadAsync();
				if (rr.Token == IniToken.End) return reader.Error;
				var error = state.Handle(rr);
				if (error.Code != IniErrorCode.None) return error;
			}
			return reader.Error;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Overwrites existing keys. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="fullKey">The key.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError OnlyLast(Dictionary<string, CommentedValue> dict, string fullKey, CommentedValue value)
		{
			dict[fullKey] = value;
			return default;
		}
		/// <summary>
		/// Handles adding a value into <paramref name="dict"/>. Only adds if they key does not exist in the dictionary. Always returns success.
		/// </summary>
		/// <param name="dict">The dictionary to add to.</param>
		/// <param name="fullKey">The key.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>Always <see langword="default"/>.</returns>
		public static IniError OnlyFirst(Dictionary<string, CommentedValue> dict, string fullKey, CommentedValue value)
		{
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
		/// <param name="fullKey">The key.</param>
		/// <param name="value">The value to add.</param>
		/// <returns>If the key does not exist, <see langword="default"/>. Otherwise, <see cref="IniErrorCode.ValueAlreadyPresent"/>.</returns>
		public static IniError Single(Dictionary<string, CommentedValue> dict, string fullKey, CommentedValue value)
		{
			if (dict.ContainsKey(fullKey))
			{
				return new(IniErrorCode.ValueAlreadyPresent, string.Concat("Section & key already present. Full Key: \"", fullKey, "\". Value is: \"", value.Value, "\""));
			}
			dict[fullKey] = value;
			return default;
		}
	}
}
