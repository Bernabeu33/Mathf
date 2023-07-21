using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameLoader;

public enum LoaderMode
{
    Async,
    Sync,
}

public class AssetBundleManager : MonoBehaviour
{
    private static AssetBundleManager instance;

    public static AssetBundleManager Instance
    {
        get { return instance; }
        set { instance = value; }
    }
    
    private static readonly Dictionary<Type, Dictionary<string, BaseLoader>> _loadersPool = new Dictionary<Type, Dictionary<string, BaseLoader>>();

    // <summary>
    /// 进行垃圾回收
    /// </summary>
    internal static readonly Dictionary<BaseLoader, float> UnUsesLoaders =  new Dictionary<BaseLoader, float>();
    // <summary>
    /// 缓存起来要删掉的，供DoGarbageCollect函数用, 避免重复的new List
    /// </summary>
    private static readonly List<BaseLoader> CacheLoaderToRemoveFromUnUsed = new List<BaseLoader>();
    
    public static bool hasDestroy = false;
    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }
    
    public static Dictionary<string, BaseLoader> GetTypeDict(Type type)
    {
        Dictionary<string, BaseLoader> typesDict;
        if (!_loadersPool.TryGetValue(type, out typesDict))
        {
            typesDict = _loadersPool[type] = new Dictionary<string, BaseLoader>();
        }

        return typesDict;
    }

    private void Update()
    {
        CheckGcCollect();
    }
    
    /// <summary>
    /// 上次做GC的时间
    /// </summary>
    private static float _lastGcTime = -1;

    private static float _GCIntervailTime = 1;
    
    private const float LoaderDisposeTime = 0;
    public static void CheckGcCollect()
    {
        if (_lastGcTime.Equals(-1) || (Time.time - _lastGcTime) >= _GCIntervailTime)
        {
            DoGarbageCollect();
            _lastGcTime = Time.time;
        }
    }
    
    /// <summary>
    /// 进行垃圾回收
    /// </summary>
    internal static void DoGarbageCollect()
    {
        foreach (var kv in UnUsesLoaders)
        {
            var loader = kv.Key;
            var time = kv.Value;
            if ((Time.time - time) >= LoaderDisposeTime)
            {
                CacheLoaderToRemoveFromUnUsed.Add(loader);
            }
        }

        for (var i = CacheLoaderToRemoveFromUnUsed.Count - 1; i >= 0; i--)
        {
            try
            {
                var loader = CacheLoaderToRemoveFromUnUsed[i];
                UnUsesLoaders.Remove(loader);
                CacheLoaderToRemoveFromUnUsed.RemoveAt(i);
                loader.Dispose();
            }
            catch (Exception e)
            {
                // Log.LogException(e);
            }
        }

        if (CacheLoaderToRemoveFromUnUsed.Count > 0)
        {
            UnityEngine.Debug.LogError("[DoGarbageCollect]CacheLoaderToRemoveFromUnUsed not empty!!");
        }
    }


    private void OnDestroy()
    {
        hasDestroy = true;
    }
}
