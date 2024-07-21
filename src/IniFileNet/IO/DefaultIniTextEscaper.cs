namespace IniFileNet.IO
{
	using System;
#if NET8_0_OR_GREATER
	using System.Buffers;
#endif
	using System.Runtime.CompilerServices;

	/// <summary>
	/// The default text escaper.
	/// </summary>
	public sealed class DefaultIniTextEscaper : IIniTextEscaper
	{
		/// <summary>
		/// The default instance.
		/// </summary>
		public static readonly DefaultIniTextEscaper Default = new();
#if NET8_0_OR_GREATER
		private static readonly SearchValues<char> valueEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n']);
		private static readonly SearchValues<char> keyEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':',]);
		private static readonly SearchValues<char> sectionEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '[', ']',]);
		private static readonly SearchValues<char> commentEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#']);
#else
		private static readonly ReadOnlyMemory<char> valueEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n' };
		private static readonly ReadOnlyMemory<char> keyEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':', };
		private static readonly ReadOnlyMemory<char> sectionEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '[', ']', };
		private static readonly ReadOnlyMemory<char> commentEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n', ';', '#' };
#endif
		/// <inheritdoc/>
		public void Escape(ReadOnlySpan<char> text, ValidTextCallback validTextCallback, EscapeCharCallback charCallback, IniWriteToken token)
		{
			var chrs = (token switch
			{
				IniWriteToken.Section => sectionEscapeChars,
				IniWriteToken.Comment => commentEscapeChars,
				IniWriteToken.Key => keyEscapeChars,
				IniWriteToken.Value => valueEscapeChars,
				_ => throw new ArgumentOutOfRangeException(nameof(token), "The provided value was not a valid value: " + ((int)token).ToString()),
			})
#if NET8_0_OR_GREATER
			;
#else
			.Span;
#endif
			ReadOnlySpan<char> str = text;
			int idx = str.IndexOfAny(chrs);
			while (idx != -1)
			{
				validTextCallback(str.Slice(0, idx));
				charCallback(str[idx] switch
				{
					'\\' => '\\',
					'\0' => '0',
					'\a' => 'a',
					'\b' => 'b',
					'\r' => 'r',
					'\n' => 'n',
					'=' => '=',
					':' => ':',
					'[' => '[',
					']' => ']',
					';' => ';',
					'#' => '#',
					_ => default,
				});
				if (idx + 1 >= str.Length) return;
				str = str.Slice(idx + 1);
				idx = str.IndexOfAny(chrs);
			}
			validTextCallback(str);
		}
	}
}
