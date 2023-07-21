using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class TestChainedBinary : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        ChainedBinaryTree<string> tree = new ChainedBinaryTree<string>("A");
        TreeNode<string> tree1 = tree.insertLeftNode("B", tree.getRootNode());
        TreeNode<string> tree2 = tree.insertRightNode("C", tree.getRootNode());
        tree.insertLeftNode("D", tree1);
        TreeNode<string> tree3 = tree.insertRightNode("E", tree1);
        tree.insertRightNode("F", tree2);
        tree.insertLeftNode("G", tree3);
        
        string result = string.Empty;
        tree.PreOrderTraversal(tree.getRootNode(), ref result);
        Debug.Log("先序遍历:" + result);

        result = string.Empty;
        tree.MidTrvarsal(tree.getRootNode(), ref result);
        Debug.Log("中序遍历:" + result);
        
        result = string.Empty;
        tree.AfterTrvarsal(tree.getRootNode(), ref result);
        Debug.Log("后序遍历:" + result);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
