namespace IniFileNet.Test
{
	using System.Collections.Generic;
	using Xunit;

	public static class IniValueAcceptorDictionaryBuilderTests
	{
		[Fact]
		public static void AddMethods()
		{
			IniValueAcceptorDictionaryBuilder builder = new([]);
			IniValueAcceptorOnlyFirst onlyfirst1 = builder.OnlyFirst("onlyfirst1");
			IniValueAcceptorOnlyFirst<short> onlyfirst2 = builder.OnlyFirst("onlyfirst2", IniParse.Int16);
			IniValueAcceptorOnlyLast onlylast1 = builder.OnlyLast("onlylast1");
			IniValueAcceptorOnlyLast<int> onlylast2 = builder.OnlyLast("onlylast2", IniParse.Int32);
			IniValueAcceptorSingle single1 = builder.Single("single1");
			IniValueAcceptorSingle<long> single2 = builder.Single("single2", IniParse.Int64);
			IniValueAcceptorMany many1 = builder.Many("many1");
			IniValueAcceptorMany<double, List<double>> many2 = builder.Many<double, List<double>>("many2", IniParse.Double);

			IReadOnlyDictionary<string, IIniValueAcceptor> a = builder.Acceptors;
			Assert.Equal(8, a.Count);
			Assert.Same(onlyfirst1, Assert.Contains("onlyfirst1", a));
			Assert.Same(onlyfirst2, Assert.Contains("onlyfirst2", a));
			Assert.Same(onlylast1, Assert.Contains("onlylast1", a));
			Assert.Same(onlylast2, Assert.Contains("onlylast2", a));
			Assert.Same(single1, Assert.Contains("single1", a));
			Assert.Same(single2, Assert.Contains("single2", a));
			Assert.Same(many1, Assert.Contains("many1", a));
			Assert.Same(many2, Assert.Contains("many2", a));
		}
	}
}