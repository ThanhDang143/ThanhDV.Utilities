# SceneSwitcher

Small Unity Editor utility that adds a scene dropdown to the right side of the main Toolbar. Switch scenes with one click. Two modes:
* All Scenes: recursively scan a custom folder (default: `Assets/Games`).
* Build Settings: only enabled scenes from Build Settings.

## How to use

1. Keep `SceneSwitcher.cs` inside an `Editor` folder (already at `Assets/CustomPackages/Utilities/Editor/SceneSwitcher.cs`). It auto-initializes on domain load.
2. Toolbar UI shows:
	 * Toggle button: All Scenes / Build Settings
	 * (All Scenes mode) Text field for folder path
	 * Scene popup
3. (All Scenes) enter a valid folder path; list updates automatically. Pref key: `SceneSwitcher_CustomScenePath`.
4. Pick a scene in the popup to open it. Unsaved changes trigger Unity’s save prompt.
5. Controls are disabled while in Play Mode.

Notes:
* Auto-refreshes when scenes change, project refresh, or active scene changes.
* Refresh only happens when the set differs (cheap comparison).
* Remembers last folder and mode via EditorPrefs.