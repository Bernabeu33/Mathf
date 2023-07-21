using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG;
using DG.Tweening;
using UnityEngine.Experimental.GlobalIllumination;
using Random = System.Random;

public class AStar : MonoBehaviour
{
    public Tilemap tileMap;
    public GameObject player;

    private GridNodes gridNodes;


    private Node startNode;
    private Node endNode;
    
    private int[,] map;

    private List<Node> openNodeList;
    private List<Node> closeNodeList;
    private Tween modeT;

    public TileBase selectTile;
    public TileBase normalTile;
    Random rondom;
    private void Awake()
    {
        rondom = new Random();
        openNodeList = new List<Node>();
        closeNodeList = new List<Node>();
        GenerateMap();
        player.transform.position = Vector3.zero;

    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
           Vector3 mousePos = Input.mousePosition;
           Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
           worldPos.z = 0;
           worldPos.x = (float)Math.Round(worldPos.x);
           worldPos.y = (float)Math.Round(worldPos.y);
           Node node = gridNodes.GetGridNode((int)worldPos.x+5, (int)worldPos.y+5);
           if (node == null)
           {
               return;
           }

           if (tileMap.GetTile(new Vector3Int(node.arrayPos.x - 5, node.arrayPos.y - 6, 0)) == null)
           {
               return;
           }

           Vector3 pos= player.transform.position;
           pos.x = (float)Math.Round(pos.x);
           pos.y = (float)Math.Round(pos.y);
           Node s_node = gridNodes.GetGridNode((int)pos.x+5, (int)pos.y+5);
           if(s_node == null)
               return;

           if (endNode != null)
           {
               tileMap.SetTile(new Vector3Int(endNode.arrayPos.x - 5, endNode.arrayPos.y - 6, 0), normalTile);
           }
           startNode = s_node;
           endNode = node;
           
           tileMap.SetTile(new Vector3Int(endNode.arrayPos.x -5, endNode.arrayPos.y -6, 0), selectTile);
           StartFindPath();
        }
    }

    private void StartFindPath()
    {
        modeT.Kill();
        openNodeList.Clear();
        closeNodeList.Clear();
      //  player.transform.position = startNode.gridPos;
        bool findPath = FindShortestPath();
        if (findPath)
        {
            closeNodeList.RemoveAt(0);
               
            Vector3[] path =  getPath();
            int time = rondom.Next(1, 3);
            modeT = player.transform.DOPath(path, time).SetEase(Ease.Linear);
        }
    }

    private Vector3[] getPath()
    {
        List<Vector3> pathList = new List<Vector3>();
        Node endNode = closeNodeList[closeNodeList.Count - 1];
        pathList.Add(endNode.gridPos);
        while (endNode.parentNode != startNode)
        {
            endNode =  endNode.parentNode;;
            pathList.Add(endNode.gridPos); 
        }
      
        pathList.Reverse();
        return pathList.ToArray();
    }
    
    private void GenerateMap()
    {
        BoundsInt area = tileMap.cellBounds;
        Vector3 size = tileMap.size;
        gridNodes = new GridNodes((int)size.x, (int)size.y);
        map = new int[(int)size.x, (int)size.y];
        for (int i = area.xMin; i < area.xMax; i++)
        {
            for (int j = area.yMin; j < area.yMax; j++)
            {
                TileBase tile = tileMap.GetTile(new Vector3Int(i, j, 0));
                if (tile == null)
                {
                    map[i+area.xMax - 1, j + 1 + area.yMax] = 1;
                    Node node = gridNodes.GetGridNode(i+area.xMax - 1, j + 1 + area.yMax);
                    if (node != null)
                    {
                        node.isObstacle = true;
                    }
                }
                else
                {
                    map[i+area.xMax - 1, j + 1 + area.yMax] = 0;
                }
            }
        }
    }

    public bool FindShortestPath()
    {
        bool pathFound = false;
        openNodeList.Add(startNode);
        while (openNodeList.Count > 0)
        {
            openNodeList.Sort();
            Node closeNode = openNodeList[0];
            openNodeList.RemoveAt(0);
            closeNodeList.Add(closeNode);
            if (closeNode == endNode)
            {
                pathFound = true;
                break;
            }
            // 计算周围八个点
            EvaluateNeighbourNodes(closeNode);
        }
        
        return pathFound;
    }

    private Node GetValidNeighbourNode(int x, int y)
    {
        if (x > gridNodes.width || y >= gridNodes.height || x < 0 || y < 0)
        {
            return null;
        }

        Node neighbourNode = gridNodes.GetGridNode(x, y);
        if (neighbourNode != null && (neighbourNode.isObstacle || closeNodeList.Contains(neighbourNode)))
        {
            return null;
        }

        return neighbourNode;
    }

    private int GetDistance(Node nodeA, Node nodeB)
    {
        int xDis = Mathf.Abs(nodeA.gridPos.x - nodeB.gridPos.x);
        int yDis = Mathf.Abs(nodeA.gridPos.y - nodeB.gridPos.y);
        if (xDis != yDis)
        {
            return (xDis > yDis) ? (14 * yDis + 10 * (xDis - yDis)):(14 * xDis + 10 * (yDis - xDis));
        }

        return xDis * 14;
    }

    private void EvaluateNeighbourNodes(Node curNode)
    {
        Vector2Int curNodePos = curNode.arrayPos;
        Node validNode = null;

        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if(x == 0 && y == 0)
                    continue;
                validNode = GetValidNeighbourNode(curNodePos.x + x, curNodePos.y + y);
                if (validNode != null)
                {
                    if (!openNodeList.Contains(validNode) && checkCanWalk(curNode, validNode))
                    {
                        // 检查是否能走障碍区域，斜着走
                        validNode.gCost = curNode.gCost + GetDistance(curNode, validNode);
                        validNode.hCost = GetDistance(validNode, endNode);
                        validNode.parentNode = curNode;
                        openNodeList.Add(validNode);
                    }
                }
            }   
        }
    }

    private bool checkCanWalk(Node curNode, Node nextNode)
    {
        bool canWalk = true;
        Node node1 = gridNodes.GetGridNode(curNode.arrayPos.x, nextNode.arrayPos.y);
        Node node2 = gridNodes.GetGridNode(nextNode.arrayPos.x, curNode.arrayPos.y);
        if (node1 != null && node1.isObstacle)
            canWalk = false;
        if (node2 != null && node2.isObstacle)
            canWalk = false;
        return canWalk;
    }
    
}
