namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;

	public static class Chk
	{
		public static void IniResult<T>(T expected, IniErrorCode code, IniResult<T> actual)
		{
			Assert.Equal(expected, actual.Value);
			Assert.Equal(code, actual.Error.Code);
		}
		public static void IniResult<T>(T expected, IniErrorCode code, string? msg, IniResult<T> actual)
		{
			Assert.Equal(expected, actual.Value);
			IniError(code, msg, actual.Error);
		}
		public static void IniError(IniErrorCode code, string? msg, IniError actual)
		{
			Assert.Equal(code, actual.Code);
			Assert.Equal(msg, actual.Msg);
		}
		public static async Task CheckAllIniDictionaryReader(string ini, IniReaderOptions opt, IniError expectedError, Action<KeyValuePair<string, string>>[] elementInspectors)
		{
			foreach (AddDictionaryValue<string> func in StringLastFirstSingleDelegates())
			{
				{
					IniDictionaryReader<string> readerSync = new();
					IniError actualError = readerSync.Load(new(new StringReader(ini), null, opt), func);
					Assert.Equal(expectedError.Code, actualError.Code);
					Assert.Equal(expectedError.Msg, actualError.Msg);
					Assert.Collection(readerSync.Dictionary, elementInspectors);
				}
				{
					IniDictionaryReader<string> readerAsync = new();
					IniError actualError = await readerAsync.LoadAsync(new(new StringReader(ini), null, opt), func);
					Assert.Equal(expectedError.Code, actualError.Code);
					Assert.Equal(expectedError.Msg, actualError.Msg);
					Assert.Collection(readerAsync.Dictionary, elementInspectors);
				}
			}
		}
		//public static async Task CheckAllIniDictionaryReader(string ini, IniReaderOptions opt, IniError expectedError, Action<KeyValuePair<string, IniValue<string>>>[] elementInspectors)
		//{
		//	foreach (AddDictionaryValue<IniValue<string>> func in IniValueLastFirstSingleDelegates())
		//	{
		//		{
		//			IniDictionaryReader<IniValue<string>> readerSync = new();
		//			IniError actualError = readerSync.Load(new(new StringReader(ini), opt), func);
		//			Assert.Equal(expectedError.Code, actualError.Code);
		//			Assert.Equal(expectedError.Msg, actualError.Msg);
		//			Assert.Collection(readerSync.Dictionary, elementInspectors);
		//		}
		//		{
		//			IniDictionaryReader<IniValue<string>> readerAsync = new();
		//			IniError actualError = await readerAsync.LoadAsync(new(new StringReader(ini), opt), func);
		//			Assert.Equal(expectedError.Code, actualError.Code);
		//			Assert.Equal(expectedError.Msg, actualError.Msg);
		//			Assert.Collection(readerAsync.Dictionary, elementInspectors);
		//		}
		//	}
		//}
		public static AddDictionaryValue<string>[] StringLastFirstSingleDelegates()
		{
			return [IniDictionaryReader.StringOnlyLast, IniDictionaryReader.StringOnlyFirst, IniDictionaryReader.StringSingle];
		}
		//public static AddDictionaryValue<IniValue<string>>[] IniValueLastFirstSingleDelegates()
		//{
		//	return [IniDictionaryReader.IniValueOnlyLast, IniDictionaryReader.IniValueOnlyFirst, IniDictionaryReader.IniValueSingle];
		//}
		public static void IniValueKvp(string expectedKey, string expectedValue, string[] expectedComments, KeyValuePair<string, IniValue<string>> kvp)
		{
			Assert.Equal(expectedKey, kvp.Key);
			IniValue(expectedValue, expectedComments, kvp.Value);
		}
		public static void IniValue(string expectedValue, string[] expectedComments, IniValue<string> actual)
		{
			Assert.Equal(expectedValue, actual.Value);
			Assert.Equal(expectedComments.Length, actual.Comments.Count);
			for (int i = 0; i < expectedComments.Length; i++)
			{
				Assert.Equal(expectedComments[i], actual.Comments[i]);
			}
		}
		public static void IniContent(IniContentType type, ReadOnlySpan<char> content, IniContent actual)
		{
			Assert.Equal(type, actual.Type);
			Assert.Equal(content, actual.Content);
		}
	}
}