using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private int rounds = 0;
    public GameObject normalEnemy;
    public static List<GameObject> enemies = new List<GameObject>();
    
    int getRounds() {
        return rounds;
    }

    void generateRound() {
        rounds++;
        Debug.Log("Round " + rounds);
        for(int i = 0; i < rounds; i++) {
            GameObject enemy = Instantiate(normalEnemy, new Vector3(Random.Range(-5.0f, 5.0f), 
                                            Random.Range(-5.0f, 5.0f), 0.0f), Quaternion.identity) as GameObject;
            enemies.Add(enemy);
        }
    }

    void Start()
    {
        generateRound();
    }

    // Update is called once per frame
    void Update()
    {
        if(enemies.Count > 0) {
            return;
        }
        generateRound();
    }
}
