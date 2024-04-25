namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.IO;
	using System.Threading.Tasks;
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
		public async Task Next(IniToken token, string content)
		{
			{
				var actual = reader.Read();
				Assert.Equal(token, actual.Token);
				Assert.Equal(content, actual.Content);
			}
			{
				var actual = await readerAsync.ReadAsync();
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