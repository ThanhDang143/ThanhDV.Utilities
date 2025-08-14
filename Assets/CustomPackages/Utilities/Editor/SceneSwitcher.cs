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
        private static int _selectedIndex;
        private static string _lastActiveScene = "";
        private static VisualElement _toolbarUI;
        private static VisualElement _rightContainer;
        private static string _customScenePath;

        private const float DROPDOWN_BOX_HEIGHT = 20f;
        private const string DEFAULT_SCENE_PATH = "Assets/Games";
        private const string SCENE_PATH_PREF_KEY = "SceneSwitcher_CustomScenePath";

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
            RefreshSceneList();
            SelectCurrentScene();

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
            EditorApplication.delayCall += EnsureToolbarUI;
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
            CheckAndRefreshScenes();

            if (_selectedIndex >= _sceneNames.Length)
                _selectedIndex = 0;

            var isPlaying = EditorApplication.isPlaying;

            GUILayout.BeginHorizontal();

            EditorGUI.BeginDisabledGroup(isPlaying);
            var newFetchAllScenes = GUILayout.Toggle(FetchAllScenes
                , FetchAllScenes ? "All Scenes" : "Build Settings"
                , "Button"
                , GUILayout.Height(DROPDOWN_BOX_HEIGHT));
            if (newFetchAllScenes != FetchAllScenes)
            {
                FetchAllScenes = newFetchAllScenes;
                RefreshSceneList();
                SelectCurrentScene();
            }

            if (FetchAllScenes)
            {
                GUILayout.Space(5);
                var newPath = EditorGUILayout.TextField(CustomScenePath, GUILayout.Width(150), GUILayout.Height(DROPDOWN_BOX_HEIGHT));
                if (newPath != CustomScenePath)
                {
                    CustomScenePath = newPath;
                    RefreshSceneList();
                    SelectCurrentScene();
                }
            }

            var popupStyle = new GUIStyle(EditorStyles.popup)
            {
                fixedHeight = DROPDOWN_BOX_HEIGHT
            };

            var newIndex = EditorGUILayout.Popup(_selectedIndex, _sceneNames, popupStyle, GUILayout.Width(150), GUILayout.Height(DROPDOWN_BOX_HEIGHT));

            if (newIndex != _selectedIndex)
            {
                _selectedIndex = newIndex;
                LoadScene(_sceneNames[_selectedIndex]);
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();
        }

        private static void RefreshSceneList()
        {
            if (FetchAllScenes)
            {
                string path = CustomScenePath;
                if (Directory.Exists(path))
                {
                    _sceneNames = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories)
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                }
                else
                {
                    _sceneNames = Array.Empty<string>();
                    Debug.LogWarning($"Scene path '{path}' does not exist. Please enter a valid directory.");
                }
            }
            else
            {
                _sceneNames = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToArray();
            }
        }

        private static void CheckAndRefreshScenes()
        {
            string[] currentScenes;
            if (FetchAllScenes)
            {
                string path = CustomScenePath;
                if (Directory.Exists(path))
                {
                    currentScenes = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories)
                        .Select(Path.GetFileNameWithoutExtension)
                        .ToArray();
                }
                else
                {
                    currentScenes = Array.Empty<string>();
                }
            }
            else
            {
                currentScenes = EditorBuildSettings.scenes
                    .Where(scene => scene.enabled)
                    .Select(scene => Path.GetFileNameWithoutExtension(scene.path))
                    .ToArray();
            }

            if (currentScenes.SequenceEqual(_sceneNames)) return;
            _sceneNames = currentScenes;
            SelectCurrentScene();
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

        private static void LoadScene(string sceneName)
        {
            string scenePath;

            if (FetchAllScenes)
            {
                string path = CustomScenePath;
                if (Directory.Exists(path))
                {
                    scenePath = Directory.GetFiles(path, "*.unity", SearchOption.AllDirectories)
                        .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p) == sceneName);
                }
                else
                {
                    Debug.LogError($"Scene path '{path}' does not exist.");
                    return;
                }
            }
            else
            {
                scenePath = EditorBuildSettings.scenes
                    .FirstOrDefault(scene => scene.enabled && scene.path.Contains(sceneName))?.path;
            }

            if (!string.IsNullOrEmpty(scenePath))
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(scenePath);
                }
            }
            else
            {
                Debug.LogError($"Scene not found: {sceneName}");
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