using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class LoadAssetBundles : MonoBehaviour
{
    AssetBundle myLoadedAssetBundle;
    public string[] bundles;

    AssetBundle[] LoadedBundles;

    bool arrayFull;
    const int mainSceneIndex = 9;


    void Awake()
    {
        //LoadBundleList();
    }

    void Update()
    {
    }

    void LoadBundleList()
    {
        foreach (string s in bundles)
        {
            string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", s);
            AssetBundle bundle = AssetBundle.LoadFromFile(path);
            Debug.Log(bundle == null ? "Bundle is not loaded" : "Bundle " + bundle.name + " is loaded");
        }
        Debug.Log("Starting load main scene");
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    void OnEnable()
    {
        /*DontDestroyOnLoad(this.gameObject);
        bundles = AssetDatabase.GetAllAssetBundleNames();
        LoadedBundles = new AssetBundle[bundles.Length];
        //LoadAllAssets();
        AlternateLoad();*/
    }

    void LoadAllAssets()
    {
        for (int i = 0; i < bundles.Length; i++)
        {
            string s = bundles[i];
            StartCoroutine(LoadAssetBundleAsync(s, i));
        }
    }

    bool LoadAssetBundle(string bundle)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundle);
        Debug.Log(path);


        myLoadedAssetBundle = AssetBundle.LoadFromFile(path);
        Debug.Log(myLoadedAssetBundle == null ? "Failed to load asset" : "Asset bundle successfully loaded");
        if (myLoadedAssetBundle == null)
            return false;
        else
            return true;
    }

    IEnumerator LoadAssetBundleAsync(string bundle, int index)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", bundle);
        AssetBundleCreateRequest req = AssetBundle.LoadFromFileAsync(path);
        yield return req;

        myLoadedAssetBundle = req.assetBundle;
        if (myLoadedAssetBundle != null)
            LoadedBundles[index] = myLoadedAssetBundle;

        //Check if all assets are loaded
        bool temp = true;
        for (int i = 0; i < LoadedBundles.Length; i++)
        {
            if (LoadedBundles[i] == null)
            {
                temp = false;
            }
        }

        if (temp)
            LoadMainScene();
    }

    void LoadMainScene()
    {
        Debug.Log("All bundle loaded, Loading main scene !");
        string[] scenes = LoadedBundles[mainSceneIndex].GetAllScenePaths();
        string scenePath = System.IO.Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadSceneAsync(scenePath);
    }

    void AlternateLoad()
    {
        // string[] dependencies = new string[] { "sound", "other", "characters01", "ui", "fx", "cosmetics", "bridge01", "voices01", "fonts", "assetbundle01", "jinmu" };

        string path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", "loading");
        AssetBundle loaded = AssetBundle.LoadFromFile(path);
        Debug.Log(loaded == null ? "Asset not loaded" : "Asset loaded");

        /* foreach (string s in dependencies)
         {
             path = Path.Combine(Application.streamingAssetsPath, "AssetBundles", s);
             AssetBundle.LoadFromFile(path);
         }
 */

        string[] scenes = loaded.GetAllScenePaths();
        string sceneName = System.IO.Path.GetFileNameWithoutExtension(scenes[0]);
        SceneManager.LoadSceneAsync(sceneName);
    }
}