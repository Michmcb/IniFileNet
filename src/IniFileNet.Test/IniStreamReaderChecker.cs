namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.IO;
	using Xunit;
	public readonly struct IniStreamReaderChecker
	{
		private readonly IniStreamReader reader;
		private readonly IniStreamReader readerAsync;
		public IniStreamReaderChecker(string ini, IniReaderOptions options = default)
		{
			reader = new(new StringReader(ini), options);
			readerAsync = new(new StringReader(ini), options);
		}
		public void Next(IniToken token, string content)
		{
			{
				var actual = reader.Read();
				Assert.Equal(token, actual.Token);
				Assert.Equal(content, actual.Content);
			}
			{
				// HACK So evil....we're doing this because we test the span reader in the same method as the streams and you can't have ref structs in async tasks. Eventually we'll break those into different methods.
				var actual = readerAsync.ReadAsync().Result;
				Assert.Equal(token, actual.Token);
				Assert.Equal(content, actual.Content);
			}
		}
		public void Error(IniErrorCode errorCode)
		{
			Assert.Equal(errorCode, reader.Error.Code);
		}
	}
}