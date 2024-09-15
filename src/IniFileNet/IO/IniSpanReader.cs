#pragma warning disable IDE0057 // Use range operator
namespace IniFileNet.IO
{
	using System;
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
								goto case '=';
							}
							else { goto default; }
						case '=':
							_state = IniSpanReaderBlockState.Error;
							_error = IniErrorCode.EmptyKeyName;
							return new(IniContentType.Error, Block);
						default:
							{
								if (globalFlag == 0)
								{
									_state = IniSpanReaderBlockState.Key;
									return new(IniContentType.StartKey, default);
								}
								else
								{
									_state = IniSpanReaderBlockState.Error;
									_error = IniErrorCode.GlobalKeyNotAllowed;
									return new(IniContentType.Error, Block);
								}
							}
					}
				case IniSpanReaderBlockState.Key:
					{
						int start = _position;
						IdxEsc adv = Options.IgnoreKeyEscapes ? AdvanceToAny(Block, _position, keyEnd) : AdvanceToAnyEscapes(Block, _position, keyEnd, Backslash);
						_position = adv.Index;
						if (_position >= Block.Length)
						{
							_position = start;
							if (IsFinalBlock)
							{
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.KeyDelimiterNotFound;
								return new(IniContentType.Error, Block);
							}
							else
							{
								return new IniContent(IniContentType.End, default);
							}
						}
						else
						{
							_state = IniSpanReaderBlockState.KeyEnded;
							return new IniContent(adv.Escape ? IniContentType.KeyEscaped : IniContentType.Key, Block.Slice(start, _position - start));
						}
					}
				case IniSpanReaderBlockState.KeyEnded:
					{
						// Increment the position beyond the = sign
						_state = IniSpanReaderBlockState.PreValue;
						return GetCharContentAndTryAdvancePosition(IniContentType.EndKey);
					}
				case IniSpanReaderBlockState.PreValue:
					{
						_state = IniSpanReaderBlockState.Value;
						return new(IniContentType.StartValue, default);
					}
				case IniSpanReaderBlockState.Value:
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
										return new(IniContentType.Error, Block);
									}
									else if (Options.AllowLineContinuations)
									{
										// Trailing line continuation, so just chop it off
										valueContent = Block.Slice(start, (Block.Length - 1) - start);
										_state = IniSpanReaderBlockState.ValueEnded;
									}
									else
									{
										// Keep the trailing slash; it's just another character
										valueContent = Block.Slice(start);
										_state = IniSpanReaderBlockState.ValueEnded;
									}
								}
								else
								{
									valueContent = Block.Slice(start);
									_state = IniSpanReaderBlockState.ValueEnded;
								}
							}
							else
							{
								// If the very last character of the block happens to be a backslash, then we make sure to NOT return that as part of the value.
								// The reason being that backslashes require 2 characters to be correctly interpreted.
								if (Block[Block.Length - 1] == '\\' && (Options.AllowLineContinuations || !Options.IgnoreValueEscapes))
								{
									// hop back to just before the \ so it gets copied over into the new block
									_position = Block.Length - 1;
									// Additionally, if the content is empty, then that means we've consumed everything up to the backslash
									// So we say that we hit the end to avoid getting stuck in an infinite loop of empty value content
									var content = Block.Slice(start, (Block.Length - 1) - start);
									if (content.Length == 0)
									{
										return new(IniContentType.End, default);
									}
									else
									{
										valueContent = content;
									}
								}
								else
								{
									valueContent = Block.Slice(start);
								}
							}
						}
						else
						{
							if (Options.AllowLineContinuations && Block[_position - 1] == '\\')
							{
								int end = _position - 1;
								++_position;
								valueContent = Block.Slice(start, end - start);
							}
							else
							{
								_state = IniSpanReaderBlockState.ValueEnded;
								valueContent = Block.Slice(start, _position - start);
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
				case IniSpanReaderBlockState.ValueEnded:
					{
						_state = IniSpanReaderBlockState.Any;
						return new(IniContentType.EndValue, _position < Block.Length ? Block.Slice(_position, 1) : default);
					}
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
									return new(IniContentType.Error, Block);
								}
								else
								{
									commentContent = Block.Slice(start, (Block.Length - 1) - start);
								}
							}
							else
							{
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
								if (Block[Block.Length - 1] == '\\' && (!Options.IgnoreCommentEscapes))
								{
									_position = Block.Length - 1;
								}
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
						_state = IniSpanReaderBlockState.Global;
						if (Options.IgnoreComments)
						{
							goto start;
						}
						else
						{
							return new(IniContentType.EndComment, _position < Block.Length ? Block.Slice(_position, 1) : default);
						}
					}
				case IniSpanReaderBlockState.CommentEnded:
					{
						_state = IniSpanReaderBlockState.Any;
						if (Options.IgnoreComments)
						{
							goto start;
						}
						else
						{
							return new(IniContentType.EndComment, _position < Block.Length ? Block.Slice(_position, 1) : default);
						}
					}
				case IniSpanReaderBlockState.Section:
					{
						int start = _position;
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.SectionCloseBracketNotFound;
								return new(IniContentType.Error, Block);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}
						IdxEsc adv = Options.IgnoreSectionEscapes ? AdvanceToAny(Block, _position, sectionEnd) : AdvanceToAnyEscapes(Block, _position, sectionEnd, Backslash);
						_position = adv.Index;
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_position = start;
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.SectionCloseBracketNotFound;
								return new(IniContentType.Error, Block);
							}
							// If we have some content then we can return that, otherwise just return End
							else if (_position - start <= 0)
							{
								return new(IniContentType.End, default);
							}
						}
						else if (Block[_position] == ']')
						{
							_state = IniSpanReaderBlockState.SectionEnded;
						}
						return new(adv.Escape ? IniContentType.SectionEscaped : IniContentType.Section, Block.Slice(start, _position - start));
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
							_state = IniSpanReaderBlockState.Error;
							_error = IniErrorCode.SectionIsNotOnlyThingOnLine;
							return new(IniContentType.Error, Block);
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
		private static IdxEsc AdvanceToAny(ReadOnlySpan<char> block, int position, System.Buffers.SearchValues<char> chars)
