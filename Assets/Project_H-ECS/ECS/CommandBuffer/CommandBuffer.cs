using System;
using System.Collections.Generic;
using Project_H.ECS.Component;
using UnityEngine;

namespace Project_H.ECS
{
	public class CommandBuffer
	{
		private byte _storageId;
		private Queue<ICommandAction> _commandActions = new();

		public CommandBuffer(byte storageId)
		{
			_storageId = storageId;
		}

		public void Add<T>(in Entity entity, in T component) where T : IComponent
		{
			if (_storageId != entity.GetRelatedStoreId()) Debug.LogError($"Wrong storage need: {_storageId} is: {entity.GetRelatedStoreId()}");
			var action = UnsafeObjectPool.ObjectFactory.Get<CommandBufferAction<T>>();
			action.Init(_storageId, entity, component, CommandBufferAction.Add);
			_commandActions.Enqueue(action);
		}

		public void Add<T>(in Entity entity) where T : IComponent
		{
			if (default(T) == null)
			{
				Debug.LogError($"Use another add method for ReffType components - {typeof(T)}");
				return;
			}

			if (_storageId != entity.GetRelatedStoreId()) Debug.LogError($"Wrong storage need: {_storageId} is: {entity.GetRelatedStoreId()}");
			var action = UnsafeObjectPool.ObjectFactory.Get<CommandBufferAction<T>>();
			action.Init(_storageId, entity, default, CommandBufferAction.Add);
			_commandActions.Enqueue(action);
		}


		public void Remove<T>(in Entity entity) where T : IComponent
		{
			if (_storageId != entity.GetRelatedStoreId()) Debug.LogError($"Wrong storage need: {_storageId} is: {entity.GetRelatedStoreId()}");
			var action = UnsafeObjectPool.ObjectFactory.Get<CommandBufferAction<T>>();
			action.Init(_storageId, entity, default, CommandBufferAction.Remove);
			_commandActions.Enqueue(action);
		}

		public void RemoveEntity(in Entity entity)
		{
			if (_storageId != entity.GetRelatedStoreId()) Debug.LogError($"Wrong storage need: {_storageId} is: {entity.GetRelatedStoreId()}");
			var action = UnsafeObjectPool.ObjectFactory.Get<RemoveEntityAction>();
			action.Init(in entity);
			_commandActions.Enqueue(action);
		}

		public enum CommandBufferAction
		{
			None,
			Add,
			Remove,
			RemoveEntity,
		}

		private interface ICommandAction
		{
			public void Playback();
			public void Reset();
		}

		public void Playback()
		{
			while (_commandActions.TryDequeue(out ICommandAction action))
			{
				action.Playback();
				action.Reset();
			}
		}


		private class RemoveEntityAction : ICommandAction
		{
			private Entity _entity;
			private bool _inited = false;

			public void Init(in Entity entity)
			{
				_inited = true;
				_entity = entity;
			}

			public void Playback()
			{
				if (_inited == false)
				{
					Debug.LogError("Cannot be deleted - action was not initialized");
					return;
				}

				Storage.GetStorage(_entity.GetRelatedStoreId()).RemoveEntity(in _entity);
			}

			public void Reset()
			{
				_inited = false;
				_entity = default;
				UnsafeObjectPool.ObjectFactory.Return(this);
			}
		}

		private class CommandBufferAction<T> : ICommandAction where T : IComponent
		{
			private CommandBufferAction _action;
			private byte _storageID;
			private Entity _entity;
			private T _component;

			public void Init(byte storageId, in Entity entity, in T component, CommandBufferAction commandBufferAction)
			{
				_action = commandBufferAction;
				_storageID = storageId;
				_entity = entity;
				_component = component;
			}

			public void Reset()
			{
				_action = CommandBufferAction.None;
				_component = default;
				_entity = default;
				_storageID = byte.MaxValue;
				UnsafeObjectPool.ObjectFactory.Return(this);
			}

			public void Playback()
			{
				switch (_action)
				{
					case CommandBufferAction.None:
						Debug.LogError("CannotBeNONE");
						break;
					case CommandBufferAction.Add:
						_entity.AddOrSet<T>(_component, out _);
						break;
					case CommandBufferAction.Remove:
						_entity.RemoveSilent<T>();
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
		}
	}
}