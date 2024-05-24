namespace IniFileNet
{
	using System;
	using System.Collections.Generic;
	/// <summary>
	/// A single section of an ini file, as read only.
	/// </summary>
	public sealed class ReadOnlyIniSection
	{
		/// <summary>
		/// Creates a new instance, using <see cref="Array.Empty{T}"/> as the <see cref="Comments"/>.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <param name="keyValues">The keys and values.</param>
		public ReadOnlyIniSection(string name, IReadOnlyList<IniKeyValue<string>> keyValues) : this(name, keyValues, Array.Empty<string>()) { }
		/// <summary>
		/// Creates a new instance.
		/// </summary>
		/// <param name="name">The section name.</param>
		/// <param name="keyValues">The keys and values.</param>
		/// <param name="comments">The comments.</param>
		public ReadOnlyIniSection(string name, IReadOnlyList<IniKeyValue<string>> keyValues, IReadOnlyList<string> comments)
		{
			Name = name;
			KeyValues = keyValues;
			Comments = comments;
		}
		/// <summary>
		/// The name of the section. May be an empty string if this represents global keys, but will never be null.
		/// </summary>
		public string Name { get; }
		/// <summary>
		/// The keys and values of this section. Key may or may not be unique, depending on the rules enforced in the source file.
		/// </summary>
		public IReadOnlyList<IniKeyValue<string>> KeyValues { get; }
		/// <summary>
		/// The comments preceding this section. This may be an empty collection if comments were ignored.
		/// </summary>
		public IReadOnlyList<string> Comments { get; }
		/// <summary>
		/// Loops through all <see cref="KeyValues"/>, and if there is an <see cref="IIniValueAcceptor"/> with a matching key in <paramref name="acceptors"/>, invokes
		/// <see cref="IIniValueAcceptor.Accept(string)"/> with the key and value.
		/// </summary>
		/// <param name="acceptors">The acceptors.</param>
		/// <returns>The first <see cref="IniError"/> that an <see cref="IIniValueAcceptor"/> returned.</returns>
		public IniError AcceptAll(IReadOnlyDictionary<string, IIniValueAcceptor> acceptors)
		{
			foreach (var kv in KeyValues)
			{
				if (acceptors.TryGetValue(kv.Key, out IIniValueAcceptor? acceptor))
				{
					var e = acceptor.Accept(kv.Value);
					if (e.Code != IniErrorCode.None) return e;
				}
			}
			return default;
		}
	}
}
