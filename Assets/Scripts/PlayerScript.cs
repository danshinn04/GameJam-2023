using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    private float speed = 5.0f;
    Vector3 cVector;
    public GameObject bullet;
    public GameObject pellet;
    public Rigidbody2D rb;
    
    private int gunType = 2;

    // pistolCoolDown
    private float pistolCoolDown = 0.0f;

    // shotgunCoolDown
    private float shotgunCoolDown = 0.0f;

    // automaticCoolDown
    private float automaticCoolDown = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        cVector = new Vector3(0, 0, 0);
    }

    // shotgun, weapon
    // pistol weapon
    // rifle weapon

    void rotatePlayer() {
        Vector3 charVector = Camera.main.WorldToScreenPoint(transform.position);
        cVector = Input.mousePosition - charVector;
        float angle = Mathf.Atan2(cVector.y, cVector.x) * Mathf.Rad2Deg;
 
        //transform.position = charVector;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    void shootPistol() {
        if(pistolCoolDown == 0.0f) {
            // mouseCoolDown
            pistolCoolDown = 0.05f;
            GameObject projectile = Instantiate(bullet, transform.position, transform.rotation);
            projectile.GetComponent<BulletScript>().direction = cVector;
            projectile.GetComponent<BulletScript>().speed = 12.0f;
        }
    }

    void shootAuto() {
        if(automaticCoolDown == 0.0f) {
            // mouseCoolDown
            automaticCoolDown = 0.10f;
            GameObject projectile = Instantiate(bullet, transform.position, transform.rotation);
            projectile.GetComponent<BulletScript>().direction = cVector;
            projectile.GetComponent<BulletScript>().speed = 12.0f;
        }
    }

    void shootShotgun() {
        if(shotgunCoolDown == 0.0f) {
            // mouseCoolDown
            int pellets = 4;
            shotgunCoolDown = 0.65f;
            for(int i = -pellets + 1; i < pellets; i++) {
                float angle = i / (float) pellets * 10.0f;
                float angelRad= Mathf.Deg2Rad * angle;
                
                Vector2 rotatedVector = Quaternion.Euler(0, 0, angle) * cVector;
                rotatedVector.Normalize();

                GameObject pellet = Instantiate(bullet, transform.position, transform.rotation);
                pellet.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);
                pellet.GetComponent<BulletScript>().setDamage(15.0f);
                pellet.GetComponent<BulletScript>().direction = rotatedVector;
                pellet.GetComponent<BulletScript>().speed = 16.0f + Random.Range(1,5);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float h = 0;
        float v = 0;
        
        //Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //mouseWorldPos.z = 0f;

        if(Input.GetKey(KeyCode.W)) {
            v = 1;
        }
        if(Input.GetKey(KeyCode.S)) {
            v = -1;
        }
        if(Input.GetKey(KeyCode.A)) {
            h = -1;
        }
        if(Input.GetKey(KeyCode.D)) {
            h = 1;
        }

        rotatePlayer();
        if(gunType == 0 && Input.GetMouseButtonDown(0)) {
            shootPistol();
        }
        if(gunType == 1 && Input.GetMouseButton(0)) {
            shootAuto();
        }
        if(gunType == 2 && Input.GetMouseButtonDown(0)) {
            shootShotgun();
        }

        Vector2 change = new Vector2(h, v).normalized;
        rb.velocity = change * speed;

        if(pistolCoolDown > 0.0f) {
            pistolCoolDown = Mathf.Max(pistolCoolDown - Time.deltaTime, 0.0f);
        }
        if(automaticCoolDown > 0.0f) {
            automaticCoolDown = Mathf.Max(automaticCoolDown - Time.deltaTime, 0.0f);
        }
        if(shotgunCoolDown > 0.0f) {
            shotgunCoolDown = Mathf.Max(shotgunCoolDown - Time.deltaTime, 0.0f);
        }
    }
}
