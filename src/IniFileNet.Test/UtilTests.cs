namespace IniFileNet.Test
{
	using Xunit;

	public static class UtilTests
	{
		[Fact]
		public static void ResetAll()
		{
			IniValueAcceptorSingle s1 = new();
			IniValueAcceptorSingle s2 = new();
			IniValueAcceptorSingle s3 = new();
			foreach (var v in new IIniValueAcceptor[] { s1, s2, s3 })
			{
				v.Accept("k", "v");
			}

			Util.ResetAll(s1, s2, s3);
			Assert.Null(s1.Value);
			Assert.Null(s2.Value);
			Assert.Null(s3.Value);
		}
	}
}