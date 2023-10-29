using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    private Vector3 dir;
    private Vector3 goal;
    private Rigidbody2D rb;
    private GameObject player;
    public GameObject block;
    private Color ogColor;

    private float health = 100.0f;
    private float speed = 6.0f;
    private bool chase = false;
    private bool idle = false;

    public Animator anim;
    private static readonly int Angry = Animator.StringToHash("Angry");

    // hitDamage
    private float hitDamage = 0.0f;
    private float deathSequence = 0.0f;
    private float attention = 0.0f;
    private float wander = 0.0f;
    private float explodeTimer = 0.0f;

    private List<(int, int)> path = null;
    private int nx = 0;
    private int ny = 0;

    // layerMask
    public LayerMask Ignore;

    public AudioSource audioSource;
    public AudioClip beep;
    public AudioClip hit;
    public AudioClip boom;
    

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        player = GameObject.Find("Player");
        ogColor = GetComponent<SpriteRenderer>().color;
        idle = true;
    }

    public void takeDamage(float damage) {
        health -= damage;
        hitDamage = 0.1f;
        audioSource.PlayOneShot(hit, 0.4f);
    }

    bool IsValidMove(int[][] maze, int y, int x, HashSet<(int, int)> visited)
    {
        return (y >= 0 && y < maze.Length) && (x >= 0 && x < maze[0].Length) && (maze[y][x] == 0) && !visited.Contains((y, x));
    }
    
    List<(int, int)> MazeSolver(int[][] maze, (int, int) start, (int, int) end) {
        // Directions for moving up, down, left, and right
        
        float n = GameManager.CurrentMap[0].Length;
        float m = GameManager.CurrentMap.Length;

        int[][] directions = { new int[] { -1, 0 }, new int[] { 1, 0 }, new int[] { 0, -1 }, new int[] { 0, 1 } };

        Queue<(int, int)> queue = new Queue<(int, int)>();
        HashSet<(int, int)> visited = new HashSet<(int, int)>();
        Dictionary<(int, int), (int, int)> previous = new Dictionary<(int, int), (int, int)>();

        queue.Enqueue(start);
        visited.Add(start);
        previous[start] = (0, 0);

        while (queue.Count > 0)
        {
            (int currY, int currX) = queue.Dequeue();

            // If we've reached the destination
            if ((currY, currX) == end)
            {
                Debug.Log("solved");
                List<(int, int)> path = new List<(int, int)>();
                while ((currY, currX) != start)
                {
                    //Instantiate(block, new Vector3(currX - (n / 2.0f), currY - (m / 2.0f), 0.0f), Quaternion.identity);
                    path.Add((currY, currX));
                    (currY, currX) = previous[(currY, currX)];
                }
                path.Add(start);
                path.Reverse();
                return path;
            }

            foreach (int[] direction in directions)
            {
                int newY = currY + direction[0];
                int newX = currX + direction[1];
                if (IsValidMove(maze, newY, newX, visited))
                {
                    Debug.Log(maze[newY][newX]);
                    queue.Enqueue((newY, newX));
                    visited.Add((newY, newX));
                    previous[(newY, newX)] = (currY, currX);
                }
            }
        }

        return null;
    }

    public void sentry() {
        chase = false;

        float n = GameManager.CurrentMap[0].Length;
        float m = GameManager.CurrentMap.Length;

        if(GameManager.Px >= 0 && GameManager.Px < n && GameManager.Py >= 0 && GameManager.Py < m) {
            int x = (int)GameManager.Px;
            int y = (int)GameManager.Py;

            Vector3 target = player.transform.position - transform.position;

            if(target.magnitude < 7.0f || path != null) {
                anim.SetBool(Angry, true);
                
                int ex = (int)(transform.position.x + (n / 2.0f));
                int ey = (int)(transform.position.y + (m / 2.0f));
                
                if(ex < 0 || ex >= n || ey < 0 || ey >= m) {
                    return;
                }

                if(path != null && path.Count > 0) {
                    if((goal - transform.position).magnitude < 1.2f) {
                        audioSource.PlayOneShot(beep, 0.4f);

                        path.RemoveAt(0);
                        goal = player.transform.position;
                        if(path.Count != 0) {
                            (ny, nx) = path[0];
                            goal = new Vector3(nx - (n / 2.0f), ny - (m / 2.0f), 0.0f);
                        }
                        dir = goal - transform.position;
                    }
                } else {
                    path = MazeSolver(GameManager.CurrentMap, (ey, ex), (y, x));
                    path.RemoveAt(0);

                    goal = player.transform.position;
                    if(path.Count != 0) {
                        (ny, nx) = path[0];
                        goal = new Vector3(nx - (n / 2.0f), ny - (m / 2.0f), 0.0f);
                    }
                    dir = goal - transform.position;
                }
            
                if(target.magnitude < 2.0f) {
                    goal = player.transform.position;
                    dir = goal - transform.position;

                    if(explodeTimer < 0.0f) {
                        health = 0.0f;
                        player.GetComponent<PlayerScript>().takeDamage(20.0f);
                        return;
                    }
                    if(explodeTimer == 0.0f) {
                        explodeTimer = 1.0f;
                    }
                    explodeTimer -= Time.deltaTime;
                    transform.position += dir.normalized * (speed * Time.deltaTime);
                    return;
                }

                explodeTimer = 0.0f;
            }
            
            transform.position += dir.normalized * (speed * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetComponent<SpriteRenderer>().color = ogColor;
        if(hitDamage != 0.0f) {
            hitDamage = Mathf.Max(0.0f, hitDamage - Time.deltaTime);

            Color dmg = Color.Lerp(ogColor, Color.red, hitDamage / 0.1f);
            GetComponent<SpriteRenderer>().color = dmg;
        }
        if(health <= 0.0f) {
            if(deathSequence == 0.0f) {
                Destroy(GetComponent<Rigidbody2D>());
                Destroy(GetComponent<CircleCollider2D>());
                GameManager.EnemyList.Remove(gameObject);
                deathSequence = 0.4f;
                audioSource.PlayOneShot(boom, 0.4f);
            }
            if(deathSequence > 0.0f) {
                GetComponent<SpriteRenderer>().color = new Color(deathSequence, deathSequence, deathSequence, deathSequence);
                float amt = 1.0f - deathSequence;
                transform.localScale += new Vector3(amt * 0.05f, amt * 0.05f, 1.0f);
            }
            if(deathSequence <= 0.0f) {
                transform.localScale = new Vector3(0, 0, 0);
            }
            deathSequence -= Time.deltaTime;
            
            if(deathSequence <= -1.5f) {
                Destroy(gameObject);
            }

            return;
        }

        sentry();

        /*
        if(chase || attention > 0.0f) {
            Vector3 target = player.transform.position - transform.position;
            float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg - 90.0f;
            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
            rb.velocity = target.normalized * speed;
            attention = Mathf.Max(0.0f, attention - Time.deltaTime);

            if(target.magnitude < 2.0f) {
                if(explodeTimer < 0.0f) {
                    health = 0.0f;
                    player.GetComponent<PlayerScript>().takeDamage(20.0f);
                    return;
                }
                if(explodeTimer == 0.0f) {
                    explodeTimer = 1.0f;
                }
                explodeTimer -= Time.deltaTime;
                return;
            }

            explodeTimer = 0.0f;
        }*/
        /*
        if(!idle && !chase && attention == 0.0f) {
            idle = true;
        }
        if(idle) {
            if(Random.Range(0.0f, 10.0f) < 0.01f && wander == 0.0f) {
                direction = new Vector2(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90.0f;
                transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
                wander = 2.5f;
            }
            if(wander > 0.0f) {
                wander = Mathf.Max(0.0f, wander - Time.deltaTime);
                
                Debug.Log(direction);
                Debug.Log(rb.velocity);

                rb.velocity = direction * speed;
            }
        }*/
    }
}
