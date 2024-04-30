namespace IniFileNet.Test
{
	using System;
	using Xunit;

	public static class IniExceptionTests
	{
		[Fact]
		public static void Constructors()
		{
			IniException ex1 = new(IniErrorCode.ValueInvalid);
			Assert.Equal(IniErrorCode.ValueInvalid, ex1.IniErrorCode);
			Assert.Null(ex1.InnerException);

			IniException ex2 = new(IniErrorCode.DuplicateKey, "Some message");
			Assert.Equal(IniErrorCode.DuplicateKey, ex2.IniErrorCode);
			Assert.Equal("Some message", ex2.Message);
			Assert.Null(ex2.InnerException);

			Exception exInner = new("Foo");
			IniException ex3 = new(IniErrorCode.EmptySectionName, "Some message", exInner);
			Assert.Equal(IniErrorCode.EmptySectionName, ex3.IniErrorCode);
			Assert.Equal("Some message", ex3.Message);
			Assert.Same(exInner, ex3.InnerException);
		}
	}
}