namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;
	public ref struct IniSpanReaderChecker
	{
		private IniSpanReader reader;
		private int pos;
		public IniSpanReaderChecker(IniSpanReader reader, int pos)
		{
			this.reader = reader;
			this.pos = pos;
		}
		public IniSpanReaderChecker(ReadOnlySpan<char> ini, IniReaderOptions options = default, bool isFinalBlock = true)
		{
			reader = new IniSpanReader(ini, IniSpanReaderState.Init(options), isFinalBlock);
		}
		public IniSpanReaderChecker(ReadOnlySpan<char> ini, int size, IniReaderOptions options = default, bool isFinalBlock = true)
		{
			reader = new IniSpanReader(ini[..size], IniSpanReaderState.Init(options), isFinalBlock);
		}
		public readonly IniReaderOptions Options => reader.Options;
		public readonly int Position => reader.Position;
		public readonly IniSpanReaderChecker NewBlock(ReadOnlySpan<char> fullIni, bool isFinalBlock, int size)
		{
			int pos = reader.Position + this.pos;
			ReadOnlySpan<char> str = fullIni.Length >= pos + size ? fullIni.Slice(pos, size) : fullIni.Slice(pos);
			IniSpanReader newReader = new(str, reader.GetState().NewBlock(default, default, out _), isFinalBlock);
			return new(newReader, pos);
		}
		public void Next(IniContentType type, ReadOnlySpan<char> content)
		{
			Chk.IniContent(type, content, reader.Read());
		}
		public readonly void Error(IniErrorCode errorCode)
		{
			Assert.Equal(errorCode, reader.ErrorCode);
		}
	}
}