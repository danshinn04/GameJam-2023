//using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    private int rounds;
    public GameObject normalEnemy;
    public GameObject player;
    public static List<GameObject> enemies = new List<GameObject>();

    public Tilemap tilemap;
    public TileBase ruleTile;

    private Vector2Int start = new Vector2Int(1, 1);
    private Vector2Int end = new Vector2Int(3, 3);
    private List<Vector2Int> solutionPath;
    private List<int> exits = new List<int>();

    public static float px = 0.0f;
    public static float py = 0.0f;

    public static int[][] currentMap;

    int getRounds()
    {
        return rounds;
    }

    private int[][] testMap = {
        new int[] {1,1,1,1,1,1,1,1,1,1,1,1,1},
        new int[] {1,0,0,0,0,0,1,0,0,0,0,0,1},
        new int[] {1,0,0,0,0,0,1,0,0,0,0,0,1},
        new int[] {1,0,0,0,0,0,0,0,0,0,0,0,1},
        new int[] {1,0,0,0,0,0,0,0,0,0,0,0,1},
        new int[] {1,1,1,1,1,1,1,1,1,1,0,0,1},
        new int[] {1,0,0,0,0,0,0,0,0,0,0,0,1},
        new int[] {1,0,0,0,0,0,0,0,0,0,0,0,1},
        new int[] {1,0,0,0,0,0,0,0,0,0,0,0,1},
        new int[] {1,1,1,1,1,1,1,1,1,1,1,1,1},
    };

    private int[][] a1 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };

    private int[][] a2 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,0,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };

    private int[][] a3 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,0,1,0,1},
        new int[] {1,1,1,1,1}
    };

    private int[][] a4 = {
        new int[] {1,1,1,1,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,0,1},
        new int[] {1,0,0,0,1},
        new int[] {1,1,1,1,1}
    };

    int[][] RandomMapGeneration(int size)
    {
        List<int[][]> maps = new List<int[][]> { a1, a2, a3, a4 };
        List<int> exits = new List<int>();

        int[][] matL1 = maps[Random.Range(0, maps.Count)];
        List<List<int>> L1 = new List<List<int>>();
        for(int i=0; i<matL1.Length; i++) {
            L1.Add(new List<int>(matL1[i]));
        }

        for (int i = 0; i < size; i++) {
            L1 = AppendXGeneration(L1, maps[Random.Range(0, maps.Count)]);
        }

        List<List<int>> finalList = L1;
        
        for (int i = 0; i < size - 1; i++)
        {
            int[][] matL2 = maps[Random.Range(0, maps.Count)];
            List<List<int>> L2 = new List<List<int>>();
            for (int j = 0; j < matL2.Length; j++) {
                L2.Add(new List<int>(matL2[j]));
            }
            for (int j = 0; j < size - 1; j++) {
                L2 = AppendXGeneration(L2, maps[Random.Range(0, maps.Count)]);
            }
            finalList = AppendYGeneration(finalList, L2);
        }

        int n = finalList.Count;
        int m = finalList[0].Count;

        int[][] finalMap = new int[n][];
        for(int i=0; i<n; i++) {
            finalMap[i] = new int[m];
            for(int j=0; j<m; j++) {
                //Debug.Log("i" + "," + "j");
                finalMap[i][j] = finalList[i][j];
            }
            Debug.Log(L1[i].Count);
        }
        
        return finalMap;
    }

    private void MapToTile()
    {
        var width = currentMap[0].Length;
        var height = currentMap.Length;

        Vector2Int offset = new Vector2Int(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));

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

    void generateRound()
    {
        rounds++;

        Debug.Log("Round " + rounds);

        // Generate a random map of size 5x5 (or any other size)
        //Tuple<List<int[]>, List<int>> result = RandomMapGeneration(5);
        //currentMap = result.Item1.ToArray();
        for (int i = 0; i < rounds; i++)
        {
            GameObject enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-5.0f, 5.0f),
                                            Random.Range(-5.0f, 5.0f), 0.0f), Quaternion.identity) as GameObject;
            enemies.Add(enemy);
        }
    }

    void Start()
    {
        generateRound();
        //currentMap = RandomMapGeneration(3);
        currentMap = testMap;
        tilemap.ClearAllTiles();
        MapToTile();
    }

    void Update()
    {
        if (enemies.Count > 0)
        {
            return;
        }

        generateRound();
    }

    public List<List<int>> AppendXGeneration(List<List<int>> arrlist1, int[][] arrlist2)
    {
        List<List<int>> newList = new List<List<int>>();
        int Thiccy = Random.Range(0, 2) == 1 ? 3 : 1;

        for (int idx = 0; idx < arrlist1.Count; idx++)
        {
            List<int> newRow = arrlist1[idx];
            for(int i = 1; i < arrlist2[0].Length; i++)  {
                newRow.Add(arrlist2[idx][i]);
            }
            //if (idx == Thiccy)
            //{
                //newRow[arrlist1[0].Count] = 0;
            //}
            newList.Add(newRow);
        }
        return newList;
    }

    public List<List<int>> AppendYGeneration(List<List<int>> arrList1, List<List<int>> arrList2) {
        int lenofarr1 = arrList1[0].Count;
        int p = lenofarr1 / 2;
        int ran = Random.Range(1, p) * 2 - 1;

        List<int> a = arrList1[arrList1.Count - 1];
        //a[ran] = 0;

        List<List<int>> newMap = arrList1.GetRange(0, arrList1.Count - 1);
        newMap.Add(a);
        newMap.AddRange(arrList2.GetRange(1, arrList2.Count - 1));

        exits.Add(ran);

        Debug.Log(newMap.Count);

        return newMap;
    }
}
