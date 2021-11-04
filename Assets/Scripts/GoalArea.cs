using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalArea : MonoBehaviour
{
    GameObject UI;
    GameObject WinScreen;

    // Start is called before the first frame update
    void Start()
    {
        UI = PlayerManager.instance.UI;
        WinScreen = PlayerManager.instance.WinScreen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            UI.SetActive(false);
            WinScreen.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            PlayerManager.gamePaused = true;
        }
    }
}