#else
		private static IdxEsc AdvanceToAny(ReadOnlySpan<char> block, int position, ReadOnlySpan<char> chars)
#endif
		{
			int i = block.Slice(position).IndexOfAny(chars);
			return i == -1
				? new(block.Length, false)
				: new(i + position, false);
		}
#if NET8_0_OR_GREATER
		private static IdxEsc AdvanceToAnyEscapes(ReadOnlySpan<char> block, int position, SearchValues<char> chars, SearchValues<char> backslash)
#else
		private static IdxEsc AdvanceToAnyEscapes(ReadOnlySpan<char> block, int position, ReadOnlySpan<char> chars, char backslash)
#endif
		{
			bool esc = false;
			int off = position;
			ReadOnlySpan<char> str = block.Slice(off);
			int index;
			while (true)
			{
				int i = str.IndexOfAny(chars);
				if (i == -1)
				{
					// TODO we need to check the block for backslashes as well don't we?
					index = block.Length;
					break;
				}
				else if (i > 0 && str[i - 1] == '\\')
				{
					// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
					// If there's nothing further, that counts as NOT finding anything
					esc = true;
					if (i + 1 > str.Length)
					{
						index = block.Length;
						break;
					}
					str = str.Slice(i + 1);
					off += i + 1;
				}
				else
				{
					if (!esc)
					{
						esc = block.Slice(position, i)
#if NET8_0_OR_GREATER
							.ContainsAny(backslash);
#else
							.IndexOf(backslash) != -1;
#endif
					}
					index = i + off;
					break;
				}
			}
			// if we hit the end of a block and the very last character is a backslash then we can't return that; we need the next character to know what the escape sequence is
			if (esc && index + 1 > block.Length)
			{
				--index;
			}
			return new(index, esc);
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