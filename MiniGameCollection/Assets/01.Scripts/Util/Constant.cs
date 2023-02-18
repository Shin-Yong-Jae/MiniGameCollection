using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Constant
{
    public static bool applicationIsQuitting = false;

    public static readonly string RESOURCES_BUNDLE_PATH = "Assets/Resources/";

    public static readonly string EXT_ATLAS = ".spriteatlas";
    public static readonly string EXT_CSV = ".csv";
    public static readonly string EXT_PREFAB = ".Prefab";
    public static readonly string EXT_ASSET = ".Asset";

    // PlayerPrefs
    public static readonly string NEW_PLAYER = "New_Player";

    [RuntimeInitializeOnLoadMethod]
    static void RunOnStart()
    {
        Application.quitting += () => applicationIsQuitting = true;
    }
}
