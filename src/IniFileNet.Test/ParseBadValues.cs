namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.Threading.Tasks;
	using Xunit;
	public static class ParseBadValues
	{
		public const string BadValueEscapeSequenceIni = "Foo=B\\xar";
		public static readonly IniReaderOptions BadValueEscapeSequenceOpt = new(allowGlobalKeys: true);
		[Fact]
		public static async Task BadValueEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(BadValueEscapeSequenceIni, BadValueEscapeSequenceOpt);
			await c1.Next(IniToken.Key, "Foo");
			await c1.Next(IniToken.Error, "Error unescaping at char 4 in stream, char 4 in block. Invalid escape sequence at index 1 of text:B\\xar");
			c1.Error(IniErrorCode.InvalidEscapeSequence);

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(BadValueEscapeSequenceIni, BadValueEscapeSequenceOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Invalid escape sequence at index 1 of text:B\\xar"), []);
		}
		public const string TrailingSlashEscapeSequenceIni = "Key=Value\\";
		public static readonly IniReaderOptions TrailingSlashEscapeSequenceOpt = new(allowGlobalKeys: true);
		[Fact]
		public static void TrailingSlashEscapeSequenceSpan()
		{
			// TODO we should allow trailing slashes in some cases, because they might be a legit value, such as a windows path.
			IniSpanReaderChecker c = new(TrailingSlashEscapeSequenceIni, TrailingSlashEscapeSequenceOpt);
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Key, "Key");
			c.Next(IniContentType.EndKey, "=");
			c.Next(IniContentType.StartValue, default);
			c.Next(IniContentType.Error, "Key=Value\\");
			c.Next(IniContentType.Error, "Key=Value\\");
		}
		[Fact]
		public static async Task TrailingSlashEscapeSequenceStream()
		{
			var (c1, c2) = Checks.For(TrailingSlashEscapeSequenceIni, TrailingSlashEscapeSequenceOpt);
			await c1.Next(IniToken.Key, "Key");
			await c1.Next(IniToken.Error, "\\");
			await c1.Next(IniToken.Error, "\\");

			await c2.Error(IniErrorCode.InvalidEscapeSequence);

			await Chk.CheckAllIniDictionaryReader(TrailingSlashEscapeSequenceIni, TrailingSlashEscapeSequenceOpt, new IniError(IniErrorCode.InvalidEscapeSequence, "Error at char 10 in stream, char 0 in block. This is the block in which the error was encountered:\\"), []);
		}
	}
}