namespace IniFileNet
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Convenience class to create <see cref="IIniValueAcceptor"/>s and add them to a <see cref="Dictionary{TKey, TValue}"/>.
	/// </summary>
	public sealed class IniValueAcceptorDictionaryBuilder
	{
		/// <summary>
		/// Creates a new instance that adds new acceptors to <paramref name="acceptors"/>.
		/// </summary>
		/// <param name="acceptors">The dictionary to add to.</param>
		public IniValueAcceptorDictionaryBuilder(Dictionary<string, IIniValueAcceptor> acceptors)
		{
			Acceptors = acceptors;
		}
		/// <summary>
		/// The dictionary to add to.
		/// </summary>
		public Dictionary<string, IIniValueAcceptor> Acceptors { get; }
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorOnlyFirst OnlyFirst(string key)
		{
			IniValueAcceptorOnlyFirst i = new();
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <param name="parse">The parser for <typeparamref name="T"/>.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorOnlyFirst<T> OnlyFirst<T>(string key, Func<string, IniResult<T>> parse)
		{
			IniValueAcceptorOnlyFirst<T> i = new(parse);
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorOnlyLast OnlyLast(string key)
		{
			IniValueAcceptorOnlyLast i = new();
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <param name="parse">The parser for <typeparamref name="T"/>.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorOnlyLast<T> OnlyLast<T>(string key, Func<string, IniResult<T>> parse)
		{
			IniValueAcceptorOnlyLast<T> i = new(parse);
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorSingle Single(string key)
		{
			IniValueAcceptorSingle i = new();
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <param name="parse">The parser for <typeparamref name="T"/>.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorSingle<T> Single<T>(string key, Func<string, IniResult<T>> parse)
		{
			IniValueAcceptorSingle<T> i = new(parse);
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorMany Many(string key)
		{
			IniValueAcceptorMany i = new();
			Acceptors[key] = i;
			return i;
		}
		/// <summary>
		/// Creates a new acceptor for <paramref name="key"/>, returns it, and adds it to <see cref="Acceptors"/>.
		/// </summary>
		/// <param name="key">The key for the acceptor.</param>
		/// <param name="parse">The parser for <typeparamref name="T"/>.</param>
		/// <returns>The acceptor.</returns>
		public IniValueAcceptorMany<T, C> Many<T, C>(string key, Func<string, IniResult<T>> parse) where C : ICollection<T>, new()
		{
			IniValueAcceptorMany<T, C> i = new(parse);
			Acceptors[key] = i;
			return i;
		}
	}
}
