namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using System.Collections.Generic;
	using Xunit;

	public static class IniDictionaryReaderTests
	{
		private static readonly ReadOnlyMemory<char> delim = ".".AsMemory();
		[Fact]
		public static void StringOnlyLastTest()
		{
			Dictionary<string, string> dict = [];
			IDictionary<string, string> d = dict;
			IniDictionaryReader.StringOnlyLast(dict,"", "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value1", Assert.Contains("key", d));
			IniDictionaryReader.StringOnlyLast(dict, "", "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value2", Assert.Contains("key", d));
		}
		[Fact]
		public static void StringOnlyFirstTest()
		{
			Dictionary<string, string> dict = [];
			IDictionary<string, string> d = dict;
			IniDictionaryReader.StringOnlyFirst(dict, "", "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value1", Assert.Contains("key", d));
			IniDictionaryReader.StringOnlyFirst(dict, "", "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value1", Assert.Contains("key", d));
		}
		[Fact]
		public static void StringSingleTest()
		{
			Dictionary<string, string> dict = [];
			IDictionary<string, string> d = dict;
			IniDictionaryReader.StringSingle(dict, "", "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value1", Assert.Contains("key", d));
			Chk.IniError(IniErrorCode.ValueAlreadyPresent, "Section & key already present. Full Key: \"key\". Value is: \"value2\"", IniDictionaryReader.StringSingle(dict, "", "key", delim, "value2", ["section comments"], ["kv comments"]));
			Assert.Equal("value1", Assert.Contains("key", d));
		}
		[Fact]
		public static void StringManyTest()
		{
			Dictionary<string, List<string>> dict = [];
			IDictionary<string, List<string>> d = dict;
			IniDictionaryReader.StringMany(dict, "", "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Equal("value1", Assert.Single(Assert.Contains("key", d)));
			IniDictionaryReader.StringMany(dict, "", "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
			Assert.Collection(Assert.Contains("key", d),
			[
				x => Assert.Equal("value1", x),
				x => Assert.Equal("value2", x),
			]);
		}
		//[Fact]
		//public static void IniValueOnlyLastTest()
		//{
		//	Dictionary<string, IniValue<string>> dict = [];
		//	IDictionary<string, IniValue<string>> d = dict;
		//	IniDictionaryReader.IniValueOnlyLast(dict, "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//	IniDictionaryReader.IniValueOnlyLast(dict, "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value2", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//}
		//[Fact]
		//public static void IniValueOnlyFirstTest()
		//{
		//	Dictionary<string, IniValue<string>> dict = [];
		//	IDictionary<string, IniValue<string>> d = dict;
		//	IniDictionaryReader.IniValueOnlyFirst(dict, "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//	IniDictionaryReader.IniValueOnlyFirst(dict, "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//}
		//[Fact]
		//public static void IniValueSingleTest()
		//{
		//	Dictionary<string, IniValue<string>> dict = [];
		//	IDictionary<string, IniValue<string>> d = dict;
		//	IniDictionaryReader.IniValueSingle(dict, "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//	Chk.IniError(IniErrorCode.ValueAlreadyPresent, "Section & key already present. Full Key: \"key\". Value is: \"value2\"", IniDictionaryReader.IniValueSingle(dict, "key", delim, "value2", ["section comments"], ["kv comments"]));
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Contains("key", d));
		//}
		//[Fact]
		//public static void IniValueManyTest()
		//{
		//	Dictionary<string, List<IniValue<string>>> dict = [];
		//	IDictionary<string, List<IniValue<string>>> d = dict;
		//	IniDictionaryReader.IniValueMany(dict, "key", delim, "value1", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Chk.IniValue(delim, "value1", ["section comments"], ["kv comments"], Assert.Single(Assert.Contains("key", d)));
		//	IniDictionaryReader.IniValueMany(dict, "key", delim, "value2", ["section comments"], ["kv comments"]).ThrowIfError();
		//	Assert.Collection(Assert.Contains("key", d),
		//	[
		//		x => Assert.Equal("value1", x),
		//		x => Assert.Equal("value2", x),
		//	]);
		//}
	}
}