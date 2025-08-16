# WeightedRandomList

Generic weighted random selection list that returns an item based on probability proportional to its weight.

## How to Use
- Declare `WeightedRandomList<T>` with the element type you want (e.g. `int`, `string`, `GameObject`, `ScriptableObject`, etc.).
- Use `Add(item, weight)` to add an element with its weight (weight > 0; float values allow flexible ratios).
- Call `Random()` to get a random element following the current weighted distribution.
- Access the number of elements via the `Count` property.

## Example
```csharp
var lootTable = new WeightedRandomList<string>();
lootTable.Add("Common", 70f);    // ~70%
lootTable.Add("Rare", 25f);      // ~25%
lootTable.Add("Legendary", 5f);  // ~5%

string drop = lootTable.Random();
UnityEngine.Debug.Log($"Drop: {drop}");

// Update weight
lootTable.SetWeight("Rare", 30f);

// Remove an item
lootTable.Remove("Common");

// Safe retrieval
if (lootTable.TryRandom(out var another))
	UnityEngine.Debug.Log($"Another: {another}");
```