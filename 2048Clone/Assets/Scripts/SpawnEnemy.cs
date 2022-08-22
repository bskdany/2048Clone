using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemySprite;

    private void Start()
    {
        gameObject.transform.position = new Vector3(Random.Range(-10.0f, 10.0f), Random.Range(-5.0f, 5.0f), 0);
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject a = Instantiate(enemySprite);
        Destroy(gameObject);
    }
}
