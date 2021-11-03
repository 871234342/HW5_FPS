using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ExploderControl : MonoBehaviour
{
    public float detectRadius = 7f;
    public float chaseRadius = 21f;

    public float explodeTime = 2f;
    public float blastRadius = 4f;
    public int blastDamage = 50;
    public LayerMask explodeMask;

    bool triggered = false;
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

        if (!agent.enabled) return;

        if (!triggered)
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
                ExplodeTrigger();
            }
        }
        else
        {
            explodeTime -= Time.deltaTime;
            if (explodeTime <= 0)
            {
                ExplodeFinish();
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
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, blastRadius);
    }

    public void ExplodeTrigger()
    {
        Debug.Log("Exploding!");
        if (!triggered)
        {
            triggered = true;
            agent.speed = 0;
        }
    }

    void ExplodeFinish()
    {
        Debug.Log("Exploded!");
        GameObject effect =  Instantiate(PlayerManager.instance.explodeEffect, transform.position, Quaternion.identity);
        effect.SetActive(true);

        Collider[] victims = Physics.OverlapSphere(transform.position, blastRadius, explodeMask);
        foreach (Collider hit in victims)
        {
            float distance = Vector3.Distance(transform.position, hit.gameObject.transform.position);
            if (hit.name == "Player")
            {
                hit.gameObject.GetComponent<PlayerInfo>().Hurt((int)(blastDamage * (1 - distance / blastRadius)));
            }
            else
            {
                hit.gameObject.GetComponent<EnemyInfo>().Hurt((int)(blastDamage * (1 - distance / blastRadius)));
                hit.gameObject.GetComponent<EnemyInfo>().ExplodeForce(20f, transform.position, blastRadius);
            }
        }

        this.GetComponent<EnemyInfo>().Dead(true);
    }
}