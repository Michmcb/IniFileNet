namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using Xunit;

	public static class SyntaxTests
	{
		[Fact]
		public static void IsLegalKey()
		{
			Assert.False(Syntax.IsLegalKey(""));
			Assert.False(Syntax.IsLegalKey("   "));
			Assert.False(Syntax.IsLegalKey(";"));
			Assert.False(Syntax.IsLegalKey("#"));
			Assert.False(Syntax.IsLegalKey("="));
			Assert.False(Syntax.IsLegalKey(":"));
			Assert.False(Syntax.IsLegalKey("\r"));
			Assert.False(Syntax.IsLegalKey("\n"));
			Assert.True(Syntax.IsLegalKey("key"));
		}
		[Fact]
		public static void IsLegalSectionName()
		{
			Assert.False(Syntax.IsLegalSectionName(""));
			Assert.False(Syntax.IsLegalSectionName(";"));
			Assert.False(Syntax.IsLegalSectionName("#"));
			Assert.False(Syntax.IsLegalSectionName("["));
			Assert.False(Syntax.IsLegalSectionName("]"));
			Assert.False(Syntax.IsLegalSectionName("\r"));
			Assert.False(Syntax.IsLegalSectionName("\n"));
			Assert.True(Syntax.IsLegalSectionName("section"));
		}
	}
}