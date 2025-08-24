using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Project_H.ECS;
using Project_H.ECS.Component;
using UnityEngine;

namespace Project_H.ECS
{
	public partial class Storage
	{
		#region Static

		public const int MaxComponentsCount = 1024;
		public const int MaxEntitiesCount = 100_000;
		private static Storage[] _storages = new Storage[byte.MaxValue];
		private static byte _storageCount;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Storage GetStorage(byte storage)
		{
			return _storages[storage];
		}

		public static Entity ConstructEntity(int entityId, byte storeId)
		{
			return new Entity(storeId, entityId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static Storage CreateStorage()
		{
			if (_storageCount == byte.MaxValue)
			{
				Debug.LogError("Cannot create more than 256 storages");
				return null;
			}

			var storage = new Storage(_storageCount);
			_storages[_storageCount] = storage;

			_storageCount++;
			return storage;
		}

		#endregion

		private byte _storageId;
		private int _currentIndex = 0;

		private Queue<int> _reusableIndexes = new();

		private Dictionary<BitMask, SparseSet> _archetypesEntity = new();

		private Dictionary<int, List<ArchetypesHolder>> _compBitToArchetypes = new();
		private SparseSet<IComponentContainer> _components = new(MaxComponentsCount);
		private SparseSet<EntityInfo> _entitiesInfoSparset = new(MaxEntitiesCount);


		public Span<int> GetAllEntities()
		{
			return _entitiesInfoSparset.GetAllIDs();
		}


		private Storage(byte id)
		{
			_storageId = id;
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Entity CreateEntity()
		{
			int entityIndex = _currentIndex;
			if (_reusableIndexes.TryDequeue(out int index))
			{
				entityIndex = index;
			}
			else
			{
				if (_currentIndex == int.MaxValue - 1) Debug.LogError("Out of index range");
				_currentIndex++;
			}

			var entity = new Entity(_storageId, entityIndex);
			var entityInfo = UnsafeObjectPool.ObjectFactory.Get<EntityInfo>();
			_entitiesInfoSparset.Add(entityIndex, entityInfo);
			entityInfo.SetData(entityIndex, _storageId);

			return entity;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RemoveEntity(in Entity entity)
		{
			if (!_entitiesInfoSparset.Has(in entity.ID))
			{
				Debug.LogError("cant remove entity");
				return;
			}

			_entitiesInfoSparset.Remove(entity.ID, out var entityInfo);
			_reusableIndexes.Enqueue(entity.ID);

			//remove from component containers
			var componentIndexes = entityInfo.GetComponentIndexes();

			for (int i = 0; i < componentIndexes.Length; i++)
			{
				var componentIndex = componentIndexes[i];
				var componentContainer = _components.GetRef(in componentIndex);
				componentContainer.Remove(in entity);
			}

			//remove from archetypes
			foreach (var kvp in _archetypesEntity)
			{
				if (entityInfo.Matches(kvp.Key))
				{
					var archetypeEntities = kvp.Value;
					archetypeEntities.Remove(entity.ID);
				}
			}

			entityInfo.Reset();
			UnsafeObjectPool.ObjectFactory.Return(entityInfo);
		}


		public byte GetStorageId()
		{
			return _storageId;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has<T>(in Entity entity) where T : IComponent
		{
			var compIndex = ComponentTypeRegistry.GetBitIndex(typeof(T));
			var compContainer = GetOrCreateComponentContainer<T>(compIndex);
			return compContainer.Has(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get<T>(in Entity entity) where T : IComponent
		{
			var compIndex = ComponentTypeRegistry.GetBitIndex(typeof(T));
			var compContainer = GetOrCreateComponentContainer<T>(compIndex);
			if (!compContainer.Has(in entity))
			{
				throw new NullReferenceException($"Cannot get component from entity {entity.ID} type:{typeof(T)}");
			}

			return ref compContainer.Get(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T TryGetRef<T>(in Entity entity, out bool exist) where T : IComponent
		{
			var compIndex = ComponentTypeRegistry.GetBitIndex(typeof(T));
			var compContainer = GetOrCreateComponentContainer<T>(compIndex);
			if (!compContainer.Has(in entity))
			{
				exist = false;
				return ref Unsafe.NullRef<T>();
			}

			exist = true;
			return ref compContainer.Get(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool AddOrSet<T>(in Entity entity, in T component, out T replacedComponent) where T : IComponent
		{
			var typeT = typeof(T);
			int bitT = ComponentTypeRegistry.GetBitIndex(typeT);
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);

			if (entityInfo.HasBit(bitT))
			{
				SetInternal(bitT, entityInfo, in entity, in component, out replacedComponent);
				return true;
			}

			AddInternal(bitT, entityInfo, in entity, in component);
			replacedComponent = default;
			return false;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add<T>(in Entity entity, in T component) where T : IComponent
		{
			var typeT = typeof(T);
			int bitT = ComponentTypeRegistry.GetBitIndex(typeT);
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			if (entityInfo.HasBit(bitT))
			{
				throw new NullReferenceException($"Cannot ADD component to entity {entity.ID} type:{typeof(T)} already here");
				return;
			}

			AddInternal(bitT, entityInfo, in entity, in component);
		}

		private static int d = 0;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddInternal<T>(int bitT, EntityInfo entityInfo, in Entity entity, in T component) where T : IComponent
		{
			entityInfo.SetBit(bitT);
			var componentContainerObject = GetOrCreateComponentContainer<T>(bitT);
			componentContainerObject.Add(in entity, in component);

			if (_compBitToArchetypes.TryGetValue(bitT, out var archetypes))
			{
				for (var i = 0; i < archetypes.Count; i++)
				{
					var archetype = archetypes[i];
					ref var archetypeMask = ref archetype.GetBitmask();
					if (entityInfo.Matches(in archetypeMask))
					{
						archetype.GetSparset().Add(entity.ID);
					}
				}
			}
		}

		public ComponentContainer<T> GetOrCreateComponentContainer<T>(int compBitIdx) where T : IComponent
		{
			if (!_components.TryGet(compBitIdx, out var componentContainerObject))
			{
				componentContainerObject = new ComponentContainer<T>();
				_components.Add(compBitIdx, componentContainerObject);
			}

			return (ComponentContainer<T>)componentContainerObject;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove<T>(in Entity entity, bool silenced = false) where T : IComponent
		{
			var type = typeof(T);
			int bitT = ComponentTypeRegistry.GetBitIndex(type);
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			if (!entityInfo.HasBit(bitT))
			{
				if (!silenced)
				{
					throw new NullReferenceException($"Cannot remove component it was not added {type} entityId: {entity.ID}");
				}

				return;
			}


			if (_compBitToArchetypes.TryGetValue(bitT, out var archetypes))
			{
				for (var i = 0; i < archetypes.Count; i++)
				{
					var archetype = archetypes[i];
					ref var archetypeBitmask = ref archetype.GetBitmask();
					if (!entityInfo.Matches(in archetypeBitmask)) continue;
					archetype.GetSparset().Remove(entity.ID);
				}
			}

			_entitiesInfoSparset.GetRef(entity.ID).ClearBit(bitT);
			_components.GetRef(bitT).Remove(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Remove<T>(in Entity entity, out T removedComponent) where T : IComponent
		{
			var type = typeof(T);
			int bitT = ComponentTypeRegistry.GetBitIndex(type);
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			if (!entityInfo.HasBit(bitT))
			{
				removedComponent = default;
				return false;
			}

			if (_compBitToArchetypes.TryGetValue(bitT, out var archetypes))
			{
				foreach (var archetype in archetypes)
				{
					ref var archetypeBitmask = ref archetype.GetBitmask();
					if (!entityInfo.Matches(in archetypeBitmask)) continue;
					archetype.GetSparset().Remove(entity.ID);
				}
			}

			_entitiesInfoSparset.GetRef(entity.ID).ClearBit(bitT);
			GetOrCreateComponentContainer<T>(bitT).Remove(in entity, out removedComponent);
			return true;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set<T>(in Entity entity, in T component) where T : IComponent
		{
			var typeT = typeof(T);
			var bitT = ComponentTypeRegistry.GetBitIndex(typeT);
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			if (!entityInfo.HasBit(bitT))
			{
				throw new NullReferenceException($"Cannot set component it was not added {typeT} entityId: {entity.ID}");
				return;
			}

			SetInternal(bitT, entityInfo, in entity, in component, out _);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SetInternal<T>(int bitT, EntityInfo entityInfo, in Entity entity, in T component, out T replacedComponent) where T : IComponent
		{
			var bitCompIdx = ComponentTypeRegistry.GetBitIndex(typeof(T));
			var compContainer = GetOrCreateComponentContainer<T>(bitCompIdx);
			compContainer.Set(entity, in component, out replacedComponent);
		}

		public bool TryGet<T>(in Entity entity, out T comp) where T : IComponent
		{
			var compIndex = ComponentTypeRegistry.GetBitIndex(typeof(T));
			var compContainer = GetOrCreateComponentContainer<T>(compIndex);
			if (compContainer.Has(in entity))
			{
				comp = compContainer.Get(in entity);
				return true;
			}

			comp = default;
			return false;
		}

		public bool IsAlive(in Entity entity)
		{
			return _entitiesInfoSparset.Has(entity.ID);
		}

		public object[] GetEntityComponents(in Entity entity)
		{
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			var componentIndexes = entityInfo.GetComponentIndexes();
			var components = new object[componentIndexes.Length];
			for (int i = 0; i < componentIndexes.Length; i++)
			{
				var componentContainer = _components.GetRef(componentIndexes[i]);
				components[i] = componentContainer.GetComponentObj(in entity);
			}

			return components;
		}

		public void GetComponentTypes(in Entity entity, List<Type> types)
		{
			types.Clear();
			var entityInfo = _entitiesInfoSparset.GetRef(entity.ID);
			var indexes = entityInfo.GetComponentIndexes();
			for (int i = 0; i < indexes.Length; i++)
			{
				var idx = indexes[i];
				var componentContainer = _components.GetRef(in idx);
				types.Add(componentContainer.GetCompType());
			}
			
		}
	}
}