# Debug Drawing Extension

Utility extension for drawing debug geometry so you can visualize gameplay logic, collisions, directions, areas of effect, etc. without spawning temporary Meshes or helper GameObjects.

## API Groups

1. `DebugExt` – Uses `Debug.DrawLine / Debug.DrawRay` (visible in Play Mode / Editor if Gizmos is enabled). Supports `duration` and `depthTest`.
2. `GizmosExt` – Uses `Gizmos.*` (only visible in Scene View when Gizmos is toggled). No `duration`.
3. `MethodsDebug` – Reflection helpers to list methods of an object or a type.

## Basic Usage

```csharp
using ThanhDV.Utilities;
using UnityEngine;

public class DebugExample : MonoBehaviour
{
	void Update()
	{
		DebugExt.DrawArrow(transform.position, transform.forward * 3, Color.red);
	}

    void OnDrawGizmosSelected()
    {
        GizmosExt.DrawArrow(transform.position, transform.forward * 3, Color.red);
    }
}
```

## Function List (condensed)

- `DrawPoint(position, [color], [scale], [duration], [depthTest])`
- `DrawBounds(bounds, [color], [duration], [depthTest])`
- `DrawLocalCube(transform|matrix, size, [color], [center], [duration], [depthTest])`
- `DrawCircle(position, [up], [color], [radius], [duration], [depthTest])`
- `DrawWireSphere(position, [color], [radius], [duration], [depthTest])`
- `DrawCylinder(start, end, [color], [radius], [duration], [depthTest])`
- `DrawCone(position, direction|[Vector3.up], [color], [angle], [duration], [depthTest])`
- `DrawArrow(position, direction, [color], [duration], [depthTest])`
- `DrawCapsule(start, end, [color], [radius], [duration], [depthTest])`

