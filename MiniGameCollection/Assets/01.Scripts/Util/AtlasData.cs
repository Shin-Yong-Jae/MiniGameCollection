using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
//using UnityEditor;

public class AtlasData : ScriptableObject
{
    public List<AtlasInfo> listData;

    /*
	[MenuItem("Assets/Create Asset")]	
	static void CreateAsset()
	{
        var example = CreateInstance<AtlasData>();
        
        AssetDatabase.CreateAsset(example, "Assets/AtlasDataList.asset");
		AssetDatabase.Refresh();
	}
    */

    private Dictionary<string, AtlasInfo> _dicDat;
    public Dictionary<string, AtlasInfo> dicDat
    {
        get
        {
            if (_dicDat == null)
            {
                Init();
            }
            return _dicDat;
        }
    }

    void Init()
    {
        _dicDat = new Dictionary<string, AtlasInfo>();
        for (int i = 0; i < listData.Count; ++i)
        {
            _dicDat.Add(listData[i].name, listData[i]);
        }
    }

    [ContextMenu("Fill_Auto")]
    public void Fill_Auto()
    {
        string path = Constant.RESOURCES_BUNDLE_PATH + "Image";

        string[] atlas = Directory.GetFiles(path, "*.spriteatlas", SearchOption.AllDirectories);

        for (int i = 0; i < atlas.Length; ++i)
        {
            atlas[i] = atlas[i].Replace(Constant.RESOURCES_BUNDLE_PATH, "");

            AtlasInfo info = new AtlasInfo();

            string[] split = atlas[i].Split(new char[] { '/', '\\' });

            info.name = split[split.Length - 1].Split('.')[0];
            info.path = atlas[i];

            if (!listData.Exists(x => x.name == info.name))
                listData.Add(info);
        }

    }
}


[System.Serializable]
public class AtlasInfo
{
    public string name;

    public string path;

}