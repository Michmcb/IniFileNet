namespace IniFileNet.Test
{
	using IniFileNet.IO;
	using System;
	using Xunit;

	public static class BufferWriterTests
	{
		[Fact]
		public static void NoSizeHint()
		{
			BufferWriter<byte> writer = new(1024);
			var s = writer.GetSpan();
			Assert.Equal(1024, s.Length);
		}
		[Fact]
		public static void GeneralBehaviour()
		{
			BufferWriter<byte> writer = new(1024);
			Assert.Equal(1024, writer.Capacity);
			Assert.Equal(1024, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);
			writer.Advance(16);

			Assert.Equal(1024, writer.Capacity);
			Assert.Equal(1008, writer.Free);
			Assert.Equal(16, writer.Written);
			Assert.Equal(16, writer.Span.Length);
			Assert.Equal(16, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(0, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(0, b);

			writer.SetWritten(0);
			Assert.Equal(1024, writer.Capacity);
			Assert.Equal(1024, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(0, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(0, b);

			Span<byte> s = writer.GetSpan(16);
			s.Fill(1);
			writer.Advance(16);
			Assert.Equal(1008, writer.Free);
			Assert.Equal(16, writer.Written);
			Assert.Equal(16, writer.Span.Length);
			Assert.Equal(16, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(1, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(1, b);

			writer.Clear();
			Assert.Equal(1024, writer.Capacity);
			Assert.Equal(1024, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(0, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(0, b);

			Memory<byte> m = writer.GetMemory(16);
			m.Span.Fill(1);
			writer.Advance(16);
			Assert.Equal(1008, writer.Free);
			Assert.Equal(16, writer.Written);
			Assert.Equal(16, writer.Span.Length);
			Assert.Equal(16, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(1, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(1, b);

			writer.Clear();
			Assert.Equal(1024, writer.Capacity);
			Assert.Equal(1024, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);
			foreach (byte b in writer.Span) Assert.Equal(0, b);
			foreach (byte b in writer.Memory.Span) Assert.Equal(0, b);

			// The capacity doesn't have to be exactly 2048, just at LEAST 2048
			writer.EnsureCapacity(2048);
			Assert.True(writer.Capacity >= 2048);
			Assert.Equal(writer.Capacity, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);

			// We should be able to allocate up to Array.MaxLength in size, but no more than that
			writer.EnsureCapacity(Array.MaxLength);
			Assert.Equal(Array.MaxLength, writer.Capacity);
			Assert.Equal(writer.Capacity, writer.Free);
			Assert.Equal(0, writer.Written);
			Assert.Equal(0, writer.Span.Length);
			Assert.Equal(0, writer.Memory.Length);
		}
		[Fact]
		public static void Exceptions()
		{
			Assert.Throws<ArgumentOutOfRangeException>(() => new BufferWriter<byte>(0));
			Assert.Throws<ArgumentOutOfRangeException>(() => new BufferWriter<byte>(-10));
			BufferWriter<byte> writer = new(16);
			Assert.Throws<ArgumentOutOfRangeException>(() => writer.SetWritten(-1));
			Assert.Throws<ArgumentOutOfRangeException>(() => writer.SetWritten(256));
			Assert.Throws<ArgumentOutOfRangeException>(() => writer.Advance(-1));
			Assert.Throws<InvalidOperationException>(() => writer.Advance(256));
			Assert.Throws<ArgumentOutOfRangeException>(() => writer.EnsureCapacity(-1));
			Assert.Throws<OutOfMemoryException>(() => writer.EnsureCapacity(int.MaxValue));
		}
	}
}