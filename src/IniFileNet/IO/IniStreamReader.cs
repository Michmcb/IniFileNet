﻿namespace IniFileNet.IO
{
	using System;
	using System.Globalization;
	using System.IO;
	using System.Text;
	using System.Threading.Tasks;
	/// <summary>
	/// Reads an ini text stream.
	/// </summary>
	public sealed class IniStreamReader : IDisposable
	{
		/// <summary>
		/// The default buffer size.
		/// </summary>
		public const int DefaultBufferSize = 16384;
#if NETSTANDARD2_0
		private readonly char[] buf;
#else
		private readonly Memory<char> buf;
#endif
		private int totalPosition = 0;
		private IniSpanReaderState state;
		private bool isFinalBlock;
		private int currentDataSize;
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniStreamReader(TextReader reader, IniReaderOptions options = default, int bufferSize = DefaultBufferSize, bool leaveOpen = false)
		{
			Reader = reader;
			BufferSize = bufferSize;
			LeaveOpen = leaveOpen;
			buf = new char[bufferSize];
			state = new IniSpanReaderState(default, default, options, 0);
			currentDataSize = 0;
		}
		/// <summary>
		/// The underlying <see cref="TextReader"/> being used.
		/// </summary>
		public TextReader Reader { get; }
		/// <summary>
		/// The size of the buffer to use.
		/// </summary>
		public int BufferSize { get; }
		/// <summary>
		/// <see langword="true"/> to leave <see cref="Reader"/> open after this <see cref="IniStreamReader"/> is disposed. Otherwise, false.
		/// </summary>
		public bool LeaveOpen { get; set; }
		/// <summary>
		/// Gets the current position in <see cref="Reader"/>.
		/// </summary>
		public int StreamPosition => totalPosition + state.Position;
		/// <summary>
		/// The configured options.
		/// </summary>
		public IniReaderOptions Options => state.Options;
		/// <summary>
		/// The data currently buffered into memory.
		/// </summary>
		public ReadOnlyMemory<char> Buffer => buf;
		/// <summary>
		/// The current error, if any.
		/// </summary>
		public IniError Error { get; set; }
		/// <summary>
		/// Reads the next token from the ini stream.
		/// </summary>
		public ReadResult Read()
		{
			StringBuilder contentBuilder = new();
			while (true)
			{
				// Null means we need more data
				var rr = ReadInternal(contentBuilder);
				if (rr.HasValue)
				{
					return rr.Value;
				}
				else if (isFinalBlock)
				{
					return new(IniToken.End, "");
				}
				else
				{
#if NETSTANDARD2_0
					state = state.NewBlock(buf.AsSpan(0, currentDataSize), buf.AsSpan(), out int copied);
					int charsRead = Reader.Read(buf, copied, buf.Length - copied);
#else
					state = state.NewBlock(buf.Span.Slice(0, currentDataSize), buf.Span, out int copied);
					int charsRead = Reader.Read(buf.Span.Slice(copied));
#endif
					totalPosition += copied;
					currentDataSize = copied + charsRead;
					isFinalBlock = charsRead == 0;
				}
			}
		}
		/// <summary>
		/// Reads the next token from the ini stream.
		/// </summary>
		public async Task<ReadResult> ReadAsync()
		{
			StringBuilder contentBuilder = new();
			while (true)
			{
				// Null means we need more data
				var rr = ReadInternal(contentBuilder);
				if (rr.HasValue)
				{
					return rr.Value;
				}
				else if (isFinalBlock)
				{
					return new(IniToken.End, "");
				}
				else
				{
#if NETSTANDARD2_0
					state = state.NewBlock(buf.AsSpan(0, currentDataSize), buf.AsSpan(), out int copied);
					int charsRead = await Reader.ReadAsync(buf, copied, buf.Length - copied);
#else
					state = state.NewBlock(buf.Span.Slice(0, currentDataSize), buf.Span, out int copied);
					int charsRead = await Reader.ReadAsync(buf.Slice(copied));
#endif
					totalPosition += copied;
					currentDataSize = copied + charsRead;
					isFinalBlock = charsRead == 0;
				}
			}
		}
		internal ReadResult? ReadInternal(StringBuilder contentBuilder)
		{
			IniSpanReader sr = new(
#if NETSTANDARD2_0
			buf.AsSpan(0, currentDataSize)
#else
			buf.Slice(0, currentDataSize).Span
#endif
				, state, isFinalBlock);
			IniToken token = IniToken.End;
			while (token == IniToken.End)
			{
				IniContent ic = sr.Read();
				switch (ic.Type)
				{
					// Clear out the content we have so we're ready to read in a new block
					case IniContentType.StartKey:
					case IniContentType.StartValue:
					case IniContentType.StartSection:
					case IniContentType.StartComment:
						contentBuilder.Clear();
						break;

					case IniContentType.End:
						// We need some more data
						state = sr.GetState();
						return null;

					case IniContentType.Key:
					case IniContentType.Value:
					case IniContentType.Section:
					case IniContentType.Comment:
#if NETSTANDARD2_0
						contentBuilder.Append(ic.Content.ToString());
#else
						contentBuilder.Append(ic.Content);
#endif
						break;

					case IniContentType.EndKey:
						token = IniToken.Key;
						break;
					case IniContentType.EndValue:
						token = IniToken.Value;
						break;
					case IniContentType.EndSection:
						token = IniToken.Section;
						break;
					case IniContentType.EndComment:
						token = IniToken.Comment;
						break;

					default:
					case IniContentType.Error:
						state = sr.GetState();
						string s = ic.Content.ToString();
						Error = new(sr.ErrorCode, string.Concat("Error at character ", StreamPosition.ToString(CultureInfo.InvariantCulture), " in stream. This is the data where the error was encountered: ", s));
						return new ReadResult(IniToken.Error, s);
				}
			}
			state = sr.GetState();
			// TODO optimize this; we can just have a trim method for the content builder
			string content = token switch
			{
				IniToken.Section => Options.TrimSections ? contentBuilder.ToString().Trim() : contentBuilder.ToString(),
				IniToken.Key => Options.TrimKeys ? contentBuilder.ToString().Trim() : contentBuilder.ToString(),
				IniToken.Value => Options.TrimValues ? contentBuilder.ToString().Trim() : contentBuilder.ToString(),
				_ => contentBuilder.ToString(),
			};
			return new ReadResult(token, content);
		}
		/// <summary>
		/// Disposes of <see cref="Reader"/> if <see cref="LeaveOpen"/> is <see langword="false"/>. Otherwise, does nothing.
		/// </summary>
		public void Dispose()
		{
			if (!LeaveOpen)
			{
				Reader.Dispose();
			}
		}
	}
}
