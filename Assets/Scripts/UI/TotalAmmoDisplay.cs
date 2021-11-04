using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TotalAmmoDisplay : MonoBehaviour
{
    GameObject player;
    int totalAmmo;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        totalAmmo = player.GetComponent<PlayerInfo>().totalAmmo;
        GetComponent<Text>().text = "" + totalAmmo;
    }
}
