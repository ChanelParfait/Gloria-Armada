using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class FindAudioMixerGroupReferences : EditorWindow
{
    private AudioMixerGroup searchMixerGroup;
    private AudioMixerGroup newMixerGroup;
    private List<string> results;

    [MenuItem("Tools/Find Audio Mixer Group References")]
    public static void ShowWindow()
    {
        GetWindow<FindAudioMixerGroupReferences>("Find Audio Mixer Group References");
    }

    private void OnGUI()
    {
        GUILayout.Label("Search for Audio Mixer Group References", EditorStyles.boldLabel);

        searchMixerGroup = (AudioMixerGroup)EditorGUILayout.ObjectField("Audio Mixer Group", searchMixerGroup, typeof(AudioMixerGroup), false);
        newMixerGroup = (AudioMixerGroup)EditorGUILayout.ObjectField("New Audio Mixer Group", newMixerGroup, typeof(AudioMixerGroup), false);

        if (GUILayout.Button("Find References"))
        {
            FindReferences();
        }

        if (GUILayout.Button("Replace References"))
        {
            ReplaceReferences();
        }

        if (results != null)
        {
            GUILayout.Label("References found:", EditorStyles.boldLabel);

            foreach (string result in results)
            {
                GUILayout.Label(result);
            }
        }
    }

    private void FindReferences()
    {
        results = new List<string>();

        // Search in prefabs
        string[] guids = AssetDatabase.FindAssets("t:Prefab");
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            AudioSource[] audioSources = prefab.GetComponentsInChildren<AudioSource>(true);

            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.outputAudioMixerGroup == searchMixerGroup)
                {
                    results.Add(path);
                }
            }
        }

        // Search in scenes
        string[] sceneGuids = AssetDatabase.FindAssets("t:Scene");
        foreach (string sceneGuid in sceneGuids)
        {
            string scenePath = AssetDatabase.GUIDToAssetPath(sceneGuid);
            Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

            AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource.outputAudioMixerGroup == searchMixerGroup)
                {
                    results.Add(scenePath + " (" + audioSource.gameObject.name + ")");
                }
            }

            EditorSceneManager.CloseScene(scene, true);
        }

        if (results.Count == 0)
        {
            results.Add("No references found.");
        }
    }

    private void ReplaceReferences()
    {
        if (newMixerGroup == null)
        {
            Debug.LogError("New Audio Mixer Group is not assigned.");
            return;
        }

        foreach (string result in results)
        {
            if (result.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(result);
                AudioSource[] audioSources = prefab.GetComponentsInChildren<AudioSource>(true);

                foreach (AudioSource audioSource in audioSources)
                {
                    if (audioSource.outputAudioMixerGroup == searchMixerGroup)
                    {
                        audioSource.outputAudioMixerGroup = newMixerGroup;
                        EditorUtility.SetDirty(audioSource);
                    }
                }

                PrefabUtility.SavePrefabAsset(prefab);
            }
            else
            {
                string scenePath = result.Substring(0, result.LastIndexOf(" ("));
                Scene scene = EditorSceneManager.OpenScene(scenePath, OpenSceneMode.Additive);

                AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
                foreach (AudioSource audioSource in audioSources)
                {
                    if (audioSource.outputAudioMixerGroup == searchMixerGroup)
                    {
                        audioSource.outputAudioMixerGroup = newMixerGroup;
                        EditorUtility.SetDirty(audioSource);
                    }
                }

                EditorSceneManager.SaveScene(scene);
                EditorSceneManager.CloseScene(scene, true);
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Audio Mixer Group references updated.");
    }
}
