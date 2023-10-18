#if UNITY_EDITOR
using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _Scripts.Technical
{
    [InitializeOnLoad]
    public static class DefaultSceneLoader
    {
        static DefaultSceneLoader()
        {
            EditorApplication.playModeStateChanged += LoadDefaultScene;
        }

        private static void LoadDefaultScene(PlayModeStateChange state)
        {
            if (!EditorApplication.isPlaying)
            {
                return;
            }
            if (SceneManager.GetActiveScene().buildIndex == 0)
            {
                return;
            }
            var loadDefaultSceneOnPlay = true;
            if (EditorPrefs.HasKey("LoadDefaultSceneOnPlay"))
            {
                loadDefaultSceneOnPlay = EditorPrefs.GetBool("LoadDefaultSceneOnPlay");
            }

            if (!loadDefaultSceneOnPlay)
            {
                return;
            }

            switch (state)
            {
                case PlayModeStateChange.ExitingEditMode:
                    EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                    break;
                case PlayModeStateChange.EnteredPlayMode:

                    Debug.Log("<color=green>DefaultSceneLoader: </color>Load default scene");
                    SceneManager.LoadScene(0);
                    break;
                case PlayModeStateChange.EnteredEditMode:
                    break;
                case PlayModeStateChange.ExitingPlayMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }
    }

    public class DefaultSceneLoaderWindow : EditorWindow
    {
        [MenuItem("Yaroslav3452/Default Scene Loader")]
        public static void ShowWindow()
        {
            GetWindow<DefaultSceneLoaderWindow>("Default Scene Loader");
        }

        private void OnGUI()
        {
            //current isloadDefultScene variable state
            var currentSavedState = EditorPrefs.GetBool("LoadDefaultSceneOnPlay");
            GUILayout.Label("Current Load default scene on play: " + currentSavedState);
            if (GUILayout.Button("Switch"))
            {
                //toggle state
                EditorPrefs.SetBool("LoadDefaultSceneOnPlay", !currentSavedState);
            }
        }
    }
}
#endif