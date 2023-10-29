using UnityEngine;
using UnityEngine.Tilemaps;

public class GenMapScript : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase ruleTile;

    private readonly int[,] _map =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 1, 1, 1, 1 },
        { 1, 1, 1, 1, 1 }
    };

    private void GenMap()
    {
        tilemap.ClearAllTiles();
        
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

    private void Start()
    {
        GenMap();
    }
}
