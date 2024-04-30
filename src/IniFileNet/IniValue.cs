namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A value.
	/// </summary>
	/// <typeparam name="TValue">The type of the values.</typeparam>
	public sealed class IniValue<TValue>
	{
		/// <summary>
		/// Creates a new instance, using <see cref="Array.Empty{T}"/> as the <see cref="Comments"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		public IniValue(TValue value) : this(value, Array.Empty<string>()) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="comments">The comments.</param>
		public IniValue(TValue value, IReadOnlyList<string> comments)
		{
			Value = value;
			Comments = comments;
		}
		/// <summary>
		/// The value.
		/// </summary>
		public TValue Value { get; }
		/// <summary>
		/// The comments preceding this value.
		/// </summary>
		public IReadOnlyList<string> Comments { get; }
	}
}
