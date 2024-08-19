using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconCharaTemplateManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // RankCardManager取得
        RankCardManager rcManager = GameObject.Find("RankCardManager").GetComponent<RankCardManager>();

        // ボタンコンポーネント取得
        // ボタン押下時の関数登録
        this.GetComponent<Button>().onClick.AddListener(()=>rcManager.TapNewCharaIcon(this.gameObject));

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
