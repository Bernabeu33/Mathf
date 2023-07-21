using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameLoader
{
    public class AssetsLoader : BaseLoader
    {
        public delegate void AssetFileBridgeDelegate(bool isOk, Object resultObj);
        
        public Object Asset
        {
            get { return ResultObjest as Object; }
        }
        
      
    }
}

