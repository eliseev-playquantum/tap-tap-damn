using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTimer : MonoBehaviour {
    public LevelManager levelManager; //не нужно делать public - лучше сделать private, requireComponet, getComponent в start
    public bool timerStarted = false; //нужно сделать переменную private и  сделать public переменную, которая будет брать данные этой переменной

    public Text textUITimer;

    public void StartTimer()
    {
        levelManager.time = 30; //не нужно делать здесь 30, если time определяется в инспекторе
        IEnumerator timer = Timer(); //зачем это определать, если эту корутину можно сразу запустить снизу
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
        for (;;) //к чему этот цикл фор, если можно в конце перезапускать корутину
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
