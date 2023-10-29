using UnityEngine;
using UnityEngine.UI;

public class ScrollBgScript : MonoBehaviour
{
    public RawImage img;
    private const float Speed = 0.025f;

    private void Update()
    {
        img.uvRect = new Rect(img.uvRect.position + new Vector2(Speed, Speed) * Time.deltaTime, img.uvRect.size);
    }
}
