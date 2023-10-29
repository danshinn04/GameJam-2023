using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private int rounds;
    public GameObject normalEnemy;
    public static List<GameObject> enemies = new();
    
    public Tilemap tilemap;
    public TileBase ruleTile;
    
    int getRounds() {
        return rounds;
    }
    
    private readonly int[,] _map =
    {
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1},
        { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1}
    };
    
    private void MapToTile()
    {
        var width = _map.GetLength(1);
        var height = _map.GetLength(0);
        
        Vector2Int offset = new(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));
        
        for (var y = 0; y < _map.GetLength(0); y++)
        {
            for (var x = 0; x < _map.GetLength(1); x++)
            {
                if (_map[y, x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), ruleTile);
                }
            }
        }
    }

    void generateRound() {
        rounds++;
        
        Debug.Log("Round " + rounds);

        tilemap.ClearAllTiles();
        MapToTile();

        float width = (float)_map.GetLength(1) / 2.0f - 1.0f;
        float height = (float)_map.GetLength(0) / 2.0f - 1.0f;
        
        for(int i = 0; i < rounds; i++) {
            GameObject enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-width, width), 
                                            Random.Range(-height, height), 0.0f), Quaternion.identity) as GameObject;
            enemies.Add(enemy);
        }
    }

    void Start()
    {
        generateRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count > 0) {
            return;
        }
        
        generateRound();
    }
}
