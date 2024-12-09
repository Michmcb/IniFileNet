#pragma warning disable IDE0057 // Use range operator
#pragma warning disable IDE0056 // Use index operator
namespace IniFileNet.IO
{
	using System;
	using System.Runtime.CompilerServices;
#if NET8_0_OR_GREATER
	using System.Buffers;
#endif
	/// <summary>
	/// A low level stack-allocated ini reader that parses blocks of text.
	/// Typically, you would use <see cref="IniStreamReader"/> unless you want finer control over parsing.
	/// </summary>
	public ref struct IniSpanReader
	{
#if NET8_0_OR_GREATER
		private static readonly SearchValues<char> Backslash = SearchValues.Create(new char[] { '\\' });
		private static readonly SearchValues<char> EqColon = SearchValues.Create(new char[] { '=', ':' });
		private static readonly SearchValues<char> Eq = SearchValues.Create(new char[] { '=' });
		private readonly SearchValues<char> keyEnd;
		private static readonly SearchValues<char> sectionEnd = SearchValues.Create(new char[] { ']' });
#else
		private const char Backslash = '\\';
		private readonly ReadOnlySpan<char> sectionEnd = [']'];
		private readonly ReadOnlySpan<char> keyEnd;
#endif
		private IniSpanReaderBlockState _state;
		private IniErrorCode _error;
		private int _position;
		/// <summary>
		/// Creates a new instance with the current <paramref name="state"/>.
		/// </summary>
		/// <param name="block">The current block of data to read.</param>
		/// <param name="state">The current state, or <see langword="default"/> if this is the first block.</param>
		/// <param name="isFinalBlock">If this is the final block of data, set this to <see langword="true"/>.</param>
		public IniSpanReader(ReadOnlySpan<char> block, IniSpanReaderState state, bool isFinalBlock)
		{
#if NET8_0_OR_GREATER
			keyEnd = state.Options.AllowKeyDelimiterColon ? EqColon : Eq;
#else
			keyEnd = state.Options.AllowKeyDelimiterColon ? ['=', ':'] : ['='];
#endif
			Block = block;
			_state = state.State;
			_position = state.Position;
			Options = state.Options;
			_error = state.Error;
			IsFinalBlock = isFinalBlock;
		}
		/// <summary>
		/// The current block of text.
		/// </summary>
		public ReadOnlySpan<char> Block { get; }
		/// <summary>
		/// The current position of parsing within <see cref="Block"/>.
		/// </summary>
		public readonly int Position => _position;
		/// <summary>
		/// The options this was configured with.
		/// </summary>
		public readonly IniReaderOptions Options { get; }
		/// <summary>
		/// The current state of the parser.
		/// </summary>
		public readonly IniSpanReaderBlockState State => _state;
		/// <summary>
		/// The current error code that was encountered when reading.
		/// </summary>
		public readonly IniErrorCode ErrorCode => _error;
		/// <summary>
		/// If true, this is the last block of data. If false, more data is expected.
		/// </summary>
		public bool IsFinalBlock { get; }
		/// <summary>
		/// Gets a <see cref="IniSpanReaderState"/> which can be used to restore the state of this <see cref="IniSpanReader"/>.
		/// </summary>
		public readonly IniSpanReaderState GetState()
		{
			return new IniSpanReaderState(_state, _error, Options, _position > Block.Length ? Block.Length : _position);
		}
		/// <summary>
		/// Reads content from the block of text.
		/// </summary>
		public IniContent Read()
		{
		// Instead of calling this function recursively, we just goto the beginning. The reason is because if we have lots and lots
		// of contiguous comments, then eventually we'll cause a StackOverflowException.
		start:
			char c;
			// When we're in the global state, we want to disallow reading keys.
			// Comment and CommentEnded states are a single bit different to the CommentGlobal and CommentEndedGlobal states
			// So, if we're in the global section of the file (i.e. no sections read yet) and the options specify no global keys are allowed, then we
			// Set a flag which forbids keys
			IniSpanReaderBlockState globalFlag = 0;
			switch (_state)
			{
				//default: throw new InvalidOperationException(); // Should never happen...
				case IniSpanReaderBlockState.Global:
					// If we allow global keys, then we don't set the flag.
					globalFlag = Options.AllowGlobalKeys ? 0 : IniSpanReaderBlockState.Any;
					goto case IniSpanReaderBlockState.Any;
				case IniSpanReaderBlockState.Any:
					if (_position >= Block.Length)
					{
						return new(IniContentType.End, default);
					}
					SkipWhitespace();
					if (_position >= Block.Length)
					{
						return new(IniContentType.End, default);
					}
					c = Block[_position];

					switch (c)
					{
						// We don't need to do any range checking here because we already accessed _position by index, so we know it will be fine
						case '[':
							_state = IniSpanReaderBlockState.Section;
							return new(IniContentType.StartSection, Block.Slice(_position++, 1));
						case '#':
							if (Options.AllowCommentsNumberSign)
							{
								goto case ';';
							}
							else { goto default; }
						case ';':
							_state = globalFlag | IniSpanReaderBlockState.Comment;
							if (Options.IgnoreComments)
							{
								_position++;
								goto start;
							}
							else
							{
								return new(IniContentType.StartComment, Block.Slice(_position++, 1));
							}
						case ':':
							if (Options.AllowKeyDelimiterColon)
							{
								return Error(IniErrorCode.EmptyKeyName);
							}
							else { goto default; }
						case '=':
							return Error(IniErrorCode.EmptyKeyName);
						default:
							{
								if (globalFlag == 0)
								{
									_state = IniSpanReaderBlockState.Key;
									return new(IniContentType.StartKey, default);
								}
								else
								{
									return Error(IniErrorCode.GlobalKeyNotAllowed);
								}
							}
					}
				case IniSpanReaderBlockState.Key:
					{
						if (_position >= Block.Length)
						{
							_position = Block.Length - 1;
							//if (IsFinalBlock)
							//{
							//	return Error(IniErrorCode.KeyDelimiterNotFound);
							//}
							//else
							//{
							//	return new(IniContentType.End, default);
							//}
						}

						int start = _position;
						int idx;
						int slashIdx;
						IniContentType contentType;
						if (Options.IgnoreKeyEscapes)
						{
							idx = IndexOfAny(Block, _position, keyEnd);
							slashIdx = -1;
							contentType = IniContentType.Key;
						}
						else
						{
							// We have to make sure that the escape sequence is not at the very end if we're in the final block.
							IdxEsc adv = AdvanceToAnyEscapes(Block, _position, keyEnd, Backslash);
							if (IsFinalBlock && adv.SlashIndex == Block.Length - 1)
							{
								return Error(IniErrorCode.InvalidEscapeSequence);
							}
							idx = adv.Index;
							slashIdx = adv.SlashIndex;
							contentType = slashIdx != -1 ? IniContentType.KeyEscaped : IniContentType.Key;
						}

						if (idx >= Block.Length)
						{
							if (IsFinalBlock)
							{
								return Error(IniErrorCode.KeyDelimiterNotFound);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}

						_state = IniSpanReaderBlockState.KeyEnded;
						_position = idx;
						return new IniContent
						(
							contentType,
							Options.TrimKeys ? Block.Slice(start, _position - start).Trim() : Block.Slice(start, _position - start)
						);
					}
				case IniSpanReaderBlockState.KeyEnded:
					{
						// Increment the position beyond the = sign
						_state = IniSpanReaderBlockState.PreValue;
						return GetCharContentAndTryAdvancePosition(IniContentType.EndKey);
					}
				case IniSpanReaderBlockState.PreValue:
					{
						_state = IniSpanReaderBlockState.ValueFirstPart;
						return new(IniContentType.StartValue, default);
					}
				case IniSpanReaderBlockState.ValueFirstPart:
					{
						IniContent content = HandleValue();
						if (Options.TrimValues)
						{
							content = new(content.Type, content.Content.TrimStart());
						}
						return content;
					}
				case IniSpanReaderBlockState.Value:
					return HandleValue();
				case IniSpanReaderBlockState.ValueEnded:
					return HandleNewLineAndReturn(IniContentType.EndValue, IniSpanReaderBlockState.Any);
				case IniSpanReaderBlockState.CommentGlobal:
					globalFlag = IniSpanReaderBlockState.Any;
					goto case IniSpanReaderBlockState.Comment;
				case IniSpanReaderBlockState.Comment:
					{
						int start = _position;
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_state = globalFlag | IniSpanReaderBlockState.CommentEnded;
								return new(IniContentType.Comment, default);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}
						AdvanceToNewLine();
						ReadOnlySpan<char> commentContent;
						// We don't need to care about line continutations within comments. Only have to worry about charater escapes.
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_state = globalFlag | IniSpanReaderBlockState.CommentEnded;
							}
							if (Options.IgnoreComments)
							{
								goto start;
							}
							if (Block[Block.Length - 1] == '\\' && (!Options.IgnoreCommentEscapes))
							{
								if (IsFinalBlock)
								{
									return Error(IniErrorCode.InvalidEscapeSequence);
								}
								else
								{
									// Don't keep the slash in the block because it may be an escape sequence
									// Plus, if empty, just return End so no infinite loop of empty content
									_position = Block.Length - 1;
									commentContent = Block.Slice(start, _position - start);
									if (commentContent.Length == 0) { return new(IniContentType.End, default); }
								}
							}
							else
							{
								// Trailing slash is just another character
								commentContent = Block.Slice(start);
							}
						}
						else
						{
							_state = globalFlag | IniSpanReaderBlockState.CommentEnded;
							if (Options.IgnoreComments)
							{
								goto start;
							}
							else
							{
								commentContent = Block.Slice(start, _position - start);
							}
						}
						IniContentType commentType = !Options.IgnoreCommentEscapes
#if NET8_0_OR_GREATER
							&& commentContent.IndexOfAny(Backslash) != -1
#else
							&& commentContent.IndexOf(Backslash) != -1
#endif
							? IniContentType.CommentEscaped
							: IniContentType.Comment;
						return new(commentType, commentContent);
					}
				case IniSpanReaderBlockState.CommentEndedGlobal:
					{
						if (Options.IgnoreComments)
						{
							_state = IniSpanReaderBlockState.Global;
							goto start;
						}
						else
						{
							return HandleNewLineAndReturn(IniContentType.EndComment, IniSpanReaderBlockState.Global);
						}
					}
				case IniSpanReaderBlockState.CommentEnded:
					{
						if (Options.IgnoreComments)
						{
							_state = IniSpanReaderBlockState.Any;
							goto start;
						}
						else
						{
							return HandleNewLineAndReturn(IniContentType.EndComment, IniSpanReaderBlockState.Any);
						}
					}
				case IniSpanReaderBlockState.Section:
					{
						int start = _position;
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								return Error(IniErrorCode.SectionCloseBracketNotFound);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}
						int idx;
						int slashIdx;
						IniContentType contentType;
						if (Options.IgnoreSectionEscapes)
						{
							idx = IndexOfAny(Block, _position, sectionEnd);
							slashIdx = -1;
							contentType = IniContentType.Section;
						}
						else
						{
							IdxEsc adv = AdvanceToAnyEscapes(Block, _position, sectionEnd, Backslash);
							if (IsFinalBlock && adv.SlashIndex == Block.Length - 1)
							{
								return Error(IniErrorCode.InvalidEscapeSequence);
							}
							idx = adv.Index;
							slashIdx = adv.SlashIndex;
							contentType = slashIdx != -1 ? IniContentType.SectionEscaped : IniContentType.Section;
						}
						if (idx >= Block.Length)
						{
							if (IsFinalBlock)
							{
								return Error(IniErrorCode.SectionCloseBracketNotFound);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}
						if (idx - start == 0)
						{
							return Error(IniErrorCode.EmptySectionName);
						}
						_state = IniSpanReaderBlockState.SectionEnded;
						_position = idx;
						return new IniContent
						(
							contentType,
							Options.TrimSections ? Block.Slice(start, _position - start).Trim() : Block.Slice(start, _position - start)
						);
					}
				case IniSpanReaderBlockState.SectionEnded:
					{
						_state = IniSpanReaderBlockState.SectionEndedVerifyOnlyThingOnLine;
						return GetCharContentAndTryAdvancePosition(IniContentType.EndSection);
					}
				case IniSpanReaderBlockState.SectionEndedVerifyOnlyThingOnLine:
					{
						int start = _position;
						AdvanceToNewLine();
						if (!Block.Slice(start, _position - start).IsWhiteSpace())
						{
							_position = start;
							return Error(IniErrorCode.SectionIsNotOnlyThingOnLine);
						}
						if (_position >= Block.Length)
						{
							return new(IniContentType.End, default);
						}
						else
						{
							_state = IniSpanReaderBlockState.Any;
							goto start;
						}
					}
				default:
				case IniSpanReaderBlockState.Error:
					return new(IniContentType.Error, Block);
			}
		}
		private IniContent HandleNewLineAndReturn(IniContentType contentType, IniSpanReaderBlockState newState)
		{
			ReadOnlySpan<char> content;
			if (_position < Block.Length)
			{
				if (Block[_position] == '\n')
				{
					content = Block.Slice(_position, 1);
				}
				else if (_position + 1 < Block.Length)
				{
					if (Block[_position + 1] == '\n')
					{
						content = Block.Slice(_position, 2);
					}
					else
					{
						content = Block.Slice(_position, 1);
					}
				}
				else
				{
					if (IsFinalBlock) { content = Block.Slice(_position, 1); }
					else { return new IniContent(IniContentType.End, default); }
				}
			}
			else
			{
				content = default;
			}
			_state = newState;
			return new(contentType, content);
		}
		private IniContent HandleValue()
		{
			if (_position >= Block.Length)
			{
				if (IsFinalBlock)
				{
					_state = IniSpanReaderBlockState.ValueEnded;
					return new(IniContentType.Value, default);
				}
				else
				{
					return new(IniContentType.End, default);
				}
			}
			int start = _position;
			AdvanceToNewLine();
			ReadOnlySpan<char> valueContent;
			if (_position >= Block.Length)
			{
				if (IsFinalBlock)
				{
					// If we have a block that ends with a slash, then we want to exclude the slash, and the next return should be "End"
					// The same thing applies to comments. Keys and sections also get that treatment, minus the line continuations thing.
					if (Block[Block.Length - 1] == '\\')
					{
						// Might be an escape sequence, so error
						if (!Options.IgnoreValueEscapes)
						{
							return Error(IniErrorCode.InvalidEscapeSequence);
						}
						else if (Options.AllowLineContinuations)
						{
							// Trailing line continuation, so just chop it off
							valueContent = ValueEnded(Block.Slice(start, (Block.Length - 1) - start));
						}
						else
						{
							// Keep the trailing slash; it's just another character
							valueContent = ValueEnded(Block.Slice(start));
						}
					}
					else
					{
						valueContent = ValueEnded(Block.Slice(start));
					}
				}
				else
				{
					_state = IniSpanReaderBlockState.Value;
					// If the very last character of the block happens to be a backslash, then we make sure to NOT return that as part of the value.
					// The reason being that backslashes require 2 characters to be correctly interpreted.
					if (Block[Block.Length - 1] == '\\' && (Options.AllowLineContinuations || !Options.IgnoreValueEscapes))
					{
						// hop back to just before the \ so it gets copied over into the new block
						_position = Block.Length - 1;
						// Additionally, if the content is empty, then that means we've consumed everything up to the backslash
						// So we say that we hit the end to avoid getting stuck in an infinite loop of empty value content
						if (_position == start)
						{
							return new(IniContentType.End, default);
						}
						valueContent = Block.Slice(start, _position - start);
					}
					else
					{
						// If we're trimming values then we need to leave all trailing whitespace in the block,
						// so it can be handled later.
						if (Options.TrimValues && char.IsWhiteSpace(Block[Block.Length - 1]))
						{
							int pos = Block.Length;
							while (char.IsWhiteSpace(Block[--pos])) ;
							_position = pos + 1;
							// If no content besides the trailing whitespace, then say we hit the end to avoid infinite loop
							if (_position == start)
							{
								return new(IniContentType.End, default);
							}
							valueContent = Block.Slice(start, _position - start);
						}
						else
						{
							valueContent = Block.Slice(start);
						}
					}
				}
			}
			else
			{
				_state = IniSpanReaderBlockState.Value;
				if (Options.AllowLineContinuations && Block[_position - 1] == '\\')
				{
					int end = _position - 1;
					++_position;
					valueContent = Block.Slice(start, end - start);
				}
				else
				{
					valueContent = ValueEnded(Block.Slice(start, _position - start));
				}
			}
			IniContentType valueType = !Options.IgnoreValueEscapes
#if NET8_0_OR_GREATER
				&& valueContent.IndexOfAny(Backslash) != -1
#else
				&& valueContent.IndexOf(Backslash) != -1
#endif
				? IniContentType.ValueEscaped
				: IniContentType.Value;
			return new(valueType, valueContent);
		}
		private ReadOnlySpan<char> ValueEnded(ReadOnlySpan<char> content)
		{
			_state = IniSpanReaderBlockState.ValueEnded;
			if (Options.TrimValues)
			{
				return content.TrimEnd();
			}
			else
			{
				return content;
			}
		}
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private IniContent Error(IniErrorCode code)
		{
			_state = IniSpanReaderBlockState.Error;
			_error = code;
			return new IniContent(IniContentType.Error, Block);
		}
		private IniContent GetCharContentAndTryAdvancePosition(IniContentType tok)
		{
			ReadOnlySpan<char> content = _position < Block.Length ? Block.Slice(_position, 1) : default;
			++_position;
			return new(tok, content);
		}
		private void SkipWhitespace()
		{
#if NET8_0_OR_GREATER
			int i = Block.Slice(_position).IndexOfAnyExcept(Syntax.WhitespaceChars);
#elif NET7_0_OR_GREATER
			int i = Block.Slice(_position).IndexOfAnyExcept(Syntax.WhitespaceAsMemory.Span);
#else
			int i = IndexOfAnyExcept(Block.Slice(_position), Syntax.WhitespaceAsMemory.Span);
#endif
			_position = i == -1
				? Block.Length
				: i + _position;
		}
