//using System.Collections;

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private int rounds;
    public GameObject normalEnemy;
    public static List<GameObject> enemies = new();

    public Tilemap tilemap;
    public TileBase ruleTile;

    private Vector2Int start = new(1, 1);
    private Vector2Int end = new(3, 3);
    private List<Vector2Int> solutionPath;
    private List<int> exits = new();

    private int[][] currentMap;

    private readonly int[][] _a1 = {
        new[] {1,1,1,1,1},
        new[] {1,0,1,0,1},
        new[] {1,0,1,0,1},
        new[] {1,0,0,0,1},
        new[] {1,1,1,1,1}
    };

    private readonly int[][] _a2 = {
        new[] {1,1,1,1,1},
        new[] {1,0,0,0,1},
        new[] {1,0,1,1,1},
        new[] {1,0,0,0,1},
        new[] {1,1,1,1,1}
    };

    private readonly int[][] _a3 = {
        new[] {1,1,1,1,1},
        new[] {1,0,0,0,1},
        new[] {1,0,1,0,1},
        new[] {1,0,1,0,1},
        new[] {1,1,1,1,1}
    };

    private readonly int[][] _a4 = {
        new[] {1,1,1,1,1},
        new[] {1,0,0,0,1},
        new[] {1,1,1,0,1},
        new[] {1,0,0,0,1},
        new[] {1,1,1,1,1}
    };
    
    private List<List<int>> AppendXGeneration(List<List<int>> arrlist1, int[][] arrlist2)
    {
        var newList = new List<List<int>>();
        var thiccy = Random.Range(0, 2) == 1 ? 3 : 1;

        for (int idx = 0; idx < arrlist1.Count; idx++)
        {
            var newRow = arrlist1[idx];
            
            for(int i = 1; i < arrlist2[0].Length; i++)  {
                newRow.Add(arrlist2[idx][i]);
            }
            
            if (idx == thiccy)
            {
                newRow[arrlist1[0].Count - 1] = 0;
            }
            
            newList.Add(newRow);
        }
        return newList;
    }

    private List<List<int>> AppendYGeneration(List<List<int>> arrList1, List<List<int>> arrList2) {
        var lenofarr1 = arrList1[0].Count;
        var p = lenofarr1 / 2;
        var ran = Random.Range(1, p) * 2 - 1;

        List<int> a = arrList1[arrList1.Count - 1];

        List<List<int>> newMap = arrList1.GetRange(0, arrList1.Count - 1);
        newMap.Add(a);
        newMap.AddRange(arrList2.GetRange(1, arrList2.Count - 1));

        exits.Add(ran);

        return newMap;
    }

    private int[][] RandomMapGeneration(int size)
    {
        var exits = new List<int>();
        
        var quadList = new List<int[][]> { _a1, _a2, _a3, _a4 };
        var randQuad = quadList[Random.Range(0, quadList.Count)];
        var firstRow = randQuad.Select(row => new List<int>(row)).ToList();

        for (var i = 0; i < size; i++) {
            firstRow = AppendXGeneration(firstRow, quadList[Random.Range(0, quadList.Count)]);
        }

        var finalList = firstRow;
        
        for (var i = 0; i < size - 1; i++)
        {
            var anotherRandQuad = quadList[Random.Range(0, quadList.Count)];
            var nextRow = anotherRandQuad.Select(row => new List<int>(row)).ToList();

            for (var j = 0; j < size - 1; j++) {
                nextRow = AppendXGeneration(nextRow, quadList[Random.Range(0, quadList.Count)]);
            }
            
            finalList = AppendYGeneration(finalList, nextRow);
        }
        
        var finalMap = new int[finalList.Count][];

        var idx = 0;
        foreach (var row in finalList)
        {
            finalMap[idx] = row.ToArray();
            idx++;
        }
        
        return finalMap;
    }

    private void MapToTile()
    {
        var width = currentMap[0].Length;
        var height = currentMap.Length;

        var offset = new Vector2Int(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                if (currentMap[y][x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), ruleTile);
                }
            }
        }
    }

    private void GenerateRound()
    {
        rounds++;

        Debug.Log("Round " + rounds);

        for (int i = 0; i < rounds; i++)
        {
            GameObject enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-5.0f, 5.0f),
                Random.Range(-5.0f, 5.0f), 0.0f), Quaternion.identity);
            enemies.Add(enemy);
        }
    }

    private void Start()
    {
        GenerateRound();
        
        currentMap = RandomMapGeneration(5);
        tilemap.ClearAllTiles();
        MapToTile();
    }

    private void Update()
    {
        if (enemies.Count > 0)
        {
            return;
        }

        GenerateRound();
    }
}
