using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Again()
    {
        PlayerManager.gamePaused = false;
        SceneManager.LoadScene("Main");
    }

    public void Leave()
    {
        Application.Quit();
    }
}
