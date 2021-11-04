using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetATK : MonoBehaviour
{
    GameObject player;
    [SerializeField] private int increase;
    [SerializeField] private int cost;
    private int count;

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
        player.GetComponent<PlayerInfo>().ATKup();
        player.GetComponent<PlayerInfo>().SpendResource(cost);
        cost = Inflate(cost);
    }

    private int Inflate(int prev)
    {
        count++;
        transform.Find("Text").gameObject.GetComponent<Text>().text = "ATK - " + prev * count;
        return prev * count;
    }
}
