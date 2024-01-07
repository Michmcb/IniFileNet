namespace IniFileNet.Test
{
	using Xunit;
	public static class ParsedTests
	{
		[Fact]
		public static void CtorProperties()
		{
			IniResult<int> p = new(10, new IniError(IniErrorCode.KeyDelimiterNotFound, "Blash"));
			Assert.Equal(10, p.Value);
			Assert.Equal(IniErrorCode.KeyDelimiterNotFound, p.Error.Code);
			Assert.Equal("Blash", p.Error.Msg);
		}
	}
}