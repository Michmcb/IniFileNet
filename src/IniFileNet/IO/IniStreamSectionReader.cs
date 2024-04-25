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
		/// <summary>
		/// Creates a new instance, wrapping the provided <paramref name="reader"/>.
		/// </summary>
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
		/// Returns <see langword="false"/> when an error is encountered or there are no more sections to read. Check the <see cref="IniStreamReader.Error"/> property for any errors.
		/// </summary>
		/// <param name="section">The read section.</param>
		/// <returns><see langword="true"/> on reading a section, <see langword="false"/> on an error or when there are no more sections to read.</returns>
		public bool TryReadNext([NotNullWhen(true)] out ReadOnlyIniSection? section)
		{
			List<KeyValue> keyValues = [];
			var (comments, commentsReadOnly) = Util.GetCommentList(Reader.Options.IgnoreComments);
			string key = "";
			while (true)
			{
				// TODO this has to be async; but we can't have an out parameter because async methods don't allow that! Have to change the return type to hold the bool and the ReadOnlyIniSection.
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
						(comments, commentsReadOnly) = Util.GetCommentList(Reader.Options.IgnoreComments);
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
