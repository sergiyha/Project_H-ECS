### Project_H ECS

Легкий високопродуктивний ECS-плагін для Unity і не тільки з архетипами на бітмасках, `SparseSet` з O(1) операціями та нульовими алокаціями під час ітерацій
запитів.

## Ідея

Використовуючи ECS для геймплею дуже часто потрібно видаляти і додавати велику купу компонентів(наприклад тегів) для відпрацювання відповідної логіки. Багато
фреймворків виконують це змінюючи розмір чанку або перенесенням цих компонентів в новий
чанк який потрібно алокувати. Також можливий підхід збільшення основного масиву данних при досяганні краю попередньо виділеної памяті. При такому підході мінусом є швидкодія тому, що відбувається копіювання даних в новий масив і при збільшенні наступного копіювання буде ще більше. В Project_H ECS алокації присутні тільки під час створення нових елементів і лише першоразово. Після створення нового чанку він назавжди лишається в памяті - для запобігання створення сміття. Є можливість налаштування розміру чанку самостійно - керуючись вашими побажаннями і потребами проекту. Цей
фреймворк є в якомусь сенсі духовним наступником Entitas, тому що як і Entitas він не про швидкодію, а про чистий, простий та безпечний код який можна писати,
як заманеться маестро.

## Основи

- **Компонент**: будь-який тип, що імплементує `IComponent`.
- **Сутність**: значимий тип `Entity` з `ID`.
- **Сховище**: `Storage` управляє життєвим циклом сутностей/компонентів і виконує запити.
- **Системи**: вони відсутні, логіку з ітерації запитів можна використовувати в будь-якому місці.

Обмеження і константи:

- `Storage.MaxComponentsCount = 1024` - збільшення цього значення призводить до збільшеного використання ОЗУ
- `Storage.ChunkSize = 128` - розмір чанку значень в спарсеті(граничне значення при якому спарсет починає розширюватись) 
- До 256 інстансів `Storage` (по одному байту на ідентифікатор)
- Запити підтримують до 10 компонентів одночасно

## Швидкий старт

```csharp
using Project_H.ECS;
using Project_H.ECS.Component;

public struct Health : IComponent { public int Value; }
public struct Damage : IComponent { public int Value; }

void Example()
{
    var storage = Storage.CreateStorage();
    var e = storage.CreateEntity();
    e.Add(new Health { Value = 100 });
    e.Add(new Damage { Value = 10 });

    var q = storage.GetQuery<Query<Health, Damage>>();

    foreach (var (entity, health, damage) in q)
    {
        health.Value -= damage.Value;
        entity.Set(health);
    }
}
```

## API

### Компоненти

- `interface IComponent { }` – інтерфейс що маркує компонента.
- Підтримуються `struct` і `class`. Для `class` використовуйте `Add(entity, component)`.

### Entity та розширення

- Створення: `var e = storage.CreateEntity();`
- Життєвий цикл:
    - `storage.RemoveEntity(in e)`
    - `e.IsAlive()`
- Операції з компонентами:
    - `e.Add<T>(in component)` / `e.Add<T>()`
    - `e.AddOrSet<T>(in component, out T replaced)`
    - `e.Set<T>(in component)`
    - `ref var c = ref e.Get<T>()`
    - `e.TryGet<T>(out T comp)` / `ref e.TryGetRef<T>(out bool exist)`
    - `e.Has<T>()`
    - `e.Remove<T>()` / `e.Remove<T>(out T removed)` / `e.RemoveSilent<T>()`
    - `e.GetComponents()` -> `object[]`
    - `e.GetComponentTypes(List<Type>)`

### Storage

- Ініціалізація:
    - `var storage = Storage.CreateStorage()`
    - `Storage.GetStorage(byte id)`
- Службове:
    - `GetAllEntities()` -> `Span<int>`
    - Компонентні операції (дзеркалять `Entity`-розширення): `Add/Set/Get/Has/TryGet/Remove/Remove(out)`
    - Отримання усіх компонентів сутності: `GetEntityComponents(in Entity)`
    - Отримання типів компонентів ентіті: `GetComponentTypes(in Entity, List<Type>)`

