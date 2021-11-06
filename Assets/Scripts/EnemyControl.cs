using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyControl : MonoBehaviour
{
    public float detectRadius = 7f;
    public float chaseRadius = 21f;
    public int baseAttackDamage = 10;
    [SerializeField] public int level;
    private int attackDamage;

    private float attackInterval = 1f;
    private float attackCooldown = 0f;

    bool alerted = false;
    NavMeshAgent agent;
    GameObject target;
    Animator animator;
    [SerializeField] public int baseHealth = 100;
    [SerializeField] private int HP;
    private float deathInterval;

    [SerializeField] bool isDead = false;
    [SerializeField] LayerMask terrainMask;
    [SerializeField] private Vector3 speed;
    [SerializeField] bool isGrounded;

    private GameObject loot;
    Rigidbody rb;
    Collider col;


    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player;
        agent = GetComponent<NavMeshAgent>();
        rb = this.GetComponent<Rigidbody>();
        col = this.GetComponent<Collider>();
        loot = PlayerManager.instance.loot;
        isGrounded = true;
        HP = (int)(baseHealth * (1 + level * 0.2f));
        animator = GetComponent<Animator>();
        if (agent.enabled) agent.destination = target.transform.position;
        //level = this.gameObject.GetComponent<EnemyInfo>().level;
        attackDamage = (int)(baseAttackDamage * (1 + level * 0.15f));
        
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            if (clip.name == "zombie_attack")
            {
                attackInterval = clip.length;
            }
            else if(clip.name == "zombie_death_standing")
            {
                deathInterval = clip.length;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gamePaused) return;
        if (isDead) return;

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
            isGrounded = IsGrounded();
            return;
        }

        // Behavior control
        float distance = Vector3.Distance(target.transform.position, transform.position);
        attackCooldown -= Time.deltaTime;
        if (agent.enabled)
        {
            if (distance <= detectRadius)
            {
                alerted = true;
                agent.SetDestination(target.transform.position);
            }
            else if (alerted && distance <= chaseRadius)
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                alerted = false;
                agent.SetDestination(transform.position);
            }

            if (distance <= agent.stoppingDistance)
            {
                FaceTarget();
                if (attackCooldown <= 0)
                {
                    attackCooldown = attackInterval;
                    Attack(attackDamage);
                }
            }
        }
    }

    void FaceTarget()
    {
        Vector3 direction = (target.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectRadius);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, chaseRadius);
    }

    void Attack(int attackDamage)
    {
        target.GetComponent<PlayerInfo>().Hurt(attackDamage, transform.position);
        animator.SetTrigger("Attack");
    }

    public void Hurt(int damage)
    {
        if (isDead) return;
        Debug.Log(this.name + ": " + HP + "\\" + (int)(baseHealth * (1 + level * 0.2f)));
        HP -= damage;
        if (HP <= 0)
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
        drop = Instantiate(loot, transform.position, Quaternion.identity);
        drop.GetComponent<LootInfo>().resource = Random.Range(10, 20) * (level + 1);

        animator.SetTrigger("Dead");
        isDead = true;
        col.enabled = false;
        Destroy(gameObject, deathInterval);
    }

    public void ExplodeForce(float explosionForce, Vector3 explosionPosition, float radius)
    {
        agent.enabled = false;

        float distance = Vector3.Distance(transform.position, explosionPosition);
        Vector3 dir = Vector3.Normalize(transform.position - explosionPosition);
        float push = (1 - distance / radius) * 6;

        speed = dir * push;
        speed.y = explosionForce;
        Vector3 pos = transform.position;
        pos.y += 0.1f;
        transform.position = pos;
        isGrounded = false;
    }

    private bool IsGrounded()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.y = 0.01f;
        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, terrainMask);
    }
}
