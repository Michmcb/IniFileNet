namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Threading.Tasks;
	using Xunit;
	using Xunit.Sdk;

	public static class Chk
	{
		public static void IniError(IniErrorCode code, string? msg, IniError actual)
		{
			Assert.Equal(code, actual.Code);
			Assert.Equal(msg, actual.Msg);
		}
		public static async Task CheckAllIniDictionaryReader(string ini, IniReaderOptions opt, IniError expectedError, Action<KeyValuePair<string, CommentedValue>>[] elementInspectors)
		{
			foreach ((IniDictionaryReader reader, Func<IniStreamReader, IniError> func) in GetReaderLastFirstSingle())
			{
				var actualError = func(new(new StringReader(ini), opt));
				Assert.Equal(expectedError.Code, actualError.Code);
				Assert.Equal(expectedError.Msg, actualError.Msg);
				Assert.Collection(reader.Dictionary, elementInspectors);
			}
			foreach ((IniDictionaryReader reader, Func<IniStreamReader, Task<IniError>> func) in GetReaderLastFirstSingleAsync())
			{
				var actualError = await func(new(new StringReader(ini), opt));
				Assert.Equal(expectedError.Code, actualError.Code);
				Assert.Equal(expectedError.Msg, actualError.Msg);
				Assert.Collection(reader.Dictionary, elementInspectors);
			}
		}
		public static(IniDictionaryReader reader, Func<IniStreamReader, Task<IniError>> func)[] GetReaderLastFirstSingleAsync()
		{
			IniDictionaryReader r1 = new();
			IniDictionaryReader r2 = new();
			IniDictionaryReader r3 = new();
			return [(r1, r1.LoadOnlyLastValuesAsync), (r2, r2.LoadOnlyFirstValuesAsync), (r3, r3.LoadSingleValuesAsync)];
		}
		public static (IniDictionaryReader reader, Func<IniStreamReader, IniError> func)[] GetReaderLastFirstSingle()
		{
			IniDictionaryReader r1 = new();
			IniDictionaryReader r2 = new();
			IniDictionaryReader r3 = new();
			return [(r1, r1.LoadOnlyLastValues), (r2, r2.LoadOnlyFirstValues), (r3, r3.LoadSingleValues),];
		}
		public static void CommentedValueKvp(string expectedKey, string expectedValue, string[] expectedComments, KeyValuePair<string, CommentedValue> kvp)
		{
			Assert.Equal(expectedKey, kvp.Key);
			CommentedValue(expectedValue, expectedComments, kvp.Value);
		}
		public static void CommentedValue(string expectedValue, string[] expectedComments, CommentedValue actual)
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
			SpanEqual(content, actual.Content);
		}
		public static void SpanEqual(ReadOnlySpan<char> expected, ReadOnlySpan<char> actual)
		{
			int i = 0;
			for (; i < Math.Min(expected.Length, actual.Length); i++)
			{
				if (expected[i] != actual[i])
				{
					break;
				}
			}
			if (i == expected.Length && i == actual.Length)
			{
				return;
			}
			throw new EqualException(new string(expected), new string(actual), i, i);
		}
	}
}