namespace IniFileNet.Test
{
	using Xunit;

	public static class IniResultTests
	{
		[Fact]
		public static void Ok()
		{
			{
				IniResult<int> iniResultOk = new(20, new IniError(IniErrorCode.None, null));
				Assert.True(iniResultOk.Ok(out int value, out IniError error));
				Assert.Equal(20, value);
				Chk.IniError(IniErrorCode.None, null, error);
			}
			{
				IniResult<int> iniResult = new(default, new IniError(IniErrorCode.EmptySectionName, "Foo"));;
				Assert.False(iniResult.Ok(out int value, out IniError error));
				Assert.Equal(default, value);
				Chk.IniError(IniErrorCode.EmptySectionName, "Foo", error);
			}
		}
		[Fact]
		public static void ValueOrException()
		{
			IniResult<int> iniResultOk = new(20, new IniError(IniErrorCode.None, null));
			Assert.Equal(20, iniResultOk.ValueOrException());
			IniResult<int> iniResultBad = new(default, new IniError(IniErrorCode.ValueInvalid, "Foo"));
			var ex = Assert.ThrowsAny<IniException>(() => iniResultBad.ValueOrException());
			Assert.Equal(IniErrorCode.ValueInvalid, ex.IniErrorCode);
			Assert.Equal("Foo", ex.Message);
		}
	}
}