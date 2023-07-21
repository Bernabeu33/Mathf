using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainedBinaryTree<T>
{
   private TreeNode<T> head { get; set; }

   public ChainedBinaryTree()
   {
      head = null;
   }

   public ChainedBinaryTree(T _head)
   {
      this.head = new TreeNode<T>(_head);
   }

   /// <summary>
   /// 获得该节点的左节点
   /// </summary>
   /// <param name="treeNode"></param>
   /// <returns></returns>
   public TreeNode<T> getLeftNode(TreeNode<T> treeNode)
   {
      if (treeNode == null)
         return null;
      return treeNode.leftChild;
   }

   /// <summary>
   /// 获得该节点的右节点
   /// </summary>
   /// <param name="treeNode"></param>
   /// <returns></returns>
   public TreeNode<T> getRightNode(TreeNode<T> treeNode)
   {
      if (treeNode == null)
         return null;
      return treeNode.rightChild;
   }

   /// <summary>
   /// 获得跟节点
   /// </summary>
   /// <returns></returns>
   public TreeNode<T> getRootNode()
   {
      return head;
   }

   /// <summary>
   /// 插入左节点
   /// </summary>
   /// <param name="value"></param>
   /// <param name="node"></param>
   /// <returns></returns>
   public TreeNode<T> insertLeftNode(T value, TreeNode<T> node)
   {
      if (node == null)
         throw new ArgumentNullException("参数错误");
      TreeNode<T> treeNode = new TreeNode<T>(value);
      TreeNode<T> childNode = node.leftChild;
      treeNode.leftChild = childNode;
      node.leftChild = treeNode;
      return treeNode;
   }

   /// <summary>
   /// 插入右节点
   /// </summary>
   /// <param name="val"></param>
   /// <param name="node"></param>
   /// <returns></returns>
   public TreeNode<T> insertRightNode(T val, TreeNode<T> node)
   {
      if (node == null)
         throw new ArgumentNullException("参数错误");
      TreeNode<T> treeNode = new TreeNode<T>(val);
      TreeNode<T> childNode = node.rightChild;
      treeNode.rightChild = childNode;
      node.rightChild = treeNode;
      return treeNode;
   }

   /// <summary>
   /// 删除当前节点的左节点
   /// </summary>
   /// <param name="node"></param>
   /// <returns></returns>
   public TreeNode<T> deleteLeftNode(TreeNode<T> node)
   {
      if (node == null||node.leftChild == null)
         throw new ArgumentNullException("参数错误");
      TreeNode<T> leftNode = node.leftChild;
      node.leftChild = null;
      return leftNode;
   }
   
   /// <summary>
   /// 删除当前节点的右节点
   /// </summary>
   /// <param name="node"></param>
   /// <returns></returns>
   public TreeNode<T> deleteRightNode(TreeNode<T> node)
   {
      if (node == null||node.rightChild == null)
         throw new ArgumentNullException("参数错误");
      TreeNode<T> rightChild = node.rightChild;
      node.rightChild = null;
      return rightChild;
   }

   /// <summary>
   /// 先序遍历: 跟节点 左节点  右节点
   /// </summary>
   /// <param name="node"></param>
   public void PreOrderTraversal(TreeNode<T> node, ref string result)
   {
      if (head == null)
      {
         Debug.Log("当前树为空");
         return;
      }
      
      if (node != null)
      {
         result = result + node.data + " ";
         PreOrderTraversal(node.leftChild, ref result);
         PreOrderTraversal(node.rightChild, ref result);
      }
   }

   /// <summary>
   /// 中序遍历：左节点 跟节点 右边节点
   /// </summary>
   /// <param name="node"></param>
   public void MidTrvarsal(TreeNode<T> node, ref string result)
   {
      if (head == null)
      {
         Debug.Log("当前树为空");
         return;
      }

      if (node != null)
      {
         MidTrvarsal(node.leftChild, ref result);
         result = result + node.data + " ";
         MidTrvarsal(node.rightChild, ref  result);
      }
   }

   public void AfterTrvarsal(TreeNode<T> node, ref string result)
   {
      if (head == null)
      {
         Debug.Log("当前树为空");
         return;
      }

      if (node != null)
      {
         AfterTrvarsal(node.leftChild, ref result);
         AfterTrvarsal(node.rightChild, ref result);
         result = result + node.data + " ";
      }
   }

   public bool ValidLeadNode(TreeNode<T> node)
   {
      if (node == null||node.leftChild == null)
         throw new ArgumentNullException("参数错误");
      if (node.leftChild != null && node.rightChild != null)
      {
         Debug.Log($"节点{node.data}不是叶子节点");
         return false;
      }
      Debug.Log($"节点{node.data}是叶子节点");
      return true;
   }
   
   
   
   

}
