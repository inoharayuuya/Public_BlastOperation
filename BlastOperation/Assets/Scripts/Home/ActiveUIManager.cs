using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Const;
using UnityEngine.UI;

public class ActiveUIManager : MonoBehaviour
{
    // 定数
    public const int MAX_ORG = 4;  // 1編成に入るキャラの数
    private const float TIME_POINTER_DOWN = 2.0f;// 長押しして何秒経ったら表示するかに使用

    // 使うキャンバス、パネル
    [SerializeField] private GameObject[] useUI = new GameObject[12];

    // 使うキャンバス
    [SerializeField] GameObject canvasHome;
    [SerializeField] GameObject canvasChara;
    [SerializeField] GameObject canvasGacha;

    // 使うパネル等
    [SerializeField] GameObject homeChara;         // ホームに配置されているキャラ
    [SerializeField] GameObject tapArea;           // ホームキャラのタップ領域
    [SerializeField] GameObject menuPanel;         // メニューパネル
    [SerializeField] GameObject charaPanel;        // キャラパネル
    [SerializeField] GameObject organizationPanel; // 編成パネル
    [SerializeField] GameObject charaListPanel;    // キャラ一覧パネル
    [SerializeField] GameObject orgConfilmDialog;  // 編成確認ダイアログ
    [SerializeField] GameObject charaDetailPanel;  // キャラ詳細パネル
    [SerializeField] GameObject questPanel;        // クエスト選択パネル
    [SerializeField] GameObject questSelect;       // アニメーションするクエスト選択画面
    [SerializeField] Button questSelectBackButton; // クエスト選択画面の戻るボタン
    [SerializeField] GameObject questListPanel;    // クエスト一覧パネル
    [SerializeField] GameObject questConfilmPanel; // クエスト最終確認パネル

    // 編成キャラ
    public GameObject orgChara;
    // 編成モードかどうかのフラグ
    public bool isOrg;
    // 一括編成モードかどうかのフラグ
    public bool isBulkOrg;

    // キャラ長押し時に使用するタイマー
    private float pointerDownTimer;
    // キャラ長押し判定フラグ
    private bool isDown;

    // キャラ一覧用
    private bool isList;

    // 一括編成のキャラを表示する領域
    public GameObject bulkOrgBg;
    // 一括編成のキャラ数を把握するカウント
    public int bulkCnt;
    // キャラのプレファブ
    [SerializeField] GameObject charaTemplate;

    // 編成中のキャラ
    public GameObject[] org= new GameObject[MAX_ORG];

    // クエスト詳細テンプレートのプレファブ
    [SerializeField] GameObject detailTemplate;

    // クエスト選択中かどうか
    private bool isQuest;

    // 編成用の配列

    #region パブリック関数 - ボタンOnClick時呼ばれる処理

    #region 全項目共通

    /// <summary>
    /// 全てのUIを非表示にする関数
    /// </summary>
    private void AnActiveAll()
    {
        for (int i = 0; i < useUI.Length; i++)
        {

            useUI[i].SetActive(false);
        }

        // 編成フラグオフ
        isOrg = false;
        isBulkOrg = false;
        isQuest = false;
        // 一覧フラグオフ
        isList = false;

    }

    /// <summary>
    /// 各戻るボタン押下時処理
    /// </summary>
    /// <param name="_activePanel">表示するパネル(ひとつ前に表示していたパネル)</param>
    public void TapBackButton(GameObject _activePanel)
    {
        // 現在開いているパネルは非表示
        //_anactivePanel.SetActive(false);

        // ひとつ前に開いていたパネルを表示
        _activePanel.SetActive(true);

        // 編成モードオフ
        isOrg = false;
        isBulkOrg = false;
    }

    public void TapQuestBackButton()
    {
        isQuest = false;
        homeChara.SetActive(true);
    }

    #endregion


    #region ホーム関連

    /// <summary>
    /// ホームボタン押下時に呼び出す処理
    /// </summary>
    public void TapHomeButton()
    {
        questSelect.GetComponent<Animator>().SetTrigger("Back");
        questSelectBackButton.interactable = true;

        // すべてのUIを非表示
        AnActiveAll();

        // ホームのキャンバスを表示
        canvasHome.SetActive(true);

        // ホームのキャラを表示
        homeChara.SetActive(true);
        tapArea.SetActive(true);
    }

