using UnityEngine;

public class CustomCursorScript : MonoBehaviour
{
    public Texture2D idleTex;
    public Texture2D shootingTex;
    
    private Vector2 _offset;
    
    private void Start()
    {
        _offset = new Vector2(idleTex.width / 2f, idleTex.height / 2f);
        Cursor.SetCursor(idleTex, _offset, CursorMode.Auto);
    }

    private void Update()
    {
        Cursor.SetCursor(Input.GetMouseButton(0) ? shootingTex : idleTex, _offset, CursorMode.Auto);
    }
}
