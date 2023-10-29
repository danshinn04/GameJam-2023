using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public GameObject normalEnemy;
    public Tilemap tilemap;
    public TileBase ruleTile;
    
    public static readonly List<GameObject> EnemyList = new();

    private int[][] _currentMap;
    private int _roundNum = 1;

    private static readonly List<int[,]> CellsList = new() {
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1},
            {1,0,0,1,0,0,1},
            {1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,1,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        },
        new [,] {
            {1,1,1,1,1,1,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,0,0,0,0,0,1},
            {1,1,1,1,1,1,1}
        },
    };

    private static List<int> GetRow(int[,] mat, int rowNum)
    {
        return Enumerable.Range(0, mat.GetLength(1)).Select(idx => mat[rowNum, idx]).ToList();
    }

    private static List<List<int>> GenerateMapRow(int size)
    {
        var mapRow = new List<List<int>>();
        var firstRandCell = CellsList[Random.Range(0, CellsList.Count)];

        for (var row = 0; row < firstRandCell.GetLength(0); row++)
        {
            mapRow.Add(GetRow(firstRandCell, row));
        }

        for (var i = 0; i < size - 1; i++)
        {
            var randCell = CellsList[Random.Range(0, CellsList.Count)];

            // 0 for top, 1 for below
            var rand = Random.Range(0, 2);
            var width = mapRow[0].Count;
                
            if (rand == 0)
            {
                mapRow[1][width - 1] = 0;
                mapRow[2][width - 1] = 0;
            }
            else
            {
                mapRow[4][width - 1] = 0;
                mapRow[5][width - 1] = 0;
            }

            for (var row = 0; row < mapRow.Count; row++)
            {
                mapRow[row].AddRange(GetRow(randCell, row).GetRange(1, randCell.GetLength(1) - 1));
            }
        }

        return mapRow;
    }

    private static int[][] GenerateFullMap(int w, int h)
    {
        var fullMap = new List<List<int>>();
        fullMap.AddRange(GenerateMapRow(w));

        for (var i = 0; i < h - 1; i++)
        {
            var mapRow = GenerateMapRow(w);
            mapRow.RemoveRange(0, 1);

            var width = fullMap[0].Count;
            var height = fullMap.Count;

            for (var col = 0; col < width; col += 6)
            {
                if (col + 1 == width) break;
                
                // 0 for left, 1 for right
                if (Random.Range(0, 2) == 0)
                {
                    fullMap[height - 1][col + 1] = 0;
                    fullMap[height - 1][col + 2] = 0;
                }
                else
                {
                    fullMap[height - 1][col + 4] = 0;
                    fullMap[height - 1][col + 5] = 0;
                }
            }
            
            fullMap.AddRange(mapRow);
        }
        
        return fullMap.Select(row => row.ToArray()).ToArray();
    }

    private void MapToTile()
    {
        var width = _currentMap[0].Length;
        var height = _currentMap.Length;

        var offset = new Vector2Int(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (_currentMap[y][x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), ruleTile);
                }
            }
        }
    }

    private void GenerateRound()
    {
        Debug.Log("Round " + _roundNum);

        _currentMap = GenerateFullMap(5, 3);
        tilemap.ClearAllTiles();
        MapToTile();

        for (var i = 0; i < _roundNum; i++)
        {
            var enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-5.0f, 5.0f),
                Random.Range(-5.0f, 5.0f), 0.0f), Quaternion.identity);
            
            EnemyList.Add(enemy);
        }
        
        _roundNum++;
    }

    private void Start()
    {
        GenerateRound();
    }

    private void Update()
    {
        if (EnemyList.Count > 0)
        {
            return;
        }

        GenerateRound();
    }
}
