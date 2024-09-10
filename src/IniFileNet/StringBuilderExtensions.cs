namespace IniFileNet
{
	using System.Text;

	/// <summary>
	/// Extensions for <see cref="StringBuilder"/>
	/// </summary>
	public static class StringBuilderExtensions
	{
		/// <summary>
		/// Converts <paramref name="sb"/> to a string, with all leading and trailing whitespace characters removed.
		/// </summary>
		/// <param name="sb"></param>
		/// <returns></returns>
		public static string ToStringTrimmed(this StringBuilder sb)
		{
			int end;
			int start;
			for (start = 0; start < sb.Length; start++)
			{
				if (!char.IsWhiteSpace(sb[start])) break;
			}
			for (end = sb.Length - 1; end >= start; end--)
			{
				if (!char.IsWhiteSpace(sb[end])) break;
			}
			return sb.ToString(start, (end - start) + 1);
		}
	}
}
