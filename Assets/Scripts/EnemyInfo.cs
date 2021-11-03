using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInfo : MonoBehaviour
{
    public int health = 100;
    

    [SerializeField]
    bool alreadyDead = false;
    [SerializeField]
    LayerMask teraainMask;
    [SerializeField]
    private Vector3 speed;

    private GameObject loot;
    Rigidbody rb;
    Collider col;
    NavMeshAgent agent;
 
    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();
        agent = this.GetComponent<NavMeshAgent>();
        col = this.GetComponent<Collider>();
        loot = PlayerManager.instance.loot;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += speed * Time.deltaTime;

        if (IsGrounded())
        {
            speed = Vector3.zero;
            agent.enabled = true;
        }
        else
        {
            speed.y -= 9.8f * Time.deltaTime;
            agent.enabled = false;
        }
    }

    public void Hurt(int damage)
    {
        if (alreadyDead) return;
        Debug.Log(this.name + " got hurt");
        health -= damage;
        if (health <= 0)
        {
            Debug.Log(this.name + " killed");
            alreadyDead = true;
            Dead();
        }
    }

    public void Dead(bool ignore = false)
    {
        // trigger special behavior
        if (this.GetComponent<ExploderControl>() != null && !ignore)
        {
            this.GetComponent<ExploderControl>().ExplodeTrigger();
            return;
        }

        Debug.Log("Enemy Died");
        GameObject drop;
        drop = Instantiate(loot, transform.Find("Bottom").position, Quaternion.identity);
        drop.GetComponent<LootInfo>().resource = Random.Range(10, 20);

        Destroy(gameObject);
    }

    public void ExplodeForce(float explosionForce, Vector3 explosionPosition, float radius)
    {
        agent.enabled = false;

        float distance = Vector3.Distance(transform.position, explosionPosition);
        Vector3 dir = Vector3.Normalize(transform.position - explosionPosition);
        float push = (1 - distance / radius) * 6;

        //rb.AddExplosionForce(explosionForce, explosionPosition, explosionRadius, upwardsModifier, mode);
        speed = dir * push;
        speed.y = 4;
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        transform.position = pos;
    }

    private bool IsGrounded()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.x -= 0.1f;
        bound.y = 0.1f;
        bound.z -= 0.1f;

        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, teraainMask);
    }
}
