using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupAssetBundles : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject loader = new GameObject();
        DontDestroyOnLoad(loader);
        loader.AddComponent<LoadAssetBundles>();
    }
}
