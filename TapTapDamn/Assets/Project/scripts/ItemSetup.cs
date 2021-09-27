using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ItemSetup : MonoBehaviour {
    [Header("Circle & Arrows")]
    public Transform CenterObj;
    public Transform KeyCenter;
    public GameObject prefab;
    public Transform CircleCentre;
    public Vector2 CenterOffset = new Vector2(-0.05f, 0.3f);
    public float Radius;
    public int Count = 6;

    [Space(15)]
    [Header("Reel")]
    public Reel reel;
    public Transform Sprinter;
    public float SprinterRadius = 5.15f;
    public Vector2 SprinterCenterOffset = new Vector2(0.12f, 0.25f);
    public int sectors = 48;
    [Range(0.0f, 1.0f)]
    public float speed = .1f;

    [Header("Level manager:")]
    public LevelManager levelManager;

    float AngleNow = 0; //--- Z rotation

    List<GameObject> CreatedPrefs = new List<GameObject>();

    public bool RCh; //--- вращение по часовой стрелке
    private float _x, _y;
    
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
                Item item = instPref.GetComponent<Item>();
                item.levelManager = levelManager;
                item.itemClass = new ItemClass();
                item.itemClass.id = i;
                item.itemClass.enable = false;
                CreatedPrefs.Add(instPref);
            }
        }
        CenterObj.rotation = Quaternion.Euler(new Vector3(15, 12, -2));

        //---стартуем вращение
        reel.start = true;
    }
    #endregion

    #region HELL mode:
    public void CreatePrefsHellMode()
    {
        ClearPrefs();
        reel.speed = 3;//---устанавливаем скорость вращения рулетки

        float angle = 360 / Count;
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

