using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplay : MonoBehaviour
{
    GameObject player;
    int ammo, capacity;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        ammo = player.GetComponent<PlayerInfo>().ammo;
        capacity = player.GetComponent<PlayerInfo>().magazineCap;
        GetComponent<Text>().text = ammo + "\\" + capacity;
    }
}
