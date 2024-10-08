﻿namespace IniFileNet.Test
{
	using IniFileNet.IO;
	public static class Checks
	{
		public static (IniStreamReaderChecker c1, IniStreamSectionReaderChecker c2) For(string ini, IniReaderOptions options = default)
		{
			return (new IniStreamReaderChecker(ini, options), new IniStreamSectionReaderChecker(ini, options));
		}
		public static (IniStreamReaderChecker c1, IniStreamSectionReaderChecker c2) For(string ini, int bufferSize, IniReaderOptions options = default)
		{
			return (new IniStreamReaderChecker(ini, bufferSize, options), new IniStreamSectionReaderChecker(ini, options));
		}
	}
}