
# ManualUpdate

`ManualUpdate` is a small utility that lets you run “update-like” logic **manually** from a central place, instead of relying on many Unity `MonoBehaviour.Update()` calls.

It is useful when you want:

- One or many independent update loops you control (pause, step, custom order, custom timing)
- To avoid scattering update logic across many components
- To update non-`MonoBehaviour` objects

The API is intentionally minimal: implement one interface and register/unregister.

Namespace: `ThanhDV.Utilities`

## How it works

- `ManualUpdater` is an **instance-based** container that owns a set of `IManualUpdate` items.
- You can create multiple updaters (`a`, `b`, `c`) and call `a.ManualUpdate()`, `c.ManualUpdate()`, `b.ManualUpdate()` in any order, from any loop.
- Registration and removal are queued and applied at the start of `ManualUpdate()` to keep iteration safe.
- Destroyed Unity objects are cleaned up automatically (`UnityEngine.Object` fake-null).

## Quick Start (Unity)

### 1) Create a driver that runs an updater

```csharp
using UnityEngine;
using ThanhDV.Utilities;

public sealed class ManualUpdateDriver : MonoBehaviour
{
	public ManualUpdater Updater { get; private set; }

	private void Awake()
	{
		Updater = new ManualUpdater();
	}

	private void Update()
	{
		Updater.ManualUpdate();
	}
}
```

### 2) Register updateables into a specific updater

This example shows a component that registers into a chosen `ManualUpdateDriver`.

```csharp
using UnityEngine;
using ThanhDV.Utilities;

public sealed class SpinManually : MonoBehaviour, IManualUpdate
{
	[SerializeField] private ManualUpdateDriver driver;
	[SerializeField] private float speed = 180f;

	private void OnEnable()
	{
		if (driver != null && driver.Updater != null)
		{
			driver.Updater.Register(this);
		}
	}

	private void OnDisable()
	{
		if (driver != null && driver.Updater != null)
		{
			driver.Updater.Unregister(this);
		}
	}

	public void ExecuteUpdate()
	{
		transform.Rotate(0f, speed * Time.deltaTime, 0f);
	}
}
```

## Behavior details

- **Last call wins (per frame)**: if you call `Register(x)` then `Unregister(x)` before the next `ManualUpdate()` flush, `x` will end up unregistered (and vice-versa).
- **Safe while ticking**: calling `Register/Unregister` from inside `ExecuteUpdate()` will not modify the active set while it is being iterated.

## Known limitation / existing issue

- **You cannot register before a `ManualUpdater` exists.**
	Because this design is instance-based (no static global registry), updateables must know *which updater instance* they belong to.
	In Unity, this typically means:
	- Create the updater early (e.g., a driver in the startup scene, created in `Awake()`), or
	- Inject/pass the updater (or driver) into the object before it tries to register.

## Notes / best practices

- Keep the number of updaters small and purposeful (e.g., one for gameplay, one for UI), unless you truly need many groups.
- `IManualUpdate.ExecuteUpdate()` has no parameters. If you need delta time, use `Time.deltaTime` (Unity main thread) or define your own interface.

