namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;
	using Xunit.Sdk;

	public static class Chk
	{
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