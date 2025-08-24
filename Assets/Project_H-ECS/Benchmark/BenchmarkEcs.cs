using System.Collections.Generic;
using System.Diagnostics;
using Project_H.ECS.Component;
using Project_H.ECS.ECSUtills;
using UnityEngine;
using UnityEngine.Profiling;
using Debug = UnityEngine.Debug;

namespace Project_H.ECS
{
	#region USINGS

	using Query1 = Query
	<
		BenchmarkEcs.Component1
	>;
	using WrappedQuery1 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1
	>;
	using Query2 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2
	>;
	using WrappedQuery2 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2
	>;
	using Query3 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3
	>;
	using WrappedQuery3 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3
	>;
	using Query4 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4
	>;
	using WrappedQuery4 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4
	>;
	using Query5 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5
	>;
	using WrappedQuery5 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5
	>;
	using Query6 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6
	>;
	using WrappedQuery6 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6
	>;
	using Query7 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7
	>;
	using WrappedQuery7 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7
	>;
	using Query8 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8
	>;
	using WrappedQuery8 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8
	>;
	using Query9 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8,
		BenchmarkEcs.Component9
	>;
	using WrappedQuery9 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8,
		BenchmarkEcs.Component9
	>;
	using Query10 = Query
	<
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8,
		BenchmarkEcs.Component9,
		BenchmarkEcs.Component10
	>;
	using WrappedQuery10 = WrappedQuery
	<
		BenchmarkEcs,
		BenchmarkEcs.Component1,
		BenchmarkEcs.Component2,
		BenchmarkEcs.Component3,
		BenchmarkEcs.Component4,
		BenchmarkEcs.Component5,
		BenchmarkEcs.Component6,
		BenchmarkEcs.Component7,
		BenchmarkEcs.Component8,
		BenchmarkEcs.Component9,
		BenchmarkEcs.Component10
	>;

	#endregion

	public class BenchmarkEcs : MonoBehaviour
	{
		private List<Entity> _entities = new List<Entity>();
		private Stopwatch stopwatch;
		private int i;

		private Storage storage;

		private Query1 _query1;
		private Query2 _query2;
		private Query3 _query3;
		private Query4 _query4;
		private Query5 _query5;
		private Query6 _query6;
		private Query7 _query7;
		private Query8 _query8;
		private Query9 _query9;
		private Query10 _query10;

		private WrappedQuery1 _wrappedQuery1;
		private WrappedQuery2 _wrappedQuery2;
		private WrappedQuery3 _wrappedQuery3;
		private WrappedQuery4 _wrappedQuery4;
		private WrappedQuery5 _wrappedQuery5;
		private WrappedQuery6 _wrappedQuery6;
		private WrappedQuery7 _wrappedQuery7;
		private WrappedQuery8 _wrappedQuery8;
		private WrappedQuery9 _wrappedQuery9;
		private WrappedQuery10 _wrappedQuery10;

		#region DELEGATES

		private WrappedQuery1.WrappedQueryDelegate _wq1d = OnWQ1D;

		private static void OnWQ1D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt
		)
		{
		}

		private Query1.QueryDelegate _q1d = OnQ1;

		private static void OnQ1(Entity e, ref Component1 componentt)
		{
		}

		private WrappedQuery2.WrappedQueryDelegate _wq2d = OnWQ2D;

