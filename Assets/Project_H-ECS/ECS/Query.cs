using System;
using System.Collections;
using System.Collections.Generic;
using Project_H.ECS.Component;
using UnsafeObjectPool;

namespace Project_H.ECS
{
	public class Query<T> : Storage.AQuery, IEnumerable<Query<T>.QueryResult>
		where T : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		private Enumerator _enumerator;
		public delegate void QueryDelegate(Entity e, ref T componentT);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;

			public void Reset()
			{
				entity = default;
				value0 = default;
			}

			public void Deconstruct(out Entity entity, out T component0)
			{
				entity = this.entity;
				component0 = value0;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T> : Query<T>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT);
	}


	public class Query<T, T1> : Storage.AQuery, IEnumerable<Query<T, T1>.QueryResult>
		where T : IComponent
		where T1 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1> : Query<T, T1>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1);
	}


	public class Query<T, T1, T2> : Storage.AQuery, IEnumerable<Query<T, T1, T2>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2> : Query<T, T1, T2>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2);
	}


	public class Query<T, T1, T2, T3> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3> : Query<T, T1, T2, T3>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3);
	}


	public class Query<T, T1, T2, T3, T4> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4> : Query<T, T1, T2, T3, T4>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4);
	}


	public class Query<T, T1, T2, T3, T4, T5> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5> : Query<T, T1, T2, T3, T4, T5>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5);
	}


	public class Query<T, T1, T2, T3, T4, T5, T6> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5, T6>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		protected ComponentContainer<T6> _componentContainerT6;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5, ref T6 componentT6);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
			_componentContainerT6 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T6>(ComponentTypeRegistry.GetBitIndex(typeof(T6)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5)),
				ComponentTypeRegistry.GetBitIndex(typeof(T6))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;
			public T6 value6;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5, out T6 component6)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
				component6 = value6;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5, T6> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					_result.value6 = _query._componentContainerT6.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5, T6> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5, T6> : Query<T, T1, T2, T3, T4, T5, T6>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5, ref T6 componentT6);
	}


	public class Query<T, T1, T2, T3, T4, T5, T6, T7> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5, T6, T7>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		protected ComponentContainer<T6> _componentContainerT6;
		protected ComponentContainer<T7> _componentContainerT7;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5, ref T6 componentT6, ref T7 componentT7);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
			_componentContainerT6 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T6>(ComponentTypeRegistry.GetBitIndex(typeof(T6)));
			_componentContainerT7 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T7>(ComponentTypeRegistry.GetBitIndex(typeof(T7)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5)),
				ComponentTypeRegistry.GetBitIndex(typeof(T6)),
				ComponentTypeRegistry.GetBitIndex(typeof(T7))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;
			public T6 value6;
			public T7 value7;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5, out T6 component6, out T7 component7)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
				component6 = value6;
				component7 = value7;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5, T6, T7> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					_result.value6 = _query._componentContainerT6.Get(in entity);
					_result.value7 = _query._componentContainerT7.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5, T6, T7> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5, T6, T7> : Query<T, T1, T2, T3, T4, T5, T6, T7>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5, ref T6 componentT6, ref T7 componentT7);
	}


	public class Query<T, T1, T2, T3, T4, T5, T6, T7, T8> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5, T6, T7, T8>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		protected ComponentContainer<T6> _componentContainerT6;
		protected ComponentContainer<T7> _componentContainerT7;
		protected ComponentContainer<T8> _componentContainerT8;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
			_componentContainerT6 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T6>(ComponentTypeRegistry.GetBitIndex(typeof(T6)));
			_componentContainerT7 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T7>(ComponentTypeRegistry.GetBitIndex(typeof(T7)));
			_componentContainerT8 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T8>(ComponentTypeRegistry.GetBitIndex(typeof(T8)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5)),
				ComponentTypeRegistry.GetBitIndex(typeof(T6)),
				ComponentTypeRegistry.GetBitIndex(typeof(T7)),
				ComponentTypeRegistry.GetBitIndex(typeof(T8))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;
			public T6 value6;
			public T7 value7;
			public T8 value8;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5, out T6 component6, out T7 component7, out T8 component8)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
				component6 = value6;
				component7 = value7;
				component8 = value8;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5, T6, T7, T8> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					_result.value6 = _query._componentContainerT6.Get(in entity);
					_result.value7 = _query._componentContainerT7.Get(in entity);
					_result.value8 = _query._componentContainerT8.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5, T6, T7, T8> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5, T6, T7, T8> : Query<T, T1, T2, T3, T4, T5, T6, T7, T8>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8);
	}


	public class Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
		where T9 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		protected ComponentContainer<T6> _componentContainerT6;
		protected ComponentContainer<T7> _componentContainerT7;
		protected ComponentContainer<T8> _componentContainerT8;
		protected ComponentContainer<T9> _componentContainerT9;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8, ref T9 componentT9);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
			_componentContainerT6 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T6>(ComponentTypeRegistry.GetBitIndex(typeof(T6)));
			_componentContainerT7 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T7>(ComponentTypeRegistry.GetBitIndex(typeof(T7)));
			_componentContainerT8 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T8>(ComponentTypeRegistry.GetBitIndex(typeof(T8)));
			_componentContainerT9 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T9>(ComponentTypeRegistry.GetBitIndex(typeof(T9)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5)),
				ComponentTypeRegistry.GetBitIndex(typeof(T6)),
				ComponentTypeRegistry.GetBitIndex(typeof(T7)),
				ComponentTypeRegistry.GetBitIndex(typeof(T8)),
				ComponentTypeRegistry.GetBitIndex(typeof(T9))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity), ref _componentContainerT9.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;
			public T6 value6;
			public T7 value7;
			public T8 value8;
			public T9 value9;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5, out T6 component6, out T7 component7, out T8 component8, out T9 component9)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
				component6 = value6;
				component7 = value7;
				component8 = value8;
				component9 = value9;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> _query;
			private int _index;
			private QueryResult _result;

			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					_result.value6 = _query._componentContainerT6.Get(in entity);
					_result.value7 = _query._componentContainerT7.Get(in entity);
					_result.value8 = _query._componentContainerT8.Get(in entity);
					_result.value9 = _query._componentContainerT9.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5, T6, T7, T8, T9> : Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9>, Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
		where T9 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity), ref _componentContainerT9.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8, ref T9 componentT9);
	}


	public class Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Storage.AQuery, IEnumerable<Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>.QueryResult>
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
		where T9 : IComponent
		where T10 : IComponent
	{
		protected ComponentContainer<T> _componentContainerT;
		protected ComponentContainer<T1> _componentContainerT1;
		protected ComponentContainer<T2> _componentContainerT2;
		protected ComponentContainer<T3> _componentContainerT3;
		protected ComponentContainer<T4> _componentContainerT4;
		protected ComponentContainer<T5> _componentContainerT5;
		protected ComponentContainer<T6> _componentContainerT6;
		protected ComponentContainer<T7> _componentContainerT7;
		protected ComponentContainer<T8> _componentContainerT8;
		protected ComponentContainer<T9> _componentContainerT9;
		protected ComponentContainer<T10> _componentContainerT10;
		private Enumerator _enumerator;

		public delegate void QueryDelegate(Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3, ref T4 componentT4,
			ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8, ref T9 componentT9, ref T10 componentT10);

		public override void Init(byte storeId)
		{
			base.Init(storeId);
			_componentContainerT = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T>(ComponentTypeRegistry.GetBitIndex(typeof(T)));
			_componentContainerT1 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T1>(ComponentTypeRegistry.GetBitIndex(typeof(T1)));
			_componentContainerT2 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T2>(ComponentTypeRegistry.GetBitIndex(typeof(T2)));
			_componentContainerT3 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T3>(ComponentTypeRegistry.GetBitIndex(typeof(T3)));
			_componentContainerT4 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T4>(ComponentTypeRegistry.GetBitIndex(typeof(T4)));
			_componentContainerT5 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T5>(ComponentTypeRegistry.GetBitIndex(typeof(T5)));
			_componentContainerT6 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T6>(ComponentTypeRegistry.GetBitIndex(typeof(T6)));
			_componentContainerT7 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T7>(ComponentTypeRegistry.GetBitIndex(typeof(T7)));
			_componentContainerT8 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T8>(ComponentTypeRegistry.GetBitIndex(typeof(T8)));
			_componentContainerT9 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T9>(ComponentTypeRegistry.GetBitIndex(typeof(T9)));
			_componentContainerT10 = Storage.GetStorage(storeId).GetOrCreateComponentContainer<T10>(ComponentTypeRegistry.GetBitIndex(typeof(T10)));
		}

		protected override void CreateArchetype()
		{
			_archetype = new[]
			{
				ComponentTypeRegistry.GetBitIndex(typeof(T)),
				ComponentTypeRegistry.GetBitIndex(typeof(T1)),
				ComponentTypeRegistry.GetBitIndex(typeof(T2)),
				ComponentTypeRegistry.GetBitIndex(typeof(T3)),
				ComponentTypeRegistry.GetBitIndex(typeof(T4)),
				ComponentTypeRegistry.GetBitIndex(typeof(T5)),
				ComponentTypeRegistry.GetBitIndex(typeof(T6)),
				ComponentTypeRegistry.GetBitIndex(typeof(T7)),
				ComponentTypeRegistry.GetBitIndex(typeof(T8)),
				ComponentTypeRegistry.GetBitIndex(typeof(T9)),
				ComponentTypeRegistry.GetBitIndex(typeof(T10))
			};
		}

		public void ExecuteOn(QueryDelegate action)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				action?.Invoke(entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity), ref _componentContainerT9.Get(in entity), ref _componentContainerT10.Get(in entity));
			}
		}

		public Enumerator GetEnumerator()
		{
			_enumerator ??= new Enumerator(this);
			_enumerator.Reset();
			return _enumerator;
		}

		IEnumerator<QueryResult> IEnumerable<QueryResult>.GetEnumerator() => GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public class QueryResult
		{
			public Entity entity;
			public T value0;
			public T1 value1;
			public T2 value2;
			public T3 value3;
			public T4 value4;
			public T5 value5;
			public T6 value6;
			public T7 value7;
			public T8 value8;
			public T9 value9;
			public T10 value10;

			public void Reset()
			{
				entity = default;
				value0 = default;
				value1 = default;
				value2 = default;
				value3 = default;
				value4 = default;
				value5 = default;
				value6 = default;
				value7 = default;
				value8 = default;
				value9 = default;
				value10 = default;
			}

			public void Deconstruct(out Entity entity, out T component0, out T1 component1, out T2 component2, out T3 component3, out T4 component4,
				out T5 component5, out T6 component6, out T7 component7, out T8 component8, out T9 component9, out T10 component10)
			{
				entity = this.entity;
				component0 = value0;
				component1 = value1;
				component2 = value2;
				component3 = value3;
				component4 = value4;
				component5 = value5;
				component6 = value6;
				component7 = value7;
				component8 = value8;
				component9 = value9;
				component10 = value10;
			}
		}

		public class Enumerator : IEnumerator<QueryResult>
		{
			private Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> _query;
			private int _index;
			private QueryResult _result;
			
			public bool MoveNext()
			{
				if (++_index < _query.GetCount())
				{
					return true;
				}

				Reset();
				return false;
			}

			public void Reset()
			{
				_result.Reset();
				_index = -1;
			}

			public QueryResult Current
			{
				get
				{
					var entity = Storage.ConstructEntity(_query.GetByIndex(_index), _query._storeId);
					_result.entity = entity;
					_result.value0 = _query._componentContainerT.Get(in entity);
					_result.value1 = _query._componentContainerT1.Get(in entity);
					_result.value2 = _query._componentContainerT2.Get(in entity);
					_result.value3 = _query._componentContainerT3.Get(in entity);
					_result.value4 = _query._componentContainerT4.Get(in entity);
					_result.value5 = _query._componentContainerT5.Get(in entity);
					_result.value6 = _query._componentContainerT6.Get(in entity);
					_result.value7 = _query._componentContainerT7.Get(in entity);
					_result.value8 = _query._componentContainerT8.Get(in entity);
					_result.value9 = _query._componentContainerT9.Get(in entity);
					_result.value10 = _query._componentContainerT10.Get(in entity);
					return _result;
				}
			}

			object IEnumerator.Current => throw new InvalidOperationException("struct cannot be boxed");

			public void Dispose()
			{
			}

			public Enumerator(Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> queryResults)
			{
				_query = queryResults;
				_index = -1;
				_result = new QueryResult();
			}
		}
	}

	public class WrappedQuery<TTarget, T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> : Query<T, T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>,
		Storage.IWrappedQuery<TTarget>
		where TTarget : class
		where T : IComponent
		where T1 : IComponent
		where T2 : IComponent
		where T3 : IComponent
		where T4 : IComponent
		where T5 : IComponent
		where T6 : IComponent
		where T7 : IComponent
		where T8 : IComponent
		where T9 : IComponent
		where T10 : IComponent
	{
		public void ExecuteOn(WrappedQueryDelegate @delegate, TTarget target)
		{
			var entities = _entities.GetEntities();
			var count = GetCount();
			for (int i = 0; i < count; i++)
			{
				var entity = Storage.ConstructEntity(entities[i], _storeId);
				@delegate?.Invoke(target, entity, ref _componentContainerT.Get(in entity), ref _componentContainerT1.Get(in entity),
					ref _componentContainerT2.Get(in entity), ref _componentContainerT3.Get(in entity), ref _componentContainerT4.Get(in entity),
					ref _componentContainerT5.Get(in entity), ref _componentContainerT6.Get(in entity), ref _componentContainerT7.Get(in entity),
					ref _componentContainerT8.Get(in entity), ref _componentContainerT9.Get(in entity), ref _componentContainerT10.Get(in entity));
			}
		}

		public delegate void WrappedQueryDelegate(TTarget target, Entity e, ref T componentT, ref T1 componentT1, ref T2 componentT2, ref T3 componentT3,
			ref T4 componentT4, ref T5 componentT5, ref T6 componentT6, ref T7 componentT7, ref T8 componentT8, ref T9 componentT9, ref T10 componentT10);
	}
}