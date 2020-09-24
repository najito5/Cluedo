using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance = default;
    public static T Instance
    {
        get
        {
            if (instance == null) return default;
            return instance;
        }
    }

    [SerializeField, Header("Keep it through levels ?")] bool keep = false;

    protected virtual void Awake()
    {
        InitSingleton();
    }
    void InitSingleton()
    {
        if (instance != null && instance != this)
        {
            Destroy(this);
        }
        if (keep) DontDestroyOnLoad(transform);
        instance = this as T;
        name += $"[{typeof(T).Name}]";

    }




}
