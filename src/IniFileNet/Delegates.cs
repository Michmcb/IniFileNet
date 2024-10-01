namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// This delegate is intended to attempt to add the value of <paramref name="section"/> and <paramref name="key"/> and <paramref name="value"/> to <paramref name="dictionary"/>.
	/// If any errors occur, they must be returned as an <see cref="IniError"/>.
	/// </summary>
	/// <param name="dictionary">The dictionary to add to.</param>
	/// <param name="section">The section name.</param>
	/// <param name="key">The key.</param>
	/// <param name="sectionKeyDelimiter">The delimiter that must go between <paramref name="section"/> and <paramref name="key"/>, provided <paramref name="section"/> is not empty.</param>
	/// <param name="value">The value.</param>
	/// <param name="sectionComments">The comments preceding the section.</param>
	/// <param name="keyValueComments">The comments preceding the key and value.</param>
	/// <returns>On success, <see langword="default"/>. On failure, an <see cref="IniError"/>.</returns>
	public delegate IniError AddDictionaryValue<T>(Dictionary<string, T> dictionary, string section, string key, ReadOnlyMemory<char> sectionKeyDelimiter, string value, IReadOnlyList<string> sectionComments, IReadOnlyList<string> keyValueComments);
}
