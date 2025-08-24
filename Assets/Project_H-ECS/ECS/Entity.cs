using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Project_H.ECS
{
	[DebuggerTypeProxy(typeof(EntityDebugView))]
	public readonly struct Entity : IEquatable<Entity>
	{
		public readonly int ID;
		private readonly byte _storageID;

		public byte GetRelatedStoreId() => _storageID;

		public Entity(byte storageID, int id)
		{
			_storageID = storageID;
			ID = id;
		}

		public bool Equals(Entity other)
		{
			return ID == other.ID;
		}

		public override bool Equals(object obj)
		{
			throw new InvalidOperationException("structs not allowed to box");
		}

		public override int GetHashCode()
		{
			return HashCode.Combine(ID, _storageID);
		}
	}

	sealed class EntityDebugView
	{
		private readonly Entity _entity;

		public EntityDebugView(in Entity entity)
		{
			_entity = entity;
			Components = entity.GetComponents();
		}

		public int ID => _entity.ID;

		public byte StoreID => _entity.GetRelatedStoreId();
		public object[] Components { get; }
	}

	public class EntityInfo
	{
		private int _id;
		private byte _storageID;
		private BitMask _mask = new(Storage.MaxComponentsCount);

		public void SetData(int entityID, byte storageID)
		{
			_id = entityID;
			_storageID = storageID;
		}

		public int GetId() => _id;

		public void Reset()
		{
			_id = -1;
			_storageID = 0;
			_mask.Clear();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetBit(int bit)
		{
			_mask.SetBit(bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void ClearBit(int bit)
		{
			_mask.ClearBit(bit);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Matches(in BitMask queryMask)
		{
			return _mask.Matches(in queryMask);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Span<int> GetComponentIndexes()
		{
			return _mask.GetIndexes();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool HasBit(int bit)
		{
			return _mask.HasBit(bit);
		}
	}
}