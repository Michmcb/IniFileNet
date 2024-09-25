namespace IniFileNet.Test
{
	using Xunit;
	public static class IniErrorTests
	{
		[Fact]
		public static void ToStringMessage()
		{
			IniError e = new(IniErrorCode.EmptyKeyName, "Test");
			Assert.Equal("EmptyKeyName: Test", e.ToString());
		}
		[Fact]
		public static void ToException()
		{
			IniException ex = new IniError(IniErrorCode.SectionCloseBracketNotFound, "BlahBlah").ToException();
			Assert.Equal(IniErrorCode.SectionCloseBracketNotFound, ex.IniErrorCode);
			Assert.Equal("BlahBlah", ex.Message);

			IniException ex2 = new IniError(IniErrorCode.DuplicateKey, null).ToException();
			Assert.Equal(IniErrorCode.DuplicateKey, ex2.IniErrorCode);
			Assert.Equal("", ex2.Message);
		}
		[Fact]
		public static void Throw()
		{
			IniError e1 = new(IniErrorCode.SectionCloseBracketNotFound, "BlahBlah");
			Assert.Throws<IniException>(e1.ThrowIfError);

			IniError e2 = new(IniErrorCode.None, "BlahBlah");
			e2.ThrowIfError();
		}
	}
}