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
    [SerializeField] private bool offGround = false;

    private GameObject loot;
    Rigidbody rb;
    Collider col;

    public AudioClip attack;
    AudioSource audio;


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
        detectRadius *= (level + 1) * 0.2f;
        chaseRadius *= (level + 1) * 0.3f;
        attackDamage = (int)(baseAttackDamage * (1 + level * 0.15f));

        audio = GetComponent<AudioSource>();

        if (animator == null) return;
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

        // Fall out of field check
        StartCoroutine("CheckOutOfField");
    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gamePaused) return;
        if (isDead) return;

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
                if(animator != null) animator.SetFloat("MoveSpeed", 1f);
            }
            else if (alerted && distance <= chaseRadius)
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                alerted = false;
                agent.SetDestination(transform.position);
                if (animator != null) animator.SetFloat("MoveSpeed", 0f);
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

        audio.Play();

        if (animator != null) animator.SetTrigger("Attack");


    }

    public void Hurt(int damage)
    {
        if (isDead) return;
        alerted = true;
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
        drop.GetComponent<LootInfo>().resource = Random.Range(5, 15) * (level + 1);

        if (animator != null) animator.SetTrigger("Dead");
        isDead = true;
        col.enabled = false;
        Destroy(gameObject, deathInterval);
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

    private bool IsGrounded()
    {
        Vector3 boxCenter = this.transform.position;
        boxCenter.y -= col.bounds.size.y * 0.5f;

        Vector3 bound = col.bounds.size / 2;
        bound.y = 0.01f;
        //Debug.Log("Center: " + boxCenter + ", bound: " + bound + ", Result: " + Physics.CheckBox(boxCenter, bound, Quaternion.identity, terrainMask));
        return Physics.CheckBox(boxCenter, bound, Quaternion.identity, terrainMask);
    }

    IEnumerator ForceOffGround()
    {
        offGround = true;
        for(float i = 0.1f; i >+ 0; i -= Time.deltaTime)
        {
            yield return null;
        }
        offGround = false;
    }

    IEnumerator CheckOutOfField()
    {
        for(; ; )
        {
            if(transform.position.y < -500)
            {
                Destroy(this);
            }
            yield return new WaitForSeconds(5f);
        }

    }
}
