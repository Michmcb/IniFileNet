namespace IniFileNet.Test
{
	using Xunit;

	public static class CommentedValueTests
	{
		[Fact]
		public static void EmptyComments()
		{
			CommentedValue cv = new("Foo");
			Assert.Equal("Foo", cv.Value);
			Assert.Empty(cv.Comments);
		}
	}
}