using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnsafeObjectPool;

namespace Project_H.ECS
{
	public partial class Storage
	{
		private Dictionary<Type, AQuery> _aQueriesCollection = new();
		private BitMask _reusableTempCompBitmask = new(MaxComponentsCount);

		public T GetQuery<T>() where T : AQuery, new()
		{
			var queryType = typeof(T);
			if (!_aQueriesCollection.TryGetValue(queryType, out AQuery query))
			{
				query = ObjectFactory.Get<T>();
				query.Init(_storageId);
				_aQueriesCollection.Add(queryType, query);
			}

			return (T)query;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private SparseSet TryGetEntitiesSet(Span<int> archetype)
		{
			_reusableTempCompBitmask.Clear();
			_reusableTempCompBitmask.SetBits(archetype);
			if (!_archetypesEntity.TryGetValue(_reusableTempCompBitmask, out var archetypeSparset))
			{
				var newArchetypeBitmask = new BitMask(MaxComponentsCount);
				newArchetypeBitmask.SetBits(_reusableTempCompBitmask.GetIndexes());
				archetypeSparset = new SparseSet(MaxEntitiesCount);
				_archetypesEntity[newArchetypeBitmask] = archetypeSparset;
				FillArchetypeCollection(in newArchetypeBitmask, archetypeSparset);
				InitCompBitToArchetype(in archetype, archetypeSparset, in newArchetypeBitmask);
			}

			return archetypeSparset;
		}

		private void InitCompBitToArchetype(in Span<int> bits, SparseSet archetypeEnttSparset, in BitMask bitMask)
		{
			for (int i = 0; i < bits.Length; i++)
			{
				if (!_compBitToArchetypes.TryGetValue(bits[i], out var list))
				{
					list = new();
					_compBitToArchetypes[bits[i]] = list;
				}

				list.Add(new ArchetypesHolder(in bitMask, archetypeEnttSparset));
			}
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void FillArchetypeCollection(in BitMask archetypeMask, SparseSet sparseSet)
		{
			foreach (var entityInfo in _entitiesInfoSparset)
			{
				if (entityInfo.Matches(archetypeMask))
				{
					sparseSet.Add(entityInfo.GetId());
				}
			}
		}


		public interface IWrappedQuery<TTarget> where TTarget : class
		{
		}

		public abstract class AQuery
		{
			protected byte _storeId;
			protected SparseSet _entities;
			protected int[] _archetype;

			protected abstract void CreateArchetype();

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public void GetEntities(List<Entity> entities)
			{
				entities.Clear();
				foreach (var enttId in _entities.GetEntities())
				{
					entities.Add(new Entity(_storeId, enttId));
				}
			}

			public Span<int> GetEntities()
			{
				return _entities.GetEntities();
			}

			public virtual void Init(byte storeId)
			{
				CreateArchetype();
				_entities = GetStorage(storeId).TryGetEntitiesSet(_archetype);
				_storeId = storeId;
			}


			protected int GetCount()
			{
				return _entities.GetCount();
			}

			protected int GetByIndex(int index)
			{
				return _entities.GetByIndex(index);
			}


			public void RetrieveQueryBitMask(ref BitMask reusableTempCompBitmask)
			{
				reusableTempCompBitmask.Clear();
				foreach (var bit in _archetype)
				{
					reusableTempCompBitmask.SetBit(bit);
				}
			}
		}

		private class ArchetypesHolder
		{
			private BitMask _bitMask;
			private SparseSet _entities;

			public ArchetypesHolder(in BitMask bitMask, SparseSet archetypeEnttSparset)
			{
				_bitMask = bitMask;
				_entities = archetypeEnttSparset;
			}

			public ref BitMask GetBitmask() => ref _bitMask;
			public SparseSet GetSparset() => _entities;
		}
	}
}