using UnityEngine;

public enum Gun
{
    Pistol = 0,
    Automatic = 1,
    Shotgun = 2
}

public class PlayerScript : MonoBehaviour
{
    public GameObject bullet;
    public GameObject pellet;
    public Rigidbody2D rb;
    
    public Animator anim;
    private static readonly int IsShooting = Animator.StringToHash("isShooting");
    
    private const float Speed = 5.0f;
    private Gun _gun = Gun.Pistol;
    private Vector3 _cVector = new(0, 0, 0);

    private float _pistolCooldown;
    private float _automaticCooldown;
    private float _shotgunCooldown;

    private void RotatePlayer() {
        var charVector = Camera.main.WorldToScreenPoint(transform.position);
        _cVector = Input.mousePosition - charVector;
        
        var angle = Mathf.Atan2(_cVector.y, _cVector.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }

    private void ShootPistol()
    {
        if (_pistolCooldown != 0.0f) return;
        
        _pistolCooldown = 0.05f;
        
        var projectile = Instantiate(bullet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
        projectile.GetComponent<BulletScript>().direction = _cVector;
        projectile.GetComponent<BulletScript>().speed = 12.0f;
    }

    private void ShootAuto()
    {
        if (_automaticCooldown != 0.0f) return;
        
        _automaticCooldown = 0.10f;
        
        var projectile = Instantiate(bullet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
        projectile.GetComponent<BulletScript>().direction = _cVector;
        projectile.GetComponent<BulletScript>().speed = 12.0f;
    }

    private void ShootShotgun()
    {
        if (_shotgunCooldown != 0.0f) return;
        
        _shotgunCooldown = 0.65f;
        var pellets = 4;
        
        for (var i = -pellets + 1; i < pellets; i++) {
            var angle = i / (float) pellets * 10.0f;
                
            Vector2 rotatedVector = Quaternion.Euler(0, 0, angle) * _cVector;
            rotatedVector.Normalize();

            var pellet = Instantiate(bullet, transform.position + (0.5f * _cVector.normalized), transform.rotation);
            pellet.transform.localScale = new Vector3(0.175f, 0.175f, 0.0f);
            pellet.GetComponent<BulletScript>().setDamage(15.0f);
            pellet.GetComponent<BulletScript>().direction = rotatedVector;
            pellet.GetComponent<BulletScript>().speed = 16.0f + Random.Range(1,5);
        }
    }

    private void Update()
    {
        float h = 0;
        float v = 0;

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

        if (Input.GetKey(KeyCode.Alpha1))
        {
            _gun = Gun.Pistol;
        }
        if (Input.GetKey(KeyCode.Alpha2))
        {
            _gun = Gun.Automatic;
        }
        if (Input.GetKey(KeyCode.Alpha3))
        {
            _gun = Gun.Shotgun;
        }

        RotatePlayer();
        
        switch (_gun)
        {
            case Gun.Pistol when Input.GetMouseButtonDown(0):
                ShootPistol();
                anim.SetBool(IsShooting, true);
                break;
            case Gun.Automatic when Input.GetMouseButton(0):
                ShootAuto();
                anim.SetBool(IsShooting, true);
                break;
            case Gun.Shotgun when Input.GetMouseButtonDown(0):
                ShootShotgun();
                anim.SetBool(IsShooting, true);
                break;
            default:
                anim.SetBool(IsShooting, false);
                break;
        }

        var change = new Vector2(h, v).normalized;
        if(change.magnitude > 0.0f) {
            float n = GameManager.CurrentMap[0].Length;
            float m = GameManager.CurrentMap.Length;

            // Unity raycasting sucks ass
            GameManager.Px = (transform.position.x + transform.localScale.x / 2.0f + (n / 2.0f));
            GameManager.Py = (transform.position.y + transform.localScale.y / 2.0f + (m / 2.0f));
        }

        rb.velocity = change * Speed;

        if(_pistolCooldown > 0.0f) {
            _pistolCooldown = Mathf.Max(_pistolCooldown - Time.deltaTime, 0.0f);
        }
        if(_automaticCooldown > 0.0f) {
            _automaticCooldown = Mathf.Max(_automaticCooldown - Time.deltaTime, 0.0f);
        }
        if(_shotgunCooldown > 0.0f) {
            _shotgunCooldown = Mathf.Max(_shotgunCooldown - Time.deltaTime, 0.0f);
        }
    }
}
