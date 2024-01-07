namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.IO;
	using Xunit;
	public readonly struct IniStreamSectionReaderChecker
	{
		private readonly IniStreamSectionReader reader;
		public IniStreamSectionReaderChecker(string ini, IniReaderOptions options = default)
		{
			reader = new IniStreamSectionReader(new IniStreamReader(new StringReader(ini), options));
		}
		public void Next(ReadOnlyIniSection expected)
		{
			Assert.True(reader.TryReadNext(out ReadOnlyIniSection? actual));
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
		public void End()
		{
			Assert.False(reader.TryReadNext(out _));
			Assert.Equal(IniErrorCode.None, reader.Reader.Error.Code);
		}
		public void Error(IniErrorCode errorCode)
		{
			Assert.False(reader.TryReadNext(out _));
			Assert.Equal(errorCode, reader.Reader.Error.Code);
		}
	}
}