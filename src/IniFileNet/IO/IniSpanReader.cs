namespace IniFileNet.IO
{
	using System;
	using System.Buffers;

	/// <summary>
	/// A low level stack-allocated ini reader that parses blocks of text.
	/// Typically, you would use <see cref="IniStreamReader"/> unless you want finer control over parsing.
	/// </summary>
	public ref struct IniSpanReader
	{
		private IniSpanReaderBlockState _state;
		private IniErrorCode _error;
		private int _position;
		private readonly SearchValues<char> keyEndChars;
		public IniSpanReader(ReadOnlySpan<char> block, IniSpanReaderState state, bool isFinalBlock)
		{
			keyEndChars = state.Options.AllowKeyDelimiterColon ? Syntax.EqColonSemicolon : Syntax.EqSemicolon;
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
			// Done: Keys cannot be empty
			// Done: Keys cannot contain ;
			// TODO: Leading/Trailing whitespace around key name has to be ignored
			// TODO: Check if Line continuations with trailing backslash are supposed to eat the newline characters or not
			// Done: Sections must appear on their own line
			// Done: Case insensitivity (This is implemented by string comparers and client code)
			// Done: Allow/disallow global keys (keys with no preceding section)
			char c;
			// When we're in the global state, we want to disallow reading keys.
			// We take advantage of the fact that the Comment and CommentEnded states are a single bit to the CommentGlobal and CommentEndedGlobal states
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
								return Read();
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
							else goto default;
						case '=':
							_state = IniSpanReaderBlockState.Error;
							_error = IniErrorCode.EmptyKeyName;
							return new(IniContentType.Error, Block.Slice(_position));
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
									return new(IniContentType.Error, Block.Slice(_position));
								}
							}
					}
				case IniSpanReaderBlockState.Key:
					{
						int start = _position;

						_position = AdvanceToAny(Block, _position, keyEndChars);
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_position = start;
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.KeyDelimiterNotFound;
								return new(IniContentType.Error, Block[start..]);
							}
							else
							{
								_position = start;
								return new IniContent(IniContentType.End, default);
							}
						}
						else
						{
							c = Block[_position];
							if (c == ';')
							{
								_position = start;
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.SemicolonInKeyName;
								return new(IniContentType.Error, Block[start..]);
							}
							_state = IniSpanReaderBlockState.KeyEnded;
							return new IniContent(IniContentType.Key, Block[start.._position]);
						}
					}
				case IniSpanReaderBlockState.KeyEnded:
					{
						// Increment the position beyond the = sign
						_state = IniSpanReaderBlockState.PreValue;
						return GetCharContentAndAdvancePosition(IniContentType.EndKey);
					}
				case IniSpanReaderBlockState.PreValue:
					{
						_state = IniSpanReaderBlockState.Value;
						return new(IniContentType.StartValue, default);
					}
				case IniSpanReaderBlockState.Value:
					{
						int start = _position;
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
						AdvanceToNewLine();
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								// If the very last character of a stream happens to be a backslash and we're allowing line continuations then we have to just pretend that
								// it doesn't exist
								_state = IniSpanReaderBlockState.ValueEnded;
								var content = (Block[Block.Length - 1] == '\\' && Options.AllowLineContinuations)
									? Block[start..(Block.Length - 1)]
									: Block[start..];
								return new(IniContentType.Value, content);
							}
							else
							{
								// If the very last character of the block happens to be a backslash then we make sure to NOT return that as part of the value
								// because that may mean we end up skipping a line continuation as the first character of the next block may be \r or \n
								if (Block[Block.Length - 1] == '\\' && Options.AllowLineContinuations)
								{
									// hop back to just before the \ so it gets copied over into the new block
									_position = Block.Length - 1;
									// Additionally, if the content is empty, then that means we've consumed everything up to the backslash
									// So we say that we hit the end to avoid getting stuck in an infinite loop of empty value content
									var content = Block[start..(Block.Length - 1)];
									if (content.Length == 0)
									{
										return new(IniContentType.End, default);
									}
									else
									{
										return new(IniContentType.Value, content);
									}
								}
								else
								{
									return new(IniContentType.Value, Block[start..]);
								}
							}
						}
						else
						{
							if (Options.AllowLineContinuations && Block[_position - 1] == '\\')
							{
								int end = _position - 1;
								++_position;
								return new(IniContentType.Value, Block[start..end]);
							}
							else
							{
								_state = IniSpanReaderBlockState.ValueEnded;
								return new(IniContentType.Value, Block[start.._position]);
							}
						}
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
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_state = globalFlag | IniSpanReaderBlockState.CommentEnded;
							}
							return Options.IgnoreComments ? Read() : new(IniContentType.Comment, Block[start..]);
						}
						else
						{
							_state = globalFlag | IniSpanReaderBlockState.CommentEnded;
							return Options.IgnoreComments ? Read() : new(IniContentType.Comment, Block[start.._position]);
						}
					}
				case IniSpanReaderBlockState.CommentEndedGlobal:
					{
						_state = IniSpanReaderBlockState.Global;
						return Options.IgnoreComments ? Read() : new(IniContentType.EndComment, _position < Block.Length ? Block.Slice(_position, 1) : default);
					}
				case IniSpanReaderBlockState.CommentEnded:
					{
						_state = IniSpanReaderBlockState.Any;
						return Options.IgnoreComments ? Read() : new(IniContentType.EndComment, _position < Block.Length ? Block.Slice(_position, 1) : default);
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
								return new(IniContentType.Error, Block[start..]);
							}
							else
							{
								return new(IniContentType.End, default);
							}
						}
						AdvanceTo(']');
						if (_position >= Block.Length)
						{
							if (IsFinalBlock)
							{
								_position = start;
								_state = IniSpanReaderBlockState.Error;
								_error = IniErrorCode.SectionCloseBracketNotFound;
								return new(IniContentType.Error, Block[start..]);
							}
							return new(IniContentType.Section, Block[start..]);
						}
						else
						{
							_state = IniSpanReaderBlockState.SectionEnded;
							return new(IniContentType.Section, Block[start.._position]);
						}
					}
				case IniSpanReaderBlockState.SectionEnded:
					{
						_state = IniSpanReaderBlockState.SectionEndedVerifyOnlyThingOnLine;
						return GetCharContentAndAdvancePosition(IniContentType.EndSection);
					}
				case IniSpanReaderBlockState.SectionEndedVerifyOnlyThingOnLine:
					{
						int start = _position;
						AdvanceToNewLine();
						if (!Block[start.._position].IsWhiteSpace())
						{
							_position = start;
							_state = IniSpanReaderBlockState.Error;
							_error = IniErrorCode.SectionIsNotOnlyThingOnLine;
							return new(IniContentType.Error, Block[start..]);
						}
						if (_position >= Block.Length)
						{
							return new(IniContentType.End, default);
						}
						else
						{
							_state = IniSpanReaderBlockState.Any;
							return Read();
						}
					}
				default:
				case IniSpanReaderBlockState.Error:
					return new(IniContentType.Error, _position < Block.Length ? Block[_position..] : default);
			}
		}
		private IniContent GetCharContentAndAdvancePosition(IniContentType tok)
		{
			ReadOnlySpan<char> content = _position < Block.Length ? Block.Slice(_position, 1) : default;
			++_position;
			return new(tok, content);
		}
		private void SkipWhitespace()
		{
			int i = Block[_position..].IndexOfAnyExcept(" \t\n\r");
			_position = i == -1
				? Block.Length
				: i + _position;
		}
		private void AdvanceTo(char c)
		{
			int i = Block[_position..].IndexOf(c);
			_position = i == -1
				? Block.Length
				: i + _position;
		}
		private static int AdvanceToAny(ReadOnlySpan<char> block, int position, SearchValues<char> c)
		{
			int i = block[position..].IndexOfAny(c);
			return i == -1
				? block.Length
				: i + position;
		}
		private void AdvanceToNewLine()
		{
			int i = Block[_position..].IndexOfAny(Syntax.NewLineChars);
			_position = i == -1
				? Block.Length
				: i + _position;
		}
	}
}
