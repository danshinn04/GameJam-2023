using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new(0.0f, 0.0f, -10.0f);

    // Update is called once per frame
    void Update()
    {
        if (player != null)
        {
            transform.position = new Vector3 (player.transform.position.x + offset.x, player.transform.position.y + offset.y, offset.z);
        }
    }
}
