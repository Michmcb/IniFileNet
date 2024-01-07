namespace IniFileNet
{
	using System;

	public class IniException : Exception
	{
		public IniException(IniErrorCode iniErrorCode) : base()
		{
			IniErrorCode = iniErrorCode;
		}
		public IniException(IniErrorCode iniErrorCode, string message) : base(message)
		{
			IniErrorCode = iniErrorCode;
		}
		public IniException(IniErrorCode iniErrorCode, string message, Exception innerException) : base(message, innerException)
		{
			IniErrorCode = iniErrorCode;
		}
		public IniErrorCode IniErrorCode { get; }
	}
}
