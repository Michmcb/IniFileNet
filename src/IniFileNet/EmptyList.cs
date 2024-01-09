namespace IniFileNet
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	/// <summary>
	/// A list that is always empty, no matter how many items you add to it.
	/// </summary>
	public static class EmptyList
	{
		/// <summary>
		/// Gets the statically allocated <see cref="EmptyList{T}"/>.
		/// </summary>
		/// <typeparam name="T">The type that this list "holds".</typeparam>
		public static EmptyList<T> Get<T>()
		{
			return EmptyList<T>.instance;
		}
	}
	/// <summary>
	/// A list that is always empty, no matter how many items you add to it.
	/// </summary>
	/// <typeparam name="T">The type that this list "holds".</typeparam>
	public sealed class EmptyList<T> : IList<T>, IReadOnlyList<T>
	{
		internal static readonly EmptyList<T> instance = [];
		/// <summary>
		/// Creates a new instance. Prefer using <see cref="EmptyList.Get{T}"/>.
		/// </summary>
		internal EmptyList() { }
		/// <summary>
		/// Always throws <see cref="IndexOutOfRangeException"/>.
		/// </summary>
		public T this[int index] => throw IndexOutOfRange();
		/// <summary>
		/// Always throws <see cref="IndexOutOfRangeException"/>.
		/// </summary>
		T IList<T>.this[int index] { get => throw IndexOutOfRange(); set => throw IndexOutOfRange(); }
		/// <summary>
		/// Always returns 0.
		/// </summary>
		public int Count => 0;
		/// <summary>
		/// Always returns <see langword="true"/>
		/// </summary>
		public bool IsReadOnly => true;
		/// <summary>
		/// Does nothing
		/// </summary>
		public void Add(T item) { }
		/// <summary>
		/// Does nothing
		/// </summary>
		public void Clear() { }
		/// <summary>
		/// Always returns <see langword="false"/>
		/// </summary>
		public bool Contains(T item) { return false; }
		/// <summary>
		/// Does nothing
		/// </summary>
		public void CopyTo(T[] array, int arrayIndex) { }
		/// <summary>
		/// Returns an empty sequence
		/// </summary>
		public IEnumerator<T> GetEnumerator() { return Enumerable.Empty<T>().GetEnumerator(); }
		/// <summary>
		/// Always returns -1
		/// </summary>
		public int IndexOf(T item) { return -1; }
		/// <summary>
		/// Does nothing
		/// </summary>
		public void Insert(int index, T item) { }
		/// <summary>
		/// Always returns <see langword="false"/>
		/// </summary>
		public bool Remove(T item) { return false; }
		/// <summary>
		/// Always throws <see cref="IndexOutOfRangeException"/>.
		/// </summary>
		public void RemoveAt(int index)
		{
			throw IndexOutOfRange();
		}
		/// <summary>
		/// Returns an empty sequence
		/// </summary>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
		private static IndexOutOfRangeException IndexOutOfRange() { return new("Index out of range; there are no elements in this list"); }
	}
}
