namespace IniFileNet.IO
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Wraps a <see cref="IniStreamReader"/> and collects values into sections.
	/// </summary>
	public sealed class IniStreamSectionReader : IDisposable
	{
		private bool seenEnd;
		private string nextSectionName;
		public IniStreamSectionReader(IniStreamReader reader, bool leaveOpen = false)
		{
			nextSectionName = "";
			seenEnd = false;
			Reader = reader;
			LeaveOpen = leaveOpen;
		}
		/// <summary>
		/// The <see cref="IniStreamReader"/> to read from.
		/// </summary>
		public IniStreamReader Reader { get; }
		/// <summary>
		/// <see langword="true"/> to leave <see cref="Reader"/> open after this <see cref="IniStreamSectionReader"/> is disposed. Otherwise, false.
		/// </summary>
		public bool LeaveOpen { get; set; }
		/// <summary>
		/// Attempts to read the next section. Global keys, if they are allowed, will be in a section with a name of empty string.
		/// Returns <see langword="false"/> when an error is encountered or there are no more sections to read. Check the <see cref="IniStreamReader.ErrorCode"/> property for any errors.
		/// </summary>
		/// <param name="section">The read section.</param>
		/// <returns><see langword="true"/> on reading a section, <see langword="false"/> on an error or when there are no more sections to read.</returns>
		public bool TryReadNext([NotNullWhen(true)] out ReadOnlyIniSection? section)
		{
			List<KeyValue> keyValues = [];
			var (comments, commentsReadOnly) = GetCommentList();
			string key = "";
			while (true)
			{
				ReadResult rr = Reader.Read();
				switch (rr.Token)
				{
					case IniToken.Section:
						section = new(nextSectionName, keyValues, commentsReadOnly);
						nextSectionName = rr.Content;
						if (section.Name.Length > 0 || Reader.Options.AllowGlobalKeys)
						{
							return true;
						}
						break;
					case IniToken.Key:
						key = rr.Content;
						break;
					case IniToken.Value:
						keyValues.Add(new(key, rr.Content, commentsReadOnly));
						(comments, commentsReadOnly) = GetCommentList();
						break;
					case IniToken.Comment:
						comments.Add(rr.Content);
						break;
					case IniToken.Error:
						section = null;
						return false;
					case IniToken.End:
						if (seenEnd)
						{
							section = null;
							return false;
						}
						section = new(nextSectionName, keyValues, commentsReadOnly);
						seenEnd = true;
						return true;
				}
			}
		}
		/// <summary>
		/// Returns either a <see cref="List{T}"/> or a <see cref="EmptyList{T}"/>, depending on the value of <see cref="IniReaderOptions.IgnoreComments"/>.
		/// </summary>
		/// <returns>The same object typed as both <see cref="IList{T}"/> and <see cref="IReadOnlyList{T}"/>.</returns>
		public (IList<string> comments, IReadOnlyList<string> commentsReadOnly) GetCommentList()
		{
			IList<string> comments;
			IReadOnlyList<string> commentsReadOnly;
			if (Reader.Options.IgnoreComments)
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
		/// <summary>
		/// Disposes of <see cref="Reader"/> if <see cref="LeaveOpen"/> is <see langword="false"/>. Otherwise, does nothing.
		/// </summary>
		public void Dispose()
		{
			if (!LeaveOpen)
			{
				Reader.Dispose();
			}
		}
	}
}
