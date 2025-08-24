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
		public SparseSet(int entriesCount)
		{
			_sparse = new int[entriesCount];
			_dense = new int[entriesCount];
			_values = new T[entriesCount];
		}

		private readonly int[] _sparse;
		private readonly int[] _dense;
		private readonly T[] _values;
		private int count = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(int id, in T component)
		{
			if (Has(id))
			{
				return;
			}

			_sparse[id] = count;
			_dense[count] = id;
			_values[count] = component;
			count++;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(in int entityID)
		{
			int index = _sparse[entityID];
			return index < count && _dense[index] == entityID;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T GetRef(in int entityID)
		{
			return ref _values[_sparse[entityID]];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGet(in int entityId, out T value)
		{
			int index = _sparse[entityId];
			if (index < count && _dense[index] == entityId)
			{
				value = _values[index];
				return true;
			}

			value = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool TryGetRef(in int entityId, ref T value)
		{
			int index = _sparse[entityId];
			if (index < count && _dense[index] == entityId)
			{
				value = ref _values[index];
				return true;
			}

			value = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(int id, in T component, out T replacedComponent)
		{
			ref var val = ref _values[_sparse[id]];
			replacedComponent = val;
			val = component;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(int entityID, out T removedItem)
		{
			int index = _sparse[entityID];
			removedItem = _values[index];
			int last = count - 1;
			int lastEntity = _dense[last];

			_values[index] = _values[last];
			_dense[index] = lastEntity;
			_sparse[lastEntity] = index;

			count--;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> GetAllValues()
		{
			return _values.AsSpan(0, count);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetAllIDs()
		{
			return _dense.AsSpan(0, count);
		}
	}

	public class ArrayWrapper<T>
	{
		private int _indexToAdd = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ArrayWrapper(int count)
		{
			_array = new T[count];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(T obj)
		{
			if (_indexToAdd == _array.Length) Debug.LogError("Cannot Add more");
			_array[_indexToAdd] = obj;
			_indexToAdd++;
		}

		private T[] _array;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<T> GetArray()
		{
			return _array.AsSpan(0, _indexToAdd);
		}
	}
}