using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootInfo : MonoBehaviour
{
    public int resource = 10;

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == "Player")
        {
            other.gameObject.GetComponent<PlayerInfo>().PickLoot(resource);
            Destroy(gameObject);
        }
    }
}
