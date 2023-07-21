using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.XR;

/// <summary>
/// 按地址加载,使用地址字符串来加载资产
/// </summary>
public class LoadWithAddress : MonoBehaviour
{
    /// <summary>
    /// 
    /// </summary>
    public string address;
    private AsyncOperationHandle<GameObject> handle;

    private void Start()
    {
        handle = Addressables.LoadAssetAsync<GameObject>(address);
        handle.Completed += HandleComplete;
    }
    
    private void HandleComplete(AsyncOperationHandle<GameObject> operation)
    {
        if (operation.Status == AsyncOperationStatus.Succeeded)
        {
            Instantiate(operation.Result, transform);
        }
        else
        {
            Debug.LogError($"Asset for {address} failed to load.");
        }
    }

    private void OnDestroy()
    {
        // 释放资源
        Addressables.Release(handle);
    }
}

