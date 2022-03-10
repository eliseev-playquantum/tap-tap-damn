using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemSetup : MonoBehaviour {
    [Header("Circle & Arrows")]
    public Transform CenterObj; //скрипт сам может найти центр объект - необязательно это вписывать самому
    public Transform KeyCenter; //зачем определять тут, а изменять в другом месте - нужно из этого выделить отдельный класс для определения и изменения
    public GameObject prefab; //что это за префаб - неправильное название
    public Transform CircleCentre;
    public Vector2 CenterOffset = new Vector2(-0.05f, 0.3f); //что это за значение? нужно чтобы скрипт сам определял центр
    public float Radius; //лучше сделать private и serizlizeField
    public int Count = 6; //лучше сделать private и serizlizeField

    [Space(15)]
    [Header("Reel")]
    public Reel reel; //зачем определять тут, а изменять в другом месте - это неправильно
    public Transform Sprinter; //сделать private
    public float SprinterRadius = 5.15f; //сделать private
    public Vector2 SprinterCenterOffset = new Vector2(0.12f, 0.25f);  //сделать private и serializeField
    public int sectors = 48;  //сделать private
    [Range(0.0f, 1.0f)]
    public float speed = .1f;


    [SerializeField]
    private Sprite keyUp;
    [SerializeField]
    private Sprite keyUpFill;
    [SerializeField]
    private Sprite keyDown;
    [SerializeField]
    private Sprite keyDownFill;

    [Header("Level manager:")]
    public LevelManager levelManager; //нужно сделать private, reqireComponent и GetComponent

    float AngleNow = 0; //--- Z rotation

    List<GameObject> CreatedPrefs = new List<GameObject>();  //лучше создать List с item - так как его цель хранить именно этот класс, а не геймобджект


    public bool RCh; //--- вращение по часовой стрелке
    private float _x, _y; //что за x и y неправильное название для переменных
    
    #region History mode:
    public void CreatePrefsHistoryMode()
    {
        ClearPrefs();
        float angle = 360 / Count;
        for (int i = 1; i <= Count; i++)
        {
            float x = (CircleCentre.position.x + CenterOffset.x) + Radius * Mathf.Cos(2 * Mathf.PI * i / Count);
            float y = (CircleCentre.position.y + CenterOffset.y) + Radius * Mathf.Sin(2 * Mathf.PI * i / Count);
            if (Random.Range(0, 2) != 0)
            {
                GameObject instPref = Instantiate(prefab, CenterObj);
                instPref.transform.position = new Vector3(x, y, 0.4f);
                instPref.transform.Rotate(new Vector3(0, 0, angle * (i - 1) - angle));
                Item item = instPref.GetComponent<Item>(); //в таком случае лучше инстациировать клас Item, а не геймОбджект с Item
                item.levelManager = levelManager;
                item.itemClass = new ItemClass();
                item.itemClass.id = i;
                item.itemClass.enable = false;
                item.spriteKeyDownFill = keyDownFill;
                item.spriteKeyDown = keyDown;
                item.spriteKeyUp = keyUp;
                item.spriteKeyUpFill = keyUpFill;
                CreatedPrefs.Add(instPref);
            }
        }
        CenterObj.rotation = Quaternion.Euler(new Vector3(15, 12, -2)); //непонятные цифры, что это значит?

        //---стартуем вращение
        reel.start = true;
    }
    #endregion

    #region HELL mode:
    public void CreatePrefsHellMode()
    {
        ClearPrefs();
        reel.speed = 3;//---устанавливаем скорость вращения рулетки //зачем здесь устанавливать скорость, что это за цифра, лучше пусть этот класс сам в себе это будет определять

        float angle = 360 / Count; //одинаковые действия из метода, который выше - можно выделить это в отдельный метод
        for (int i = 1; i <= Count; i++)
        {
            float x = (KeyCenter.position.x + CenterOffset.x) + Radius * Mathf.Cos(2 * Mathf.PI * i / Count);
            float y = (KeyCenter.position.y + CenterOffset.y) + Radius * Mathf.Sin(2 * Mathf.PI * i / Count);

            GameObject instPref = Instantiate(prefab, KeyCenter);
            instPref.transform.position = new Vector3(x, y, 0.4f);
            instPref.transform.Rotate(new Vector3(0, 0, angle * (i - 1) - angle));
            Item item = instPref.GetComponent<Item>();
            item.levelManager = levelManager;
            item.itemClass = new ItemClass();
            item.itemClass.id = i;
            item.itemClass.enable = false;
            CreatedPrefs.Add(instPref);
        }

        CenterObj.rotation = Quaternion.Euler(new Vector3(15, 12, -2));

        //---стартуем вращение
        reel.start = true;
    }
    #endregion
        
    #region PvP mode:
    public void CreatePrefsPvPMode()
    {

    }
    #endregion

    public void ClearPrefs()
    {
        foreach (GameObject go in CreatedPrefs)
            Destroy(go);
        CreatedPrefs.Clear();
        KeyCenter.transform.localScale = new Vector3(1, 1, 1); 
        KeyCenter.localRotation = Quaternion.Euler(Vector3.zero); 
        CenterObj.rotation = Quaternion.Euler(Vector3.zero);        
        levelManager.items.Clear();
    }
}

