namespace IniFileNet
{
	using System;

	/// <summary>
	/// An exception encountered when parsing an ini file.
	/// </summary>
	public class IniException : Exception
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniException(IniErrorCode iniErrorCode) : base()
		{
			IniErrorCode = iniErrorCode;
		}
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniException(IniErrorCode iniErrorCode, string message) : base(message)
		{
			IniErrorCode = iniErrorCode;
		}
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public IniException(IniErrorCode iniErrorCode, string message, Exception innerException) : base(message, innerException)
		{
			IniErrorCode = iniErrorCode;
		}
		/// <summary>
		/// The error code.
		/// </summary>
		public IniErrorCode IniErrorCode { get; }
	}
}
