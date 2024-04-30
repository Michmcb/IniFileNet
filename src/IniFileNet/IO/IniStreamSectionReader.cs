namespace IniFileNet.IO
{
	using System;
	using System.Threading.Tasks;
	/// <summary>
	/// Wraps a <see cref="IniStreamReader"/> and collects values into sections.
	/// </summary>
	public sealed class IniStreamSectionReader : IDisposable
	{
		private readonly IniStreamSectionReaderState state;
		/// <summary>
		/// Creates a new instance, wrapping the provided <paramref name="reader"/>.
		/// </summary>
		public IniStreamSectionReader(IniStreamReader reader, bool leaveOpen = false)
		{
			state = new(reader.Options.IgnoreComments);
			Reader = reader;
			LeaveOpen = leaveOpen;
			Section = state.CompleteSection;
		}
		/// <summary>
		/// The <see cref="IniStreamReader"/> to read from.
		/// </summary>
		public IniStreamReader Reader { get; }
		/// <summary>
		/// <see langword="true"/> to leave <see cref="Reader"/> open after this <see cref="IniStreamSectionReader"/> is disposed. Otherwise, false.
		/// </summary>
		public bool LeaveOpen { get; set; }
		/// <summary>
		/// The section that was just read, or an empty section if nothing has been read yet, or when an error has been encountered. Never <see langword="null"/>.
		/// </summary>
		public ReadOnlyIniSection Section { get; private set; }
		/// <summary>
		/// Attempts to read the next section. Global keys, if they are allowed, will be in a section with a name of empty string.
		/// Sets the <see cref="Section"/> property to the read section when returning <see langword="true"/>.
		/// Returns <see langword="false"/> when an error is encountered or there are no more sections to read. Check the <see cref="IniStreamReader.Error"/> property for any errors.
		/// </summary>
		/// <returns><see langword="true"/> on reading a section, <see langword="false"/> on an error or when there are no more sections to read.</returns>
		public bool NextSection()
		{
			bool go = true;
			while (go)
			{
				ReadResult rr = Reader.Read();
				go = state.Handle(rr, Reader.Options);
			}
			Section = state.CompleteSection;
			return state.Ok;
		}
		/// <summary>
		/// Attempts to read the next section. Global keys, if they are allowed, will be in a section with a name of empty string.
		/// Sets the <see cref="Section"/> property to the read section when returning <see langword="true"/>.
		/// Returns <see langword="false"/> when an error is encountered or there are no more sections to read. Check the <see cref="IniStreamReader.Error"/> property for any errors.
		/// </summary>
		/// <returns><see langword="true"/> on reading a section, <see langword="false"/> on an error or when there are no more sections to read.</returns>
		public async Task<bool> NextSectionAsync()
		{
			bool go = true;
			while (go)
			{
				ReadResult rr = await Reader.ReadAsync();
				go = state.Handle(rr, Reader.Options);
			}
			Section = state.CompleteSection;
			return state.Ok;
		}
		/// <summary>
		/// Disposes of <see cref="Reader"/> if <see cref="LeaveOpen"/> is <see langword="false"/>. Otherwise, does nothing.
		/// </summary>
		public void Dispose()
		{
			if (!LeaveOpen)
			{
				Reader.Dispose();
			}
		}
	}
}
