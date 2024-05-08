namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Utility functions.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Calls <see cref="IIniValueAcceptor.Reset"/> on all <paramref name="acceptors"/>.
		/// </summary>
		public static void ResetAll(params IIniValueAcceptor[] acceptors)
		{
			ResetAll((IEnumerable<IIniValueAcceptor>)acceptors);
		}
		/// <summary>
		/// Calls <see cref="IIniValueAcceptor.Reset"/> on all <paramref name="acceptors"/>.
		/// </summary>
		public static void ResetAll(IEnumerable<IIniValueAcceptor> acceptors)
		{
			foreach (var acceptor in acceptors) { acceptor.Reset(); }
		}
		/// <summary>
		/// Returns <paramref name="section"/> and <paramref name="key"/>, delimited by <paramref name="delimiter"/>.
		/// If <paramref name="section"/> is empty, just returns <paramref name="key"/>.
		/// It is not valid for <paramref name="key"/> to be empty, however; that will throw an <see cref="ArgumentException"/>.
		/// </summary>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="delimiter">The delimiter.</param>
		/// <exception cref="ArgumentException">When <paramref name="key"/> is null or empty.</exception>
		public static string GetFullKey(ReadOnlySpan<char> section, ReadOnlySpan<char> key, ReadOnlySpan<char> delimiter)
		{
			if (key.Length == 0) throw new ArgumentException("", nameof(key));
			if (section.Length == 0)
			{
				return key.ToString();
			}
			else
			{
#if NETSTANDARD2_0
				char[] chars = new char[section.Length + delimiter.Length + key.Length];
				Span<char> str = chars;
				section.CopyTo(str);
				delimiter.CopyTo(str.Slice(section.Length));
				key.CopyTo(str.Slice(section.Length + delimiter.Length));
				return new string(chars);
#else
				return string.Concat(section, delimiter, key);
#endif
			}
		}
		/// <summary>
		/// Returns either a new <see cref="List{T}"/> or a new <see cref="EmptyList{T}"/>, depending on the value of <paramref name="ignoreComments"/>.
		/// </summary>
		/// <param name="ignoreComments">If true, then returns <see cref="EmptyList{T}"/>, effectively ignoring all comments addded.</param>
		/// <returns>The same object typed as both <see cref="IList{T}"/> and <see cref="IReadOnlyList{T}"/>.</returns>
		public static (IList<string> comments, IReadOnlyList<string> commentsReadOnly) GetCommentList(bool ignoreComments)
		{
			IList<string> comments;
			IReadOnlyList<string> commentsReadOnly;
			if (ignoreComments)
			{
				EmptyList<string> c = EmptyList.Get<string>();
				comments = c;
				commentsReadOnly = c;
			}
			else
			{
				List<string> c = [];
				comments = c;
				commentsReadOnly = c;
			}
			return (comments, commentsReadOnly);
		}
	}
}
