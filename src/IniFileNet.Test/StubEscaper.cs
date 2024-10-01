namespace IniFileNet.Test
{
	using IniFileNet;
	using System;
	using System.Buffers;

	public sealed class StubEscaper : IIniTextEscaper
	{
		private readonly IniTokenContext allow;
		private readonly string? errMsg;
		public StubEscaper(IniTokenContext allow, string? errMsg)
		{
			this.allow = allow;
			this.errMsg = errMsg;
		}
		public OperationStatusMsg Escape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock)
		{
			charsConsumed = 0;
			charsWritten = 0;
			return allow == tokenContext
				? new(OperationStatus.Done, null)
				: new(OperationStatus.InvalidData, errMsg);
		}
		public OperationStatusMsg Unescape(ReadOnlySpan<char> text, Span<char> destination, IniTokenContext tokenContext, out int charsConsumed, out int charsWritten, bool isFinalBlock)
		{
			charsConsumed = 0;
			charsWritten = 0;
			return allow == tokenContext
				? new(OperationStatus.Done, null)
				: new(OperationStatus.InvalidData, errMsg);
		}
	}
}