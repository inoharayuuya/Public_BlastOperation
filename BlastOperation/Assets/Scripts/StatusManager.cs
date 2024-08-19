using System.Collections;
using System.Collections.Generic;
using Const;
using System;
using UnityEngine;
using UnityEngine.UI;

using Json;

public class StatusManager : MonoBehaviour
{
    // ホームに表示するキャラの数
    private const int HOMECHARA_NUMBER = 3;

    // クエスト数
    private const int QUEST_NUMBER = 3;

    //private const string USER_ID = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918";
    //private const string TEST_URL = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";


    // ステータスのテキスト
    [SerializeField] private Text staminaText;
    [SerializeField] private Text crystalText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text rankText;
    [SerializeField] private Text cardRankText;

    // ランクカードのテキスト
    [SerializeField] private InputField rNameText;
    [SerializeField] private InputField rCommentText;

    // ホームのキャラ3体
    [SerializeField] private Image[] homeCharaImages = new Image[HOMECHARA_NUMBER];

    // クエスト
    [SerializeField] private GameObject[] questObjcts = new GameObject[QUEST_NUMBER];

    // クエスト詳細に渡すようの配列
    public string[] questName = new string[QUEST_NUMBER]; 
    public string[] questDetail = new string[QUEST_NUMBER];

    // 所持キャラクター
    [SerializeField] private GameObject hasCharacterView;  // 表示領域
    [SerializeField] private GameObject charaTemplate;     // キャラクターのテンプレート

    // 編成
    [SerializeField] private GameObject orgCharacterView;  // 編成キャラ表示領域
    [SerializeField] private GameObject orgCharaTemplate;  // 編成キャラのテンプレート

    public string jsonHasChara;

    // ローディングパネル
    [SerializeField] private GameObject startLoading;
    [SerializeField] private GameObject loadingSmall;

