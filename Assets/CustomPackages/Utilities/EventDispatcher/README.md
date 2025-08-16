# EventDispatcher

Lightweight in-process event bus for Unity. Lets unrelated components communicate without direct references. Supports:

1. Events with data (`Action<T>`)
2. Marker events without data (`Action` keyed by generic type T)

Singleton based: access via `EventDispatcher.Instance`.

## How to use

1. Define event types (struct/class). Add fields only if you need to pass data.
2. Register / unregister in `OnEnable` / `OnDisable` (or `OnDestroy`).
3. Post events from anywhere.

Notes:
* Type `T` is the dictionary key. For no‑data events an internal cached key `FullTypeName_NoData` is used.
* Exceptions inside listeners are caught and logged so other listeners still run.
* Intended for main Unity thread (not fully thread-safe for concurrent writes).

## Example

```csharp
using UnityEngine;
using ThanhDV.Utilities; // contains EventDispatcher

// With data

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