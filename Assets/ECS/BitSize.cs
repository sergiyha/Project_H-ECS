using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Project_H.ECS
{

	public struct BitMask : IEquatable<BitMask>
	{
		private int[] _bits;
		private int[] _ids;

		public BitMask(int sizeInBits)
		{
			_bits = new int[(sizeInBits + 31) / 32];
			_ids = new int[sizeInBits / 4];
		}

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public void EnsureSize(int bit)
		// {
		// 	int index = bit / 32;
		// 	if (_bits.Length <= index)
		// 	{
		// 		int newSize = Math.Max(index + 1, _bits.Length * 2);
		// 		Array.Resize(ref _bits, newSize);
		// 		//should clear unused bits
		// 	}
		// }


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBit(int bit)
		{
			//EnsureSize(bit);
			int index = bit / 32;
			if (index >= _bits.Length)
				throw new IndexOutOfRangeException("Bit index exceeds bitmask size.");
			_bits[index] |= 1 << (bit % 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBits(Span<int> bits)
		{
			for (int i = 0; i < bits.Length; i++)
			{
				SetBit(bits[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBits(List<int> bits)
		{
			for (int i = 0; i < bits.Count; i++)
			{
				SetBit(bits[i]);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearBit(int bit)
		{
			int index = bit / 32;
			if (index < _bits.Length)
				_bits[index] &= ~(1 << (bit % 32));
			else
			{
				throw new IndexOutOfRangeException("Bit index exceeds bitmask size.");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBit(int bit)
		{
			int index = bit / 32;
			return index < _bits.Length && (_bits[index] & (1 << (bit % 32))) != 0;
		}

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public bool Matches(in BitMask otherMast)
		// {
		// 	int len = otherMast._bits.Length;
		// 	if (_bits.Length < len) return false;
		//
		// 	for (int i = 0; i < len; i++)
		// 	{
		// 		if(otherMast._bits[i] == 0)continue;
		// 		if ((otherMast._bits[i] & _bits[i]) != otherMast._bits[i])
		// 			return false;
		// 	}
		//
		// 	return true;
		// }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(in BitMask otherMast)
		{
			int len = otherMast._bits.Length;
			if (_bits.Length < len) return false;

			for (int i = 0; i < len; i++)
			{
				int other = otherMast._bits[i];
				if (other == 0) continue;

				if ((other & _bits[i]) != other)
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
		public Span<int> GetIndexes()
		{
			int count = 0;

			for (int i = 0; i < _bits.Length; i++)
			{
				if (_bits[i] == 0) continue;
				uint chunk = (uint)_bits[i];

				for (int bit = 0; bit < 32; bit++)
				{
					if ((chunk & (1 << bit)) != 0)
					{
						if (count >= _ids.Length)
							throw new InvalidOperationException("Buffer too small");

						_ids[count++] = i * 32 + bit;
					}
				}
			}

			return _ids.AsSpan(0, count);
		}

		//if you have problem with unsafe version use this
		// public bool Equals(BitMask other)
		// {
		// 	for (int i = 0; i < _bits.Length; i++)
		// 	{
		// 		if (_bits[i] != other._bits[i])
		// 			return false;
		// 	}
		// 		
		// 	return true;
		// }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe bool Equals(BitMask other)
		{
			if (_bits.Length != other._bits.Length)
				return false;

			fixed (int* a = _bits, b = other._bits)
			{
				for (int i = 0; i < _bits.Length; i++)
				{
					if (a[i] != b[i])
						return false;
				}
			}

			return true;
		}

		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("cannot be boxed");
		}

		//very base implementation
		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public override int GetHashCode()
		// {
		// 	HashCode hashCode = new HashCode();
		// 	for (int i = 0; i < _bits.Length; i++)
		// 	{
		// 		hashCode.Add(_bits[i]);
		// 	}
		// 	return hashCode.ToHashCode();
		// }

		//FNV-1a 64-bit hash
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode()
		{
			unchecked
			{
				const ulong fnvOffset = 14695981039346656037UL;
				const ulong fnvPrime = 1099511628211UL;
				ulong hash = fnvOffset;

				for (int i = 0; i < _bits.Length; i++)
				{
					hash ^= (ulong)_bits[i];
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