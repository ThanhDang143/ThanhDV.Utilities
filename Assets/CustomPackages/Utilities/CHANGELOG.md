# Changelog
All notable changes to this project will be documented in this file.

## [0.0.22] - 2025-09-05
### Updated
- PersistentOrthographicCamera logics
### Remove
- Removed UIBackground (use AspectRatioFitter instead)

## [0.0.22] - 2025-09-05
### Updated
- UIAdaptation logic

## [0.0.21] - 2025-08-16
### Added
- `WeightedRandomList<T>`.
- README: `SceneSwitcher`, `EventDispatcher`.

### Changed
- README updates: `RectTransformExtensions`, `DebugDrawingExtension`, `Singleton`, `UIAdaptation`.

## [0.0.20] - 2025-08-14
### Changed
- Renamed namespaces and class names (breaking if referenced externally).

## [0.0.19] - 2025-07-16
### Changed
- `EventDispatcher`: readability, performance, debuggability improvements.

### Removed
- `DebugExtention.Log()` (use standard Unity logging or provided alternatives).

## [0.0.18] - 2025-06-17
### Added
- `SceneSwitcher`.

## [0.0.17] - 2025-05-28
### Changed
- `RectTransformExtensions`: auto-set anchor to `MiddleCenter`.

## [0.0.16] - 2025-05-28
### Changed
- `EventDispatcher`: `Register` → `Subscribe`, `Unregister` → `Unsubscribe`.

## [0.0.15] - 2025-05-28
### Added
- `UIBackground`: `GetSize()`, `GetSetupSize()`.

## [0.0.14] - 2025-05-28
### Fixed
- `EventDispatcher`: ArgumentNullException addValueFactory at line