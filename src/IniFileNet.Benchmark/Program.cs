﻿namespace IniFileNet.Benchmark
{
	using BenchmarkDotNet.Attributes;
	using BenchmarkDotNet.Jobs;
	using BenchmarkDotNet.Running;

	internal class Program
	{
		static void Main(string[] args)
		{
			// We tested and using a switch makes the value acceptors a bit slower
			BenchmarkRunner.Run<AcceptorTypes>();
		}
	}
	[SimpleJob(RuntimeMoniker.Net80)]
	[RPlotExporter]
	[MemoryDiagnoser]
	public class AcceptorTypes
	{
		private readonly IniValueAcceptorOnlyFirst f1 = new("key");
		private readonly IniValueAcceptorOnlyLast l1 = new("key");
		private readonly IniValueAcceptorSingle s1 = new("key");
		[Benchmark]
		public IniError AcceptFirst1()
		{
			return f1.Accept("Value");
		}
		[Benchmark]
		public IniError AcceptLast1()
		{
			return l1.Accept("Value");
		}
		[Benchmark]
		public IniError AcceptSingle1()
		{
			return s1.Accept("Value");
		}
	}
}