    /// <summary>
    /// 所持キャラ取得
    /// </summary>
    /// <returns></returns>
    public IEnumerator HasCharaAPI()
    {
        WWWForm form = new WWWForm();

        //var jsonHasChara = JsonUtility.ToJson(jsonStatus.player_info.has_characters);

        // リクエストパラメーターをセット
        form.AddField("user_id", MainData.instance.userId);
        form.AddField("player_info", JsonUtility.ToJson(MainData.instance.playerInfo));

        print("所持キャラUSERID:" + MainData.instance.userId);
        print("所持キャラリクエストプレイヤー情報:" + JsonUtility.ToJson(MainData.instance.playerInfo));


        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_CHARA;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();
        print("キャラ取得API" + res);
        // Jsonデータをシリアライズ (シリアライズはJsonからclass)
        var jsonCharas = JsonUtility.FromJson<JsonCharaData>(res);
        

        // スクロール領域のオブジェクト削除

        // 子オブジェクト数取得
        var count = hasCharacterView.transform.childCount;
        if (count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject child = hasCharacterView.transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        if (jsonCharas.result == Common.SUCCES)
        {
            Debug.Log("成功");

            // 所持キャラクター分ヌルチェック
            for (int i = 0; i < jsonCharas.character_ids.Length; i++)
            {
                // ヌルかチェック 所持キャラのidがキャラマスにあるかどうか見る
                if (jsonCharas.character_ids[i] != 0)
                {
                    for (int j = 0; j < jsonCharas.character_masters.Length; j++)
                    {
                        // キャラマスのj番目がヌルじゃないかチェック
                        if (jsonCharas.character_masters[j] != null)
                        {
                            // 取得したホームキャラのidがキャラマスのidと一致していたらデータを取り出す
                            if (jsonCharas.character_ids[i] == jsonCharas.character_masters[j].id)
                            {
                                for (int k = 0; k < jsonCharas.character_masters.Length; k++)
                                {
                                    if (jsonCharas.character_masters[k] != null)
                                    {
                                        // 取得したホームキャラのidがキャラマスのidと一致していたらデータを取り出す
                                        if (jsonCharas.character_ids[i] == jsonCharas.character_masters[k].id)
                                        {

                                            Debug.Log("はずきゃらid　　" + jsonCharas.character_ids[i]);

                                            // プレファブをスクロール領域に生成
                                            var tmp = Instantiate(charaTemplate, hasCharacterView.transform);

                                            //使用するキャラ画像を取得 
                                            var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + jsonCharas.character_masters[k].name_id);
                                            // sprite変更
                                            tmp.GetComponent<Image>().sprite = charaSprite;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            yield break;
        }
        else
        {
            Debug.Log("失敗");
        }

    }

    /// <summary>
    /// ステータス取得
    /// </summary>
    /// <returns></returns>
    public IEnumerator StatusAPI()
    {
        //yield return null;
        // パラメータ設定
        WWWForm form = new WWWForm();

        // リクエストパラメーターをセット
        form.AddField("user_id", MainData.instance.userId);

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_GET_MAIN_DATA;

        print(url + "で通信開始");
        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();
        // レスポンスを表示
        //print(res);


        // デバッグ用
        //var res = (Resources.Load("GachaJsonOnce",typeof(TextAsset))as TextAsset).text;
        //var res = (Resources.Load("StatusJson", typeof(TextAsset)) as TextAsset).text;
        // Jsonデータをシリアライズ (シリアライズはJsonからclass)
        var jsonStatus = JsonUtility.FromJson<JsonMainData>(res);

        jsonHasChara = JsonUtility.ToJson(jsonStatus.player_info);



        // 通信成功時処理
        if (jsonStatus.result == Common.SUCCES)
        {
            OrganizationManager.isGet = true;
            OrganizationManager.isFirst = true;  // trueが正しい

            print("通信成功");
            print("コメント  " + jsonStatus.player_info.comment);

            // ステータスを表示
            staminaText.text = (jsonStatus.player_info.energy).ToString() + "ノ" + (jsonStatus.player_info.energy).ToString();
            crystalText.text = (jsonStatus.player_info.crystal).ToString();
            nameText.text = jsonStatus.player_info.name;
            rankText.text = jsonStatus.player_info.rank;
            cardRankText.text = jsonStatus.player_info.rank;

            // ギルドカードのテキスト更新
            rNameText.text = jsonStatus.player_info.name;
            rCommentText.text = jsonStatus.player_info.comment;

            // パーティIDの取得
            var partyId = jsonStatus.player_info.party_id;

            // ホームのキャラ数分ヌルチェック TODO
            for (int i = 0; i < HOMECHARA_NUMBER; i++)
            {
                // ヌルかチェック ホームキャラのidがキャラマスにあるかどうか見る
                if (jsonStatus.player_info.party_infos[partyId].box_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // キャラマスのj番目がヌルじゃないかチェック
                        if (jsonStatus.character_masters[j] != null)
                        {
                            //TODO　↓の条件を通信して取得してきたidと比較に変更

                            // 取得したホームキャラのidがキャラマスのidと一致していたらデータを取り出す
                            if (jsonStatus.player_info.party_infos[partyId].box_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                //使用するキャラ画像を取得 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Body/" + jsonStatus.character_masters[j].name_id);
                                // sprite変更
                                homeCharaImages[i].GetComponent<Image>().sprite = charaSprite;
                            }
                        }
                    }
                }
            }

            // 編成のキャラ数分ヌルチェック
            for (int i = 0; i < jsonStatus.player_info.party_infos[0].box_ids.Length; i++)
            {
                Destroy(OrganizationManager.orgChara[i]);

                // ヌルかチェック 編成キャラのidがキャラマスにあるかどうか見る
                if (jsonStatus.player_info.party_infos[partyId].box_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // キャラマスのj番目がヌルじゃないかチェック
                        if (jsonStatus.character_masters[j] != null)
                        {
                            // 取得した編成キャラのidがキャラマスのidと一致していたらデータを取り出す
                            if (jsonStatus.player_info.party_infos[partyId].box_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                // プレファブをスクロール領域に生成
                                var tmp = Instantiate(orgCharaTemplate, orgCharacterView.transform);

                                //使用するキャラ画像を取得 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + jsonStatus.character_masters[j].name_id);
                                // sprite変更
                                tmp.GetComponent<Image>().sprite = charaSprite;
                                OrganizationManager.orgChara[i] = tmp;
                            }
                        }
                    }
                }
            }

            // 所持キャラクター分ヌルチェック
            /*for (int i = 0; i < jsonStatus.player_info.has_character_ids.Length; i++)
            {
                // ヌルかチェック 所持キャラのidがキャラマスにあるかどうか見る
                if (jsonStatus.player_info.has_character_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // キャラマスのj番目がヌルじゃないかチェック
                        if (jsonStatus.character_masters[j] != null)
                        {
                            // 取得したホームキャラのidがキャラマスのidと一致していたらデータを取り出す
                            if (jsonStatus.player_info.has_character_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                Debug.Log("はずきゃらid　　" + jsonStatus.player_info.has_character_ids[i]);

                                // プレファブをスクロール領域に生成
                                var tmp = Instantiate(charaTemplate, hasCharacterView.transform);

                                //使用するキャラ画像を取得 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Body/" + jsonStatus.character_masters[j].name_id);
                                // sprite変更
                                tmp.GetComponent<Image>().sprite = charaSprite;
                            }
                        }
                    }
                }
            }*/

            // クエスト情報更新
            for (int i = 0; i < QUEST_NUMBER; i++)
            {
                // クエスト名変更
                var questNameText = questObjcts[i].transform.Find("QuestNameText").GetComponent<Text>();
                questNameText.text = jsonStatus.stage_masters[i].name;

                // クエスト詳細に渡すようの配列に格納
                questName[i] = jsonStatus.stage_masters[i].name;
                questDetail[i] = jsonStatus.stage_masters[i].note;

                // クリアしていたら画像表示
                if (jsonStatus.player_info.progress_rates[i].is_clear)
                {
                    GameObject clearImage = questObjcts[i].transform.Find("ClearImage").gameObject;
                    clearImage.SetActive(true);
                }
            }
        }
        else
        {
            print("情報取得しっぱい" + res);
            yield break;
        }
    }

    public void TestButton()
    {
        // 通信開始
        StartCoroutine(StatusAPI());
    }

    public void TapCharaListButton()
    {
        loadingSmall.SetActive(true);
        // 通信開始
        StartCoroutine(HasCharaAPI());
        loadingSmall.SetActive(false);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        startLoading.SetActive(true);
        yield return StartCoroutine(StatusAPI());
        startLoading.GetComponent<Animator>().SetTrigger("Out");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

}
