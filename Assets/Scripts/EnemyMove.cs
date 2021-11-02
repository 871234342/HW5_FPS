using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMove : MonoBehaviour
{
    public float speed = 2;
    public float detectRadius = 7f;
    public float chaseRadius = 21f;
    public float attackDamage = 10;

    public float attackSpeed = 1f;
    private float attackCooldown = 0f;

    bool alerted = false;
    NavMeshAgent agent;
    GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(target.transform.position, transform.position);
        attackCooldown -= Time.deltaTime;

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
                attackCooldown = 1f / attackSpeed;
                Attack(attackDamage);
            }
            //attack target
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

    void Attack(float attackDamage)
    {
        target.GetComponent<PlayerInfo>().Hurt(attackDamage);
    }
}
