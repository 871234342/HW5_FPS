using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region Singleton

    public static PlayerManager instance;

    private void Awake()
    {
        instance = this;
    }
    #endregion

    public GameObject player;
    public GameObject loot;
    public GameObject explodeEffect;
    public GameObject exploder;
    public int difficulty = 0;
    public float gameTime = 0f;

    [SerializeField]
    private float spawnInterval = 5f;
    private float spawnCooldown;
    private float spawnChance;
    private bool spawnable = true;

    private void Start()
    {
        spawnCooldown = spawnInterval;
    }

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (gameTime >= 10)
        {
            difficulty++;
            //Debug.Log("Difficulty raised to " + difficulty);
            gameTime = 0;
        }

        spawnCooldown -= Time.deltaTime;
        if (spawnable && spawnCooldown <= 0)
        {
            spawnable = false;
            Instantiate(exploder, player.transform.position + new Vector3(10, 0, 0), Quaternion.identity);
        }
    }
}
