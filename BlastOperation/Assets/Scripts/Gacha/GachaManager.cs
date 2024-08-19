using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Const;
using Json;
using System.Linq;

public class GachaManager : MonoBehaviour
{
    // 画面の幅、高さ
    private const int SCREEN_WIDTH  = 360;
    private const int SCREEN_HEIGHT = 720;

    // 1連、10連
    private const int GACHA_NUMBER_ONCE = 0;
    private const int GACHA_NUMBER_TEN  = 1;

    private const int MAX_CHARA_NUMBER = 10;
    private const int HASCHARA_NUMBER = 30;
    private const int MAX_WEAPON = 2;
    private const float WAIT_TIME = 0.5f;

    // ランクによって色を変更する時の色の数
    private const int RANK_SPRITES_NUMBER = 3;
    // 金、銀、銅の配列番号
    private const int RANK_SPRITES_COPPER = 0;
    private const int RANK_SPRITES_SILVER = 1;
    private const int RANK_SPRITES_GOLD   = 2;

    // キャラ表示に使用する定数
    private const int CHARA_WIDTH = 100;      // キャラ画像の幅
    private const int POSITION_PLUS_X = 130;  // 表示するx座標変更用
    private const int POSITION_PLUS_Y = -100; // 表示するy座標変更用

    // デバッグ用
    //private const string USER_ID = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918";
    //private const string TEST_URL = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";

    // ガチャのキャラクターテンプレート
    [SerializeField] private GameObject gachaCharaTemplate;

    // 西田追記.クリスタル書き換え用
    [SerializeField] private Text crystalText;

    // 表示領域
    [SerializeField] private GameObject showArea;

    // ランク用のスプライト
    [SerializeField] private Sprite[] rankSprites = new Sprite[RANK_SPRITES_NUMBER];

    // キャラ表示座標用
    private Vector3 pos;

    // 通信成功フラグ
    private bool isSucces;
    // ガチャ演出スタートフラグ
    static public bool isStart;
    // ガチャ演出終了フラグ
    private bool isEnd;
    // ガチャ演出ターンスタートフラグ
    private bool isTurnStart;
    // 何回回すか(1連か10連)かのフラグ
    private int gachaNumber;

    // ガチャで獲得したキャラ(テンプレート)を格納する用の配列
    private GameObject[] getCharas = new GameObject[MAX_CHARA_NUMBER];

    // ガチャ終了ボタン
    [SerializeField] private GameObject exitButton;

    // ガチャ演出パネル
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject gachaAnimPanel;
    [SerializeField] private GameObject tapArea;
    [SerializeField] private GameObject tapText;
    [SerializeField] private GameObject moneyImage;

    // ローディング
    [SerializeField] private GameObject loadingSmall;

    // 10連か同課のフラグ
    private bool isSingle;

    /// <summary>
    /// 一連ボタン押下時処理
    /// </summary>
    public void TapOnceButton()
    {
        // ガチャの回数は1回
        gachaNumber = GACHA_NUMBER_ONCE;

        // 通信開始
        StartCoroutine(GachaAPI());
    }

    /// <summary>
    /// 十連ボタン押下時処理
    /// </summary>
    public void TapTenButton()
    {
        // ガチャの回数は10回
        gachaNumber = GACHA_NUMBER_TEN;

        loadingSmall.SetActive(true);
        // 通信開始
        StartCoroutine(GachaAPI());
        loadingSmall.SetActive(false);
    }

    /// <summary>
    /// ガチャキャラのプレファブを生成する処理
    /// </summary>
    private void CreatePrefab(int _i,int _j,JsonGachaData _jsonGacha)
    {
        // 1連の場合は生成するx座標を中央に変更
        if(gachaNumber == GACHA_NUMBER_ONCE)
        {
            pos.x = SCREEN_WIDTH - CHARA_WIDTH;
        }

        // プレファブ生成
        var chara = Instantiate(gachaCharaTemplate, new Vector3(pos.x + (CHARA_WIDTH), pos.y, 0), Quaternion.identity, showArea.transform);

        // 子オブジェクト(ランクの色のImage)を取得
        var child = chara.transform.GetChild(0);

        // ランクごとに色のImageを変更
        switch (_jsonGacha.character_masters[_j].rank)
        {
            // 銅
            case "F":
            case "B":
                // スプライトを取得し、変更
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_COPPER];
                break;

            // 銀
            case "A":
                // スプライトを取得し、変更
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_SILVER];
                break;

