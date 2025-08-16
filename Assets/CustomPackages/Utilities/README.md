# Utilities
This library contains several reusable utilities for Unity development.
- Main features:
  - Singleton: Derive from the generic base to get a safe global instance.
  - SceneSwitcher: Switch scenes with one click.
  - EventDispatcher: Type-safe publish / subscribe without string keys.
  - RectTransform Extensions: Quick anchoring, positioning, sizing helpers.
  - UIAdaptation: Auto sizing and responsive adjustments.
  - DebugExtensions: Visual debug helpers.
  - WeightedRandomList<T>: Random selection based on weights.

## Installation
### Unity Package Manager
```
https://github.com/ThanhDang143/ThanhDV.Utilities.git?path=/Assets/CustomPackages/Utilities
```

1. In Unity, open **Window** → **Package Manager**.
2. Press the **+** button, choose "**Add package from git URL...**"
3. Enter url above and press **Add**.

### Scoped Registry

1. In Unity, open **Project Settings** → **Package Manager** → **Add New Scoped Registry**
- ``Name`` ThanhDVs
- ``URL`` https://upm.thanhdv.icu
- ``Scope(s)`` thanhdv

2. In Unity, open **Window** → **Package Manager**.
- Press the **+** button, choose "**Add package by name...**" → ``thanhdv.utilities``
- or
- Press the **Packages** button, choose "**My Registries**"

## How to use.
- Each utility has (or will have) its own README with specific API and examples.
- If a README is missing: open the script and check XML doc comments for interim guidance.
- Changelog updates in CHANGELOG.md