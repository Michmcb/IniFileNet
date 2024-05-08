#if NETSTANDARD2_0
namespace IniFileNet.IO
{
	using System;
	using System.IO;

	internal static class TextWriterExtensions
	{
		internal static void Write(this TextWriter tw, ReadOnlySpan<char> str)
		{
			tw.Write(str.ToString());
		}
	}
}
#endif