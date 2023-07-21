using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IComparable<Node>
{
    public Vector3Int gridPos;
    public int gCost = 0;
    public int hCost = 0;
    public int FCost => gCost + hCost;
    public bool isObstacle = false;
    public Node parentNode;

    public Vector2Int arrayPos;

    public Node(Vector3Int pos, int _x,int _y)
    {
        gridPos = pos;
        arrayPos = new Vector2Int(_x, _y);
        parentNode = null;
    }
    

    public int CompareTo(Node other)
    {
        int result = FCost.CompareTo(other.FCost);
        if (result == 0)
        {
            result = gCost.CompareTo(other.gCost);
        }

        return result;
    }
}
