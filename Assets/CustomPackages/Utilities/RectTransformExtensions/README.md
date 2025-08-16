# RectTransform Extensions

Utility extension methods for Unity `RectTransform` to quickly set Anchors, Pivot and Offsets with clear, chainable calls.

## How to use

1. Add the namespace: `using ThanhDV.Utilities;`
2. Call the extension methods on any `RectTransform`:
   - `SetAnchor(minX, minY, maxX, maxY)`
   - `SetPivot(x, y)`
   - `SetOffset(left, right, top, bottom)`
3. All methods return the same `RectTransform` so you can chain them.

## Example

```csharp
var rt = panel.GetComponent<RectTransform>();

rt.SetAnchor(0.5f, 0.5f, 0.5f, 0.5f)
  .SetPivot(0.5f, 0.5f)
  .SetOffset(-100f, 100f, 50f, -50f); 

rt.SetAnchor(0f, 0f, 1f, 0f)
  .SetPivot(0.5f, 0f)
  .SetOffset(16f, 16f, 120f, 0f);

rt.SetAnchor(1f, 1f, 1f, 1f)
  .SetPivot(1f, 1f)
  .SetOffset(-300f, 0f, 0f, -180f);

rt.SetAnchor(0f, 0f, 1f, 1f)
  .SetPivot(0.5f, 0.5f)
  .SetOffset(32f, 32f, 64f, 64f);

rt.SetAnchor(0.5f, 1f, 0.5f, 1f)
  .SetPivot(0.5f, 1f)
  .SetOffset(-200f, 200f, 0f, -120f);
```