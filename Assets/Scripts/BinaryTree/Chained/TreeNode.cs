using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

public class TreeNode<T>
{
    /// <summary>
    /// 节点数据
    /// </summary>
    public T data { get; set; }
    /// <summary>
    /// 左节点
    /// </summary>
    public TreeNode<T> leftChild { get; set; }
    /// <summary>
    /// 右节点
    /// </summary>
    public TreeNode<T> rightChild { get; set; }

    public TreeNode()
    {
        data = default(T);
        leftChild = null;
        rightChild = null;
    }

    public TreeNode(T item)
    {
        this.data = item;
        leftChild = null;
        rightChild = null;
    }
}