#if !NET7_0_OR_GREATER
		private static int IndexOfAnyExcept(ReadOnlySpan<char> block, ReadOnlySpan<char> chars)
		{
			switch (chars.Length)
			{
				case 4: return IndexOfAnyExcept(block, chars[0], chars[1], chars[2], chars[3]);
				// As of right now, we are only EVER using this with Syntax.WhitespaceChars. So we just throw if it's anything other than 4
				// If we ever use other lengths, we'll implement them. Anything above 5 will use the loop below. It's fully implemented, just not tested.
				default:
					throw new NotImplementedException();
			}
			//for (int i = 0; i < block.Length; ++i)
			//{
			//	// If at least one of the characters match, then keep going.
			//	// If none of the characters match, return.
			//	char c1 = block[i];
			//	int j = 0;
			//	for (; j < chars.Length; ++j)
			//	{
			//		if (c1 == chars[j]) break;
			//	}
			//	if (j == chars.Length) return i;
			//}
			//return -1;
		}
		private static int IndexOfAnyExcept(ReadOnlySpan<char> block, char c0, char c1, char c2, char c3)
		{
			for (int i = 0; i < block.Length; ++i)
			{
				char c = block[i];
				if (c != c0 && c != c1 && c != c2 && c != c3) { return i; }
			}
			return -1;
		}
