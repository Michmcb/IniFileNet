namespace IniFileNet
{
	using IniFileNet.IO;
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
