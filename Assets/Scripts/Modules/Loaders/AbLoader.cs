using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace GameLoader
{
    public class AbLoader : BaseLoader
    {
        public delegate void CAssetBundleLoaderDelegate(bool isOk, AssetBundle ab);
        
        public static Action<string> NewAssetBundleLoaderEvent;
        
        public AssetBundle Bundle
        {
            get { return ResultObjest as AssetBundle; }
        }
        
        private string RelativeResourceUrl;
        private List<UnityEngine.Object> _loadedAssets;
        
        /// <summary>
        /// AssetBundle加载方式
        /// </summary>
        private LoaderMode _loaderMode;
        
        private float beginTime;
        private string dependFrom = string.Empty;
        
        /// <summary>
        /// 加载ab
        /// </summary>
        /// <param name="url">资源路径</param>
        /// <param name="callback">加载完成的回调</param>
        /// <param name="loaderMode">Async异步，sync同步</param>
        /// <returns></returns>
        public static AbLoader Load(string url, CAssetBundleLoaderDelegate callback = null,
            LoaderMode loaderMode = LoaderMode.Async, bool forceCreateNew = false)
        {
            // if(!AssetBundleConfig.IsEditorMode && !url.EndsWith(AssetBundleConfig.AssetBundleSuffix))
            //     url = url + AssetBundleConfig.AssetBundleSuffix;
            url = url.ToLower();
            LoaderDelgate newCallback = null;
            if (callback != null)
            {
                newCallback = (isOk, obj) => callback(isOk, obj as AssetBundle);
            }
            var newLoader = AutoNew<AbLoader>(url, newCallback, forceCreateNew, loaderMode);

            return newLoader;
        }
        
        private static bool _hasPreloadAssetBundleManifest = false;
        private static AssetBundle _mainAssetBundle;
        private static AssetBundleManifest _assetBundleManifest;
        
          public override void Init(string url, params object[] args)
        {
            base.Init(url);
#if UNITY_EDITOR
            //if (AssetBundleConfig.IsEditorMode)
           // {
           //LoadInEditor(url);
          //      return;
           // }
#endif
            //PreLoadManifest();
            _loaderMode = (LoaderMode)args[0];

            if (NewAssetBundleLoaderEvent != null)
                NewAssetBundleLoaderEvent(url);

            RelativeResourceUrl = url;
            AssetBundleManager.Instance.StartCoroutine(LoadAssetBundle(url));
        }
        
        /// <summary>
        /// 依赖的AssetBundleLoader
        /// </summary>
        private AbLoader[] _depLoaders;
        private IEnumerator LoadAssetBundle(string relativeUrl)
        {
            var abPath = relativeUrl.ToLower();
            //var deps = _assetBundleManifest.GetAllDependencies(abPath);
            var deps = new string[] { };  //AssetBundleManager.Instance.getAbDeps(abPath);
            _depLoaders = new AbLoader[deps.Length];
            for (var d = 0; d < deps.Length; d++)
            {
                var dep = deps[d];
                _depLoaders[d] = AbLoader.Load(dep, null, _loaderMode);
                if(_depLoaders[d].dependFrom == string.Empty)
                    _depLoaders[d].dependFrom = relativeUrl;
            }
            for (var l = 0; l < _depLoaders.Length; l++)
            {
                var loader = _depLoaders[l];
                while (!loader.IsCompleted)
                {
                    yield return null;
                }
            }
            relativeUrl = relativeUrl.ToLower();

          //  if (MApp.Instance.IsLogAbLoadCost) beginTime = Time.realtimeSinceStartup;

          string _fullUrl = string.Empty; //PathMgr.GetAbFullPath(relativeUrl);
             
            if (string.IsNullOrEmpty(_fullUrl))
            {
                OnFinish(null);
                yield break;
            }
      
            AssetBundle assetBundle = null;
            if (_loaderMode == LoaderMode.Sync)
            {
                assetBundle = AssetBundle.LoadFromFile(_fullUrl);
            }
            else
            {
                var request = AssetBundle.LoadFromFileAsync(_fullUrl);
                while (!request.isDone)
                {
                    if (IsReadyDisposed) // 中途释放
                    {
                        OnFinish(null);
                        yield break;
                    }
                    Progress = request.progress;
                    yield return null;
                }
                assetBundle = request.assetBundle;
            }
            if (assetBundle == null)
                Debug.LogErrorFormat("assetBundle is NULL: {0}", RelativeResourceUrl);
           // if (MApp.Instance.IsLogAbLoadCost) 
               // Debug.LogFormat("[Finish] Load AssetBundle {0}, CostTime {1}s {2}", relativeUrl, Time.realtimeSinceStartup - beginTime,dependFrom);
            OnFinish(assetBundle);
        }

        protected override void OnFinish(object resultObj)
        {
            base.OnFinish(resultObj);
#if UNITY_EDITOR
            if (true)//AssetBundleConfig.IsSimulateMode)
            {
                if (Url.EndsWith("material.assetbundle"))
                {
                    var bundle = resultObj as AssetBundle;
                    foreach (var asset in bundle.LoadAllAssets())
                    {
                        var mat = asset as Material;
                        if (mat != null)
                        {
                            //mat.shader = ShaderManager.Find(mat.shader.name);
                        }
                    }
                }
            }
#endif
            
        }

        protected override void DoDispose()
        {
            base.DoDispose();
            if (Bundle != null && RefCount<=0)
            {
                Bundle.Unload(true);
            }

            if (_depLoaders != null && _depLoaders.Length > 0)
            {
                foreach (var depLoader in _depLoaders)
                {
                    // if (depLoader.Bundle != null && depLoader.RefCount<=0) 
                    //     depLoader.Bundle.Unload(true);
                    depLoader.Release();
                }
            }

            _depLoaders = null;


            if (_loadedAssets != null)
            {
                foreach (var loadedAsset in _loadedAssets)
                {
                    Object.DestroyImmediate(loadedAsset, true);
                }
                _loadedAssets.Clear();
            }
        }

        public override void Release()
        {
            if (Application.isEditor)
            {
                if (Url != null && Url.Contains("Arial"))
                {
                    Debug.LogErrorFormat("要释放Arial字体！！错啦！！builtinextra:{0}", Url);
                }
            }

            base.Release();
        }

        /// 舊的tips~忽略
        /// 原以为，每次都通过getter取一次assetBundle会有序列化解压问题，会慢一点，后用AddWatch调试过，发现如果把.assetBundle放到Dictionary里缓存，查询会更慢
        /// 因为，估计.assetBundle是一个纯Getter，没有做序列化问题。  （不保证.mainAsset）
        public void PushLoadedAsset(Object getAsset)
        {
            if (_loadedAssets == null)
                _loadedAssets = new List<Object>();
            _loadedAssets.Add(getAsset);
        }

        private void LoadInEditor(string path)
        {
#if UNITY_EDITOR
            // Object getAsset = UnityEditor.AssetDatabase.LoadAssetAtPath(
            //     "Assets/" + AssetBundleConfig.AssetsFolderName + "/" + path + ".prefab", typeof(UnityEngine.Object));
            // if (getAsset == null)
            // {
            //     Debug.LogErrorFormat("Asset is NULL(from {0} Folder): {1}", AssetBundleConfig.AssetsFolderName, path);
            // }

            OnFinish(null);
#endif
        }
    
    }
}

