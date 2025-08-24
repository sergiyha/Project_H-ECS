using System.Collections.Generic;
using Project_H.ECS.ECSUtills;

namespace Project_H.ECSUtills
{
	public struct StateTracker<T> where T : unmanaged
	{
		private T _old;
		private T _current;

		public bool IsChanged()
		{
			return !EqualityComparer<T>.Default.Equals(_old, _current);
		}

		public void Reset()
		{
			_old = default;
			_current = default;
		}

		public void Apply()
		{
			_old = _current;
		}

		public void Update(T state)
		{
			_current = state;
		}

		public T GetOldState() => _old;
		public T GetCurrentState() => _current;
	}

	public static class StateTrackerExtensions
	{
		public static void Toggle(this ref StateTracker<bool> boolStateTracker)
		{
			boolStateTracker.Update(!boolStateTracker.GetOldState());
		}

		public static bool IsCurrentZero(this ref StateTracker<float> floatStateTracker)
		{
			return floatStateTracker.GetCurrentState() == 0;
		}

		public static bool IsOldZero(this ref StateTracker<float> floatStateTracker)
		{
			return floatStateTracker.GetOldState() == 0;
		}
		
	}
}