### Запити (Query)

- Запити мають підтримку до 10 компонентів: `Query<T>`, `Query<T,T1>`, ..., до `Query<T,...,T10>`
- Отримання запиту: `var q = storage.GetQuery<Query<T,...>>();`
- Всі запити кешуються, тому не виникатиме ситуація зі створенням дубльованого запиту

```csharp
foreach (var (e, c0, c1, c2) in storage.GetQuery<Query<C0, C1, C2>>())
{
    // робота з посиланнями/копіями через Set/AddOrSet
}
```

Виконання запиту на делегованому методі:

- Можливість зміни як `struct` через посилання `ref`

```csharp
namespace SomeNP
{
	using QueryType = Query<HealthComponent, StaminaComponent>;
	
	public struct HealthComponent : IComponent
	{
		public float Value;
	}

	public struct StaminaComponent : IComponent
	{
		public float Value;
	}

	public class QueryTest
	{
		private Storage _storage;
		private QueryType _query;
		private QueryType.QueryDelegate _queryDelegate = Execute;

		private static void Execute(Entity e, ref HealthComponent component, ref StaminaComponent component1)
		{
			Debug.LogError($"HealthComponent: {component.Value} StaminaComponent{component1.Value}");
		}

		public QueryTest()
		{
			_storage = Storage.CreateStorage();
			var entity = _storage.CreateEntity();
			entity.Add<HealthComponent>(new HealthComponent() { Value = 100 });
			entity.Add<StaminaComponent>(new StaminaComponent() { Value = 50 });
			_query = _storage.GetQuery<QueryType>();
		}

		public void ExecuteQuery()
		{
			_query.ExecuteOn(_queryDelegate);
			//або
			_query.ExecuteOn(Execute);
			//abo
			_query.ExecuteOn(static (Entity _, ref HealthComponent _, ref StaminaComponent _) => { });
		}
	}
	
}
```

### WrappedQuery

Виклик делегата з контекстом цільового об’єкта:

- дає можливість уникати аллокацій повязані з замиканнями(сlosures) використовуючи делегати з таргетом :

```csharp
namespace SomeNP
{
	using WrappedQueryType = WrappedQuery<WrappedQueryTest, HealthComponent, StaminaComponent>;
	
	public struct HealthComponent : IComponent
	{
		public float Value;
	}

	public struct StaminaComponent : IComponent
	{
		public float Value;
	}
	public class WrappedQueryTest
	{
		private Storage _storage;
		private WrappedQueryType _query;
		private WrappedQueryType.WrappedQueryDelegate _queryDelegate = Execute;

		private static void Execute(WrappedQueryTest target, Entity e, ref HealthComponent component, ref StaminaComponent component1)
		{
			Debug.LogError($"HealthComponent: {component.Value} StaminaComponent{component1.Value} and can use target without closure)");
		}

		public WrappedQueryTest()
		{
			_storage = Storage.CreateStorage();
			var entity = _storage.CreateEntity();
			entity.Add<HealthComponent>(new HealthComponent() { Value = 100 });
			entity.Add<StaminaComponent>(new StaminaComponent() { Value = 50 });
			_query = _storage.GetQuery<WrappedQueryType>();
		}

		public void ExecuteQuery()
		{
			_query.ExecuteOn(_queryDelegate, this); //жодних алокацій
			//або
			_query.ExecuteOn(Execute, this); //алокації залежать від методу, якщо метод має замикання то кожен раз під час виклику буде алокація
			//abo
			_query.ExecuteOn(static (WrappedQueryTest _, Entity _, ref HealthComponent _, ref StaminaComponent _) => { }, this); //жодних алокацій
		}
	}
}
```

### CommandBuffer

- Відкладені структурні зміни з гарантією цільового `Storage`
- Має функціональний характер ніж оптимізований тому, що використовує базове API над `Entity`  - тобто використання `CommandBuffer` не є швидшим за використання
  Add/Set/Remove

