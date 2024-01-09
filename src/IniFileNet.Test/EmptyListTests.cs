namespace IniFileNet.Test
{
	using System;
	using System.Collections.Generic;
	using Xunit;
	public static class EmptyListTests
	{
		[Fact]
		public static void Indexing()
		{
			EmptyList<int> x = EmptyList.Get<int>();
			Assert.Throws<IndexOutOfRangeException>(() => x[0]);
			IList<int> y = x;
			Assert.Throws<IndexOutOfRangeException>(() => y[0] = 1);
			Assert.Throws<IndexOutOfRangeException>(() => y[0]);
			Assert.Throws<IndexOutOfRangeException>(() => x.RemoveAt(0));
		}
		[Fact]
		public static void Properties()
		{
			EmptyList<int> x = EmptyList.Get<int>();
			Assert.Empty(x);
			Assert.True(x.IsReadOnly);
		}
		[Fact]
		public static void StuffThatDoesNothing()
		{
			EmptyList<int> x = EmptyList.Get<int>();
			x.Add(0);
			x.Insert(0, 0);
			Assert.False(x.Remove(0));
			Assert.DoesNotContain(0, x);
			Assert.Equal(-1, x.IndexOf(0));
			x.Clear();
			int[] y = [1];
			x.CopyTo(y, 0);
			Assert.Equal(1, y[0]);
		}
	}
}