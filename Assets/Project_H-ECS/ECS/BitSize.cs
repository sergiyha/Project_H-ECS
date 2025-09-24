using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project_H_ECS.ECS.Helper;
using UnityEngine;

namespace Project_H.ECS
{
	public struct BitMask : IEquatable<BitMask>
	{
		private ulong[] _bits;
		private int[] _ids;

		public BitMask(int sizeInBits)
		{
			_bits = new ulong[(sizeInBits + 63) / 64];
			_ids = new int[sizeInBits / 4]; // still a heuristic buffer
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBit(int bit)
		{
			int index = bit >> 6;
			if (index >= _bits.Length)
				throw new IndexOutOfRangeException("Bit index exceeds bitmask size.");
			_bits[index] |= 1UL << (bit & 63);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBits(Span<int> bits)
		{
			for (int i = 0; i < bits.Length; i++)
				SetBit(bits[i]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBits(List<int> bits)
		{
			for (int i = 0; i < bits.Count; i++)
				SetBit(bits[i]);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearBit(int bit)
		{
			int index = bit >> 6; //bit / 64;
			if (index < _bits.Length)
				_bits[index] &= ~(1UL << (bit & 63));
			else
				throw new IndexOutOfRangeException("Bit index exceeds bitmask size.");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBit(int bit)
		{
			int index = bit >> 6;
			return index < _bits.Length && (_bits[index] & (1UL << (bit & 63))) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(in BitMask otherMask)
		{
			int len = otherMask._bits.Length;
			if (_bits.Length < len) return false;

			for (int i = 0; i < len; i++)
			{
				ulong other = otherMask._bits[i];
				if (other == 0) continue;

				if ((_bits[i] & other) != other)
					return false;
			}

			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Clear()
		{
			Array.Clear(_bits, 0, _bits.Length);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public Span<int> GetIndexes()
		// {
		// 	int count = 0;
		//
		// 	for (int i = 0; i < _bits.Length; i++)
		// 	{
		// 		ulong chunk = _bits[i];
		// 		if (chunk == 0) continue;
		//
		// 		for (int bit = 0; bit < 64; bit++)
		// 		{
		// 			if ((chunk & (1UL << bit)) != 0)
		// 			{
		// 				if (count >= _ids.Length)
		// 					throw new InvalidOperationException("Buffer too small");
		//
		// 				_ids[count++] = i * 64 + bit;
		// 			}
		// 		}
		// 	}
		//
		// 	return _ids.AsSpan(0, count);
		// }
		public Span<int> GetIndexes()
		{
			int count = 0;

			for (int i = 0; i < _bits.Length; i++)
			{
				ulong chunk = _bits[i];
				while (chunk != 0)
				{
					int bit = MathUtil.TrailingZeroCount(chunk);
					int index = (i << 6) + bit;

					if (count >= _ids.Length)
						throw new InvalidOperationException("Buffer too small");

					_ids[count++] = index;

					chunk &= chunk - 1; // clear lowest set bit
				}
			}

			return _ids.AsSpan(0, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public unsafe bool Equals(BitMask other)
		// {
		// 	if (_bits.Length != other._bits.Length)
		// 		return false;
		//
		// 	fixed (ulong* a = _bits, b = other._bits)
		// 	{
		// 		for (int i = 0; i < _bits.Length; i++)
		// 		{
		// 			if (a[i] != b[i])
		// 				return false;
		// 		}
		// 	}
		//
		// 	return true;
		// }
		public bool Equals(BitMask other)
			=> _bits.AsSpan().SequenceEqual(other._bits);

		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("cannot be boxed");
		}


		// FNV-1a 64-bit hash
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		//[MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public override int GetHashCode()
		// {
		// 	HashCode hc = new HashCode();
		// 	foreach (var b in _bits)
		// 		hc.Add(b);
		// 	return hc.ToHashCode();
		// }
		public override int GetHashCode()
		{
			unchecked
			{
				const ulong fnvOffset = 14695981039346656037UL;
				const ulong fnvPrime = 1099511628211UL;
				ulong hash = fnvOffset;

				for (int i = 0; i < _bits.Length; i++)
				{
					hash ^= _bits[i];
					hash *= fnvPrime;
				}

				return (int)(hash ^ (hash >> 32));
			}
		}
	}

	static class ComponentTypeRegistry
	{
		private static Dictionary<Type, int> _typeToBit = new();
		private static int _nextBitIndex = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetBitIndex(Type type)
		{
			if (!_typeToBit.TryGetValue(type, out int index))
			{
				if (_nextBitIndex == Storage.MaxComponentsCount - 1)
				{
					Debug.LogError($"Cannot have more than {Storage.MaxComponentsCount} components");
				}

				index = _nextBitIndex++;
				_typeToBit[type] = index;
			}

			return index;
		}
	}
}