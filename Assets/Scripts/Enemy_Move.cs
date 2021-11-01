using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy_Move : MonoBehaviour
{
    public GameObject target;
    public float speed = 2;

    NavMeshAgent agent;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.destination = target.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        if (agent.enabled)  agent.destination = target.transform.position;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Enter"+collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            Debug.Log("STOP PLEASE");
            agent.enabled = false;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        Debug.Log("Leave"+collision.gameObject.tag);
        if (collision.gameObject.tag == "Player")
        {
            agent.enabled = true;
        }
    }
}
