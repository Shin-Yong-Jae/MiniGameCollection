using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class ResourceManager : Singleton_Awake<ResourceManager>
{
    private Dictionary<string, AtlasInfo> _scriptableAtlas;
    public Dictionary<string, AtlasInfo> scriptableAtlas
    {
        get
        {
            if (_scriptableAtlas == null)
                _scriptableAtlas = (Load_Resource("Data/AtlasDataList.asset", typeof(AtlasData)) as AtlasData).dicDat;
            return _scriptableAtlas;
        }
    }

    public Dictionary<string, Object> _dicObject = new Dictionary<string, Object>();

    private Dictionary<string, SpriteAtlas> _dicSpriteAtlas = new Dictionary<string, SpriteAtlas>();

    public SpriteAtlas Get_Atlas(string name)
    {
        if (!_dicSpriteAtlas.ContainsKey(name))
        {
            _dicSpriteAtlas.Add(name, Load_Resource(scriptableAtlas[name].path, typeof(SpriteAtlas)) as SpriteAtlas);
        }

        return _dicSpriteAtlas[name];
    }

    public Sprite Get_Sprite(string name, string szResourceName)
    {
        SpriteAtlas atlas = Get_Atlas(name);

        return atlas.GetSprite(szResourceName);
    }

    public Object Load_Resource(string path, System.Type type)
    {
        if (_dicObject.ContainsKey(path))
            return _dicObject[path];

        Object resource = null;

        // 빌드 포함 리소스
        if (null == resource)
        {
            resource = Resources.Load(Path.ChangeExtension(path, null), type);
        }

        // 저장
        if (null != resource)
            _dicObject.Add(path, resource);

#if QA_Define
        if (null == resource)
            D.Log("Resource null : " + path);
#endif

        return resource;
    }

    public void Release_Resource()
    {
        foreach (var pair in _dicObject)
        {
            Resources.UnloadAsset(pair.Value);
        }
        _dicObject.Clear();

    }
}
