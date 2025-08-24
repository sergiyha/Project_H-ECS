using System;
using System.Collections.Generic;


namespace UnsafeObjectPool
{
	public class ObjectPool<T> where T : new()
	{
		private Stack<T> _objects = new();

		public T Get()
		{
			if (_objects.TryPop(out T obj))
			{
				return obj;
			}

			return new T();
		}

		public void Return(T obj)
		{
			_objects.Push(obj);
		}
	}

	public static class ObjectFactory
	{
		private static Dictionary<Type, object> _factories = new();

		public static T Get<T>() where T : new()
		{
			ObjectPool<T> factoryT = null;
			if (_factories.TryGetValue(typeof(T), out var factory))
			{
				factoryT = (ObjectPool<T>)factory;
			}
			else
			{
				factoryT = new ObjectPool<T>();
				_factories[typeof(T)] = factoryT;
			}

			return factoryT.Get();
		}

		public static void Return<T>(T obj) where T : new()
		{
			((ObjectPool<T>)_factories[typeof(T)]).Return(obj);
		}
	}
}