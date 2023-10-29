using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public GameObject player;
    private Vector3 offset = new Vector3(0.0f, 0.0f, -10.0f);
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3 (player.transform.position.x + offset.x, player.transform.position.y + offset.y, offset.z);
    }
}