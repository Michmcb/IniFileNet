namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using Xunit;
	public static class IniStreamReaderTests
	{
		[Fact]
		public static void DefaultBufferSize()
		{
			StringReader ss = new("blah=blah");
			using IniStreamReader isr = new (ss, default);
			Assert.Equal(IniStreamReader.DefaultBufferSize, isr.BufferSize);
			Assert.Equal(IniStreamReader.DefaultBufferSize, isr.Buffer.Length);
		}
		[Fact]
		public static void Disposable()
		{
			StringReader ss = new("blah=blah");
			new IniStreamReader(ss, default).Dispose();
			Assert.Throws<ObjectDisposedException>(() => ss.Read());
		}
	}
}