using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Vector2 direction;
    private Rigidbody2D rb;
    private GameObject player;
    private Color ogColor;

    private float health = 100.0f;
    private float speed = 3.0f;
    private float vision = 10.0f;
    private float distance  = 0.0f;

    private bool chase = false;

    // hitDamage
    private float hitDamage = 0.0f;
    private float deathSequence = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        ogColor = GetComponent<SpriteRenderer>().color;
        distance = (transform.position - player.transform.position).magnitude;
    }

    public void takeDamage(float damage) {
        health -= damage;
        hitDamage = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color = ogColor;
        if(hitDamage != 0.0f) {
            hitDamage = Mathf.Max(0.0f, hitDamage - Time.deltaTime);
            GetComponent<SpriteRenderer>().color = Color.black;
        }
        if(health < 0.0f) {
            if(deathSequence == 0.0f) {
                Destroy(GetComponent<Rigidbody2D>());
                Destroy(GetComponent<CircleCollider2D>());
                deathSequence = 0.4f;
            }
            if(deathSequence > 0.0f) {
                deathSequence -= Time.deltaTime;
                GetComponent<SpriteRenderer>().color = new Color(deathSequence, deathSequence, deathSequence, deathSequence);
                float amt = 1.0f - deathSequence;
                transform.localScale += new Vector3(amt * 0.05f, amt * 0.05f, 1.0f);
            }

            if(deathSequence <= 0.0f) {
                Destroy(gameObject);
            }

            return;
        }

        Vector3 target = player.transform.position - transform.position;
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90.0f;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        if(distance < vision) {
            chase = true;
        }
        if(chase) {
            rb.velocity = target.normalized * speed;
        }
    }
}
