using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour {
    public LevelManager levelManager;
    public bool timerStarted = false;

    public Text textUITimer;

    private void Start()
    {
        //textUITimer = GetComponent<Text>();
    }

    public void StartTimer()
    {
        levelManager.time = 30;
        IEnumerator timer = Timer();
        StartCoroutine(timer);
        timerStarted = true;
    }

    public void StopTimer()
    {
        //GameMetaData.instance.
        StopAllCoroutines();
        timerStarted = false;
    }

    IEnumerator Timer()
    {
        for (;;)
        {
            levelManager.time -= 1;
            textUITimer.text = TimeString((int)levelManager.time);
            if (levelManager.time == 0) StopAllCoroutines();
            yield return new WaitForSeconds(1f);
        }
    }

    public string TimeString(int timesec)
    {
        string result = "";
        if (timesec < 10)
            result = "0" + timesec.ToString();
        else
            result = timesec.ToString();
        return result;
    }

}
