using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridNodes
{
    public int width;
    public int height;
    private Node[,] girdNode;

    public GridNodes(int width, int height)
    {
        this.width = width;
        this.height = height;

        girdNode = new Node[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                girdNode[x, y] = new Node(new Vector3Int(x - 5, y - 5, 0), x, y);
            }
        }
    }

    public Node GetGridNode(int xPos, int yPos)
    {
        if (xPos < width && yPos < height)
        {
            return girdNode[xPos, yPos];
        }
        Debug.Log("超出网格范围" + xPos);
        return null;
    }
    

}
