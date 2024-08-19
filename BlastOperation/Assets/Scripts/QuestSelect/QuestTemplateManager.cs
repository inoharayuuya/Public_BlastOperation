using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestTemplateManager : MonoBehaviour
{
    ActiveUIManager uiManager;
    QuestAnimationsManager animManager;

    // Start is called before the first frame update
    void Start()
    {
        // ActiveUIManager取得
        uiManager = GameObject.Find("ActiveUIManager").GetComponent<ActiveUIManager>();
        // QuestAnimationsManager取得
        animManager = GameObject.Find("QuestSelect").GetComponent<QuestAnimationsManager>();

        // ボタンコンポーネント取得
        // ボタン押下時の関数登録
        this.GetComponent<Button>().onClick.AddListener(uiManager.TapQuest);
        this.GetComponent<Button>().onClick.AddListener(animManager.ListAnActive);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
