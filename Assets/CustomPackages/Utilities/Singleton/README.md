# Singleton Utilities

Light‑weight implementations of the Singleton pattern for:
- `Singleton<T>`: Plain C# (non-Unity) classes.
- `MonoSingleton<T>`: Scene (non-persistent) `MonoBehaviour`.
- `PersistentMonoSingleton<T>`: `MonoBehaviour` that survives scene loads (`DontDestroyOnLoad`).

## When to Use
- Use for globally accessible, unique service objects (e.g., Config, AudioRouter, SaveSystem).  
- Avoid for data containers that could be passed explicitly, or where multiple instances are beneficial (e.g., pooled managers).

## Variants

### Singleton<T>
- Pure C# (no Unity API).
- Lazy, typically thread-safe if implemented with `static` initialization (depends on your implementation).
- Example:
```csharp
public sealed class GameConfig : Singleton<GameConfig>
{
    public string CurrentLocale { get; set; } = "en";
    // Optional: private constructor to prevent external instantiation.
    private GameConfig() { }
}

// Usage
var locale = GameConfig.Instance.CurrentLocale;
```

### MonoSingleton<T>
- First access searches the active scene for an existing instance; if none, optionally creates one (depends on implementation).
- Destroyed when the scene is unloaded.
```csharp
public class AudioManager : MonoSingleton<AudioManager>
{
    public void PlayClick() { /* ... */ }
}

// Usage (in any script)
AudioManager.Instance.PlayClick();
```

### PersistentMonoSingleton<T>
- Same as `MonoSingleton<T>` but marked with `DontDestroyOnLoad`.
- Useful for systems: Audio, Save, Telemetry, Addressables bootstrap.
```csharp
public class SaveSystem : PersistentMonoSingleton<SaveSystem>
{
    public void Save() { /* ... */ }
}
```