namespace IniFileNet.IO
{
	using IniFileNet;
	using System;
	using System.Collections.Generic;

	internal sealed class IniDictionaryReaderState<T>
	{
		private readonly ReadOnlyMemory<char> sectionKeyDelimiter;
		private readonly AddDictionaryValue<T> addValue;
		private readonly bool ignoreComments;
		private string key;
		private string section;
		private IList<string> comments;
		private IReadOnlyList<string> lastSectionComments;
		private IReadOnlyList<string> commentsReadOnly;
		public IniDictionaryReaderState(ReadOnlyMemory<char> sectionKeyDelimiter, AddDictionaryValue<T> addValue, Dictionary<string, T> dict, bool ignoreComments)
		{
			key = "";
			section = "";
			(comments, commentsReadOnly) = Util.GetCommentList(ignoreComments);
			lastSectionComments = Array.Empty<string>();
			this.sectionKeyDelimiter = sectionKeyDelimiter;
			this.addValue = addValue;
			Dict = dict;
			this.ignoreComments = ignoreComments;
		}
		public Dictionary<string, T> Dict { get; }
		internal IniError Handle(ReadResult rr)
		{
			switch (rr.Token)
			{
				case IniToken.Section:
					section = rr.Content;
					// All of the comments that we have seen so far apply to this section
					lastSectionComments = commentsReadOnly;
					(comments, commentsReadOnly) = Util.GetCommentList(ignoreComments);
					return default;
				case IniToken.Comment:
					comments.Add(rr.Content);
					return default;
				case IniToken.Key:
					key = rr.Content;
					return default;
				case IniToken.Value:
					string fullKey = string.IsNullOrEmpty(section) ? key : string.Concat(section, sectionKeyDelimiter, key);
					var c = commentsReadOnly;
					(comments, commentsReadOnly) = Util.GetCommentList(ignoreComments);
					return addValue(Dict, section, key, sectionKeyDelimiter, rr.Content, lastSectionComments, c);
				default:
				case IniToken.End:
				case IniToken.Error:
				return default;
			}
		}
	}
}
