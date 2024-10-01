namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;
	public readonly struct IniStreamSectionReaderChecker
	{
		private readonly IniStreamSectionReader readerSync;
		private readonly IniStreamSectionReader readerAsync;
		public IniStreamSectionReaderChecker(string ini, IniReaderOptions options = default)
		{
			readerSync = new IniStreamSectionReader(new IniStreamReader(new StringReader(ini), DefaultIniTextEscaper.Default, options));
			readerAsync = new IniStreamSectionReader(new IniStreamReader(new StringReader(ini), DefaultIniTextEscaper.Default, options));
		}
		public async Task Next(ReadOnlyIniSection expected)
		{
			Assert.True(readerSync.NextSection());
			Assert.True(await readerAsync.NextSectionAsync());
			Check(expected, readerSync.Section);
			Check(expected, readerAsync.Section);
		}
		private static void Check(ReadOnlyIniSection expected, ReadOnlyIniSection actual)
		{
			Assert.Equal(expected.Name, actual.Name);
			Assert.Equal(expected.KeyValues.Count, actual.KeyValues.Count);
			Assert.Equal(expected.Comments.Count, actual.Comments.Count);
			for (int i = 0; i < Math.Min(expected.KeyValues.Count, actual.KeyValues.Count); i++)
			{
				var ekv = expected.KeyValues[i];
				var akv = actual.KeyValues[i];
				Assert.Equal(ekv.Key, akv.Key);
				Assert.Equal(ekv.Value, akv.Value);
				for (int j = 0; j < Math.Min(ekv.Comments.Count, ekv.Comments.Count); j++)
				{
					Assert.Equal(ekv.Comments[j], ekv.Comments[j]);
				}
			}
			for (int i = 0; i < Math.Min(expected.Comments.Count, actual.Comments.Count); i++)
			{
				Assert.Equal(expected.Comments[i], actual.Comments[i]);
			}
		}
		public async Task End()
		{
			var s1 = readerSync.Section;
			var s2 = readerAsync.Section;
			Assert.False(readerSync.NextSection());
			Assert.False(await readerAsync.NextSectionAsync());
			Assert.Same(s1, readerSync.Section);
			Assert.Same(s2, readerAsync.Section);
			Assert.Equal(IniErrorCode.None, readerSync.Reader.Error.Code);
			Assert.Equal(IniErrorCode.None, readerAsync.Reader.Error.Code);
		}
		public async Task Error(IniErrorCode errorCode)
		{
			Assert.False(readerSync.NextSection());
			Assert.False(await readerAsync.NextSectionAsync());
			Assert.Equal(0, readerSync.Section.Name.Length);
			Assert.Equal(0, readerAsync.Section.Name.Length);
			Assert.Empty(readerSync.Section.KeyValues);
			Assert.Empty(readerAsync.Section.KeyValues);
			Assert.Empty(readerSync.Section.Comments);
			Assert.Empty(readerAsync.Section.Comments);
			Assert.Equal(errorCode, readerSync.Reader.Error.Code);
			Assert.Equal(errorCode, readerAsync.Reader.Error.Code);
		}
	}
}