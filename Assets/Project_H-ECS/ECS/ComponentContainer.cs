using System;
using System.Runtime.CompilerServices;
using Project_H.ECS;
using Project_H.ECS.Component;

namespace Project_H.ECS
{
	public interface IComponentContainer
	{
		bool Has(in Entity entity);

		public Type GetCompType();
		public void Remove(in Entity entity);
		public object GetComponentObj(in Entity entity);
	}

	public class ComponentContainer<T> : IComponentContainer where T : IComponent
	{
		private ValuedSparseSet<T> _valuedSparseSet = new(Storage.EntityChunkCount, Storage.ChunkSize);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref T Get(in Entity entity) => ref _valuedSparseSet.GetRef(in entity.ID);

		public object GetComponentObj(in Entity entity) => _valuedSparseSet.GetRef(in entity.ID);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Add(in Entity entityId, in T component) => _valuedSparseSet.Add(entityId.ID, component);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Set(in Entity entity, in T component, out T replacedComponent) => _valuedSparseSet.Set(entity.ID, in component, out replacedComponent);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(in Entity entity)
		{
			_valuedSparseSet.Remove(entity.ID, out _);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Remove(in Entity entity, out T removedComponent)
		{
			_valuedSparseSet.Remove(entity.ID, out removedComponent);
		}


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Has(in Entity entity) => _valuedSparseSet.Has(entity.ID);

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Type GetCompType() => typeof(T);
	}
}