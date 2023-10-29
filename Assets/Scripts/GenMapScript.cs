using UnityEngine;
using UnityEngine.Tilemaps;

public class GenMapScript : MonoBehaviour
{
    public Tilemap tilemap;
    public TileBase ruleTile;

    private readonly int[,] _q1 =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1 },
    };
    
    private readonly int[,] _q2 =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1 },
    };
    
    private readonly int[,] _q3 =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 0, 1, 0, 1 },
        { 1, 1, 1, 1, 1 },
    };
    
    private readonly int[,] _q4 =
    {
        { 1, 1, 1, 1, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 1, 1, 0, 1 },
        { 1, 0, 0, 0, 1 },
        { 1, 1, 1, 1, 1 },
    };

    private void RenderMap()
    {
        tilemap.ClearAllTiles();
        
        var width = _q1.GetLength(1);
        var height = _q1.GetLength(0);
        
        Vector2Int offset = new(-Mathf.FloorToInt(width / 2f), -Mathf.FloorToInt(height / 2f));
        
        for (var y = 0; y < _q1.GetLength(0); y++)
        {
            for (var x = 0; x < _q1.GetLength(1); x++)
            {
                if (_q1[y, x] == 1)
                {
                    tilemap.SetTile(new Vector3Int(x + offset.x, y + offset.y, 0), ruleTile);
                }
            }
        }
    }

    private void Start()
    {
        RenderMap();
    }
}
