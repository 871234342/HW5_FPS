using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    GameObject player;
    private int maxHealth;
    private int health;
    private float ratio;

    // Start is called before the first frame update
    void Start()
    {
        player = PlayerManager.instance.player;
    }

    // Update is called once per frame
    void Update()
    {
        maxHealth = player.GetComponent<PlayerInfo>().maxHealth;
        health = Mathf.Max(player.GetComponent<PlayerInfo>().health, 0);
        ratio = (float)health / maxHealth;
        transform.Find("HealthText").gameObject.GetComponent<Text>().text = health + "\\" + maxHealth;
        Vector3 newScale = transform.Find("Healthbar").localScale;
        newScale.x = ratio;
        transform.Find("Healthbar").localScale = newScale;
        ratio = (Mathf.Clamp(ratio, 0.2f, 0.8f) - 0.2f) / 0.6f;
        transform.Find("Healthbar").gameObject.GetComponent<Image>().color = new Color32((byte)(255 * (1 - ratio)), (byte)(255 * ratio), 0, 255);
    }
}
