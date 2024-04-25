namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// A single comented value
	/// </summary>
	public sealed class CommentedValue
	{
		/// <summary>
		/// Creates a new instance, using <see cref="Array.Empty{T}"/> as the <see cref="Comments"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		public CommentedValue(string value) : this(value, Array.Empty<string>()) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <param name="comments">The comments.</param>
		public CommentedValue(string value, IReadOnlyList<string> comments)
		{
			Value = value;
			Comments = comments;
		}
		/// <summary>
		/// The value.
		/// </summary>
		public string Value { get; }
		/// <summary>
		/// The comments preceding this value.
		/// </summary>
		public IReadOnlyList<string> Comments { get; }
	}
}