            // 金
            case "S":
                // スプライトを取得し、変更
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_GOLD];
                break;

        }

        //使用するキャラ画像を取得 
        var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + _jsonGacha.character_masters[_j].name_id);
        // キャラの画像を変更
        chara.GetComponent<Image>().sprite = charaSprite;

        // 次は右隣りに生成するためx座標をプラス
        pos.x += POSITION_PLUS_X;

        // 半分までいったらy座標変更
        if (_i == 4)
        {
            // xは左端、yは少し下に
            pos.x = 0;
            pos.y = SCREEN_HEIGHT + POSITION_PLUS_Y;
        }

        // 獲得キャラ配列に格納
        getCharas[_i] = chara;
    }


    public IEnumerator GachaAPI()
    {
        // パラメータ設定
        WWWForm form = new WWWForm();

        isSingle = true;

        // リクエストパラメーターをセット
        form.AddField("user_id", MainData.instance.userId);
        form.AddField("is_single", Convert.ToInt32(isSingle).ToString());

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_GACHA;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();
        // レスポンスを表示
        //print(res);
        
        

        // デバッグ用
        //var res = (Resources.Load("GachaJsonOnce",typeof(TextAsset))as TextAsset).text;
        //var res = (Resources.Load("GachaJson",typeof(TextAsset))as TextAsset).text;
        // Jsonデータをシリアライズ (シリアライズはJsonからclass)
        var jsonGacha = JsonUtility.FromJson<JsonGachaData>(res);

        // 通信成功時処理
        if (jsonGacha.result == Common.SUCCES)
        {
            isSucces = true;
            // 情報を書き換える
            MainData.instance.playerInfo.crystal = jsonGacha.player_info.crystal;
            
            // UIに反映
            crystalText.text = (jsonGacha.player_info.crystal).ToString();

            print("通信成功");

            // ガチャ演出開始
            gachaPanel.SetActive(true);
            tapArea.SetActive(true);
            tapText.SetActive(true);
            //moneyImage.SetActive(false);

            // ガチャで引いた数分ヌルチェック　10連か1連か見る
            for (int i = 0; i < jsonGacha.character_ids.Length; i++)
            {
                // ヌルかチェック 引いたのキャラのidがキャラマスにあるかどうか見る
                if (jsonGacha.character_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonGacha.character_masters.Length; j++)
                    {
                        // キャラマスのj番目がヌルじゃないかチェック
                        if (jsonGacha.character_masters[j] != null)
                        {
                            // ガチャでゲットしたキャラのidがキャラマスのidと一致していたらデータを取り出す
                            if (jsonGacha.character_ids[i] == jsonGacha.character_masters[j].id)
                            {
                                // プレファブ生成処理
                                CreatePrefab(i,j,jsonGacha);

                                print(i + "体目  　name_id : " + jsonGacha.character_masters[j].name_id + "　　ランク : " + jsonGacha.character_masters[j].rank);
                            }
                        }
                    }
                }
            }


            // プレイヤー情報の書き換え
            // 追加キャラ分のループ

            // 所持キャラと追加キャラをリストに変換

            var hasCharaList = MainData.instance.playerInfo.has_character_ids.ToList();
            var addCharaList = jsonGacha.player_info.has_character_ids.ToList();

            // 所持キャラを更新
            foreach(var addChara in addCharaList)
            {
                hasCharaList.Add(addChara);
            }

            // 所持制限を超過すると切り捨て
            if(hasCharaList.Count > Common.MAX_HAS_CHARA_NUMBER)
            {
                hasCharaList = hasCharaList.GetRange(0, Common.MAX_HAS_CHARA_NUMBER);
            }

            // プレイヤー情報の書き換え
            MainData.instance.playerInfo.has_character_ids = hasCharaList.ToArray();

            yield break;
        }


    }

    /// <summary>
    /// ガチャ演出処理
    /// </summary>
    public IEnumerator GachaAnimation()
    {
        // ガチャ演出開始時
        if (isStart)
        {

            // 何連か
            if (gachaNumber == GACHA_NUMBER_ONCE)
            {
                // 1連の処理
                // WAIT_TIME秒待った後、次の処理を実行
                yield return new WaitForSeconds(WAIT_TIME);

                getCharas[0].SetActive(true);

                // フラグ初期化
                isStart = false;

                // ひっくり返す演出スタート
                isTurnStart = true;
            }
            else
            {
                // 10連の処理
                for (int i = 0; i < MAX_CHARA_NUMBER; i++)
                {
                    // WAIT_TIME秒待った後、次の処理を実行
                    yield return new WaitForSeconds(WAIT_TIME);

                    getCharas[i].SetActive(true);
                    //getCharas[i].GetComponent<Animator>().SetTrigger("Show");

                    // 半分になったらアニメーション切り替え
                    if (i >= MAX_CHARA_NUMBER/2)
                    {
                        // アニメーター取得
                        var animator = getCharas[i].GetComponent<Animator>();
                        animator.SetBool("isHalf", true);
                    }

                    if (i == MAX_CHARA_NUMBER-1)
                    {
                        // フラグ初期化
                        isStart = false;

                        // ひっくり返す演出スタート
                        isTurnStart = true;

                        // アニメーション初期化
                        //animator.SetBool("isHalf", false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// ガチャ演出ターン処理
    /// </summary>
    public IEnumerator GachaAnimationTurn()
    {
        // ガチャ演出開始時
        if (isTurnStart)
        {
            // 何連か
            if (gachaNumber == GACHA_NUMBER_ONCE)
            {
                // 1連の処理
                // WAIT_TIME秒待った後、次の処理を実行
                yield return new WaitForSeconds(WAIT_TIME);

                // 色透明にする処理
                var child = getCharas[0].transform.GetChild(0);
                child.GetComponent<Image>().color = new Color(255, 255, 255, 0);

                // フラグ初期化
                isTurnStart = false;

                // 演出終了フラグ
                isEnd = true;
            }
            else
            {
                // 10連の処理
                for (int i = 0; i < MAX_CHARA_NUMBER; i++)
                {
                    // WAIT_TIME秒待った後、次の処理を実行
                    yield return new WaitForSeconds(WAIT_TIME);

                    // 色透明にする処理
                    //var child = getCharas[i].transform.GetChild(0);
                    //child.GetComponent<Image>().color=new Color(255,255,255,0);

                    // アニメーター取得
                    if (getCharas[i] != null)
                    {
                        var animator = getCharas[i].GetComponent<Animator>();
                        animator.SetTrigger("Turn");
                    }


                    if (i == MAX_CHARA_NUMBER - 1)
                    {
                        // フラグ初期化
                        isTurnStart = false;

                        


                    }
                }
                // 演出終了フラグ
                isEnd = true;
            }
        }
    }

    /// <summary>
    /// ガチャ画面で画面タップ時処理
    /// </summary>
    public void TapGachaScreen()
    {
        gachaAnimPanel.GetComponent<Animator>().SetTrigger("Show");
        tapText.SetActive(false);

        //isStart = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 画像の表示位置用
        pos.x= 0;
        pos.y = SCREEN_HEIGHT + CHARA_WIDTH;

    }

    void Update()
    {
        // ガチャ演出スタート
        StartCoroutine(GachaAnimation());

        // ガチャ演出ターン
        StartCoroutine(GachaAnimationTurn());

        // ガチャ演出終了後処理
        if (isEnd)
        {
            // ボタン表示
            exitButton.SetActive(true);

        }

    }

    /// <summary>
    /// ガチャ終了ボタン押下時処理
    /// </summary>
    public void TapExitButton()
    {
        gachaPanel.SetActive(false);

        if (gachaNumber == GACHA_NUMBER_ONCE)
        {
            Destroy(showArea.transform.GetChild(0).gameObject);

        }
        else
        {
            // showAreaの子オブジェクト削除
            for (int i = MAX_CHARA_NUMBER; i > 0; i--)
            {
                Destroy(showArea.transform.GetChild(i - 1).gameObject);
            }
        }
        isEnd = false;

        // 画像の表示位置初期化
        pos.x = 0;
        pos.y = SCREEN_HEIGHT + CHARA_WIDTH;
    }

    

}
