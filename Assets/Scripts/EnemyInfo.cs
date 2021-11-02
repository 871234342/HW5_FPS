using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour
{
    public float health = 100f;
    public GameObject loot;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Hurt(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Dead();
        }
    }

    public void Dead()
    {
        Debug.Log("Enemy Died");
        GameObject drop;
        drop = Instantiate(loot, transform.Find("Bottom").position, Quaternion.identity);
        drop.GetComponent<LootInfo>().resource = Random.Range(10, 20);

        Destroy(gameObject);
    }
}
