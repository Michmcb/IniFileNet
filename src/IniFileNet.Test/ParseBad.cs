namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using Xunit;

	public static class ParseBad
	{
		[Fact]
		public static void EmptyKeyEquals()
		{
			string ini = "=";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.Error, "=");
			c.Next(IniContentType.Error, "=");
			c.Error(IniErrorCode.EmptyKeyName);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, "=");
			c1.Error(IniErrorCode.EmptyKeyName);

			c2.Error(IniErrorCode.EmptyKeyName);
		}
		[Fact]
		public static void EmptyKeyColon()
		{
			string ini = ":";
			IniSpanReaderChecker c = new(ini, new IniReaderOptions(allowKeyDelimiterColon: true));
			c.Next(IniContentType.Error, ":");
			c.Next(IniContentType.Error, ":");
			c.Error(IniErrorCode.EmptyKeyName);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, ":");
			c1.Error(IniErrorCode.EmptyKeyName);

			c2.Error(IniErrorCode.EmptyKeyName);
		}
		[Fact]
		public static void KeyNoEquals()
		{
			string ini = "[s]\nKey";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "s");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Key");
			c.Next(IniContentType.Error, "Key");
			c.Error(IniErrorCode.KeyDelimiterNotFound);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "s");
			c1.Next(IniToken.Error, "Key");
			c1.Next(IniToken.Error, "Key");
			c1.Error(IniErrorCode.KeyDelimiterNotFound);

			c2.Error(IniErrorCode.KeyDelimiterNotFound);
		}
		[Fact]
		public static void SectionNotOnOwnLine()
		{
			string ini = "[Section]Key=Value";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.Error, "Key=Value");
			c.Next(IniContentType.Error, "Key=Value");
			c.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Error, "Key=Value");
			c1.Next(IniToken.Error, "Key=Value");
			c1.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);

			c2.Error(IniErrorCode.SectionIsNotOnlyThingOnLine);
		}
		[Fact]
		public static void SemicolonInKeyName()
		{
			string ini = "[Section]\nKe;y=Value";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Section, "Section");
			c.Next(IniContentType.EndSection, "]");
			c.Next(IniContentType.StartKey, default);
			c.Next(IniContentType.Error, "Ke;y=Value");
			c.Next(IniContentType.Error, "Ke;y=Value");
			c.Error(IniErrorCode.SemicolonInKeyName);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Section, "Section");
			c1.Next(IniToken.Error, "Ke;y=Value");
			c1.Next(IniToken.Error, "Ke;y=Value");
			c1.Error(IniErrorCode.SemicolonInKeyName);

			c2.Error(IniErrorCode.SemicolonInKeyName);
		}
		[Fact]
		public static void GlobalKeysNotAllowed()
		{
			string ini = "Key=Value";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.Error, "Key=Value");
			c.Next(IniContentType.Error, "Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, "Key=Value");
			c1.Next(IniToken.Error, "Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static void GlobalKeysNotAllowedNumbersign()
		{
			string ini = "#Key=Value";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.Error, "#Key=Value");
			c.Next(IniContentType.Error, "#Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, "#Key=Value");
			c1.Next(IniToken.Error, "#Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static void GlobalKeysNotAllowedColon()
		{
			string ini = ":Key=Value";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.Error, ":Key=Value");
			c.Next(IniContentType.Error, ":Key=Value");
			c.Error(IniErrorCode.GlobalKeyNotAllowed);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, ":Key=Value");
			c1.Next(IniToken.Error, ":Key=Value");
			c1.Error(IniErrorCode.GlobalKeyNotAllowed);

			c2.Error(IniErrorCode.GlobalKeyNotAllowed);
		}
		[Fact]
		public static void SectionEndOfSpan1()
		{
			string ini = "[";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "");
			c.Next(IniContentType.Error, "");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			c2.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
		[Fact]
		public static void SectionEndOfSpan2()
		{
			string ini = "[Section";
			IniSpanReaderChecker c = new(ini);
			c.Next(IniContentType.StartSection, "[");
			c.Next(IniContentType.Error, "Section");
			c.Next(IniContentType.Error, "Section");
			c.Error(IniErrorCode.SectionCloseBracketNotFound);

			var (c1, c2) = Checks.For(ini, c.Options);
			c1.Next(IniToken.Error, "");
			c1.Error(IniErrorCode.SectionCloseBracketNotFound);

			c2.Error(IniErrorCode.SectionCloseBracketNotFound);
		}
	}
}