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
    public int difficulty = 0;
    public float gameTime = 0f;

    private void Update()
    {
        gameTime += Time.deltaTime;
        if (gameTime >= 10)
        {
            difficulty++;
            Debug.Log("Difficulty raised to " + difficulty);
            gameTime = 0;
        }
    }
}
