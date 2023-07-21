using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Timeline.Actions;

namespace GameLoader
{
    public abstract class BaseLoader:IAsyncObject
    {
        public delegate void LoaderDelgate(bool isOk, object resultObject);
        List<LoaderDelgate> _afterFinishedCallbacks = new List<LoaderDelgate>();
        
        public object ResultObjest { get; private set; }
        
        public object AsyncResult
        {
            get { return ResultObjest; }
        }

        public bool IsCompleted { get; private set; }

        public bool IsError { get; private set; }

        public string AsyncMessage
        {
            get { return null; }
        }

        public bool IsSuccess
        {
            get { return !IsError && ResultObjest != null && !IsReadyDisposed;}
        }
        
        /// <summary>
        /// RefCount 为 0，进入预备状态
        /// </summary>
        protected bool IsReadyDisposed { get; private set; }
        
        /// <summary>
        /// ForceNew的，非AutoNew
        /// </summary>
        public bool IsForceNew;
        
        [System.NonSerialized]
        public float InitTiming = -1;
        [System.NonSerialized]
        public float FinishTiming = -1;
        /// <summary>
        /// 加载用时
        /// </summary>
        public float FinishUsedTime
        {
            get
            {
                if (!IsCompleted) return -1f;
                return FinishTiming - InitTiming;
            }
        }

        /// <summary>
        /// 引用次数
        /// </summary>
        private int refCount = 0;

        public int RefCount
        {
            get { return refCount; }
            set { refCount = value; }
        }
        
        public string Url { get;private set; }
        /// <summary>
        /// 进度0 - 1
        /// </summary>
        public virtual float Progress { get; protected set; }

        public float progress
        {
            get { return Progress; }
        }
        
        public event Action DisposeEvent;
        public event Action<string> SetDescEvent;
        private string _desc = "";
        
        /// <summary>
        /// 描述, 额外文字, 一般用于资源Debugger用
        /// </summary>
        /// <returns></returns>
        public virtual string Desc
        {
            get { return _desc; }
            set
            {
                _desc = value;
                if (SetDescEvent != null)
                    SetDescEvent(_desc);
            }
        }

        protected BaseLoader()
        {
            RefCount = 0;
        }

        public virtual void Init(string url, params object[] args)
        {
            InitTiming = Time.realtimeSinceStartup;
            ResultObjest = null;
            IsReadyDisposed = false;
            IsError = false;
            IsCompleted = false;
            Url = url;
            Progress = 0f;
        }

        protected static T AutoNew<T>(string url, LoaderDelgate callback = null, bool forceCreateNew = false,
            params object[] initArgs) where T : BaseLoader,new()
        {
            if (string.IsNullOrEmpty(url))
            {
                Debug.LogError($"{typeof(T)}:AutoNew url 为空");
                return null;
            }
            Dictionary<string, BaseLoader> typesDict = AssetBundleManager.GetTypeDict(typeof(T));
            BaseLoader loader =null;
            typesDict.TryGetValue(url, out loader);
            if (forceCreateNew || loader == null)
            {
                loader = new T();
                if (!forceCreateNew)
                {
                    
                }
                loader.IsForceNew = forceCreateNew;
                loader.Init(url, initArgs);
            }else if (loader != null && loader.IsCompleted && loader.IsError)
            {
                loader.Init(url,initArgs);
            }
            else
            {
                if (loader.RefCount < 0)
                {
                    Debug.LogError("Error RefCount!");
                }
            }

            loader.refCount++;
            // RefCount++了，重新激活，在队列中准备清理的Loader,状态恢复使用中
            if (AssetBundleManager.UnUsesLoaders.ContainsKey(loader))
            {
                AssetBundleManager.UnUsesLoaders.Remove(loader);
                loader.Revive();
            }
            loader.AddCallback(callback);
            return loader as T;
        }

        public virtual void Revive()
        {
            IsReadyDisposed = false;
        }

        public void AddCallback(LoaderDelgate callback)
        {
            if (callback != null)
            {
                if (IsCompleted)
                {
                    if(ResultObjest ==null)
                        Debug.LogError($"Null ResultAsset{Url}");
                    callback(ResultObjest != null, ResultObjest);
                }else
                    _afterFinishedCallbacks.Add(callback);
            }
        }

        protected virtual void OnFinish(object resultObj)
        {
            ResultObjest = resultObj;
            var callbackObject = !IsReadyDisposed ? ResultObjest : null;
            FinishTiming = Time.realtimeSinceStartup;
            Progress = 1f;
            IsError = callbackObject == null;
            IsCompleted = true;
            DoCallback(IsSuccess, callbackObject);
            if (IsReadyDisposed)
            {
                Debug.LogFormat("[AbstractResourceLoader:OnFinish]时，准备Disposed {0}", Url);
            }
        }
        
        protected void DoCallback(bool isOk, object resultObj)
        {
            foreach (var callback in _afterFinishedCallbacks)
            {
                callback(isOk, resultObj);
            }
            _afterFinishedCallbacks.Clear();
        }

        public virtual void Release()
        {
            if (IsReadyDisposed )
            {
                Debug.LogWarningFormat("[{0}]repeat  dispose! {1}, Count: {2}", GetType().Name, this.Url, RefCount);
            }
            if(AssetBundleManager.hasDestroy) return;
            RefCount--;
            if (RefCount <= 0)
            {
                // 加入队列，准备Dispose
                AssetBundleManager.UnUsesLoaders[this] = Time.time;
                IsReadyDisposed = true;
                OnReadyDisposed();
            }
            
        }
        
        protected virtual void OnReadyDisposed()
        {
        }
        
        public void Dispose()
        {
            if (DisposeEvent != null)
                DisposeEvent();

            if (!IsForceNew)
            {
                var type = GetType();
                var typeDict = AssetBundleManager.GetTypeDict(type);
                if (Url != null) 
                {
                    var bRemove = typeDict.Remove(Url);
                    if (!bRemove)
                    {
                        Debug.LogWarningFormat("[{0}:Dispose]No Url: {1}, Cur RefCount: {2}", type.Name, Url, RefCount);
                    }
                }
            }

            if (IsCompleted)
                DoDispose();
            // 未完成，在OnFinish时会执行DoDispose
        }

        protected virtual void DoDispose()
        {
           
        }
        
        
    }
}

