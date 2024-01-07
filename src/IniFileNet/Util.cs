namespace IniFileNet
{
	using System.Collections.Generic;

	/// <summary>
	/// Utility functions.
	/// </summary>
	public static class Util
	{
		/// <summary>
		/// Calls <see cref="IIniValueAcceptor.Reset"/> on all <paramref name="acceptors"/>.
		/// </summary>
		public static void ResetAll(params IIniValueAcceptor[] acceptors)
		{
			ResetAll((IEnumerable<IIniValueAcceptor>)acceptors);
		}
		/// <summary>
		/// Calls <see cref="IIniValueAcceptor.Reset"/> on all <paramref name="acceptors"/>.
		/// </summary>
		public static void ResetAll(IEnumerable<IIniValueAcceptor> acceptors)
		{
			foreach (var acceptor in acceptors) { acceptor.Reset(); }
		}
	}
}
