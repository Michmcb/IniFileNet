namespace IniFileNet.IO
{
	using System.Collections.Generic;

	internal sealed class IniStreamSectionReaderState
	{
		private List<IniKeyValue<string>> keyValues;
		private IList<string> comments;
		private IReadOnlyList<string> commentsReadOnly;
		private string key;
		private readonly ReadOnlyIniSection emptySection;
		private ReadOnlyIniSection currentSection;
		internal IniStreamSectionReaderState(bool ignoreComments)
		{
			keyValues = [];
			(comments, commentsReadOnly) = Util.GetCommentList(ignoreComments);
			key = "";
			emptySection = new("", []);
			currentSection = new("", keyValues, commentsReadOnly);
			CompleteSection = emptySection;
			Ok = true;
		}
		internal bool SeenEnd;
		internal bool Ok;
		internal ReadOnlyIniSection CompleteSection;
		/// <summary>
		/// Returns true if we need more data. False if we don't.
		/// </summary>
		internal bool Handle(ReadResult rr, IniReaderOptions options)
		{
			// What do is this, basically:
			// When we hit a section, remember that as the current section, and set Section as the last section we fully read
			// When we hit a comment, push that into our list of comments that we have seen so far. And when we see a value or section, we allocate the
			// comments we've collected so far to that key/value pair or section.
			// When we hit a key and value, push that into the keyValues list (which points to a ReadOnlyIniSection)
			switch (rr.Token)
			{
				case IniToken.Section:
					CompleteSection = currentSection;
					currentSection = new(rr.Content, keyValues = [], commentsReadOnly);
					(comments, commentsReadOnly) = Util.GetCommentList(options.IgnoreComments);
					if (CompleteSection.Name.Length > 0 || options.AllowGlobalKeys)
					{
						return false;
					}
					break;
				case IniToken.Key:
					key = rr.Content;
					break;
				case IniToken.Value:
					keyValues.Add(new(key, rr.Content, commentsReadOnly));
					(comments, commentsReadOnly) = Util.GetCommentList(options.IgnoreComments);
					break;
				case IniToken.Comment:
					comments.Add(rr.Content);
					break;
				case IniToken.Error:
					Ok = false;
					return false;
				case IniToken.End:
					if (SeenEnd)
					{
						if (commentsReadOnly.Count > 0)
						{
							currentSection = new("", [], commentsReadOnly);
						}
						Ok = false;
						return false;
					}
					CompleteSection = currentSection;
					SeenEnd = true;
					return false;
			}
			return true;
		}
	}
}
