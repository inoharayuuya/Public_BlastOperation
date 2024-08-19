using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrgCharaTemplate : MonoBehaviour
{
    ActiveUIManager uiManager;

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager取得
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();

        // ボタンコンポーネント取得
        // ボタン押下時の関数登録
        this.GetComponent<Button>().onClick.AddListener(() => uiManager.TapOrgChara(this.gameObject));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
