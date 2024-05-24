namespace IniFileNet.Test
{
	using System.Collections.Generic;
	using Xunit;

	public static class ReadOnlyIniSectionTests
	{
		[Fact]
		public static void AcceptAll()
		{
			ReadOnlyIniSection sect = new("Test",
			[
				new("Key1", "Value1"),
				new("Key2", "Value2"),
				new("Key3", "Value3"),
			]);
			IniValueAcceptorOnlyLast key1 = new("key");
			IniValueAcceptorOnlyLast key4 = new("key");
			Dictionary<string, IIniValueAcceptor> acceptors = new()
			{
				["Key1"] = key1,
				["Key4"] = key4,
			};
			Assert.Equal(default, sect.AcceptAll(acceptors));
			Assert.Equal("Value1", key1.Value);
			Assert.Null(key4.Value);
		}
		[Fact]
		public static void AcceptAllDuplicate()
		{
			ReadOnlyIniSection sect = new("Test",
			[
				new("Key", "Value"),
				new("Key", "Value"),
			]);
			IniValueAcceptorSingle key = new("Key");
			Dictionary<string, IIniValueAcceptor> acceptors = new()
			{
				["Key"] = key,
			};
			Assert.Equal(new IniError(IniErrorCode.ValueAlreadyPresent, "Already accepted a value. Key: \"Key\". Value is: \"Value\""), sect.AcceptAll(acceptors));
		}
	}
}