using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour
{
    public Vector3 direction;
    public float speed;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0.0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Wall")) {
            Destroy(gameObject);
        }
        if(collision.gameObject.CompareTag("Enemy")) {
            collision.gameObject.GetComponent<EnemyScript>().takeDamage(10.0f);
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // direciton is already normalized
        transform.position += direction.normalized * speed * Time.deltaTime;
        timer += Time.deltaTime;
        if(timer > 2.0f) {
            Destroy(gameObject);
        }
    }
}