```csharp
var cb = new CommandBuffer(storage.GetStorageId());
cb.Add(e, new Health { Value = 1 });
cb.Remove<Damage>(e);
cb.RemoveEntity(e);
cb.Playback();
```

### Трекінг стану компонента

- `EntityStateTracker<T>` для `unmanaged, IComponent`:

```csharp
private class StateTrackerTest
{
	private Storage _storage;
	private Entity _entity;
	private EntityStateTracker<HealthComponent> _healthTracker;

	public struct HealthComponent : IComponent
	{
		public float Value;
	}

	public StateTrackerTest()
	{
		_storage = Storage.CreateStorage();
		var entity = _storage.CreateEntity();
		entity.Add<HealthComponent>(new HealthComponent() { Value = 100 });
		_healthTracker.Subscribe(entity);
	}

	//зміна компонента ззовні
	public void DecreaseHealth()
	{
		ref var healthComp = ref _entity.Get<HealthComponent>();
		healthComp.Value--;
	}

	//Можна виконувати один раз на фрейм в якість системі, наприклад для оновлення UI
	public void CheckIsHealthChanged()
	{
		_healthTracker.Update();
		if (_healthTracker.IsChanged())
		{
			Debug.LogError($"HealthComponent: {_healthTracker.CurrentState} було змінено");
		}

		_healthTracker.Apply();
	}
}
```

## Архітектура і продуктивність

- Архетипи формуються на базі бітмасок компонентів, зберігаються в `SparseSet` для O(1) вставки/видалення.
- `Query` тримає посилання на `SparseSet` архетипу, ітерації без боксу/алокацій, де-конструкція результатів.
- Операції додавання/видалення компонентів синхронно оновлюють відповідні архетипи.
- Об’єктний пул (`UnsafeObjectPool`) для внутрішніх допоміжних об’єктів і команд буфера.

## Приклади з BenchmarkEcs

```csharp
// Отримання запитів
var q1 = storage.GetQuery<Query<Component1>>();
var q10 = storage.GetQuery<Query<Component1, Component2, Component3, Component4, Component5, Component6, Component7, Component8, Component9, Component10>>();

// foreach-ітерація
foreach (var _ in q1) { }
foreach (var (e, c1, c2, c3, c4, c5, c6, c7, c8, c9, c10) in q10) { }

// Делегати
q1.ExecuteOn(static (Entity e, ref Component1 c) => { });
q10.ExecuteOn(static (Entity e, ref Component1 a, ref Component2 b, ref Component3 c, ref Component4 d, ref Component5 e5, ref Component6 f, ref Component7 g, ref Component8 h, ref Component9 i, ref Component10 j) => { });

// WrappedQuery
var wq1 = storage.GetQuery<WrappedQuery<MonoBehaviour, Component1>>();
wq1.ExecuteOn(static (MonoBehaviour self, Entity e, ref Component1 c) => { }, this);

// Масові операції
e.Remove<Component1>();
e.Add(new Component1());
e.AddOrSet(new Component1(), out var replaced);

// Перевірки
var has = e.Has<Component1>();
ref var comp = ref e.Get<Component1>();
```

## Переваги

- **Простий API**: мінімальний набір методів на `Entity` та `Storage`.
- **Висока продуктивність**: `SparseSet`, бітмаски, нульові алокації в запитах, відсутність боксингу.
- **Зручні запити**: до 10 компонентів, де-конструкція,/delegates, обгорнуті делегати з таргетом.
- **Безпечне оновлення архетипів**: автоматичне додавання/видалення сутностей зі всіх відповідних запитів.
- **Відкладені зміни**: `CommandBuffer` для коректного порядку структурних змін.
- **Відсутність систем**: ви не забовязані слідувати існуючими доктринами ECS. Кожен проект унікальний і потребує особого підходу до структуризації послідовності виконання логіки. 
## Недоліки
- Не хоче бути найшвидшим ECS


