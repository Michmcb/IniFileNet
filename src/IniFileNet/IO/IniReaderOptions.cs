﻿namespace IniFileNet.IO
{
	/// <summary>
	/// Options to configure how an ini file is parsed.
	/// </summary>
	public readonly struct IniReaderOptions
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniReaderOptions(bool allowGlobalKeys = false, bool allowCommentsNumberSign = false, bool allowKeyDelimiterColon = false, bool allowLineContinuations = false, bool ignoreComments = false)
		{
			AllowGlobalKeys = allowGlobalKeys;
			AllowCommentsNumberSign = allowCommentsNumberSign;
			AllowKeyDelimiterColon = allowKeyDelimiterColon;
			AllowLineContinuations = allowLineContinuations;
			IgnoreComments = ignoreComments;
		}
		/// <summary>
		/// Allows keys to appear without a section appearing first.
		/// Enabling this option is not recommended; if ini files are concatenated, it may change the section of keys, which can change the meaning of the file.
		/// If you can guarantee the ini files you are parsing will never be the result of concatenation then this option should be safe to enable.
		/// </summary>
		public bool AllowGlobalKeys { get; }
		/// <summary>
		/// Allows the number sign to be used as a comment as well as the semicolon.
		/// </summary>
		public bool AllowCommentsNumberSign { get; }
		/// <summary>
		/// Allows the colon to be used to delimit key/value pairs as well as the colon.
		/// </summary>
		public bool AllowKeyDelimiterColon { get; }
		/// <summary>
		/// Allows a backslash preceding a \r or \n to denote a value that continues onto the next line.
		/// Be careful about using windows paths if you use this option, as say C:\foo\bar\ may be misinterpreted as a line continuation!
		/// </summary>
		public bool AllowLineContinuations { get; }
		/// <summary>
		/// When reading, all comments are ignored.
		/// </summary>
		public bool IgnoreComments { get; }
	}
}
