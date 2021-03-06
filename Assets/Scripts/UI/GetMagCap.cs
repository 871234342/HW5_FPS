using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetMagCap : MonoBehaviour
{
    GameObject player;
    [SerializeField] private int increase;
    [SerializeField] private int cost;
    private int actualCost;
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        actualCost = cost;
        player = PlayerManager.instance.player;
        transform.Find("Text").gameObject.GetComponent<Text>().text = "MagCap - " + actualCost;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerInfo>().resource < actualCost)
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
        player.GetComponent<PlayerInfo>().magCapup();
        player.GetComponent<PlayerInfo>().SpendResource(actualCost);
        count++;
        float newCost = cost * Mathf.Pow(count, 1.1f);
        actualCost = (int)newCost;
        transform.Find("Text").gameObject.GetComponent<Text>().text = "MagCap - " + actualCost;
    }
}
