using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RailgunScript : EnemyScript
{
    private Vector3 origin;
    private Vector3 dir;
    private Vector3 goal;
    private Rigidbody2D rb;
    private GameObject player;
    public GameObject block;
    private GameObject laser;
    public GameObject bullet;

    private bool chase = false;
    private bool fired = false;

    // hitDamage
    private float attention = 0.0f;
    private float fireTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        SetOGColor();
        origin = transform.position;
    }

    private void Sentry() {
        Vector3 target = player.transform.position - transform.position;
        if(target.magnitude < 5.0f) {
            if(!chase) {
                //Debug.Log("track");
                chase = true;
                attention = 1.0f;
                laser = Instantiate(block, transform.position, Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    private void Update()
    {
        HurtSeqeunce();
        CheckDeath();
        
        if(isDead()){
            Destroy(laser);
            return;
        }

        Sentry();

        if(attention > 0.0f && fireTime == 0.0f) {
            float trackingTime = 0.15f;

            if(attention > trackingTime) {
                dir = player.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                laser.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                laser.transform.localScale = new Vector3(dir.magnitude, 0.2f, 0.0f);
                laser.transform.position = origin + dir.normalized * dir.magnitude / 2.0f;
                laser.GetComponent<SpriteRenderer>().color = new Color(255.0f, 0.0f, 0.0f, Mathf.Cos(attention) * 0.05f + 0.5f);
            } 
            if(attention < trackingTime && attention > 0.0f)  {
                laser.GetComponent<SpriteRenderer>().color = new Color(255.0f, 0.0f, 0.0f, attention * attention);
            }

            attention = Mathf.Max(0.0f, attention - Time.deltaTime);
            if(attention == 0.0f) {
                laser.GetComponent<SpriteRenderer>().color = new Color(255.0f, 255.0f, 255.0f, 0.5f);
                fireTime = 1.0f;
            }
        }

        // acts as a cooldown
        if(fireTime > 0.0f)  {
            // fire boolets
            if(!fired && (int) (fireTime * 15.0f) % 2 == 0) {
                fired = true;

                audioSource.PlayOneShot(beep, 0.4f);
                GameObject proj = Instantiate(bullet, transform.position, Quaternion.identity);
                proj.GetComponent<BulletScript>().setIsEnemy(true);
                proj.GetComponent<BulletScript>().setDamage(10.0f);

                proj.GetComponent<BulletScript>().direction = dir.normalized;
                proj.GetComponent<BulletScript>().speed = 20.0f;
                proj.GetComponent<BulletScript>().transform.rotation = Quaternion.AngleAxis(Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90.0f, Vector3.forward);    
            }

            // i need more boolets
            if(fired && (int) (fireTime * 15.0f) % 2 != 0) {
                fired = false;
            }
            if(laser.transform.localScale.y > 0) {
                float dy = Mathf.Max(-1.0f, laser.transform.localScale.y - 0.025f * (fireTime * fireTime));
                laser.transform.localScale = new Vector3(laser.transform.localScale.x, dy, 0.0f);
            }
            laser.GetComponent<SpriteRenderer>().color = new Color(255.0f, 255.0f, 255.0f, fireTime * fireTime);

            fireTime = Mathf.Max(0.0f, fireTime - Time.deltaTime);
            if(fireTime == 0.0f) {
                Destroy(laser);
                chase = false;
            }
        }
    }
}
