namespace IniFileNet.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using BenchmarkDotNet.Running;
	using IniFileNet.IO;
	using System.Buffers;

	internal class Program
	{
		static void Main(string[] args)
		{
			// We tested and using a switch makes the value acceptors a bit slower
			//BenchmarkRunner.Run<EscapedValue>();
		}
	}

	//[SimpleJob(RuntimeMoniker.Net80)]
	//[RPlotExporter]
	//[MemoryDiagnoser]
	//public class EscapedValue
	//{
	//	/*
	//	| Method                | Mean     | Error    | StdDev   | Allocated |
	//	|---------------------- |---------:|---------:|---------:|----------:|
	//	| CurrentMethod_NoSlash | 16.73 ns | 0.113 ns | 0.106 ns |         - |
	//	| CurrentMethod_Slash   | 15.96 ns | 0.082 ns | 0.077 ns |         - |
	//	| OtherMethod_NoSlash   | 12.08 ns | 0.102 ns | 0.095 ns |         - |
	//	| OtherMethod_Slash     | 56.68 ns | 0.171 ns | 0.143 ns |         - |
	//	 */
	//	private readonly SearchValues<char> backSlash = SearchValues.Create(new char[] { '\\' });
	//	private readonly SearchValues<char> eq = SearchValues.Create(new char[] { '=' });
	//	private readonly SearchValues<char> eqSlash = SearchValues.Create(new char[] { '=', '\\' });
	//	private readonly string NoSlash = "This is a big long key which does not have any slashes in it=Bar";
	//	private readonly string Slash = "This\\\\ is a big long\\; key which\\r has a few \\0escape sequences in it\\==Bar";
	//	[Benchmark]
	//	public IdxEsc CurrentMethod_NoSlash()
	//	{
	//		int position = 0;
	//		ReadOnlySpan<char> block = NoSlash;
	//		bool esc = false;
	//		int off = position;
	//		ReadOnlySpan<char> str = block.Slice(off);
	//		while (true)
	//		{
	//			int i = str.IndexOfAny(eq);
	//			if (i == -1)
	//			{
	//				return new(block.Length, esc);
	//			}
	//			else if (i > 0 && str[i - 1] == '\\')
	//			{
	//				// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
	//				// If there's nothing further, that counts as NOT finding anything
	//				esc = true;
	//				if (i + 1 > str.Length) return new(block.Length, esc);
	//				str = str.Slice(i + 1);
	//				off += i + 1;
	//			}
	//			else
	//			{
	//				if (!esc)
	//				{
	//					esc = block.Slice(position, i).ContainsAny(backSlash);
	//				}
	//				return new(i + off, esc);
	//			}
	//		}
	//	}
	//	[Benchmark]
	//	public IdxEsc CurrentMethod_Slash()
	//	{
	//		int position = 0;
	//		ReadOnlySpan<char> block = Slash;
	//		bool esc = false;
	//		int off = position;
	//		ReadOnlySpan<char> str = block.Slice(off);
	//		while (true)
	//		{
	//			int i = str.IndexOfAny(eq);
	//			if (i == -1)
	//			{
	//				return new(block.Length, esc);
	//			}
	//			else if (i > 0 && str[i - 1] == '\\')
	//			{
	//				// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
	//				// If there's nothing further, that counts as NOT finding anything
	//				esc = true;
	//				if (i + 1 > str.Length) return new(block.Length, esc);
	//				str = str.Slice(i + 1);
	//				off += i + 1;
	//			}
	//			else
	//			{
	//				if (!esc)
	//				{
	//					// Because it's length, it would be "position, i + position - position", so it's simplified to just i
	//					esc = block.Slice(position, i).ContainsAny(backSlash);
	//				}
	//				return new(i + off, esc);
	//			}
	//		}
	//	}
	//	[Benchmark]
	//	public IdxEsc OtherMethod_NoSlash()
	//	{
	//		int position = 0;
	//		ReadOnlySpan<char> block = NoSlash;
	//		bool esc = false;
	//		bool justSeenSlash = false;
	//		int off = position;
	//		ReadOnlySpan<char> str = block.Slice(off);
	//		while (true)
	//		{
	//			int i = str.IndexOfAny(eqSlash);
	//			if (i == -1)
	//			{
	//				return new(block.Length, esc);
	//			}
	//			else if (str[i] == '\\')
	//			{
	//				// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
	//				// If there's nothing further, that counts as NOT finding anything
	//				esc = true;
	//				justSeenSlash = true;
	//				if (i + 1 > str.Length) return new(block.Length, esc);
	//				str = str.Slice(i + 1);
	//				off += i + 1;
	//			}
	//			else
	//			{
	//				// We've just seen a slash and saw a character we're looking for at index 0; thus this character is escaped, so skip it.
	//				if (justSeenSlash && i == 0 && str.Length >= 1)
	//				{
	//					str = str.Slice(1);
	//					off++;
	//					justSeenSlash = false;
	//				}
	//				else
	//				{
	//					return new(i + off, esc);
	//				}
	//			}
	//		}
	//	}
	//	[Benchmark]
	//	public IdxEsc OtherMethod_Slash()
	//	{
	//		int position = 0;
	//		ReadOnlySpan<char> block = Slash;
	//		bool esc = false;
	//		bool justSeenSlash = false;
	//		int off = position;
	//		ReadOnlySpan<char> str = block.Slice(off);
	//		while (true)
	//		{
	//			int i = str.IndexOfAny(eqSlash);
	//			if (i == -1)
	//			{
	//				return new(block.Length, esc);
	//			}
	//			else if (str[i] == '\\')
	//			{
	//				// Escape sequence, so we need to search again, starting from 1 character ahead, if there's more to follow
	//				// If there's nothing further, that counts as NOT finding anything
	//				esc = true;
	//				justSeenSlash = true;
	//				if (i + 1 > str.Length) return new(block.Length, esc);
	//				str = str.Slice(i + 1);
	//				off += i + 1;
	//			}
	//			else
	//			{
	//				// We've just seen a slash and saw a character we're looking for at index 0; thus this character is escaped, so skip it.
	//				if (justSeenSlash && i == 0 && str.Length >= 1)
	//				{
	//					str = str.Slice(1);
	//					off++;
	//					justSeenSlash = false;
	//				}
	//				else
	//				{
	//					return new(i + off, esc);
	//				}
	//			}
	//		}
	//	}
	//}
	[SimpleJob(RuntimeMoniker.Net80)]
	[RPlotExporter]
	[MemoryDiagnoser]
	public class AcceptorTypes
	{
		private readonly IniValueAcceptorOnlyFirst f1 = new();
		private readonly IniValueAcceptorOnlyLast l1 = new();
		private readonly IniValueAcceptorSingle s1 = new();
		[Benchmark]
		public IniError AcceptFirst1()
		{
			return f1.Accept("s", "k", "Value");
		}
		[Benchmark]
		public IniError AcceptLast1()
		{
			return l1.Accept("s", "k", "Value");
		}
		[Benchmark]
		public IniError AcceptSingle1()
		{
			return s1.Accept("s", "k", "Value");
		}
	}
}
