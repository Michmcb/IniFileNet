namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;
	public ref struct IniSpanReaderChecker
	{
		private IniSpanReader reader;
		public IniSpanReaderChecker(IniSpanReader reader)
		{
			this.reader = reader;
		}
		public readonly IniReaderOptions Options => reader.Options;
		public IniSpanReaderChecker(string ini, IniReaderOptions options = default, bool isFinalBlock = true)
		{
			reader = new IniSpanReader(ini, IniSpanReaderState.Init(options), isFinalBlock);
		}
		public readonly IniSpanReaderChecker NewBlock(string ini, bool isFinalBlock)
		{
			IniSpanReader newReader = new(ini, reader.GetState().NewBlock(default, default, out _), isFinalBlock);
			return new(newReader);
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