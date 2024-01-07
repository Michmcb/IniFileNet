namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using Xunit;

	public static class IniStreamSectionReaderTests
	{
		[Fact]
		public static void Disposable()
		{
			StringReader ss = new("blah=blah");
			new IniStreamSectionReader(new IniStreamReader(ss, default)).Dispose();
			Assert.Throws<ObjectDisposedException>(() => ss.Read());
		}
	}
}