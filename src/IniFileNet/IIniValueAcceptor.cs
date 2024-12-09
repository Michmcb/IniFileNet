namespace IniFileNet
{
	/// <summary>
	/// Accepts a key and value. Typically, this is done to load/parse a value with a specific key.
	/// </summary>
	public interface IIniValueAcceptor
	{
		/// <summary>
		/// The last successfully accepted section of this value acceptor.
		/// </summary>
		string Section { get; set; }
		/// <summary>
		/// The target key of this value acceptor.
		/// </summary>
		string Key { get; }
		/// <summary>
		/// Accepts value. If an error occurred during processing, returns the error.
		/// </summary>
		/// <param name="section">The section name.</param>
		/// <param name="key">The key.</param>
		/// <param name="value">The value.</param>
		/// <returns>The error, if any.</returns>
		IniError Accept(string section, string key, string value);
		/// <summary>
		/// Resets the value, <see cref="Section"/>, and <see cref="Key"/> so this can be re-used.
		/// </summary>
		void Reset();
		/// <summary>
		/// True if this has accepted a value, false otherwise.
		/// </summary>
		bool HasValue { get; }
	}
}
