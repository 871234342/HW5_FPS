using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDisplay : MonoBehaviour
{
    GameObject player;
    private int resource;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        resource = player.GetComponent<PlayerInfo>().resource;
        GetComponent<Text>().text = "" + resource;
    }
}
