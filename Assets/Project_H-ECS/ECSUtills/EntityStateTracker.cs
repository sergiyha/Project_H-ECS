using System.Runtime.CompilerServices;
using Project_H.ECS.Component;
using UnityEngine;

namespace Project_H.ECS.ECSUtills
{
	public struct EntityStateTracker<T> where T : unmanaged, IComponent
	{
		public T CurrentState => _current;
		public T OldState => _old;

		private T _current;
		private T _old;

		private bool _isOperating;
		private Entity _trackedEntity;
		public Entity TrackedEntity => _trackedEntity;
		private bool _isFirstTime;

		public bool IsChanged()
		{
			if (!_isOperating)
			{
				Debug.LogError("Entity is not Set");
				return false;
			}

			if (!_trackedEntity.IsAlive())
			{
				Debug.LogError("Entity is not Alive");
				_isOperating = false;
				return false;
			}

			if (!_trackedEntity.Has<T>())
			{
				return false;
			}

			if (_isFirstTime) return true;

			return !AreStructsEqual(_old, _current);
		}

		private static unsafe bool AreStructsEqual<T>(T s1, T s2) where T : unmanaged
		{
			int size = sizeof(T);
			byte* p1 = (byte*)Unsafe.AsPointer(ref s1);
			byte* p2 = (byte*)Unsafe.AsPointer(ref s2); 

			for (int i = 0; i < size; i++)
			{
				if (p1[i] != p2[i])
					return false;
			}

			return true;
		}

		public void Subscribe(in Entity entity)
		{
			_isFirstTime = true;
			_trackedEntity = entity;
			_isOperating = true;
		}


		public void Update()
		{
			if (!_isOperating) return;
			if (_trackedEntity.IsAlive())
			{
				var isComponentExist = _trackedEntity.TryGet<T>(out var comp);
				if (isComponentExist)
				{
					_current = comp;
				}
			}
		}

		public void ResetState()
		{
			_isFirstTime = true;
			_old = default;
			_current = _trackedEntity.Get<T>();
		}

		public void Reset(bool resetEntity = true)
		{
			_isFirstTime = true;
			_old = default;
			_current = default;
			_isOperating = false;
			_trackedEntity = default;
		}

		public void Apply()
		{
			if (_isFirstTime)
			{
				_isFirstTime = false;
			}

			_old = _current;
		}
	}
}