		private static void OnWQ2D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1
		)
		{
		}

		private Query2.QueryDelegate _q2d = OnQ2;

		private static void OnQ2
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1
		)
		{
		}

		private WrappedQuery3.WrappedQueryDelegate _wq3d = OnWQ3D;

		private static void OnWQ3D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2
		)
		{
		}

		private Query3.QueryDelegate _q3d = OnQ3;

		private static void OnQ3
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2
		)
		{
		}

		private WrappedQuery4.WrappedQueryDelegate _wq4d = OnWQ4D;

		private static void OnWQ4D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3
		)
		{
		}

		private Query4.QueryDelegate _q4d = OnQ4;

		private static void OnQ4
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3
		)
		{
		}

		private WrappedQuery5.WrappedQueryDelegate _wq5d = OnWQ5D;

		private static void OnWQ5D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4
		)
		{
		}

		private Query5.QueryDelegate _q5d = OnQ5;

		private static void OnQ5
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4
		)
		{
		}

		private WrappedQuery6.WrappedQueryDelegate _wq6d = OnWQ6D;

		private static void OnWQ6D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5
		)
		{
		}

		private Query6.QueryDelegate _q6d = OnQ6;

		private static void OnQ6
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5
		)
		{
		}

		private WrappedQuery7.WrappedQueryDelegate _wq7d = OnWQ7D;

		private static void OnWQ7D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6
		)
		{
		}

		private Query7.QueryDelegate _q7d = OnQ7;

		private static void OnQ7
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6)
		{
		}

		private WrappedQuery8.WrappedQueryDelegate _wq8d = OnWQ8D;

		private static void OnWQ8D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7
		)
		{
		}

		private WrappedQuery9.WrappedQueryDelegate _wq9d = OnWQ9D;

		private static void OnWQ9D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7,
			ref Component9 componentt8
		)
		{
		}

		private Query8.QueryDelegate _q8d = OnQ8;

		private static void OnQ8
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7
		)
		{
		}

		private WrappedQuery10.WrappedQueryDelegate _wq10d = OnWQ10D;

		private static void OnWQ10D
		(
			BenchmarkEcs target,
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7,
			ref Component9 componentt8,
			ref Component10 componentt9
		)
		{
		}

		private Query9.QueryDelegate _q9d = OnQ10;

		private static void OnQ10
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7,
			ref Component9 componentt8
		)
		{
		}

		private Query10.QueryDelegate _q10d = OnQ11;

		private static void OnQ11
		(
			Entity e,
			ref Component1 componentt,
			ref Component2 componentt1,
			ref Component3 componentt2,
			ref Component4 componentt3,
			ref Component5 componentt4,
			ref Component6 componentt5,
			ref Component7 componentt6,
			ref Component8 componentt7,
			ref Component9 componentt8,
			ref Component10 componentt9
		)
		{
		}

		#endregion

		#region Create

		private void Create1()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
		}

		private void Create2()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
		}

		private void Create3()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
		}

		private void Create4()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
		}

		private void Create5()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
		}

		private void Create6()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
			entity.Add(new Component6());
		}

		private void Create7()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
			entity.Add(new Component6());
			entity.Add(new Component7());
		}

		private void Create8()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
			entity.Add(new Component6());
			entity.Add(new Component7());
			entity.Add(new Component8());
		}

		private void Create9()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
			entity.Add(new Component6());
			entity.Add(new Component7());
			entity.Add(new Component8());
			entity.Add(new Component9());
		}

		private void Create10()
		{
			var entity = storage.CreateEntity();
			entity.Add(new Component1());
			entity.Add(new Component2());
			entity.Add(new Component3());
			entity.Add(new Component4());
			entity.Add(new Component5());
			entity.Add(new Component6());
			entity.Add(new Component7());
			entity.Add(new Component8());
			entity.Add(new Component9());
			entity.Add(new Component10());
		}

		#endregion

		private Entity _changeableEntity;
		private EntityStateTracker<ChangeableComponent> _componentTracker;

		private void CreateQueries()
		{
			_query1 = storage.GetQuery<Query1>();
			_query2 = storage.GetQuery<Query2>();
			_query3 = storage.GetQuery<Query3>();
			_query4 = storage.GetQuery<Query4>();
			_query5 = storage.GetQuery<Query5>();
			_query6 = storage.GetQuery<Query6>();
			_query7 = storage.GetQuery<Query7>();
			_query8 = storage.GetQuery<Query8>();
			_query9 = storage.GetQuery<Query9>();
			_query10 = storage.GetQuery<Query10>();

			_wrappedQuery1 = storage.GetQuery<WrappedQuery1>();
			_wrappedQuery2 = storage.GetQuery<WrappedQuery2>();
			_wrappedQuery3 = storage.GetQuery<WrappedQuery3>();
			_wrappedQuery4 = storage.GetQuery<WrappedQuery4>();
			_wrappedQuery5 = storage.GetQuery<WrappedQuery5>();
			_wrappedQuery6 = storage.GetQuery<WrappedQuery6>();
			_wrappedQuery7 = storage.GetQuery<WrappedQuery7>();
			_wrappedQuery8 = storage.GetQuery<WrappedQuery8>();
			_wrappedQuery9 = storage.GetQuery<WrappedQuery9>();
			_wrappedQuery10 = storage.GetQuery<WrappedQuery10>();
		}

		private void IterateQueries()
		{
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q1");
			foreach (var queryResult in _query1)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q2");
			foreach (var queryResult in _query2)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q3");
			foreach (var queryResult in _query3)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q4");
			foreach (var queryResult in _query4)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q5");
			foreach (var queryResult in _query5)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q6");
			foreach (var queryResult in _query6)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q7");
			foreach (var queryResult in _query7)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q8");
			foreach (var queryResult in _query8)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q9");
			foreach (var queryResult in _query9)
			{
			}

			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_SIMPLE_Q10");
			foreach (var queryResult in _query10)
			{
			}

			Profiler.EndSample();


			//_query10.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q10_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q10");
			foreach (var (e, component1, component2, component3, component4, component5, component6, component7, component8, component9, component10) in
			         _query10)
			{
			}

			Profiler.EndSample();

			//_query9.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q9_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q9");
			foreach (var (e, component1, component2, component3, component4, component5, component6, component7, component8, component9) in _query9)
			{
			}

			Profiler.EndSample();

			//_query8.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q8_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q8");
			foreach (var (e, component1, component2, component3, component4, component5, component6, component7, component8) in _query8)
			{
			}

			Profiler.EndSample();

			//_query7.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q7_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q7");
			foreach (var (entityT, component, component1, component2, component3, component4, component5, component6) in _query7)
			{
			}

			Profiler.EndSample();

			//_query6.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q6_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q6");
			foreach (var (e, component1, component2, component3, component4, component5, component6) in _query6)
			{
			}

			Profiler.EndSample();

			//_query5.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q5_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECONSTRUCT_Q5");
			foreach (var (e, component1, component2, component3, component4, component5) in _query5)
			{
			}

			Profiler.EndSample();

			//_query4.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q4_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DECONSTRUCT_FOREACH_Q4");
			foreach (var (e, component1, component2, component3, component4) in _query4)
			{
			}

			Profiler.EndSample();

			//_query3.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q3_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DECONSTRUCT_FOREACH_Q3");
			foreach (var (e, component1, component2, component3) in _query3)
			{
			}

			Profiler.EndSample();

			//_query2.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q2_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DECONSTRUCT_FOREACH_Q2");
			foreach (var (e, component1, component2) in _query2)
			{
			}

			Profiler.EndSample();

			//_query1.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_FOREACH_Q1_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_FOREACH_DECOUSTRUCT_Q1");
			foreach (var (e, component1) in _query1)
			{
			}

			Profiler.EndSample();


			//_query1.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q1_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q1");
			_query1.ExecuteOn(_q1d);
			Profiler.EndSample();

			//_query2.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q2_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q2");
			_query2.ExecuteOn(_q2d);
			Profiler.EndSample();

			//_query3.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q3_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q3");
			_query3.ExecuteOn(_q3d);
			Profiler.EndSample();

			//_query4.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q4_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q4");
			_query4.ExecuteOn(_q4d);
			Profiler.EndSample();

			//_query5.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q5_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q5");
			_query5.ExecuteOn(_q5d);
			Profiler.EndSample();

			//_query6.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q6_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q6");
			_query6.ExecuteOn(_q6d);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q7");
			_query7.ExecuteOn(_q7d);
			Profiler.EndSample();

			//_query8.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q8_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q8");
			_query8.ExecuteOn(_q8d);
			Profiler.EndSample();

			//_query9.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q9_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q9");
			_query9.ExecuteOn(_q9d);
			Profiler.EndSample();

			//_query10.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___ITERATE_DELEGATE_Q10_ENTITIES_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ITERATE_DELEGATE_Q10");
			_query10.ExecuteOn(_q10d);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q1");
			_wrappedQuery1.ExecuteOn(_wq1d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q2");
			_wrappedQuery2.ExecuteOn(_wq2d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q3");
			_wrappedQuery3.ExecuteOn(_wq3d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q4");
			_wrappedQuery4.ExecuteOn(_wq4d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q5");
			_wrappedQuery5.ExecuteOn(_wq5d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q6");
			_wrappedQuery6.ExecuteOn(_wq6d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q7");
			_wrappedQuery7.ExecuteOn(_wq7d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q8");
			_wrappedQuery8.ExecuteOn(_wq8d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q9");
			_wrappedQuery9.ExecuteOn(_wq9d, this);
			Profiler.EndSample();

			Profiler.BeginSample("SAMPLE___ITERATE_WRAPPED_QUERY_DELEGATE_Q10");
			_wrappedQuery10.ExecuteOn(_wq10d, this);
			Profiler.EndSample();
		}


		private int _createCount = 1000;

		private void CreateEntities()
		{
			for (int i = 0; i < _createCount; i++)
			{
				Create1();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create2();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create3();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create4();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create5();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create6();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create7();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create8();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create9();
			}

			for (int i = 0; i < _createCount; i++)
			{
				Create10();
			}
		}


		void Start()
		{ 
			stopwatch = Stopwatch.StartNew();
			storage = Storage.CreateStorage();
			stopwatch.Stop();
			Debug.LogError($"Create storage: {stopwatch.ElapsedMilliseconds}");

			_changeableEntity = storage.CreateEntity();
			_changeableEntity.Add(new ChangeableComponent());
			_componentTracker.Subscribe(in _changeableEntity);

			CreateEntities();
			CreateQueries();
		}
		
		private void CheckComponent()
		{
			ref var comp = ref _changeableEntity.Get<ChangeableComponent>();
			comp.INT++;
			comp.BOOL = !comp.BOOL;
			_componentTracker.Update();
			if (_componentTracker.IsChanged())
			{
				//COMMENTED FOR PERFORMANCE REASON
				// Debug.LogError($"old: {_componentTracker.OldState.INT} {_componentTracker.OldState.BOOL} \n" +
				//                $"new: {_componentTracker.CurrentState.INT} {_componentTracker.CurrentState.BOOL}");
			}

			_componentTracker.Apply();
		}

		public void Update()
		{
			BENCHMARK();
			CheckComponent();
		}

		private void BENCHMARK()
		{
			TestRemoveAdd_Entities();
			TestRemoveAdd_Component1();
			SetTestQ1();
			HasTestQ1();
			GetTestQ1();
			TestRemoveAdd_Component12345678910();
			IterateQueries();
		}

		private void TestRemoveAdd_Entities()
		{
			//iterate thru first query's entities as it each entity contains ComponentClass1
			_query1.GetEntities(_entities);
			Profiler.BeginSample("SAMPLE___REMOVE_ENTITY");

			foreach (var entity in _entities)
			{
				storage.RemoveEntity(in entity);
			}

			Profiler.EndSample();


			Profiler.BeginSample("SAMPLE___ADD_ENTITY");
			CreateEntities();
			Profiler.EndSample();
		}

		private void TestRemoveAdd_Component12345678910()
		{
			_query10.GetEntities(_entities);
			//Profiler.BeginSample($"SAMPLE___REMOVE_COMPONENT12345678910_#####_ENTT_COUNT:_{_entities.Count}_COMP_COUNT:_{_entities.Count * 10}");
			Profiler.BeginSample($"SAMPLE___REMOVE_COMPONENT12345678910");
			foreach (var entity in _entities)
			{
				entity.Remove<Component1>();
				entity.Remove<Component2>();
				entity.Remove<Component3>();
				entity.Remove<Component4>();
				entity.Remove<Component5>();
				entity.Remove<Component6>();
				entity.Remove<Component7>();
				entity.Remove<Component8>();
				entity.Remove<Component9>();
				entity.Remove<Component10>();
			}

			Profiler.EndSample();

			//Profiler.BeginSample($"SAMPLE___ADD_COMPONENT12345678910_#####_ENTT_COUNT:_{_entities.Count}_COMP_COUNT:_{_entities.Count * 10}");
			Profiler.BeginSample($"SAMPLE___ADD_COMPONENT12345678910");

			foreach (var entity in _entities)
			{
				entity.Add(new Component1());
				entity.Add(new Component2());
				entity.Add(new Component3());
				entity.Add(new Component4());
				entity.Add(new Component5());
				entity.Add(new Component6());
				entity.Add(new Component7());
				entity.Add(new Component8());
				entity.Add(new Component9());
				entity.Add(new Component10());
			}

			Profiler.EndSample();
		}

		private void TestRemoveAdd_Component1()
		{
			//iterate thru first query's entities as it each entity contains ComponentClass1
			_query1.GetEntities(_entities);

			//Profiler.BeginSample($"SAMPLE___REMOVE_COMPONENT1_#####_ENTT_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___REMOVE_COMPONENT1");
			foreach (Entity entity in _entities)
			{
				entity.Remove<Component1>();
			}

			Profiler.EndSample();


			//Profiler.BeginSample($"SAMPLE___ADD_COMPONENT1_#####_ENTT_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ADD_COMPONENT1");
			foreach (Entity entity in _entities)
			{
				entity.Add(new Component1());
			}

			Profiler.EndSample();


			//Profiler.BeginSample($"SAMPLE___TRY_REMOVE_COMPONENT1_#####_ENTT_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___TRY_REMOVE_COMPONENT1");
			foreach (Entity entity in _entities)
			{
				var removed = entity.Remove<Component1>(out var removedComponent);
			}

			Profiler.EndSample();


			//Profiler.BeginSample($"SAMPLE___ADD_OR_SET_COMPONENT1_FIRST_#####_ENTT_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ADD_OR_SET_COMPONENT1_FIRST");
			foreach (Entity entity in _entities)
			{
				var removed = entity.AddOrSet<Component1>(new Component1(), out var replacedComponent);
			}

			Profiler.EndSample();


			//Profiler.BeginSample($"SAMPLE___ADD_OR_SET_COMPONENT1_SECOND_#####_ENTT_COUNT:_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___ADD_OR_SET_COMPONENT1_SECOND");
			foreach (Entity entity in _entities)
			{
				var removed = entity.AddOrSet<Component1>(new Component1(), out var replacedComponent);
			}

			Profiler.EndSample();
		}

		private void GetTestQ1()
		{
			//Profiler.BeginSample($"SAMPLE___GET_Q1_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___GET_Q1");
			foreach (var e in _entities)
			{
				ref var componentClass1 = ref e.Get<Component1>();
			}

			Profiler.EndSample();
		}

		private void SetTestQ1()
		{
			//Profiler.BeginSample($"SAMPLE___SET_Q1_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___SET_Q1");
			foreach (var e in _entities)
			{
				e.Set(new Component1 { K = Random.Range(0, 100) });
			}

			Profiler.EndSample();
		}

		private void HasTestQ1()
		{
			//Profiler.BeginSample($"SAMPLE___HAS_Q1_{_entities.Count}");
			Profiler.BeginSample("SAMPLE___HAS_Q1");
			foreach (var e in _entities)
			{
				var has = e.Has<Component1>();
			}

			Profiler.EndSample();
		}


		public void Unload()
		{
		}


		public void OnStartFrame()
		{
		}

		public void OnEndFrame()
		{
		}

		#region Components

		public struct Component1 : IComponent
		{
			public int K;
		}

		public struct Component2 : IComponent
		{
		}

		public struct Component3 : IComponent
		{
		}

		public struct Component4 : IComponent
		{
		}

		public struct Component5 : IComponent
		{
		}

		public struct Component6 : IComponent
		{
		}

		public struct Component7 : IComponent
		{
		}

		public struct Component8 : IComponent
		{
		}

		public struct Component9 : IComponent
		{
		}

		public struct Component10 : IComponent
		{
		}

		public struct ChangeableComponent : IComponent
		{
			public int INT;
			public bool BOOL;
		}

		#endregion
	}
}