#endif
#if NET8_0_OR_GREATER
		private static int IndexOfAny(ReadOnlySpan<char> block, int position, System.Buffers.SearchValues<char> chars)
#else
		private static int IndexOfAny(ReadOnlySpan<char> block, int position, ReadOnlySpan<char> chars)
#endif
		{
			int i = block.Slice(position).IndexOfAny(chars);
			return i == -1
				? block.Length
				: i + position;
		}
#if NET8_0_OR_GREATER
		private static IdxEsc AdvanceToAnyEscapes(ReadOnlySpan<char> block, int position, SearchValues<char> chars, SearchValues<char> backslash)
#else
		private static IdxEsc AdvanceToAnyEscapes(ReadOnlySpan<char> block, int position, ReadOnlySpan<char> chars, char backslash)
#endif
		{
			int slashIdx = -1;
			int off = position;
			ReadOnlySpan<char> str = block.Slice(off);
			int index;
			while (true)
			{
				int i = str.IndexOfAny(chars);
				if (i == -1)
				{
					index = block.Length;
					break;
				}
				else if (i > 0 && str[i - 1] == '\\')
				{
					// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
					// If there's nothing further, that counts as NOT finding anything
					// Although we don't know if e.g. \= is valid or even what it means, we do know that it IS an escape sequence, and thus
					// the = sign that we found does not have the standard meaning of a key delimiter. Thus, it is safe for us to skip it.
					slashIdx = i - 1;
					if (i + 1 >= str.Length)
					{
						index = block.Length;
						break;
					}
					str = str.Slice(i + 1);
					off += i + 1;
				}
				else
				{
					index = i + off;
					break;
				}
			}
			// if we hit the end of a block and the very last character is a backslash then we can't return that; we need the next character to know what the escape sequence is
			// this is an error in the case of a final block
			//if (index == block.Length && block[index - 1] == '\\')
			//{
			//	--index;
			//}
			if (slashIdx == -1)
			{
				slashIdx = block.Slice(position, index - position)
#if NET8_0_OR_GREATER
					.IndexOfAny(backslash);
#else
					.IndexOf(backslash);
#endif
				if (slashIdx != -1)
				{
					slashIdx += position;
				}
			}
			return new(index, slashIdx);
		}
		private void AdvanceToNewLine()
		{
#if NET8_0_OR_GREATER
			int i = Block.Slice(_position).IndexOfAny(Syntax.NewLineChars);
#else
			int i = Block.Slice(_position).IndexOfAny(Syntax.NewLineCharsAsMemory.Span);
#endif
			_position = i == -1 ? Block.Length : i + _position;
		}
	}
}
#pragma warning restore IDE0057 // Use range operator
#pragma warning restore IDE0056 // Use index operator