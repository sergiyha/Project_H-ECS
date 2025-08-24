using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Project_H.ECS;
using Project_H.ECS.Component;
using UnityEngine;
using UnsafeObjectPool;

namespace Project_H.ECS
{
	public static class Extension
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Add<T>(this in Entity entity, in T component) where T : IComponent
		{
			Storage.GetStorage(entity.GetRelatedStoreId()).Add(in entity, in component);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Add<T>(this in Entity entity) where T : IComponent
		{
			if (default(T) == null)
			{
				Debug.LogError("Use another override of Add method for ref types");
				return;
			}

			Storage.GetStorage(entity.GetRelatedStoreId()).Add(in entity, default(T));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool AddOrSet<T>(this in Entity entity, in T component, out T replacedComponent) where T : IComponent
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).AddOrSet<T>(in entity, in component, out replacedComponent);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Set<T>(this in Entity entity, in T component) where T : IComponent
		{
			Storage.GetStorage(entity.GetRelatedStoreId()).Set(in entity, in component);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T Get<T>(this in Entity entity) where T : IComponent
		{
			return ref Storage.GetStorage(entity.GetRelatedStoreId()).Get<T>(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ref T TryGetRef<T>(this in Entity entity, out bool exist) where T : IComponent
		{
			return ref Storage.GetStorage(entity.GetRelatedStoreId()).TryGetRef<T>(in entity, out exist);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryGet<T>(this in Entity entity, out T comp) where T : IComponent
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).TryGet(in entity, out comp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Has<T>(this in Entity entity) where T : IComponent
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).Has<T>(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Remove<T>(this in Entity entity) where T : IComponent
		{
			Storage.GetStorage(entity.GetRelatedStoreId()).Remove<T>(in entity);
		}

		public static void RemoveSilent<T>(this in Entity entity) where T : IComponent
		{
			Storage.GetStorage(entity.GetRelatedStoreId()).Remove<T>(in entity, true);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool Remove<T>(this in Entity entity, out T deletedComp) where T : IComponent
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).Remove(in entity, out deletedComp);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAlive(this in Entity entity)
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).IsAlive(in entity);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static object[] GetComponents(this in Entity entity)
		{
			return Storage.GetStorage(entity.GetRelatedStoreId()).GetEntityComponents(in entity);
		}

		public static void GetComponentTypes(this in Entity entity, List<Type> componentsType)
		{
			componentsType.Clear();
			Storage.GetStorage(entity.GetRelatedStoreId()).GetComponentTypes(in entity, componentsType);
		}
	}
}