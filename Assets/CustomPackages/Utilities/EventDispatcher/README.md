# EventDispatcher

Lightweight in-process event bus for Unity. Lets unrelated components communicate without direct references. Supports:

1. Events with data (`Action<T>`)
2. Marker events without data (`Action` keyed by generic type T)

**Flexible Usage**: Supports both Singleton pattern and Dependency Injection:
- **Singleton**: Access via `EventDispatcher.Instance` (default)
- **Dependency Injection**: Use `#define INJECTION_ENABLED` to disable singleton and inject EventDispatcher as dependency

## How to use

### Singleton Mode (Default)
1. Define event types (struct/class). Add fields only if you need to pass data.
2. Register / unregister in `OnEnable` / `OnDisable` (or `OnDestroy`).
3. Post events from anywhere using `EventDispatcher.Instance`.

### Dependency Injection Mode
1. Add `#define INJECTION_ENABLED` at the top of your project or in Player Settings > Scripting Define Symbols.
2. Create EventDispatcher instance and inject it through your DI container.
3. Use the injected instance instead of the static `Instance` property.

⚠️:
* Type `T` is the dictionary key. For no‑data events an internal cached key `FullTypeName_NoData` is used.
* Exceptions inside listeners are caught and logged so other listeners still run.
* Intended for main Unity thread (not fully thread-safe for concurrent writes).
* The singleton implementation is thread-safe using lock mechanism.

## Example

### Singleton Mode
```csharp
using UnityEngine;
using ThanhDV.Utilities; // contains EventDispatcher

// With data
public struct EnemyDied
{
	public int EnemyId;
	public Vector3 Position;
	public EnemyDied(int id, Vector3 pos){ EnemyId = id; Position = pos; }
}

// No data (marker)
public struct WaveCleared { }

public class ScoreSystem : MonoBehaviour
{
	void OnEnable(){
		EventDispatcher.Instance.Register<EnemyDied>(OnEnemyDied);
		EventDispatcher.Instance.Register<WaveCleared>(OnWaveCleared);
	}
	void OnDisable(){
		EventDispatcher.Instance.Unregister<EnemyDied>(OnEnemyDied);
		EventDispatcher.Instance.Unregister<WaveCleared>(OnWaveCleared);
	}
	void OnEnemyDied(EnemyDied e){ Debug.Log($"Enemy {e.EnemyId} died at {e.Position}"); }
	void OnWaveCleared(){ Debug.Log("Wave cleared!"); }
}

public class Enemy : MonoBehaviour
{
	public int Id;
	public void Die(){ EventDispatcher.Instance.Post(new EnemyDied(Id, transform.position)); }
}

public class WaveManager : MonoBehaviour
{
	public void Clear(){ EventDispatcher.Instance.Post<WaveCleared>(); }
}
```

### Dependency Injection Mode
```csharp
// Add this at the top of your project or in Scripting Define Symbols
#define INJECTION_ENABLED

// In your Reflex installer or composition root
var gameSaverObject = new GameObject("GameSaver");
var gameSaverInstance = gameSaverObject.AddComponent<GameSaver>();

builder.AddSingleton(gameSaverInstance);

// In your consumer class
public class ScoreSystem : MonoBehaviour
{
	[Inject] private EventDispatcher _eventDispatcher;
	
	void OnEnable(){
		_eventDispatcher.Register<EnemyDied>(OnEnemyDied);
		_eventDispatcher.Register<WaveCleared>(OnWaveCleared);
	}
	void OnDisable(){
		_eventDispatcher.Unregister<EnemyDied>(OnEnemyDied);
		_eventDispatcher.Unregister<WaveCleared>(OnWaveCleared);
	}
	void OnEnemyDied(EnemyDied e){ Debug.Log($"Enemy {e.EnemyId} died at {e.Position}"); }
	void OnWaveCleared(){ Debug.Log("Wave cleared!"); }
}
```