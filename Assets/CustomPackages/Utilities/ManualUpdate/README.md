
# ManualUpdate

`ManualUpdate` is a small utility that runs “update-like” logic **manually** from one or more explicit update loops.

Instead of relying on many `MonoBehaviour.Update()` methods, you create a `ManualUpdater<TMarker>` and call its `Execute()` method wherever you want (Update / FixedUpdate / LateUpdate / custom loop).

Namespace: `ThanhDV.Utilities`

## Core API

- `IManualUpdate` — implement `ExecuteUpdate()`.
- `ManualUpdater<TMarker>` — owns a set of updateables for a given marker key.
- `ManualUpdateRegistry` — static registry keyed by marker type, allowing updateables to register **before** the updater is created (pending + flush).

## Marker keys (no strings)

This package uses a **marker struct type** as the channel key.

Example markers:

```csharp
public static class ManualUpdateChannel
{
	public struct FixedUpdate { }
	public struct Update { }
	public struct LateUpdate { }
}
```

Benefits:
- No `null/empty/whitespace` ids.
- Strongly-typed channels.

## Quick start (Unity)

### 1) Create updaters for each loop

```csharp
using ThanhDV.Utilities;
using UnityEngine;

public sealed class UpdateTest : MonoBehaviour
{
	private ManualUpdater<ManualUpdateChannel.FixedUpdate> fixedUpdater;
	private ManualUpdater<ManualUpdateChannel.Update> updateUpdater;
	private ManualUpdater<ManualUpdateChannel.LateUpdate> lateUpdater;

	private void Awake()
	{
		fixedUpdater = new();
		updateUpdater = new();
		lateUpdater = new();
	}

	private void FixedUpdate() => fixedUpdater.Execute();
	private void Update()      => updateUpdater.Execute();
	private void LateUpdate()  => lateUpdater.Execute();
}
```

### 2) Updateable as MonoBehaviour

Use this pattern when the updateable is a Unity component. It unregisters automatically on disable/destroy.

```csharp
using ThanhDV.Utilities;
using UnityEngine;

public sealed class SpinManually : MonoBehaviour, IManualUpdate
{
	[SerializeField] private float speed = 180f;

	private void OnEnable()
	{
		ManualUpdateRegistry.Register<ManualUpdateChannel.Update>(this);
	}

	private void OnDisable()
	{
		ManualUpdateRegistry.Unregister<ManualUpdateChannel.Update>(this);
	}

	public void ExecuteUpdate()
	{
		transform.Rotate(0f, speed * Time.deltaTime, 0f);
	}
}
```

### 3) Updateable as non-MonoBehaviour (plain C# class)

Use this pattern for pure C# objects. You must keep a reference and call `Dispose()` (or otherwise unregister) when done.

```csharp
using System;
using ThanhDV.Utilities;
using UnityEngine;

public sealed class Test : IManualUpdate, IDisposable
{
	public Test()
	{
		ManualUpdateRegistry.Register<ManualUpdateChannel.Update>(this);
	}

	public void Dispose()
	{
		ManualUpdateRegistry.Unregister<ManualUpdateChannel.Update>(this);
	}

	public void ExecuteUpdate()
	{
		Debug.Log("Test - Update");
	}
}
```

## Behavior details

- **Safe during ticking**: registrations/removals are queued and flushed at the start of `ManualUpdater<TMarker>.Execute()`.
- **Last call wins (pending)**: if you call register then unregister (or vice-versa) before the updater exists, the last call wins.
- **Destroyed Unity objects cleanup**: during `Execute()`, the updater removes entries that are `null` or Unity “fake-null” objects.

## Important notes / pitfalls

### 1) One updater per marker

`ManualUpdateRegistry.Bind<TMarker>(...)` enforces a single updater per marker.
Creating a second `ManualUpdater<TMarker>` while the first one is still bound will throw.

If you need to rebuild an updater, dispose the old one first:

```csharp
updateUpdater.Dispose();
updateUpdater = new ManualUpdater<ManualUpdateChannel.Update>();
```

### 2) Creating updateables without keeping a reference

If you do `new Test4();` and never store it anywhere, it can still be kept alive by the registry/updater (because it holds a strong reference).
Without a reference, you cannot call `Dispose()` to unregister.

Recommended patterns:

- Keep a reference and dispose/unregister when done.
- Prefer UnityEngine.Object-based updateables (MonoBehaviour/ScriptableObject) if you want Unity destruction semantics.

