using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Item : MonoBehaviour {
    [Header("Level manager:")]
    public LevelManager levelManager;

    [Header("Item class:")]
    public ItemClass itemClass; //лучше сделать private и serizlizeField

    [Header("Item Up:")]
    public SpriteRenderer spriteUp; //лучше сделать private и serizlizeField

    [Header("Item Down:")]
    public SpriteRenderer spriteDown;//лучше сделать private и serizlizeField

    [HideInInspector]
    public Sprite spriteKeyUp;
    [HideInInspector]
    public Sprite spriteKeyUpFill;
    [HideInInspector]
    public Sprite spriteKeyDown;
    [HideInInspector]
    public Sprite spriteKeyDownFill;

    bool triggerOn = false;

    private void Start()
    {
        levelManager.items.Add(itemClass); 
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && triggerOn) //считывания нажатия кнопки мыши происходит в item - то есть на количество item это будет считываться - под нажатия нужно сделать отдельный класс
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
            spriteUp.sprite = spriteKeyUpFill;
            spriteUp.transform.DOLocalMoveY(2.2f, 0.2f);
        }
        else
        {
            spriteUp.sprite = spriteKeyUp;
            spriteUp.transform.DOLocalMoveY(0.0f, 0.2f);
        }
    }

    #region Trigger work:
    private void OnTriggerEnter(Collider other) //любой триггер может это изменить - соответственно при добавлении в игру фич вся эта система сломается - нужно использовать интерфейс
    {
        triggerOn = true;
        spriteDown.sprite = spriteKeyDownFill;
    }

    private void OnTriggerExit(Collider other)
    {
        triggerOn = false;
        spriteDown.sprite = spriteKeyDown;
    }
    #endregion
}
