namespace IniFileNet.Test
{
	using Xunit;

	public static class IniValueTests
	{
		[Fact]
		public static void EmptyComments()
		{
			IniValue<string> cv = new("Foo");
			Assert.Equal("Foo", cv.Value);
			Assert.Empty(cv.Comments);
		}
	}
}