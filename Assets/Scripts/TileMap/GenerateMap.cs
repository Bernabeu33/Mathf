using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateMap : MonoBehaviour
{
    public Tilemap tileMap;
    public List<Sprite> spList;

    private void Start()
    {
        for (int i = -5; i < 5; i++)
        {
            for (int j = -5; j < 5; j++)
            {
                tileMap.SetTile(new Vector3Int(i,j,0), CreateTile());
            }
        }
        
    }

    Tile CreateTile()
    {
        Tile tile = ScriptableObject.CreateInstance<Tile>();
        tile.sprite = spList[0];
        tile.colliderType = Tile.ColliderType.None;
        return tile;
    }
}
