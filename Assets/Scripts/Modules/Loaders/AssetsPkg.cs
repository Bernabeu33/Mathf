using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLoader
{
    public class AssetsPkg : IDisposable
    {
        private static Queue<AssetsPkg> _cachePool = new Queue<AssetsPkg>();

        public static AssetsPkg Load()
        {
            AssetsPkg pkg;
            if (_cachePool.Count > 0)
            {
                pkg = _cachePool.Dequeue();
            }
            else
            {
                pkg = new AssetsPkg();
            }
            pkg.Init();
            return pkg;
        }
        private Dictionary<string, List<BaseLoader>> _loaders = null;
        
        AssetsPkg()
        {
        }
        private void Init()
        {
            _loaders = new Dictionary<string, List<BaseLoader>>();
        }
        
        private void AddLoder(string path, BaseLoader loader)
        {
            List<BaseLoader> pathLoaders;
            _loaders.TryGetValue(path, out pathLoaders);
            if (pathLoaders == null)
            {
                pathLoaders = new List<BaseLoader>();
                _loaders.Add(path, pathLoaders);
            }
            pathLoaders.Add(loader);
        }
        
      
        
      
        
        
        public void Dispose()
        {
            
        }
    }
}

