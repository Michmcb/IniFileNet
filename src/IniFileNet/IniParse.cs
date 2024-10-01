namespace IniFileNet
{
	using System;

	/// <summary>
	/// Parse methods.
	/// </summary>
	public static class IniParse
	{
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="bool.TryParse(string?, out bool)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<bool> Boolean(string value)
		{
			return bool.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as boolean")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <paramref name="value"/> is 1 or 0. Returns <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<bool> Boolean1Or0(string value)
		{
			return value.Length == 1
				? value[0] switch
				{
					'0' => new(false, default),
					'1' => new(true, default),
					_ => new(default, new(IniErrorCode.ValueInvalid, string.Concat("The value \"", value, "\" was not 1 or 0"))),
				}
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("The value \"", value, "\" was not 1 or 0")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="byte.TryParse(string?, out byte)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<byte> Byte(string value)
		{
			return byte.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as byte")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="sbyte.TryParse(string?, out sbyte)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<sbyte> SByte(string value)
		{
			return sbyte.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as sbyte")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="short.TryParse(string?, out short)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<short> Int16(string value)
		{
			return short.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as short")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="ushort.TryParse(string?, out ushort)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<ushort> UInt16(string value)
		{
			return ushort.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as ushort")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="int.TryParse(string?, out int)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<int> Int32(string value)
		{
			return int.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as int")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="uint.TryParse(string?, out uint)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<uint> UInt32(string value)
		{
			return uint.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as uint")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="long.TryParse(string?, out long)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<long> Int64(string value)
		{
			return long.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as long")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="ulong.TryParse(string?, out ulong)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<ulong> UInt64(string value)
		{
			return ulong.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as ulong")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="decimal.TryParse(string?, out decimal)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<decimal> Decimal(string value)
		{
			return decimal.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as decimal")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="float.TryParse(string?, out float)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<float> Single(string value)
		{
			return float.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as float")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="double.TryParse(string?, out double)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<double> Double(string value)
		{
			return double.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as double")));
		}
		/// <summary>
		/// Returns a successful result and the parsed value if <see cref="Guid.TryParse(string?, out Guid)"/> is successful, or <see cref="IniErrorCode.ValueInvalid"/> on failure.
		/// </summary>
		/// <param name="value">The value to parse.</param>
		public static IniResult<Guid> Guid(string value)
		{
			return System.Guid.TryParse(value, out var r)
				? new(r, default)
				: new(default, new(IniErrorCode.ValueInvalid, string.Concat("Could not parse \"", value, "\" as Guid")));
		}
	}
}
