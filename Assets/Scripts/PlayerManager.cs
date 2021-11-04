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
    public GameObject exploder;
    public GameObject upgragePanel;
    public GameObject pauseMeun;
    public GameObject UI;
    public GameObject WinScreen;
    public GameObject Exit;
    public GameObject PreExit;
    public int difficulty = 0;
    public float gameTime = 0f;
    public static bool gamePaused = false;
    public static bool pauseMeunOpened = false;

    [SerializeField] private int exitTime;
    [SerializeField] private bool exitWarning;
    [SerializeField] private Vector3 exitPos;
    private GameObject tmpExit;
    private GameObject tmpPreExit;

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
        exitWarning = false;
        exitTime = Random.Range(480, 600);
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
        if (gameTime >= 10)
        {
            difficulty++;
            //Debug.Log("Difficulty raised to " + difficulty);
            gameTime = 0;
        }

        // spawn enemies
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
