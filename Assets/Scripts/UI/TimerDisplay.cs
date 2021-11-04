using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerDisplay : MonoBehaviour
{
    private int gameSecond;
    private int gameMinute;
    private int gameHour;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gameSecond = (int)Time.timeSinceLevelLoad;
        gameMinute = gameSecond / 60;
        gameSecond %= 60;
        gameHour = gameMinute / 60;
        gameMinute %= 60;
        //string time = string.Format("{0:d}:{1,00}:{2,00}", gameHour, gameMinute, gameSecond);
        //Debug.Log(time);
        //GetComponent<Text>().text = time;
        GetComponent<Text>().text = gameHour.ToString() + ":" + gameMinute.ToString("00") + ":" + gameSecond.ToString("00");
    }
}
