using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneDatabase : SingletonScriptableObject<CutsceneDatabase>
{
#if UNITY_EDITOR
    /// <summary>
    /// Popups creation window when missing
    /// </summary>
    [UnityEditor.InitializeOnLoadMethod]
    protected static void InitializeOnLoadMethod()
    {
        UnityEditor.EditorApplication.delayCall += () => CreateAsset();
    }
#endif

    [SerializeField]
    private List<CutsceneManifest> cutsceneManifests = new List<CutsceneManifest>();
    public ICollection CutsceneManifests => cutsceneManifests;

    private Dictionary<string, CutsceneManifest> _cutsceneManifestDictionary;
    private Dictionary<string, CutsceneManifest> CutsceneManifestDictionary 
    { 
        get 
        {
            if (_cutsceneManifestDictionary == null)
            {
                _cutsceneManifestDictionary = new Dictionary<string, CutsceneManifest>();
                foreach (var scene in cutsceneManifests)
                {
                    _cutsceneManifestDictionary.Add(scene.id, scene);
                }
            }
            return _cutsceneManifestDictionary;
        } 
    }

    public bool TryGetCutsceneManifestByEventID(string ID, out CutsceneManifest manifest)
    {
        if(CutsceneManifestDictionary.TryGetValue(ID, out manifest))
        {
            return true;
        }
        return false;
    }
}
