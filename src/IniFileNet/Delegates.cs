namespace IniFileNet
{
	using System.Collections.Generic;

	/// <summary>
	/// This delegate is intended to attempt to add the value of <paramref name="fullKey"/> and <paramref name="value"/> to <paramref name="dictionary"/>.
	/// If any errors occur, they must be returned as an <see cref="IniError"/>.
	/// </summary>
	/// <param name="dictionary">The dictionary to add to.</param>
	/// <param name="fullKey">The key to add.</param>
	/// <param name="value">The value to add.</param>
	/// <returns>On success, <see langword="default"/>. On failure, an <see cref="IniError"/>.</returns>
	public delegate IniError AddCommentedValue(Dictionary<string, CommentedValue> dictionary, string fullKey, CommentedValue value);
}
