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
    private float speed = 4.0f;

    private bool chase = false;

    // hitDamage
    private float hitDamage = 0.0f;
    private float deathSequence = 0.0f;
    private float attention = 0.0f;

    // layerMask
    public LayerMask Ignore;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        ogColor = GetComponent<SpriteRenderer>().color;
    }

    public void takeDamage(float damage) {
        health -= damage;
        hitDamage = 0.1f;
    }

    public void sentry() {
        chase = false;
        rb.velocity = new Vector2(0.0f, 0.0f);

        float n = GameManager.currentMap[0].Length;
        float m = GameManager.currentMap.Length;

        if(GameManager.px >= 0 && GameManager.px < n && GameManager.py >= 0 && GameManager.py < n) {
            int x = (int)GameManager.px;
            int y = (int)GameManager.py;

            Debug.Log("Player x, Player y: " + x + "," + y);

            Vector3 target = player.transform.position - transform.position;

            for(float i = 0; i < target.magnitude; i += 0.1f) {
                Vector3 epos = transform.position + target.normalized * i;
                int ex = (int)(epos.x + (n / 2.0f));
                int ey = (int)(epos.y + (m / 2.0f));

                if(ex < 0 || ex >= n || ey < 0 || ey >= m) {
                    return;
                }

                if(GameManager.currentMap[ey][ex] == 1) {
                    Debug.Log("Hitting a wall");
                    return;
                }
                if(ey == y && ex == x) {
                    Debug.Log("Player in sight");
                    chase = true;
                    attention = 1.0f;
                    return;
                }
            }

        }
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
                GameManager.enemies.Remove(gameObject);
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

        sentry();

        if(chase || attention > 0.0f) {
            Vector3 target = player.transform.position - transform.position;
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            rb.velocity = target.normalized * speed;
            attention = Mathf.Max(0.0f, attention - Time.deltaTime);
        }
    }
}
