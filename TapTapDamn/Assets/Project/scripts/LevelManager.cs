using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LevelManager : MonoBehaviour {
    public ItemSetup itemSetup; //не нужно делать public - лучше сделать private, requireComponet, getComponent в start
    public List<ItemClass> items = new List<ItemClass>();
    public int time = 0; //зачем здесь значение time, если рассчет time идет в отдельном классе timer
    public int level = 0; //нужно сделать private

    public LevelTimer levelTimer; //нужно сделать private B serializeField
    public Text levelText; //нужно сделать private B serializeField

    public e_GameMode selectedmode; //должно быть private

    [Header("Game canvas:")]
    public GameObject GameCanvas; //зачем с большой буквы название переменной?

    [Header("Reward window script:")]
    public RewardWindow rewardWindowScript;

    /// <summary>
    /// Обрабатываем событие по клику, приходящее от замка
    /// </summary>
    /// <param name="enable"></param>
    public void ClickManager(bool enable)
    {
        if (enable)
        {
            bool end = true;
            foreach (ItemClass item in items)
                if (item.enable == false) end = false; //не соблюдается стандартизация написания if - тут пишется if так, на следующей строке по-другому

            if (end)
            {
                MotionChangeLevel();
            }
        }

        //---рандомное вращение
        if(selectedmode == e_GameMode.Hell)
            if (Random.Range(0, 3) > 1)
                itemSetup.reel.speed *= -1;
    }

    //---стартуем уровень
    public void StartNewLevel(e_GameMode gamemode)
    {
        selectedmode = gamemode;
        GameCanvas.SetActive(true);

        SpriteRenderer[] sprites = itemSetup.KeyCenter.GetComponentsInChildren<SpriteRenderer>();
        foreach (SpriteRenderer sprite in sprites) sprite.sharedMaterial.DOFloat(1, "_Transparent", 0.3f); //не соблюдается стандартизация написания foreach - везде по-разному пишется

        switch (selectedmode)
        {
            case e_GameMode.Classic:
                itemSetup.CreatePrefsHistoryMode();
                levelTimer.StartTimer();
                break;
            case e_GameMode.Hell:
                itemSetup.CreatePrefsHellMode();
                if (!levelTimer.timerStarted)
                {
                    levelTimer.StartTimer();
                    level = 0;
                }
                break;
            case e_GameMode.PvP:
                itemSetup.CreatePrefsPvPMode();
                levelTimer.StartTimer();
                break;
        }

        level++;
        levelText.text = level.ToString();
    }

    public void LevelEnd() //правильней сделать на это event, чтобы каждый класс сам знал, что ему делать в конце уровня
    {
        //---завершили уровень
        //---показать окно награды
        Debug.Log("LevelEnd");
        levelTimer.StopTimer();
        itemSetup.reel.start = false;
        items.Clear();
        itemSetup.ClearPrefs();

        GameCanvas.SetActive(false);
        rewardWindowScript.rewardWindow.SetActive(true);
        rewardWindowScript.GameMaxLevel.text = level.ToString();
    }

    /// <summary>
    /// Движение при смене уровня
    /// </summary>
    public void MotionChangeLevel()
    {
        SpriteRenderer[] sprites = itemSetup.KeyCenter.GetComponentsInChildren<SpriteRenderer>();

        itemSetup.KeyCenter.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 45), 0.3f).OnComplete(() => {
            foreach (SpriteRenderer sprite in sprites) sprite.sharedMaterial.DOFloat(0, "_Transparent", 0.3f);
            itemSetup.KeyCenter.DOScale(0.8f, 0.3f).OnComplete(() => {
                //---code...
                if (selectedmode == e_GameMode.Hell)
                {
                    if (time > 0)
                    {
                        StartNewLevel(e_GameMode.Hell);
                    }
                    else
                    {
                        LevelEnd();
                    }
                }
                foreach (SpriteRenderer sprite in sprites) sprite.sharedMaterial.DOFloat(1, "_Transparent", 0.3f);
                itemSetup.KeyCenter.DOScale(1f, 0.3f);
            });
        });
    }

    #region Game modes:
    public void StartHellMode()
    {
        //levelTimer.StartTimer();
        //itemSetup.CreatePrefsHellMode();
        StartNewLevel(e_GameMode.Hell);
    }
    #endregion
}
