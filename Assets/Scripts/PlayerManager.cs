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
    public Terrain terrain;
    public int difficulty = 0;
    public float gameTime = 0f;

    [SerializeField] private Vector2 boundary;


    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private float spawnDistance = 10f;
    private int spawnAmount = 2;
    private float spawnRange = 4f;
    private float spawnCooldown;
    private float spawnChance;
    [SerializeField] private bool spawnable = true;

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

        if (spawnable)  spawnCooldown -= Time.deltaTime;
        if (spawnCooldown <= 0)
        {
            spawnCooldown = spawnInterval;

            Vector3 spawnCenter =
                new Vector2(
                    Random.Range(Mathf.Max(-boundary.x, player.transform.position.x - spawnDistance), Mathf.Min(boundary.x, player.transform.position.x + spawnDistance)),
                    Random.Range(Mathf.Max(-boundary.y, player.transform.position.z - spawnDistance), Mathf.Min(boundary.y, player.transform.position.z + spawnDistance))
                    );

            for (int i = 0; i < spawnAmount; i++)
            {
                Vector3 spawnpoint =
                    new Vector3(
                        Random.Range(spawnCenter.x - spawnRange, spawnCenter.x + spawnRange),
                        0,
                        Random.Range(spawnCenter.y - spawnRange, spawnCenter.y + spawnRange)
                        );
                spawnpoint.y = Terrain.activeTerrain.SampleHeight(spawnpoint) + 0.1f;
                GameObject newSpawn = Instantiate(exploder, spawnpoint, Quaternion.identity);
                newSpawn.GetComponent<ExploderControl>().damageMultiplier = difficulty / 5;
            }


            Instantiate(exploder, player.transform.position + new Vector3(10, 0, 0), Quaternion.identity);
        }
    }
}
