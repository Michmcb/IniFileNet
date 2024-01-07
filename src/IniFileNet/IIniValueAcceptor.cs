namespace IniFileNet
{
	/// <summary>
	/// Accepts a key and value. Typically, this is done to load/parse a value with a specific key.
	/// </summary>
	public interface IIniValueAcceptor
	{
		/// <summary>
		/// Accepts a key and value. If an error occurred during processing, returns the error.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>The error, if any.</returns>
		IniError Accept(string key, string value);
		/// <summary>
		/// Resets the value so this can be re-used.
		/// </summary>
		void Reset();
		/// <summary>
		/// True if this has accepted a value, false otherwise.
		/// </summary>
		bool HaveValue { get; }
	}
}
