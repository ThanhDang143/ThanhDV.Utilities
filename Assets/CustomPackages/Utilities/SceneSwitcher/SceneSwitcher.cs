#if UNITY_EDITOR
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

namespace ThanhDV.Utilities
{
    [InitializeOnLoad]
    public static class SceneSwitcher
    {
        private static string[] _sceneNames = Array.Empty<string>();
        private static string[] _scenePaths = Array.Empty<string>();
        private static int _selectedIndex;
        private static string _lastActiveScene = "";
        private static VisualElement _toolbarUI;
        private static VisualElement _rightContainer;
        private static string _customScenePath;

        private static bool _sceneListDirty = true;
        private static double _nextAllowedRefreshTime;

        private const float DROPDOWN_BOX_HEIGHT = 20f;
        private const string DEFAULT_SCENE_PATH = "Assets";
        private const string SCENE_PATH_PREF_KEY = "SceneSwitcher_CustomScenePath";

        private const double REFRESH_INTERVAL_SECONDS = 1.0;

        private static bool FetchAllScenes
        {
            get => EditorPrefs.GetBool("SceneSwitcher_FetchAllScenes", true);
            set => EditorPrefs.SetBool("SceneSwitcher_FetchAllScenes", value);
        }

        private static string CustomScenePath
        {
            get
            {
                if (string.IsNullOrEmpty(_customScenePath))
                {
                    _customScenePath = EditorPrefs.GetString(SCENE_PATH_PREF_KEY, DEFAULT_SCENE_PATH);
                }
                return _customScenePath;
            }
            set
            {
                _customScenePath = value;
                EditorPrefs.SetString(SCENE_PATH_PREF_KEY, value);
            }
        }

        static SceneSwitcher()
        {
            RefreshSceneListIfNeeded(force: true);

            EditorSceneManager.activeSceneChangedInEditMode += (_, _) => UpdateSceneSelection();
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            EditorApplication.hierarchyChanged += OnHierarchyChanged;
            EditorApplication.projectChanged += OnProjectChanged;

            EditorApplication.delayCall += EnsureToolbarUI;
        }

        private static void OnHierarchyChanged()
        {
            EditorApplication.delayCall += EnsureToolbarUI;
        }

        private static void OnProjectChanged()
        {
            MarkSceneListDirty();
            EditorApplication.delayCall += EnsureToolbarUI;
        }

        private static void MarkSceneListDirty()
        {
            _sceneListDirty = true;
        }

        private static void EnsureToolbarUI()
        {
            var toolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (toolbarType == null) return;

            var toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
            if (toolbars.Length == 0) return;

            var toolbar = toolbars[0];
            var rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null) return;

            var root = rootField.GetValue(toolbar) as VisualElement;
            _rightContainer = root?.Q("ToolbarZoneRightAlign");
            if (_rightContainer == null) return;

            if (_toolbarUI == null || _toolbarUI.parent != _rightContainer)
            {
                if (_toolbarUI != null)
                {
                    _toolbarUI.RemoveFromHierarchy();
                }

                _toolbarUI = new IMGUIContainer(OnGUI) { };
                _rightContainer.Add(_toolbarUI);

                _rightContainer.RegisterCallback<GeometryChangedEvent>(_ => EnsureToolbarUI());
                _rightContainer.RegisterCallback<DetachFromPanelEvent>(_ => EditorApplication.delayCall += EnsureToolbarUI);
            }
        }

        private static void OnGUI()
        {
            RefreshSceneListIfNeeded();

            if (_sceneNames.Length == 0)
            {
                _selectedIndex = 0;
            }
            else if (_selectedIndex >= _sceneNames.Length)
            {
                _selectedIndex = 0;
            }

            var isPlaying = EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(isPlaying);
            var newFetchAllScenes = GUILayout.Toggle(FetchAllScenes, FetchAllScenes ? "All Scenes" : "Build Settings", "Button", GUILayout.Height(DROPDOWN_BOX_HEIGHT));
            if (newFetchAllScenes != FetchAllScenes)
            {
                FetchAllScenes = newFetchAllScenes;
                MarkSceneListDirty();
            }

            if (FetchAllScenes)
            {
                CustomScenePath = DEFAULT_SCENE_PATH;
            }

            var popupStyle = new GUIStyle(EditorStyles.popup)
            {
                fixedHeight = DROPDOWN_BOX_HEIGHT
            };

            var displayNames = _sceneNames.Length == 0 ? new[] { "(No Scenes)" } : _sceneNames;
            var newIndex = EditorGUILayout.Popup(_selectedIndex, displayNames, popupStyle, GUILayout.Width(150), GUILayout.Height(DROPDOWN_BOX_HEIGHT));

            if (newIndex != _selectedIndex)
            {
                _selectedIndex = newIndex;

                if (_sceneNames.Length > 0)
                {
                    LoadSceneAtIndex(_selectedIndex);
                }
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
        }

        private static void RefreshSceneListIfNeeded(bool force = false)
        {
            if (!force)
            {
                if (!_sceneListDirty && EditorApplication.timeSinceStartup < _nextAllowedRefreshTime)
                    return;
            }

            _sceneListDirty = false;
            _nextAllowedRefreshTime = EditorApplication.timeSinceStartup + REFRESH_INTERVAL_SECONDS;

            BuildSceneList(out _sceneNames, out _scenePaths);
            SelectCurrentScene();
        }

        private static void BuildSceneList(out string[] sceneNames, out string[] scenePaths)
        {
            if (FetchAllScenes)
            {
                string path = CustomScenePath;
                if (Directory.Exists(path))
                {
                    var paths = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories);
                    scenePaths = paths;
                    sceneNames = paths.Select(Path.GetFileNameWithoutExtension).ToArray();
                }
                else
                {
                    sceneNames = Array.Empty<string>();
                    scenePaths = Array.Empty<string>();
                }
            }
            else
            {
                var scenes = EditorBuildSettings.scenes.Where(scene => scene.enabled).ToArray();
                scenePaths = scenes.Select(scene => scene.path).ToArray();
                sceneNames = scenePaths.Select(Path.GetFileNameWithoutExtension).ToArray();
            }
        }

        private static void SelectCurrentScene()
        {
            var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
            var index = Array.IndexOf(_sceneNames, currentScene);
            if (index == -1) return;
            _selectedIndex = index;
            _lastActiveScene = currentScene;
        }

        private static void UpdateSceneSelection()
        {
            var currentScene = Path.GetFileNameWithoutExtension(SceneManager.GetActiveScene().path);
            if (currentScene == _lastActiveScene) return;
            _lastActiveScene = currentScene;
            SelectCurrentScene();
        }

        private static void LoadSceneAtIndex(int sceneIndex)
        {
            if (sceneIndex < 0 || sceneIndex >= _scenePaths.Length)
                return;

            var scenePath = _scenePaths[sceneIndex];

            if (!string.IsNullOrEmpty(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
            else
            {
                Debug.Log("<color=red>[SceneSwitcher] Scene path is empty!!!");
            }
        }

        private static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.ExitingPlayMode)
            {
                EditorApplication.delayCall += EnsureToolbarUI;
            }
        }
    }
}
#endif