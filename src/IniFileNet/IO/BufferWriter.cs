namespace IniFileNet.IO
{
	using System;
	using System.Buffers;

	/// <summary>
	/// Writes data into an array of <typeparamref name="T"/>, resizing when necessary.
	/// Cannot grow larger than the maximum length of an array.
	/// </summary>
	/// <typeparam name="T">The type of data.</typeparam>
	public sealed class BufferWriter<T> : IBufferWriter<T>
	{
		private int index;
		internal T[] buf;
		/// <summary>
		/// Creates a new instance with a buffer sized to <paramref name="capacity"/>.
		/// </summary>
		/// <param name="capacity">The initial capacity.</param>
		/// <exception cref="ArgumentOutOfRangeException">Capacity is less than or equal to 0.</exception>
		public BufferWriter(int capacity = 256)
		{
			if (capacity <= 0) throw new ArgumentOutOfRangeException(nameof(capacity), "capacity cannot be less than or equal to 0");
			buf = new T[capacity];
			index = 0;
		}
		/// <summary>
		/// The current capacity of the buffer; that is, its total length, including free and consumed space.
		/// </summary>
		public int Capacity => buf.Length;
		/// <summary>
		/// The space in the buffer which has been written.
		/// </summary>
		public int Written => index;
		/// <summary>
		/// The free space left in the buffer.
		/// </summary>
		public int Free => buf.Length - index;
		/// <summary>
		/// Gets the written data in the buffer as a <see cref="ReadOnlySpan{T}"/>
		/// </summary>
		public ReadOnlySpan<T> Span => buf.AsSpan(0, index);
		/// Gets the written data in the buffer as a <see cref="ReadOnlyMemory{T}"/>
		public ReadOnlyMemory<T> Memory => buf.AsMemory(0, index);
		/// <summary>
		/// Resets the index of the buffer to 0 and clears all data.
		/// </summary>
		public void Clear()
		{
			buf.AsSpan(0, index).Clear();
			index = 0;
		}
		/// <summary>
		/// Sets the index of the buffer to the provided index.
		/// This may not be less than zero or greater than <see cref="Capacity"/>.
		/// </summary>
		/// <param name="index"></param>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="index"/> is less than 0.</exception>
		public void SetWritten(int index)
		{
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "index cannot be less than 0");
			}
			if (index > buf.Length)
			{
				throw new ArgumentOutOfRangeException(nameof(index), "index cannot be greater than " + buf.Length);
			}
			this.index = index;
		}
		/// <summary>
		/// Notifies that <paramref name="count"/> number of <typeparamref name="T"/> were written to the
		/// <see cref="Span{T}"/> or <see cref="Memory{T}"/> retrieved by a call to <see cref="GetSpan(int)"/> or <see cref="GetMemory(int)"/>.
		/// </summary>
		/// <param name="count">The number of <typeparamref name="T"/> written.</param>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="count"/> is less than 0.</exception>
		/// <exception cref="InvalidOperationException">If <paramref name="count"/> would advance the buffer past the end of its available space.</exception>
		public void Advance(int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(count), "count cannot be less than 0");
			}
			if (count + index > buf.Length)
			{
				throw new InvalidOperationException(string.Concat("Advancing beyond end of buffer; currently at index ", index.ToString(), " in a buffer ", buf.Length.ToString(), " long, and advancing by ", count.ToString(),
					". Check the calling code; it's most likely over-counting the number of elements written."));
			}
			index += count;
		}
		/// <summary>
		/// Gets a <see cref="Span{T}"/> that is at least the length specified by <paramref name="minLength"/>.
		/// It may not be exactly <paramref name="minLength"/> in length. It might be larger, but it will not be smaller.
		/// After calling this and writing to the span, call <see cref="Advance(int)"/> with how many <typeparamref name="T"/> written, 
		/// and do not write to the span again.
		/// </summary>
		/// <param name="minLength">The minimum length required.</param>
		/// <returns>A writeable <see cref="Span{T}"/>.</returns>
		public Span<T> GetSpan(int minLength = 0)
		{
			EnsureCapacity(minLength);
			return buf.AsSpan(index);
		}
		/// <summary>
		/// Gets a <see cref="Memory{T}"/> that is at least the length specified by <paramref name="minLength"/>.
		/// It may not be exactly <paramref name="minLength"/> in length. It might be larger, but it will not be smaller.
		/// After calling this and writing to the memory, call <see cref="Advance(int)"/> with how many <typeparamref name="T"/> written, 
		/// and do not write to the memory again.
		/// </summary>
		/// <param name="minLength">The minimum length required.</param>
		/// <returns>A writeable <see cref="Memory{T}"/>.</returns>
		public Memory<T> GetMemory(int minLength = 0)
		{
			EnsureCapacity(minLength);
			return buf.AsMemory(index);
		}
		/// <summary>
		/// Ensures that the buffer is at least (but not necessarily exactly) <paramref name="minLength"/> in size.
		/// It's optional to call this by yourself, as this class will ensure there is necessary capacity on each call to <see cref="GetSpan(int)"/> or <see cref="GetMemory(int)"/>.
		/// </summary>
		/// <param name="minLength">The minimum length required.</param>
		/// <exception cref="ArgumentOutOfRangeException">If <paramref name="minLength"/> is less than 0.</exception>
		/// <exception cref="OutOfMemoryException">If the buffer to be allocated exceeds the Array maximum length.</exception>
		public void EnsureCapacity(int minLength)
		{
			if (minLength < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(minLength), "minLength cannot be less than 0");
			}

			if (minLength == 0)
			{
				minLength = 1;
			}
			if (minLength > Free)
			{
				// Need more capacity
				int currentLength = buf.Length;

				// Grow by the larger of the size hint or our current length
				// (i.e. at least double the array in size, but grow by larger if requested to do so)
				int newSize = currentLength + Math.Max(minLength, currentLength);
				if ((uint)newSize > int.MaxValue)
				{
					// Just pretending for a moment that we aren't in the billions...
					// Say we have a buffer of length 10, and we've written 3 bytes to it.
					// That's 7 bytes free space, and we have a size hint of 20.
					// 10 - 7 + 20 = 23
					// Thus, we need an underlying buffer of at least 23 bytes long
					// Basically that's because we are at index 3, and the caller wants a buffer of sizeHint long.
					uint needed = (uint)(buf.Length - Free + minLength);
#if NETSTANDARD2_0
					if (needed > 0x7FFFFFC7)
					{
						ThrowOutOfMemory(needed);
					}
					newSize = 0x7FFFFFC7;
#else
					if (needed > Array.MaxLength)
					{
						ThrowOutOfMemory(needed);
					}
					newSize = Array.MaxLength;
#endif
				}
				Array.Resize(ref buf, newSize);
			}
		}
		private static void ThrowOutOfMemory(uint needed)
		{
			throw new OutOfMemoryException("Cannot allocate buffer, because it exceeds the maximum length of an array. Requested size is: " + needed.ToString());
		}
	}
}
