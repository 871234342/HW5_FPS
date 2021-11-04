using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetAmmo : MonoBehaviour
{
    GameObject player;
    [SerializeField] private int ammo;
    [SerializeField] private int cost;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerInfo>().resource < cost)
        {
            GetComponent<Button>().interactable = false;
        }
        else
        {
            GetComponent<Button>().interactable = true;
        }
    }

    public void Preesed()
    {
        player.GetComponent<PlayerInfo>().AddAmmo(ammo);
        player.GetComponent<PlayerInfo>().SpendResource(cost);
    }
}
