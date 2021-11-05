using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour
{
    GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerInfo>().reloading || player.GetComponent<PlayerInfo>().sprint)
        {
            transform.Find("Crosshairs").transform.gameObject.SetActive(false);
        }
        else
        {
            transform.Find("Crosshairs").transform.gameObject.SetActive(true);
        }
    }
}
