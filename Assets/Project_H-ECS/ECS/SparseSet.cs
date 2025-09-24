using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Project_H.ECS
{
	public class SparseSet
	{
		public SparseSet(int entriesCount)
		{
			_sparse = new int[entriesCount];
			_dense = new int [entriesCount];
		}

		//private const int MaxEntities = 100_000;

		private readonly int[] _sparse;
		private readonly int[] _dense;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetEntities()
		{
			return _dense.AsSpan(0, count);
		}

		private int count = 0;
		public int GetCount() => count;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int entityID)
		{
			if (Has(entityID)) return;

			_sparse[entityID] = count;
			_dense[count] = entityID;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(int entityID)
		{
			int index = _sparse[entityID];
			return index < count && _dense[index] == entityID;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int entityID)
		{
			if (!Has(entityID)) return;

			int index = _sparse[entityID];
			int last = count - 1;
			int lastEntity = _dense[last];

			_dense[index] = lastEntity;
			_sparse[lastEntity] = index;

			count--;
		}

		public void Clear()
		{
			count = 0;
		}

		public int GetByIndex(int index)
		{
			if (index < count)
			{
				return _dense[index];
			}

			throw new InvalidOperationException($"you cannot reach out of range enement index:{index} count{count}");
		}
	}

	public class SparseSet<T>
	{
		private static int _chunkSize = 128;
		private static int _chunkSizeMinOne = _chunkSize - 1;
		private static int _n = 7;
		private static int _chuncksCount = 1000;
		private int _capacity = -1;
		
		public SparseSet()
		{
			_sparse = new int[_chuncksCount][];
			_dense = new int[_chuncksCount][];
			_values = new T[_chuncksCount][];
		}

		private readonly int[][] _sparse;
		private readonly int[][] _dense;
		private readonly T[][] _values;
		private int count = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id, in T component)
		{
			int chunkIndex = count >> _n;
			int innerIndex = count & _chunkSizeMinOne;

			int sparseChunkIndex = id >> _n;
			int sparseInnerIndex = id & _chunkSizeMinOne;

			if (chunkIndex > _capacity)
			{
				//Debug.LogError("resize id: " + id + $"type of{typeof(T)}");
				_capacity++;
				_dense[_capacity] = new int[_chunkSize];
				_values[_capacity] = new T[_chunkSize];
			}

			var sparseChunk = _sparse[sparseChunkIndex] ?? (_sparse[sparseChunkIndex] = new int[_chunkSize]);

			sparseChunk[sparseInnerIndex] = count;
			_dense[chunkIndex][innerIndex] = id;
			_values[chunkIndex][innerIndex] = component;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(in int entityID)
		{
			var sparseChunk = entityID >> _n;
			var sparseChunkArr = _sparse[sparseChunk];
			if (sparseChunkArr == null) return false;
			var sparseChunkId = entityID & _chunkSizeMinOne;
			var index = sparseChunkArr[sparseChunkId];

			var chunkIndex = index >> _n;
			var innerIndex = index & _chunkSizeMinOne;
			return index < count && _dense[chunkIndex][innerIndex] == entityID;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetRef(in int entityID)
		{
			var sparseChunk = entityID >> _n;
			var sparseChunkId = entityID & _chunkSizeMinOne;
			int index = _sparse[sparseChunk][sparseChunkId];

			var chunkIndex = index >> _n;
			var innerIndex = index & _chunkSizeMinOne;
			return ref _values[chunkIndex][innerIndex];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGet(in int entityId, out T value)
		{
			var sparseChunk = entityId >> _n;
			var sparseChunkArr = _sparse[sparseChunk];
			if (sparseChunkArr == null)
			{
				value = default;
				return false;
			}

			var sparseChunkId = entityId & _chunkSizeMinOne;
			var index = sparseChunkArr[sparseChunkId];

			var chunkIndex = index >> _n;
			var innerIndex = index & _chunkSizeMinOne;

			if (index < count && _dense[chunkIndex][innerIndex] == entityId)
			{
				value = _values[chunkIndex][innerIndex];
				return true;
			}

			value = default;
			return false;
		}

		// [MethodImpl(MethodImplOptions.AggressiveInlining)]
		// public bool TryGetRef(in int entityId, ref T value)
		// {
		// 	int index = _sparse[entityId];
		//
		// 	var chunkIndex = index >> _n;
		// 	var innerIndex = index & _chunkSizeMinOne;
		// 	if (index < count && _dense[chunkIndex][innerIndex] == entityId)
		// 	{
		// 		value = ref _values[chunkIndex][innerIndex];
		// 		return true;
		// 	}
		//
		// 	value = default;
		// 	return false;
		// }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int entityId, in T component, out T replacedComponent)
		{
			var sparseChunk = entityId >> _n;
			var sparseChunkId = entityId & _chunkSizeMinOne;
			int index = _sparse[sparseChunk][sparseChunkId];

			var chunkIndex = index >> _n;
			var innerIndex = index & _chunkSizeMinOne;

			ref var val = ref _values[chunkIndex][innerIndex];
			replacedComponent = val;
			val = component;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int entityID, out T removedItem)
		{
			var sparseChunk = entityID >> _n;
			var sparseChunkId = entityID & _chunkSizeMinOne;

			int replaceIndex = _sparse[sparseChunk][sparseChunkId];

			int chunkValuesOldIndex = replaceIndex >> _n;
			int innerValuesOldIndex = replaceIndex & _chunkSizeMinOne;

			removedItem = _values[chunkValuesOldIndex][innerValuesOldIndex];
			int last = count - 1;

			int chunkValuesLastIndex = last >> _n;
			int innerValuesLastIndex = last & _chunkSizeMinOne;

			int lastEntityID = _dense[chunkValuesLastIndex][innerValuesLastIndex];

			var lastSparseChunk = lastEntityID >> _n;
			var lastSparseChunkId = lastEntityID & _chunkSizeMinOne;


			_values[chunkValuesOldIndex][innerValuesOldIndex] = _values[chunkValuesLastIndex][innerValuesLastIndex];
			_dense[chunkValuesOldIndex][innerValuesOldIndex] = lastEntityID;
			_sparse[lastSparseChunk][lastSparseChunkId] = replaceIndex;

			count--;
		}


		public struct Enumerator
		{
			private readonly SparseSet<T> _set;
			private int _index;

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public Enumerator(SparseSet<T> set)
			{
				_set = set;
				_index = -1;
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public bool MoveNext() => ++_index < _set.count;

			public ref T Current
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					var chunkIndex = _index >> _n;
					var innerIndex = _index & _chunkSizeMinOne;
					return ref _set._values[chunkIndex][innerIndex];
				}
			}
		}

		public Enumerator GetEnumerator() => new(this);
	}
}