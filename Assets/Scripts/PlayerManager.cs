using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    public List<GameObject> enemies;
    public List<float> enemiesSpawnRatio;
    private float enemiesSpawnTotal;
    public GameObject upgragePanel;
    public GameObject pauseMeun;
    public GameObject UI;
    public GameObject WinScreen;
    public GameObject Exit;
    public GameObject PreExit;

    private int difficulty = 0;
    private float gameTime = 0f;
    [SerializeField] private int diffRiseInverval = 60;

    public static bool gamePaused = false;
    public static bool pauseMeunOpened = false;

    private int exitTime;
    private bool exitWarning;
    [SerializeField] private Vector3 exitPos;
    private GameObject tmpExit;
    private GameObject tmpPreExit;

    [SerializeField] private Vector2 boundary;
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private Vector2 spawnDistance;
    private int spawnAmount = 2;
    [SerializeField] private float spawnRange = 4f;
    private float spawnCooldown;
    private float spawnChance;
    [SerializeField] private bool spawnable = true;

    private void Start()
    {
        spawnCooldown = spawnInterval;
        exitWarning = false;
        exitTime = Random.Range(480, 600);
        if (enemies.Count != enemiesSpawnRatio.Count)
        {
            Debug.Log("Enemies and their prob not match!!");
        }
        foreach (float i in enemiesSpawnRatio)
        {
            enemiesSpawnTotal += i;
        }
        for (int i = 1; i < enemiesSpawnRatio.Count; i++)
        {
            enemiesSpawnRatio[i] += enemiesSpawnRatio[i - 1];
        }
        if (spawnDistance.x > spawnDistance.y)
        {
            float tmp = spawnDistance.x;
            spawnDistance.x = spawnDistance.y;
            spawnDistance.y = tmp;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseMeunOpened)
            {
                gamePaused = false;
                Cursor.lockState = CursorLockMode.Locked;
                pauseMeun.SetActive(false);
            }
            else
            {
                gamePaused = true;
                Cursor.lockState = CursorLockMode.None;
                pauseMeun.SetActive(true);
            }
            pauseMeunOpened = !pauseMeunOpened;
        }

        if (gamePaused)
        {
            Time.timeScale = 0f;
            AudioListener.pause = true;
            return;
        }
        Time.timeScale = 1f;
        AudioListener.pause = false;

        // adjust difficulty
        gameTime += Time.deltaTime;
        if (gameTime >= diffRiseInverval)
        {
            difficulty++;
            spawnAmount = difficulty * 2;
            gameTime = 0;
        }

        // spawn enemies
        if (spawnable)  spawnCooldown -= Time.deltaTime;
        if (spawnCooldown <= 0)
        {
            spawnCooldown = spawnInterval;
            Vector2 spawnCenter;
            Vector2 playerPos = new Vector2(player.transform.position.x, player.transform.position.z);

            do
            {
                spawnCenter =
                new Vector2(
                    Random.Range(Mathf.Max(-boundary.x, player.transform.position.x - spawnDistance.y), Mathf.Min(boundary.x, player.transform.position.x + spawnDistance.y)),
                    Random.Range(Mathf.Max(-boundary.y, player.transform.position.z - spawnDistance.y), Mathf.Min(boundary.y, player.transform.position.z + spawnDistance.y))
                   );
            } while (Vector2.Distance(spawnCenter, playerPos) < spawnDistance.x);


            for (int i = 0; i < spawnAmount; i++)
            {
                Vector3 spawnpoint =
                    new Vector3(
                        Random.Range(spawnCenter.x - spawnRange, spawnCenter.x + spawnRange),
                        -500,
                        Random.Range(spawnCenter.y - spawnRange, spawnCenter.y + spawnRange)
                        );
                spawnpoint.y = Terrain.activeTerrain.SampleHeight(spawnpoint) + 0.1f - 200f;
                Debug.Log("Spawn At " + spawnpoint);

                float rand = Random.Range(0, enemiesSpawnTotal);
                int index = 0;
                for (; index < enemies.Count; index++)
                {
                    if (rand <= enemiesSpawnRatio[index]) break;

                }
                GameObject newSpawn = Instantiate(enemies[index], spawnpoint, Quaternion.identity);
                if (newSpawn.GetComponent<EnemyInfo>() != null)
                {
                    newSpawn.GetComponent<EnemyInfo>().level = difficulty;
                }
                else
                {
                    newSpawn.GetComponent<EnemyControl>().level = difficulty;
                }
            }
        }

        // spawn exit point
        if ((int)Time.timeSinceLevelLoad == exitTime - 30 && !exitWarning)
        {
            exitPos = new Vector3(Random.Range(-boundary.x, boundary.x), 0, Random.Range(-boundary.y, boundary.y));
            exitPos.y = Terrain.activeTerrain.SampleHeight(exitPos);
            tmpPreExit = Instantiate(PreExit, exitPos, Quaternion.identity);
            StartCoroutine("ShowWarning");
        }
        if ((int)Time.timeSinceLevelLoad == exitTime && !exitWarning)
        {
            exitWarning = true;
            Destroy(tmpPreExit);
            tmpExit = Instantiate(Exit, exitPos, Quaternion.identity);
        }
        if ((int)Time.timeSinceLevelLoad == exitTime + 60 && exitWarning)
        {
            exitWarning = false;
            Destroy(tmpExit);
            exitTime += Random.Range(240, 300);
        }
    }

    IEnumerator ShowWarning()
    {
        exitWarning = true;
        // fade in warning message in 1 second
        UI.transform.Find("Exit").gameObject.SetActive(true);
        Text txt = UI.transform.Find("Exit").gameObject.GetComponent<Text>();
        txt.text = "Evacuation Availiable Soon";
        Color32 txtColor = txt.color;
        for (float alpha = 0; alpha <= 255; alpha += 255 * Time.deltaTime)
        {
            txtColor.a = (byte)alpha;
            txt.color = txtColor;
            yield return null;
        }

        // display for 2 seconds
        for (float timer = 0; timer <= 2; timer += Time.deltaTime)
        {
            yield return null;
        }

        // fade out in 1 second
        for (float alpha = 255; alpha >= 0 ; alpha -= 255 * Time.deltaTime)
        {
            txtColor.a = (byte)alpha;
            txt.color = txtColor;
            yield return null;
        }
        //exitWarning = false;
        UI.transform.Find("Exit").gameObject.SetActive(false);
        exitWarning = false;
    }
}
