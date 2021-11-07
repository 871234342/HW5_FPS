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
    public int baseBlastDamage = 50;
    private int blastDamage;
    private int level;
    public LayerMask explodeMask;

    bool triggered = false;
    public bool alerted = false;
    NavMeshAgent agent;
    GameObject target;

    public AudioClip explode;
    AudioSource audio;

    // Start is called before the first frame update
    void Start()
    {
        target = PlayerManager.instance.player;
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.transform.position;
        level = this.gameObject.GetComponent<EnemyInfo>().level;
        blastDamage = (int)(baseBlastDamage * (1 + level * 0.15f));
        detectRadius *= 1 + (level + 1) * 0.2f;
        chaseRadius *= 1 + (level + 1) * 0.3f;

        audio = GetComponent<AudioSource>();

    }

    // Update is called once per frame
    void Update()
    {
        if (PlayerManager.gamePaused) return;
        if (!agent.enabled) return;

        float distance = Vector3.Distance(target.transform.position, transform.position);

        if (!triggered)
        {
            if (distance <= detectRadius)
            {
                alerted = true;
                GetComponent<Animation>().Play("run");
                agent.SetDestination(target.transform.position);
            }
            else if (alerted && distance <= chaseRadius)
            {
                agent.SetDestination(target.transform.position);
            }
            else
            {
                alerted = false;
                GetComponent<Animation>().Play("idle");
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
        if (!triggered)
        {
            triggered = true;
            agent.speed = 0;
            StartCoroutine("ExplodeEffect");
        }
    }

    void ExplodeFinish()
    {
        GameObject effect =  Instantiate(PlayerManager.instance.explodeEffect, transform.position, Quaternion.identity);
        effect.SetActive(true);

        PlayerManager.instance.gameObject.GetComponent<AudioSource>().Play();

        Collider[] victims = Physics.OverlapSphere(transform.position, blastRadius, explodeMask);
        foreach (Collider hit in victims)
        {
            float distance = Vector3.Distance(transform.position, hit.gameObject.transform.position);
            if (distance == 0)
            {
                Debug.Log("SELF");
                break;
            }
            distance = Mathf.Min(distance, blastRadius);
            if (hit.tag == "Player")
            {
                hit.gameObject.GetComponent<PlayerInfo>().Hurt((int)(blastDamage * (1 - distance / blastRadius)), transform.position);
            }
            else if (hit.tag == "Zombie")
            {
                Debug.Log("HIT ZOMBIE");
                hit.gameObject.GetComponent<EnemyControl>().Hurt((int)(blastDamage * (1 - distance / blastRadius)));
                hit.gameObject.GetComponent<EnemyControl>().ExplodeForce(6f, transform.position, blastRadius);
            }
            else if (hit.tag == "Exploder")
            {
                Debug.Log("HIT Exploder");
                hit.gameObject.GetComponent<EnemyInfo>().Hurt((int)(blastDamage * (1 - distance / blastRadius)));
                hit.gameObject.GetComponent<EnemyInfo>().ExplodeForce(6f, transform.position, blastRadius);
            }
        }

        this.GetComponent<EnemyInfo>().Dead(true);
    }

    IEnumerator ExplodeEffect()
    {
        GetComponent<Animation>().Play("special");
        float scale = 1f;
        Vector3 originalScale = transform.localScale;

        for (;; scale += 0.3f * Time.deltaTime / explodeTime)
        {
            transform.localScale = originalScale * scale;
            yield return null;
        }
    }
}
