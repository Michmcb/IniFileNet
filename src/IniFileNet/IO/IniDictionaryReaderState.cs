namespace IniFileNet.IO
{
	using IniFileNet;
	using System.Collections.Generic;

	internal sealed class IniDictionaryReaderState
	{
		private readonly string sectionKeyDelimiter;
		private readonly AddCommentedValue addValue;
		private readonly bool ignoreComments;
		private string key;
		private string section;
		private IList<string> comments;
		private IReadOnlyList<string> commentsReadOnly;
		public IniDictionaryReaderState(string sectionKeyDelimiter, AddCommentedValue addValue, Dictionary<string, CommentedValue> dict, bool ignoreComments)
		{
			key = "";
			section = "";
			(comments, commentsReadOnly) = Util.GetCommentList(ignoreComments);
			this.sectionKeyDelimiter = sectionKeyDelimiter;
			this.addValue = addValue;
			Dict = dict;
			this.ignoreComments = ignoreComments;
		}
		public Dictionary<string, CommentedValue> Dict { get; }
		internal IniError Handle(ReadResult rr)
		{
			switch (rr.Token)
			{
				case IniToken.Section:
					section = rr.Content;
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
					return addValue(Dict, fullKey, new CommentedValue(rr.Content, c));
				default:
				case IniToken.End:
				case IniToken.Error:
				return default;
			}
		}
	}
}
