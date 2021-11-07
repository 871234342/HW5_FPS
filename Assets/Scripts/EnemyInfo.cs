using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyInfo : MonoBehaviour
{
    [SerializeField] public int baseHealth = 100;
    public int level = 0;
    private int HP;

    public bool isDead = false;
    [SerializeField] LayerMask terrainMask;
    [SerializeField] private Vector3 speed;
    [SerializeField] bool isGrounded;
    [SerializeField] private bool offGround = false;

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
        //Debug.Log(col);
        loot = PlayerManager.instance.loot;
        isGrounded = true;
        HP = (int)(baseHealth * (1 + level * 0.2f));

        // Fall out of field check
        StartCoroutine("CheckOutOfField");
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gamePaused) return;

        if (isGrounded && !offGround)
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
        GetComponent<ExploderControl>().alerted = true;
        HP -= damage;
        if (HP <= 0)
        {
            isDead = true;
            Dead();
        }
    }

    public void Dead(bool ignore = false, float delay = 0f)
    {
        // trigger special behavior
        if (this.GetComponent<ExploderControl>() != null && !ignore)
        {
            this.GetComponent<ExploderControl>().ExplodeTrigger();
            return;
        }

        GameObject drop;
        drop = Instantiate(loot, transform.position, Quaternion.identity);
        drop.GetComponent<LootInfo>().resource = Random.Range(5, 10) * (level + 1);

        if (delay == 0) Destroy(gameObject);
        else
        {
            Destroy(gameObject, delay);
            transform.Find("mdl").gameObject.SetActive(false);
        }
    }

    public void ExplodeForce(float explosionForce, Vector3 explosionPosition, float radius)
    {
        agent.enabled = false;

        float distance = Vector3.Distance(transform.position, explosionPosition);
        Vector3 dir = Vector3.Normalize(transform.position - explosionPosition);
        float push = (1 - distance / radius) * explosionForce;

        speed = dir * push;
        speed.y = explosionForce * ((1 - distance / radius));
        Vector3 pos = transform.position;
        pos.y += 0.2f;
        transform.position = pos;
        isGrounded = false;
        StartCoroutine("ForceOffGround");
    }

    private bool IsGrounded2()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.y = 0.01f;

        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, terrainMask);
    }

    IEnumerator ForceOffGround()
    {
        offGround = true;     
        for (float i = 0.1f; i >= 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        offGround = false;
    }

    IEnumerator CheckOutOfField()
    {
        for (; ; )
        {
            if (transform.position.y < -500)
            {
                Destroy(this);
            }
            yield return new WaitForSeconds(10f);
        }

    }
}
