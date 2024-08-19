using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;
using UnityEngine.UI;

public class QuestDetailManager : MonoBehaviour
{
    // 使用するパーティ番号
    private const int USE_PARTY_NUMBER = 0; 

    // クエスト詳細画面
    [SerializeField] private GameObject questDetail;

    // ステータスマネージャー取得
    private StatusManager statusManager;

    /// <summary>
    /// クエストボタン押下時処理　クエストによって詳細テキストを変更
    /// </summary>
    /// <param name="_questId"></param>
    public void TapStageButton(int _questId)
    {
        OrganizationManager.isGet = true;
        OrganizationManager.isFirst = false;

        // ステータスマネージャー取得
        statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();

        // クエスト名変更
        var questTitleText = questDetail.transform.Find("QuestTitleText").GetComponent<Text>();
        questTitleText.text = statusManager.questName[_questId-1];

        // クエスト詳細変更
        var questDetailText = questDetail.transform.Find("QuestDetailText").GetComponent<Text>();
        questDetailText.text = statusManager.questDetail[_questId-1];

        // ステージIDとパーティ情報保存
        
        PlayerPrefs.SetInt(Common.KEY_STAGE_ID,_questId);
        PlayerPrefs.SetInt(Common.KEY_PARTY_ID,USE_PARTY_NUMBER);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// クエスト出発ボタン押下時処理
    /// </summary>
    public void TapGoButton()
    {
        Common.LoadScene(Common.SCENE_NAME_QUEST);
    }

    // Start is called before the first frame update
    void Start()
    {
        statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
