namespace IniFileNet.IO
{
	/// <summary>
	/// Options to configure how to parse an ini file.
	/// </summary>
	public readonly struct IniReaderOptions
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniReaderOptions
		(
			bool allowGlobalKeys = false,
			bool allowCommentsNumberSign = false,
			bool allowKeyDelimiterColon = false,
			bool allowLineContinuations = false,
			bool ignoreComments = false,
			bool ignoreSectionEscapes = false,
			bool ignoreKeyEscapes = false,
			bool ignoreValueEscapes = false,
			bool ignoreCommentEscapes = false,
			bool trimSections = false,
			bool trimKeys = false,
			bool trimValues = false
		)
		{
			AllowGlobalKeys = allowGlobalKeys;
			AllowCommentsNumberSign = allowCommentsNumberSign;
			AllowKeyDelimiterColon = allowKeyDelimiterColon;
			AllowLineContinuations = allowLineContinuations;
			IgnoreComments = ignoreComments;
			IgnoreSectionEscapes = ignoreSectionEscapes;
			IgnoreKeyEscapes = ignoreKeyEscapes;
			IgnoreValueEscapes = ignoreValueEscapes;
			IgnoreCommentEscapes = ignoreCommentEscapes;
			TrimSections = trimSections;
			TrimKeys = trimKeys;
			TrimValues = trimValues;
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
		/// Allows a backslash preceding a carriage return or linefeed to denote a value that continues onto the next line.
		/// Be careful about using windows paths if you use this option, as say, the final slash in C:\foo\bar\ may be misinterpreted as a line continuation!
		/// </summary>
		public bool AllowLineContinuations { get; }
		/// <summary>
		/// When reading, all comments are ignored.
		/// </summary>
		public bool IgnoreComments { get; }
		/// <summary>
		/// When reading, all escape sequences inside sections are ignored. Any content with a \ in it is assumed to be a regular string.
		/// </summary>
		public bool IgnoreSectionEscapes { get; }
		/// <summary>
		/// When reading, all escape sequences inside keys are ignored. Any content with a \ in it is assumed to be a regular string.
		/// </summary>
		public bool IgnoreKeyEscapes { get; }
		/// <summary>
		/// When reading, all escape sequences inside values are ignored. Any content with a \ in it is assumed to be a regular string.
		/// </summary>
		public bool IgnoreValueEscapes { get; }
		/// <summary>
		/// When reading, all escape sequences inside comments are ignored. Any content with a \ in it is assumed to be a regular string.
		/// </summary>
		public bool IgnoreCommentEscapes { get; }
		/// <summary>
		/// When reading, all section names will be trimmed.
		/// Respected by <see cref="IniStreamReader"/> but not <see cref="IniSpanReader"/>.
		/// </summary>
		public bool TrimSections { get; }
		/// <summary>
		/// When reading, all keys will be trimmed.
		/// Respected by <see cref="IniStreamReader"/> but not <see cref="IniSpanReader"/>.
		/// </summary>
		public bool TrimKeys { get; }
		/// <summary>
		/// When reading, all values will be trimmed.
		/// Respected by <see cref="IniStreamReader"/> but not <see cref="IniSpanReader"/>.
		/// </summary>
		public bool TrimValues { get; }
		///// <summary>
		///// Creates a new instance of <see cref="IniReaderOptions"/> with the relevant options copied.
		///// </summary>
		///// <param name="options">The value to convert.</param>
		//public static implicit operator IniReaderOptions(IniReaderOptions options) => new(options.AllowGlobalKeys, options.AllowCommentsNumberSign, options.AllowKeyDelimiterColon, options.AllowLineContinuations, options.IgnoreComments);
		///// <summary>
		///// Creates a new instance of <see cref="IniStreamReaderOptions"/> with the relevant options copied.
		///// </summary>
		///// <param name="options">The value to convert.</param>
		//public static implicit operator IniStreamReaderOptions(IniReaderOptions options) => new(options.TrimSections, options.TrimKeys, options.TrimValues);
	}
}
