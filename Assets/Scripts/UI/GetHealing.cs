using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetHealing : MonoBehaviour
{
    GameObject player;
    [SerializeField] private int cost;
    private int count = 1;

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
        player.GetComponent<PlayerInfo>().HealToFull();
        player.GetComponent<PlayerInfo>().SpendResource(cost);
        cost = Inflate(cost);
    }

    private int Inflate(int prev)
    {
        count++;
        transform.Find("Text").gameObject.GetComponent<Text>().text = "Heal - " + prev * count;
        return prev * count;
    }
}
