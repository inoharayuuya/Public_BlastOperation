using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharaTemplateManager : MonoBehaviour
{
    ActiveUIManager uiManager;

    // キャラの画像
    public static Sprite cSprite;

    private void AddEventTrigger(EventTriggerType _type, Action _event)
    {
        // イベントトリガーコンポーネント取得
        // ボタン長押し時の関数登録
        EventTrigger currentTrigger = this.GetComponent<EventTrigger>(); // イベントトリガー取得
     
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = _type;   //PointerUpイベントに追加

        entry.callback.AddListener((eventData)=> { _event(); });   //呼び出す関数の登録

        currentTrigger.triggers.Add(entry);                              // 関数の登録

    }

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager取得
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();

        // ボタンコンポーネント取得
        // ボタン押下時の関数登録
        this.GetComponent<Button>().onClick.AddListener(uiManager.TapChara);
        this.GetComponent<Button>().onClick.AddListener(TapChara2);



        AddEventTrigger(EventTriggerType.PointerDown,uiManager.PointerDownChara);
        AddEventTrigger(EventTriggerType.PointerUp, uiManager.PointerUpChara);

    } 

    public void TapChara2()
    {
        
        // 通常編成モード
        if (uiManager.isOrg)
        {
            OrganizationManager.isGet = true;
            OrganizationManager.isFirst = false;

            // キャラの画像を取得
            cSprite = GetComponent<Image>().sprite;

            uiManager.orgChara.GetComponent<Image>().sprite = cSprite;
            
            // 通常編成で編成したキャラと一括編成の時に表示するキャラ同じにする
            

            //this.gameObject.GetComponent<Image>().color = new Color(0,0,0);
        }
        
        
        // 一括編成中のキャラをタップしたら外す
        if(this.gameObject.tag == "Bulk")
        {
            //uiManager.isBulkOrg = false;
            uiManager.bulkCnt--;

            // 編成から外したときの処理作る


            Debug.Log("一括カウント : " + uiManager.bulkCnt);
            Destroy(gameObject);

        }
        else
        {
            if (uiManager.isBulkOrg)
            {
                uiManager.AddBulkOrg(this.gameObject);
            }
        }


    }

}
