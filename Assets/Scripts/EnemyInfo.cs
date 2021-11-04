using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInfo : MonoBehaviour
{
    public int health = 100;
    

    [SerializeField] bool isDead = false;
    [SerializeField] LayerMask terrainMask;
    [SerializeField] private Vector3 speed;
    [SerializeField] bool isGrounded;

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
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gamePaused) return;

        if (transform.position.y <= -1f)
        {
            Destroy(this);
        }

        if (isGrounded)
        {
            speed = Vector3.zero;
            agent.enabled = true;
        }
        else
        {
            transform.position += speed * Time.deltaTime;
            speed.y -= 9.8f * Time.deltaTime;
            agent.enabled = false;
            isGrounded = IsGrounded2();
        }
    }

    public void Hurt(int damage)
    {
        if (isDead) return;
        Debug.Log(this.name + " take " + damage + " damege.");
        health -= damage;
        if (health <= 0)
        {
            isDead = true;
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

        speed = dir * push;
        speed.y = 4;
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        transform.position = pos;
        isGrounded = false;
    }

    private bool IsGrounded()
    {
        // has bug
        // don't know why
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        //bound.x -= 0.1f;
        bound.y = 0.1f;
        //bound.z -= 0.1f;

        Debug.DrawLine(boxCenter + new Vector3(col.bounds.extents.x, 0, 0), boxCenter + new Vector3(col.bounds.extents.x, -1f, 0), Color.black, 0.5f);
        Debug.DrawLine(boxCenter - new Vector3(col.bounds.extents.x, 0, 0), boxCenter - new Vector3(col.bounds.extents.x, -1f, 0), Color.black, 0.5f);
        Debug.DrawLine(boxCenter + new Vector3(0, 0, col.bounds.extents.z), boxCenter + new Vector3(0, -1f, col.bounds.extents.z), Color.black, 0.5f);
        Debug.DrawLine(boxCenter - new Vector3(0, 0, col.bounds.extents.z), boxCenter - new Vector3(0, -1f, col.bounds.extents.z), Color.black, 0.5f);

        RaycastHit hit;
        Physics.BoxCast(col.bounds.center, col.bounds.size, Vector3.down, out hit, Quaternion.identity, 1f, terrainMask);
        if (hit.collider != null)  Debug.Log(hit.collider.name);
        else
        {
            Debug.Log("??");
        }

        return Physics.BoxCast(col.bounds.center, col.bounds.size, Vector3.down, Quaternion.identity, 1f, terrainMask);
        //return Physics.CheckBox(boxCenter, bound, Quaternion.identity, teraainMask);
    }

    private bool IsGrounded2()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.y = 0.1f;

        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, terrainMask);
    }

    private bool IsGrounded3()
    {
        Vector3 start = this.transform.position;
        float disToGround = col.bounds.extents.y;

        return Physics.Raycast(transform.position, -Vector3.up, disToGround + 0.1f);
    }
}
