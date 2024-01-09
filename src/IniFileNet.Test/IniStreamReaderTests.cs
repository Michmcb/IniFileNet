namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using Xunit;
	public static class IniStreamReaderTests
	{
		[Fact]
		public static void Disposable()
		{
			StringReader ss = new("blah=blah");
			new IniStreamReader(ss, default).Dispose();
			Assert.Throws<ObjectDisposedException>(() => ss.Read());
		}
	}
}