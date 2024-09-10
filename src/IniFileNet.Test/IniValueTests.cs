namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System.IO;
	using System.Threading.Tasks;
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