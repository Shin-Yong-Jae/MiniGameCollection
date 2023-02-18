using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton_Awake<T> : MonoBehaviour where T : MonoBehaviour
{
    protected static T _instance;
    public static T instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<T>();
            }

            if (_instance == null)
            {
                GameObject unityObject = new GameObject();
                var newSingleton = unityObject.AddComponent<T>();
                _instance = newSingleton;

                unityObject.name = _instance.GetType().Name;

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    protected virtual void Awake()
    {
        T temp = instance;
        DontDestroyOnLoad(this.gameObject);
    }
}