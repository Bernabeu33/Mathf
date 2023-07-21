using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;

/// <summary>
/// 按标签加载,可以在一次操作中加载具有相同标签的一组资源
/// </summary>
public class LoadWithLabels : MonoBehaviour
{
   public List<string> keys = new List<string>(){"11"};

   private AsyncOperationHandle<IList<GameObject>> handle;

   private void Start()
   {
      float x = 2, z = 0;
      handle = Addressables.LoadAssetsAsync<GameObject>(keys, addressable =>
      {
         if (addressable != null)
         {
            Instantiate(addressable, new Vector3(x++ *2.0f, 0, z*0.2f), Quaternion.identity, transform);
            if (x > 9)
            {
               x = 0;
               z++;
            }
         }
      }, Addressables.MergeMode.Union, false);
      handle.Completed += HandleCompleted;
   }

   void HandleCompleted(AsyncOperationHandle<IList<GameObject>> operationHandle)
   {
      if (operationHandle.Status == AsyncOperationStatus.Succeeded)
      {
         
      }
      else
      {
         Debug.LogError("Some assets did not load.");
      }
   }

   private void OnDestroy()
   {
      Addressables.Release(handle);
   }
}
