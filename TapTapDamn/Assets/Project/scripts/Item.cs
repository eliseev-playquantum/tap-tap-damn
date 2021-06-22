using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour {
    [Header("Level manager:")]
    public LevelManager levelManager;

    [Header("Item class:")]
    public ItemClass itemClass;

    [Header("Item Up:")]
    public SpriteRenderer spriteUp;

    [Header("Item Down:")]
    public SpriteRenderer spriteDown;

    bool triggerOn = false;

    private void Start()
    {
        levelManager.items.Add(itemClass);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && triggerOn)
        {
            //Debug.Log("MB_OK");
            itemClass.enable = !itemClass.enable;
            SetDownState(itemClass.enable);
        }
    }

    /// <summary>
    /// выполняем действие при таче и попадании в диапазон
    /// </summary>
    private void SetDownState(bool enable)
    {
        levelManager.items.Find(x=>x.id == itemClass.id).enable = enable;
        levelManager.ClickManager(enable);
        if (enable)
        {
            spriteUp.sprite = Resources.Load<Sprite>("Key_UpFill");
            spriteUp.transform.DOLocalMoveY(2.2f, 0.2f);
        }
        else
        {
            spriteUp.sprite = Resources.Load<Sprite>("Key_Up");
            spriteUp.transform.DOLocalMoveY(0.0f, 0.2f);
        }
    }

    #region Trigger work:
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collider enter");
        triggerOn = true;
        spriteDown.sprite = Resources.Load<Sprite>("Key_DownFill");
    }

    private void OnTriggerExit(Collider other)
    {
        //Debug.Log("Collider exit");
        triggerOn = false;
        spriteDown.sprite = Resources.Load<Sprite>("Key_Down");
    }
    #endregion
}
