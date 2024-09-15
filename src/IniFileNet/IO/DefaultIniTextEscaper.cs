namespace IniFileNet.IO
{
	using System;
#if NET6_0_OR_GREATER
	using System.Buffers;
	using System.Runtime.CompilerServices;
#endif

	/// <summary>
	/// The default text escaper. Escapes \, null, bell, backspace, carriage return, linefeed, and ; # = : [ ]
	/// </summary>
	public sealed class DefaultIniTextEscaper : IIniTextEscaper
	{
		/// <summary>
		/// The default instance. Always escapes all characters regardless of the context.
		/// </summary>
		public static readonly DefaultIniTextEscaper Default = new(true/*, true*/);
#if NET8_0_OR_GREATER
		private static readonly SearchValues<char> backSlash = SearchValues.Create(['\\']);
		private static readonly SearchValues<char> allEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':', '[', ']']);
		private static readonly SearchValues<char> valueEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n']);
		private static readonly SearchValues<char> keyEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':', '[']);
		private static readonly SearchValues<char> sectionEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '[', ']',]);
		private static readonly SearchValues<char> commentEscapeChars = SearchValues.Create(['\\', '\0', '\a', '\b']);
#else
		private static readonly char backSlash = '\\';
		private static readonly ReadOnlyMemory<char> allEscapeChars = new char[] {'\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':', '[', ']'};
		private static readonly ReadOnlyMemory<char> valueEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n' };
		private static readonly ReadOnlyMemory<char> keyEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '=', ':', '['};
		private static readonly ReadOnlyMemory<char> sectionEscapeChars = new char[] { '\\', '\0', '\a', '\b', '\r', '\n', ';', '#', '[', ']' };
		private static readonly ReadOnlyMemory<char> commentEscapeChars = new char[] { '\\', '\0', '\a', '\b' };
#endif
		private readonly bool alwaysEscapeAll;
		//private readonly bool handleLineContinuations;
		/// <summary>
		/// Creates a new instance.
		/// If <paramref name="alwaysEscapeAll"/> is <see langword="true"/>, then all characters are always escaped, even if the provided <see cref="IniTokenContext"/>
		/// does not require them to actually be escaped.
		/// If <paramref name="alwaysEscapeAll"/> is <see langword="false"/>, then characters are only escaped according to the <see cref="IniTokenContext"/> to form a valid file.
		/// If the character doesn't affect the syntax of the provided context in its unescaped form, then it will not be escaped.
		/// </summary>
		/// <param name="alwaysEscapeAll">If <see langword="true"/>, always escapes all syntax characters. Otherwise, only escapes characters when needed to preserve meaning.</param>
		//// <param name="handleLineContinuations">If <see langword="true"/>, line continuations are handled; that is, removed. Otherwise, they are retained.</param>
		public DefaultIniTextEscaper(bool alwaysEscapeAll)
		{
			this.alwaysEscapeAll = alwaysEscapeAll;
			//this.handleLineContinuations = handleLineContinuations;
		}
		/// <inheritdoc/>
		public IniError Escape(ReadOnlySpan<char> text, TextCallback validTextCallback, CharCallback charCallback, IniTokenContext token)
		{
			var chrs = alwaysEscapeAll
#if NET8_0_OR_GREATER
				? allEscapeChars
#else
				? allEscapeChars.Span
#endif
				: (token switch
				{
					IniTokenContext.Section => sectionEscapeChars,
					IniTokenContext.Comment => commentEscapeChars,
					IniTokenContext.Key => keyEscapeChars,
					IniTokenContext.Value => valueEscapeChars,
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
				switch (str[idx])
				{
					case '\\': charCallback('\\'); break;
					case '\0': charCallback('0'); break;
					case '\a': charCallback('a'); break;
					case '\b': charCallback('b'); break;
					case '\r': charCallback('r'); break;
					case '\n': charCallback('n'); break;
					case '=': charCallback('='); break;
					case ':': charCallback(':'); break;
					case '[': charCallback('['); break;
					case ']': charCallback(']'); break;
					case ';': charCallback(';'); break;
					case '#': charCallback('#'); break;
					default:
						string s;
#if NETSTANDARD2_0
						s = string.Concat("Cannot escape character '", str[idx], "' at index ", idx.ToString(), " of text:", text.ToString());
#else
						// Cannot escape character '- 25
						// ' at index - 11
						//  of text:- 9
						DefaultInterpolatedStringHandler sb = new(25 + 11 + 9 + text.Length, 3);
						sb.AppendLiteral("Cannot escape character '"); sb.AppendFormatted(str[idx]); sb.AppendLiteral("' at index - 11"); sb.AppendFormatted(idx);
						sb.AppendLiteral(" of text:"); sb.AppendFormatted(text);
						s = string.Create(null, ref sb);
#endif
						return new IniError(IniErrorCode.CannotEscapeCharacter, s);
				}
				if (idx + 1 >= str.Length) return default;
				str = str.Slice(idx + 1);
				idx = str.IndexOfAny(chrs);
			}
			validTextCallback(str);
			return default;
		}
		/// <inheritdoc/>
		public IniError Unescape(ReadOnlySpan<char> text, TextCallback textCallback, CharCallback charCallback, IniTokenContext token)
		{
			ReadOnlySpan<char> str = text;
			int idx =
#if NET8_0_OR_GREATER
			str.IndexOfAny(backSlash);
#else
			str.IndexOf(backSlash);
#endif
			while (idx != -1)
			{
				textCallback(str.Slice(0, idx));
				if (idx + 1 >= str.Length) return new IniError(IniErrorCode.InvalidEscapeSequence, string.Concat("Invalid escape sequence at index ", idx.ToString(), " of text:",
#if NETSTANDARD2_0
						text.ToString()));
#else
						text));
#endif
				char c = str[idx + 1];
				switch (c)
				{
					case '\\': charCallback('\\'); break;
					case '0': charCallback('\0'); break;
					case 'a': charCallback('\a'); break;
					case 'b': charCallback('\b'); break;
					case 'r': charCallback('\r'); break;
					case 'n': charCallback('\n'); break;
					case '=': charCallback('='); break;
					case ':': charCallback(':'); break;
					case '[': charCallback('['); break;
					case ']': charCallback(']'); break;
					case ';': charCallback(';'); break;
					case '#': charCallback('#'); break;
					// Line continuations. If we're handling them, then we do nothing. If we're not handling them, then we retain the slash and newline
					//case '\r': case '\n':
					//	if (!handleLineContinuations)
					//	{
					//		textCallback(str.Slice(idx, 2));
					//	}
					//	break;
					default:
						return new IniError(IniErrorCode.InvalidEscapeSequence, string.Concat("Invalid escape sequence at index ", idx.ToString(), " of text:",
#if NETSTANDARD2_0
						text.ToString()));
#else
						text));
#endif
				}

				if (idx + 2 >= str.Length) return default;
				str = str.Slice(idx + 2);
				idx =
#if NET8_0_OR_GREATER
				str.IndexOfAny(backSlash);
#else
				str.IndexOf(backSlash);
#endif
			}
			textCallback(str);
			return default;
		}
	}
}
