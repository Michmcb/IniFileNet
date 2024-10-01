namespace IniFileNet
{
	using System.Buffers;

	/// <summary>
	/// Holds an <see cref="OperationStatus"/> and a message.
	/// </summary>
	public readonly struct OperationStatusMsg
	{
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		public OperationStatusMsg(OperationStatus status, string? msg)
		{
			Status = status;
			Msg = msg;
		}
		/// <summary>
		/// The status.
		/// </summary>
		public OperationStatus Status { get; }
		/// <summary>
		/// The message.
		/// </summary>
		public string? Msg { get; }
	}
}