    /// <summary>
    /// メニューボタン押下時処理
    /// </summary>
    public void TapMenuButton()
    {
        tapArea.SetActive(false);
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// メニュー閉じるボタン押下時処理
    /// </summary>
    public void TapMenuBackButton()
    {
        tapArea.SetActive(true);
        menuPanel.GetComponent<Animator>().SetTrigger("Out");
    }

    /// <summary>
    /// タイトルに戻るボタン押下時処理
    /// </summary>
    public void TapToTitleButton()
    {
        Common.LoadScene(Common.SCENE_NAME_TITLE);
    }

    #endregion

    #region クエスト関連

    /// <summary>
    /// ホームのクエストボタン押下時処理
    /// </summary>
    public void TapQuestButton()
    {
        menuPanel.SetActive(false);

        isQuest = true;
        homeChara.SetActive(false);

        questPanel.SetActive(true);
        tapArea.SetActive(false);
    }

    #endregion

    /// <summary>
    /// クエスト選択のノーマルクエスト押下時
    /// </summary>
    public void TapNormalQuestButton()
    {
        questListPanel.SetActive(true);
    }


    /// <summary>
    /// クエスト一覧からクエスト押下時
    /// </summary>
    public void TapQuest()
    {
        detailTemplate.SetActive(true);
    }

    /// <summary>
    /// クエスト受けるボタン押下時
    /// </summary>
    public void TapOKButton()
    {
        canvasChara.SetActive(true);
        charaListPanel.SetActive(true);
        bulkOrgBg.SetActive(true);

        OrganizationManager.isGet = true;

        questConfilmPanel.SetActive(true);
        questListPanel.SetActive(false);
    }

    public void CanvasAnActive()
    {

        canvasChara.SetActive(false);
        charaListPanel.SetActive(false);
        bulkOrgBg.SetActive(false);
        homeChara.SetActive(false);
        tapArea.SetActive(false);
    }

    #region キャラ関連

    /// <summary>
    /// キャラボタン押下時に呼び出す処理
    /// </summary>
    public void TapCharaButton()
    {
        questSelectBackButton.interactable = true;
        questSelect.GetComponent<Animator>().SetTrigger("Back");

        // すべてのUIを非表示
        AnActiveAll();

        // キャラのキャンバスを表示
        canvasChara.SetActive(true);
        charaPanel.SetActive(true);
        homeChara.SetActive(false);
        tapArea.SetActive(false);
    }

    /// <summary>
    /// 各キャラ押下時の処理
    /// 編成モードの有無で処理を分岐
    /// </summary>
    public void TapChara()
    {
        // 編成モード中
        if (isOrg)
        {
            Debug.Log("編成モード中");

            // 編成確認ダイアログ表示
            orgConfilmDialog.SetActive(true);
        }

        // 編成モード以外
        if(isList)
        {
            Debug.Log("編成モードじゃない");

            // キャラの詳細表示
            //charaDetailPanel.SetActive(true);
        }

        // 一括編成モード
        /*if (isBulkOrg)
        {
            if (bulkCnt < MAX_ORG) {
                // プレファブを生成
                var temp = Instantiate(charaTemplate, bulkOrgBg.transform);
                temp.tag = "Bulk";
                
                bulkCnt++;
            }

        }*/
    }

    // 一括編成でキャラを追加する処理
    public void AddBulkOrg(GameObject obj)
    {
        if (bulkCnt < MAX_ORG)
        {
            // プレファブを生成
            var temp = Instantiate(charaTemplate, bulkOrgBg.transform);
            temp.tag = "Bulk";
            temp.GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;

            //org[bulkCnt] = temp;
            // 新しいキャラを設定
            org[bulkCnt].GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            
            bulkCnt++;
        }
    }



    /// <summary>
    /// 各キャラが押された場合の処理 長押し判定用
    /// CharaTemplateManagerで、eventtriggerに登録
    /// </summary>
    public void PointerDownChara()
    {
        Debug.Log("押した");
        isDown = true;
    }

    /// <summary>
    /// 各キャラから指が離された場合の処理 長押し判定用
    /// CharaTemplateManagerで、eventtriggerに登録
    /// </summary>
    public void PointerUpChara()
    {
        isDown = false;
        Debug.Log("離した");


    }

    #region 編成関連

    /// <summary>
    /// キャラ編成ボタン押下時処理
    /// </summary>
    public void TapOrganizationButton()
    {
        isList = false;

        // 
        charaPanel.SetActive(false);

        //
        organizationPanel.SetActive(true);

        // 編成フラグオン
        isOrg = true;
    }

    /// <summary>
    /// 編成中のキャラ押下時処理
    /// </summary>
    public void TapOrgChara(GameObject _gameObject)
    {
        

        // タップした編成キャラを取得
        orgChara = _gameObject;

        if (isQuest)
        {
            organizationPanel.SetActive(false);
        }
        else
        {
            // すべてのUIを非表示
            AnActiveAll();
        }

        // 編成フラグオン
        isOrg = true;

        // キャラキャンバスを表示
        canvasChara.SetActive(true);

        // キャラ一覧パネルを表示
        charaListPanel.SetActive(true);
    }

    /// <summary>
    /// 一括編成ボタン押下時処理
    /// </summary>
    public void TapBulkOrgButton()
    {
        // すべてのUIを非表示
        AnActiveAll();

        // 編成フラグオン
        isOrg = false;

        // 一括編成フラグオン
        isBulkOrg = true;

        // キャラキャンバスを表示
        canvasChara.SetActive(true);

        // キャラ一覧パネルを表示
        charaListPanel.SetActive(true);

        // 一括編成を表示
        bulkOrgBg.SetActive(true);
    }

    /// <summary>
    /// 編成確認ダイアログの"はい"ボタン押下時処理
    /// </summary>
    public void TapOrgDialogYesButton()
    {
        // ダイアログ非表示
        orgConfilmDialog.SetActive(false);

        // キャラ一覧パネル非表示
        charaListPanel.SetActive(false);

        // クエストモードか見る
        if (!isQuest) {
            // 編成パネル表示
            organizationPanel.SetActive(true);
        }
        else
        {
            canvasHome.SetActive(true);
            questSelect.SetActive(true);
            questSelect.GetComponent<Animator>().SetTrigger("Idle");
            questPanel.SetActive(true);
            questConfilmPanel.SetActive(true);
            canvasChara.SetActive(false);
        }

        OrganizationManager.isFirst = false;
        OrganizationManager.isGet = true;
    }

    /// <summary>
    /// 編成確認ダイアログの"いいえ"ボタン押下時処理
    /// </summary>
    public void TapOrgDialogNoButton()
    {
        // ダイアログ非表示
        //OrgConfilmDialog.SetActive(false);
    }

    #endregion

    #region 一覧関連

    /// <summary>
    /// キャラ一覧ボタン押下時処理
    /// </summary>
    public void TapCharaListButton()
    {
        isList = true;
        isOrg = false;
        isBulkOrg = false;

        // 
        charaPanel.SetActive(false);

        //
        charaListPanel.SetActive(true);
    }

    /// <summary>
    /// キャラ一覧パネルの戻るボタン押下処理
    /// </summary>
    public void TapCharaListBackButton()
    {
            charaListPanel.SetActive(false);

        if (!isQuest)
        {
            // 編成モード中
            if (isOrg)
            {
                // 編成画面へ戻る
                organizationPanel.SetActive(true);
            }
            // 編成モード以外
            else
            {
                // キャラパネルへ戻る
                charaPanel.SetActive(true);
            }
        }
        else
        {
            canvasChara.SetActive(false);
        }
    }

    #endregion

    #endregion


    #region ガチャ関連

    /// <summary>
    /// ガチャボタン押下時に呼び出す処理
    /// </summary>
    public void TapGachaButton()
    {
        questSelectBackButton.interactable = true;
        questSelect.GetComponent<Animator>().SetTrigger("Back");


        // すべてのUIを非表示
        AnActiveAll();

        // ガチャのキャンバスを表示
        canvasGacha.SetActive(true);
        homeChara.SetActive(false);
        tapArea.SetActive(false);

    }

    #endregion

    #endregion



    void Start()
    {


            // 一括編成の処理
            for (int i = 0; i < MAX_ORG; i++)
        {
            // テンプレートプレファブを生成
            var temp = Instantiate(charaTemplate, bulkOrgBg.transform);

            // 一括編成の見た目も編成と同じにする
            temp.tag = "Bulk";

            // 編成中のキャラ取得
            if (org[i] != null)
            {
                var orgChara = org[i];
                temp.GetComponent<Image>().sprite = orgChara.GetComponent<Image>().sprite;
                bulkCnt++;
            }
            else
            {
                Destroy(temp);
            }

        }
    }


    void Update()
    {
        //Debug.Log("くえすとｂふらｂ　" + isQuest);

        if (isDown)
        {
            pointerDownTimer += Time.deltaTime;
        }
        else
        {
            pointerDownTimer = 0;
        }

        if(pointerDownTimer >= TIME_POINTER_DOWN)
        {
            // キャラの詳細表示
            //charaDetailPanel.SetActive(true);
        }

    }
}
