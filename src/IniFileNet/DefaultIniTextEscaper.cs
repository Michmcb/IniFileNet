namespace IniFileNet
{
	using System;
	using System.Buffers;
	using System.Runtime.CompilerServices;

	/// <summary>
	/// The default text escaper. Escapes \, carriage return, linefeed, and ; # = : [ ]
	/// </summary>
	public sealed class DefaultIniTextEscaper : IIniTextEscaper
	{
		/// <summary>
		/// The default instance. Always escapes all characters regardless of the context.
		/// </summary>
		public static readonly DefaultIniTextEscaper Default = new(true);
#if NET8_0_OR_GREATER
		private static readonly SearchValues<char> backSlash = SearchValues.Create(['\\']);
		private static readonly SearchValues<char> allEscapeChars = SearchValues.Create(['\\', '\r', '\n', ';', '#', '=', ':', '[', ']']);
		private static readonly SearchValues<char> valueEscapeChars = SearchValues.Create(['\\', '\r', '\n']);
		private static readonly SearchValues<char> keyEscapeChars = SearchValues.Create(['\\', '\r', '\n', ';', '#', '=', ':', '[']);
		private static readonly SearchValues<char> sectionEscapeChars = SearchValues.Create(['\\', '\r', '\n', ';', '#', '[', ']',]);
		private static readonly SearchValues<char> commentEscapeChars = SearchValues.Create(['\\', '\r', '\n']);
#else
		private static readonly char backSlash = '\\';
		private static readonly ReadOnlyMemory<char> allEscapeChars = new char[] { '\\', '\r', '\n', ';', '#', '=', ':', '[', ']' };
		private static readonly ReadOnlyMemory<char> valueEscapeChars = new char[] { '\\', '\r', '\n' };
		private static readonly ReadOnlyMemory<char> keyEscapeChars = new char[] { '\\', '\r', '\n', ';', '#', '=', ':', '[' };
		private static readonly ReadOnlyMemory<char> sectionEscapeChars = new char[] { '\\', '\r', '\n', ';', '#', '[', ']' };
		private static readonly ReadOnlyMemory<char> commentEscapeChars = new char[] { '\\', '\r', '\n' };
#endif
		private readonly bool alwaysEscapeAll;
		/// <summary>
		/// Creates a new instance.
		/// If <paramref name="alwaysEscapeAll"/> is <see langword="true"/>, then all characters are always escaped, even if the provided <see cref="IniTokenContext"/>
		/// does not require them to actually be escaped.
		/// If <paramref name="alwaysEscapeAll"/> is <see langword="false"/>, then characters are only escaped according to the <see cref="IniTokenContext"/> to form a valid file.
		/// If the character doesn't affect the syntax of the provided context in its unescaped form, then it will not be escaped.
		/// </summary>
		/// <param name="alwaysEscapeAll">If <see langword="true"/>, always escapes all syntax characters. Otherwise, only escapes characters when needed to preserve meaning.</param>
		public DefaultIniTextEscaper(bool alwaysEscapeAll)
		{
			this.alwaysEscapeAll = alwaysEscapeAll;
		}
		/// <inheritdoc/>
		public OperationStatusMsg Escape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock)
		{
			var chrs = alwaysEscapeAll
#if NET8_0_OR_GREATER
				? allEscapeChars
#else
				? allEscapeChars.Span
#endif
				: (tokenContext switch
				{
					IniTokenContext.Section => sectionEscapeChars,
					IniTokenContext.Comment => commentEscapeChars,
					IniTokenContext.Key => keyEscapeChars,
					IniTokenContext.Value => valueEscapeChars,
					_ => throw new ArgumentOutOfRangeException(nameof(tokenContext), "The provided value was not a valid value: " + ((int)tokenContext).ToString()),
				})
#if NET8_0_OR_GREATER
			;
#else
			.Span;
#endif
			charsConsumed = 0;
			charsWritten = 0;
			ReadOnlySpan<char> str = text;
			Span<char> dest = destination;
			int idx = str.IndexOfAny(chrs);
			while (idx != -1)
			{
				if (dest.Length >= idx)
				{
					Copy(str.Slice(0, idx), dest, ref charsConsumed, ref charsWritten);
				}
				else
				{
					Copy(str.Slice(0, dest.Length), dest, ref charsConsumed, ref charsWritten);
					return new(OperationStatus.DestinationTooSmall, null);
				}
				if (dest.Length < (idx + 2))
				{
					return new(OperationStatus.DestinationTooSmall, null);
				}
				char c;
				switch (str[idx])
				{
					case '\\': c = '\\'; break;
					case '\r': c = 'r'; break;
					case '\n': c = 'n'; break;
					case '=': c = '='; break;
					case ':': c = ':'; break;
					case '[': c = '['; break;
					case ']': c = ']'; break;
					case ';': c = ';'; break;
					case '#': c = '#'; break;
					default: return new(OperationStatus.InvalidData, InvalidEscapeSequence(idx, text)); // This should never happen, since we're handling all possible characters.
				}
				dest[idx] = '\\';
				dest[idx + 1] = c;
				charsConsumed++;
				charsWritten += 2;
				str = str.Slice(idx + 1);
				if (str.Length == 0) return new(isFinalBlock ? OperationStatus.Done : OperationStatus.NeedMoreData, null);
				dest = dest.Slice(idx + 2);
				if (dest.Length == 0) return new(OperationStatus.DestinationTooSmall, null);
				idx = str.IndexOfAny(chrs);
			}
			if (dest.Length >= str.Length)
			{
				Copy(str, dest, ref charsConsumed, ref charsWritten);
			}
			else if (dest.Length > 0)
			{
				Copy(str.Slice(0, dest.Length), dest, ref charsConsumed, ref charsWritten);
				return new(OperationStatus.DestinationTooSmall, null);
			}
			return new(isFinalBlock ? OperationStatus.Done : OperationStatus.NeedMoreData, null);
		}
		/// <inheritdoc/>
		public OperationStatusMsg Unescape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock)
		{
			charsConsumed = 0;
			charsWritten = 0;
			ReadOnlySpan<char> str = text;
			Span<char> dest = destination;
			int idx =
#if NET8_0_OR_GREATER
				str.IndexOfAny(backSlash);
#else
				str.IndexOf(backSlash);
#endif
			while (idx != -1)
			{
				if (dest.Length >= idx)
				{
					Copy(str.Slice(0, idx), dest, ref charsConsumed, ref charsWritten);
				}
				else
				{
					Copy(str.Slice(0, dest.Length), dest, ref charsConsumed, ref charsWritten);
					return new(OperationStatus.DestinationTooSmall, null);
				}
				if (idx + 1 >= str.Length)
				{
					return isFinalBlock ? new(OperationStatus.InvalidData, InvalidEscapeSequence(idx, text)) : new(OperationStatus.NeedMoreData, null);
				}
				char c;
				switch (str[idx + 1])
				{
					case '\\': c = '\\'; break;
					case 'r': c = '\r'; break;
					case 'n': c = '\n'; break;
					case '=': c = '='; break;
					case ':': c = ':'; break;
					case '[': c = '['; break;
					case ']': c = ']'; break;
					case ';': c = ';'; break;
					case '#': c = '#'; break;
					default: return new(OperationStatus.InvalidData, InvalidEscapeSequence(idx, text));
				}
				dest[idx] = c;
				charsWritten++;
				charsConsumed += 2;
				str = str.Slice(idx + 2);
				if (str.Length == 0) return new(isFinalBlock ? OperationStatus.Done : OperationStatus.NeedMoreData, null);
				dest = dest.Slice(idx + 1);
				if (dest.Length == 0) return new(OperationStatus.DestinationTooSmall, null);
				idx =
#if NET8_0_OR_GREATER
					str.IndexOfAny(backSlash);
#else
					str.IndexOf(backSlash);
#endif
			}
			if (dest.Length >= str.Length)
			{
				Copy(str, dest, ref charsConsumed, ref charsWritten);
			}
			else if (dest.Length > 0)
			{
				Copy(str.Slice(0, dest.Length), dest, ref charsConsumed, ref charsWritten);
				return new(OperationStatus.DestinationTooSmall, null);
			}
			return new(isFinalBlock ? OperationStatus.Done : OperationStatus.NeedMoreData, null);
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static void Copy(ReadOnlySpan<char> src, Span<char> dest, ref int consumed, ref int written)
		{
			src.CopyTo(dest);
			consumed += src.Length;
			written += src.Length;
		}
		/// <summary>
		/// Returns an error message
		/// </summary>
		/// <param name="index"></param>
		/// <param name="text"></param>
		/// <returns></returns>
		public static string InvalidEscapeSequence(int index, ReadOnlySpan<char> text)
		{
			return string.Concat("Invalid escape sequence at index ", index.ToString(), " of text:",
#if NETSTANDARD2_0
				text.ToString());
#else
				text);
#endif
		}
	}
}