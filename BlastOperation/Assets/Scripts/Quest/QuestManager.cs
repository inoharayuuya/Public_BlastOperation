using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Const;
using Json;

public class QuestManager : MonoBehaviour
{
    #region テストデータ


    #endregion

    #region 列挙体

    /// <summary>
    /// 武器の番号
    /// </summary>
    public enum WEAPON
    {
        WEAPON_A,
        WEAPON_B
    }

    /// <summary>
    /// ゲーム中のフェーズ
    /// </summary>
    public enum GameFase
    {
        /// <summary>初期化処理</summary>
        GAME_INIT,


        /// <summary>フロア開始時</summary>
        FLOOR_START,
        /// <summary>カメラ移動準備</summary>
        CAMERA_MOVE_START,
        /// <summary>カメラ移動中</summary>
        CAMERA_MOVING,
        /// <summary>カメラ移動終了</summary>
        CAMERA_MOVE_END,
        /// <summary>ゲーム中</summary>
        GAMING,


        /// <summary>ゲームクリア後、入力でリザルトに遷移</summary>
        GAME_CLEAR,
        /// <summary>ゲームオーバー後、入力でリザルトに遷移</summary>
        GAME_OVER,


        /// <summary>リザルト表示開始、通信処理</summary>
        RESULT_START,
    }

    #endregion

    #region 変数

    [Tooltip("武器画像のパス")]
    private const string PATH_WEAPON_ICON = "Images/Icons/Weapon/";
    [Tooltip("キャラアイコンのパス")]
    private const string PATH_CHARACTER_ICON = "Images/Characters/Face/";
    [Tooltip("敵画像のパス")]
    private const string PATH_ENEMY_BODY = "Images/Characters/Body/";


    [Tooltip("パーティーの情報")]
    private JsonCharacterMaster[] partyCharas = new JsonCharacterMaster[PARTY_LIMIT];
    [Tooltip("シリアライズ後のクエストデータ")]
    private JsonQuestData questData;



    [Header("クエスト情報系")]
    [Space(10)]


    [Tooltip("敵の親オブジェクト")]
    private GameObject enemyParent;
    [Tooltip("1ターン中のヒット数")]
    private int hitCount;
    [Tooltip("経過ターン数")]
    private int turnCount;
    [Tooltip("現在のフロア数")]
    private int floorCount;
    [Tooltip("現在のゲーム状態")]
    public GameFase Fase { get; private set; }
    [Tooltip("敵プレハブ")]
    private GameObject enemyPrefab;
    [Tooltip("敵の攻撃オブジェクトの親")]
    private Transform enemyAttackObjParent;
    [Tooltip("フロアボスを倒したかどうか")]
    private bool isBossDestroyed;

    [Tooltip("初期化が終了したかどうか")]
    private bool isInitEnd;
    [Tooltip("初期化終了遅延")]
    private const float DELAY_INIT_END = 1f;



    [Header("UI系")]
    [Space(10)]

    [Tooltip("UIの入っているキャンバス"), SerializeField]
    private GameObject canvasUI;
    [Tooltip("攻撃方法のUI"), SerializeField]
    private GameObject attackUI;
    [Tooltip("ヒット数のUI"), SerializeField]
    private Text hitCntUI;
    [Tooltip("フロア数のUI"), SerializeField]
    private Text floorCntUI;
    [Tooltip("攻撃方法ボタンの配列"), SerializeField]
    private GameObject[] attackButton;
    [Tooltip("カメラに表示されるオブジェクトの親")]
    private GameObject allObjectParent;
    [Tooltip("プレイヤーのオブジェクト")]
    private Player player;
    [Tooltip("UI表示用のタイマー")]
    private float timerUI;
    [Tooltip("タップし続けたらUIが消える時間")]
    private const float LIMIT_UI_BLINK = 0.5f;
    [Tooltip("フェードイン用のImage"), SerializeField]
    private Image fadeInImage;
    [Tooltip("クエスト名"), SerializeField]
    private Text questName;
    [Tooltip("クエストのフロア数"), SerializeField]
    private Text questFloorNum;
    [Tooltip("メニューパネル"), SerializeField]
    private GameObject menuPanel;
    [Tooltip("メニューボタン"), SerializeField]
    private GameObject menuButton;
    [Tooltip("ポーズ中")]
    public bool isPause {  get; private set; }



    [Header("リザルト用")]
    [Space(10)]

    [Tooltip("リザルト用のパネル"), SerializeField]
    private GameObject resultPanel;
    [Tooltip("リザルトに進むボタン用のパネル"), SerializeField]
    private GameObject toResultPanel;
    [Tooltip("リザルトのボタン"), SerializeField]
    private GameObject toResultButton;
    [Tooltip("リザルトのテキスト"), SerializeField]
    private Text toResultText;
    [Tooltip("獲得したポイントテキスト"), SerializeField]
    private Text getRankPointText;
    [Tooltip("ランクテキスト"), SerializeField]
    private Text rankText;
    [Tooltip("次のランクまでのポイントテキスト"), SerializeField]
    private Text nextRankPointText;
    [Tooltip("報酬のパネル"), SerializeField]
    private GameObject rewardsPanel;
    [Tooltip("メイン画面に戻るボタン"), SerializeField]
    private GameObject toMainButton;
    [Tooltip("リザルト表示時のディレイ")]
    private const float DELAY_DISPLAY_RESULT = 0.3f;
    [Tooltip("初回クリアの石"), SerializeField]
    private GameObject crystalObj;
    [Tooltip("初回クリア報酬なし"), SerializeField]
    private GameObject emptyObj;
    [Tooltip("ドロップアイテム、未実装"), SerializeField]
    private GameObject dropPanel;
    [Tooltip("リザルトスキップ")]
    private bool isResultTap;




    [Header("パーティー情報系")]
    [Space(10)]

    [Tooltip("前ターンに選ばれたチームキャラ全部の攻撃方法"), SerializeField]
    private WEAPON[] selectedWeapons;

    [Tooltip("パーティーの最大総HP")]
    private int partyMaxHp;
    [Tooltip("パーティーの現在の総HP")]
    private int partyCurrentHp;
    [Tooltip("現在選択中のキャラを表示する矢印オブジェクト")]
    private GameObject choiceArrowObj;
    [Tooltip("パーティーのキャラ画像表示用配列")]
    private Image[] charaImages = new Image[PARTY_LIMIT];
    [Tooltip("パーティーの最大数")]
    private const int PARTY_LIMIT = 4;
    [Tooltip("プレイヤーの現在HPゲージ"), SerializeField]
    private Image playerCurrentHpGauge;
    [Tooltip("プレイヤーのダメージHPゲージ"), SerializeField]
    private Image playerDamageHpGauge;
    [Tooltip("HPのテキスト"), SerializeField]
    private Text playerHpText;



    [Header("カメラ移動系")]
    [Space(10)]
    [Tooltip("メインカメラ")]
    private Camera cameraMain;
    [Tooltip("カメラ初期状態の座標")]
    private readonly Vector3 START_CAMERA_POS = new Vector3(0, 0, -10);
    [Tooltip("カメラゲーム中の座標")]
    private readonly Vector3 MAIN_CAMERA_POS = new Vector3(0, 10, -10);
    [Tooltip("カメラの移動時間")]
    private const float CAM_MOVE_TIME = 0.75f;
    [Tooltip("カメラ移動が停止する閾値")]
    private const float CAM_MOVE_LIMIT = 0.1f;
    [Tooltip("壁の親オブジェクト")]
    private GameObject wallParentObj;
    [Tooltip("壁の最大サイズ")]
    private readonly Vector3 WALL_SIZE_MAX = new Vector3(1.3f, 1.3f, 1.3f);
    [Tooltip("壁の最小サイズ")]
    private readonly Vector3 WALL_SIZE_MIN = new Vector3(1f, 1f, 1f);
    [Tooltip("壁移動を終了する閾値")]
    private const float WALL_SIZE_DIFF_LIMIT = 0.1f;


    #endregion

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Time.timeScale = 1;
        Time.fixedDeltaTime = 0.02f;
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        isInitEnd = false;
        yield return StartCoroutine(Init());
    }

    private void Update()
    {
        if (isInitEnd && !isPause)
        {
            // メインの処理
            StartCoroutine(FaseControll());
        }

    }


    /// <summary>
    /// 初期化処理
    /// </summary>
    private IEnumerator Init()
    {
        // 通信開始前の黒画面
        fadeInImage.gameObject.SetActive(true);
        questName.gameObject.SetActive(false);
        questFloorNum.gameObject.SetActive(false);
        isPause = false;

        // クエスト開始時の通信処理
        yield return StartCoroutine(QuestStart());

        // スタミナを減らす
        MainData.instance.playerInfo.energy -= questData.stage_master.use_energy;


        // プレイヤーの取得
        player = GameObject.Find("PlayerBall").GetComponent<Player>();
        // 敵の親を取得
        enemyParent = GameObject.Find("Enemies");
        // 非表示
        enemyParent.SetActive(false);

        // カメラの取得
        cameraMain = Camera.main;


        // ゲームの状態の初期化
        Fase = GameFase.GAME_INIT;
        QuestInfoDisplay(questData.stage_master);

        // 敵の初期化
        QuestEnemyInit(questData.stage_master);


        // クエスト情報の初期化
        hitCount = 0;
        turnCount = 1;
        floorCount = 1;



        choiceArrowObj = GameObject.Find("ChoiceArrow");

        // パーティーの初期化
        PartyInit();
        // 矢印の位置を編成の1体目にする
        StartCoroutine(Common.Delay(1, PartyArrowDisplay));

        // 攻撃方法の初期化
        selectedWeapons = new WEAPON[Common.PARTY_LIMIT];
        AttackWeaponEnable((int)WEAPON.WEAPON_A);



        // オブジェクトの親取得

        allObjectParent = GameObject.Find("AllObjectParent");
        wallParentObj = GameObject.Find("Walls");
        wallParentObj.transform.localScale = WALL_SIZE_MAX;



        // リザルトの初期化
        toResultPanel.SetActive(false);
        resultPanel.SetActive(false);
        toMainButton.SetActive(false);
        getRankPointText.gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);
        nextRankPointText.gameObject.SetActive(false);
        rewardsPanel.SetActive(false);
        toMainButton.SetActive(false);
        isResultTap = false;


        // フェードイン用のImage初期化
        fadeInImage.color = Color.black;
        fadeInImage.gameObject.SetActive(true);

        // フロア数表示
        FloorCountDisplay();

        // 味方のHPを設定
        for (int i = 0; i < partyCharas.Length; i++)
        {
            if (partyCharas[i] != null)
            {
                partyMaxHp += partyCharas[i].hp;
            }
        }
        partyCurrentHp = partyMaxHp;

        playerHpText.text = partyCurrentHp + " / " + partyMaxHp;


        Invoke("GameInitEnd", DELAY_INIT_END);

        yield return null;
    }


    /// <summary>
    /// クエスト開始の通信処理
    /// </summary>
    private IEnumerator QuestStart()
    {
        // パラメータ設定
        WWWForm form = new WWWForm();
        // リクエストパラメーターをセット
        form.AddField("user_id", PlayerPrefs.GetString(Common.KEY_USER_ID));
        form.AddField("stage_id", PlayerPrefs.GetInt(Common.KEY_STAGE_ID));
        form.AddField("party_id", PlayerPrefs.GetInt(Common.KEY_PARTY_ID));

        // URLの設定
        var url = MainData.instance.apiUrl + Common.EP_GET_STAGE;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス取り出し
        var res = coroutine.Current.ToString();

        // データのシリアライズ
        questData = JsonUtility.FromJson<JsonQuestData>(res);

        // 通信に成功していたら処理続行
        if (questData.result == Common.SUCCES)
        {
            Fase = GameFase.FLOOR_START;
            // パーティーのキャラ情報を入れる
            partyCharas = questData.character_masters;

            questName.gameObject.SetActive(true);
            questFloorNum.gameObject.SetActive(false);

            yield return null;
        }
        else
        {
            print("通信エラー:" + questData.message);
            yield return null;
        }
    }


    /// <summary>
    /// ゲーム開始時の敵オブジェクト初期化処理
    /// クエストの敵の最大数で敵を最初に生成、プールしておく
    /// </summary>
    private void QuestEnemyInit(JsonStageMaster _stage)
    {
        // 敵プレハブの読み込み
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");

        // 全フロアの敵の最大数を計算

        var floorMax = _stage.max_floor;

        // 何フロア目の敵が最大数かを調べるための配列
        var enemiesCnt = new int[floorMax];

        int i;
        // 敵の最大数を数える、どこのフロアが最大数か調べる
        // クエストの敵の総数だけループ
        for (i = 0; i < _stage.enemy_infos.Length; i++)
        {
            // nullチェック
            if (_stage.enemy_infos[i] != null)
            {
                // その敵はどのフロアに出るか
                var index = _stage.enemy_infos[i].floor - 1;
                // その敵が出るフロア番目を加算
                enemiesCnt[index]++;
            }


        }
        // 数え終わったので最大数を算出
        int max = 0;
        for (i = 0; i < enemiesCnt.Length; i++)
        {
            if (enemiesCnt[i] > max)
            {
                max = enemiesCnt[i];
            }
        }

        // クエストで使う最大の敵の数がわかったので敵オブジェクトを生成
        for (i = 0; i < max; i++)
        {
            var obj = Instantiate(enemyPrefab, enemyParent.transform);
        }
    }

    /// <summary>
    /// 初期化処理終了
    /// </summary>
    private void GameInitEnd()
    {
        isInitEnd = true;
        Fase = GameFase.FLOOR_START;
    }

    /// <summary>
    /// faseを判定してゲーム中のフェーズを制御
    /// </summary>
    private IEnumerator FaseControll()
    {
        switch (Fase)
        {
            case GameFase.GAME_INIT:
                break;
            case GameFase.FLOOR_START:
                AttackUIEnable(false);
                HitCountUIEneble(false);
                InitCamera(MAIN_CAMERA_POS);
                break;
            case GameFase.CAMERA_MOVE_START:
                // カメラ移動準備、壁を画面外に移動
                WallsMove();
                break;
            case GameFase.CAMERA_MOVING:
                // カメラが移動
                AttackUIEnable(false);
                HitCountUIEneble(false);
                CameraMove(MAIN_CAMERA_POS);
                break;
            case GameFase.CAMERA_MOVE_END:
                // カメラ移動準備、壁を画面内に移動
                WallsMove();
                break;
            case GameFase.GAMING:
                // 敵と味方のフェーズ分け
                GameUIDisplay();
                break;
            case GameFase.GAME_CLEAR:
            case GameFase.GAME_OVER:
                break;
            case GameFase.RESULT_START:
                if (Input.GetMouseButtonDown(0))
                {
                    isResultTap = true;
                }
                break;
        }
        yield return null;
    }

    /// <summary>
    /// ゲーム開始時の情報表示
    /// </summary>
    private void QuestInfoDisplay(JsonStageMaster _stage)
    {
        questName.gameObject.SetActive(true);
        questFloorNum.gameObject.SetActive(true);

        questName.text = _stage.name;
        questFloorNum.text = "フロア数：" + _stage.max_floor.ToString();
    }



    /// <summary>
    /// UIの表示を制御
    /// </summary>
    private void GameUIDisplay()
    {
        // タップ中は一定時間で非表示
        if (player.isTap)
        {
            timerUI += Time.deltaTime;
            if (timerUI > LIMIT_UI_BLINK)
            {
                AttackUIEnable(false);
            }
        }
        // 非タップ時はタイマーが0
        else
        {
            AttackUIEnable(true);
            timerUI = 0;
        }


        // 待機中はHPゲージを更新する
        if (player.state == Player.PlayerState.WAIT_INPUT || player.state == Player.PlayerState.MOVE)
        {
            HpGaugeSync();
        }
        // 発射中は無条件で非表示
        if (player.state == Player.PlayerState.MOVE || player.state == Player.PlayerState.WAIT_ENEMY)
        {
            AttackUIEnable(false);
        }
    }

    /// <summary>
    /// 矢印の表示、経過ターン数を4で割った余りのindexを指定
    /// </summary>
    private void PartyArrowDisplay()
    {
        int index = GetPartyIndex();
        var charaImgPos = charaImages[index].gameObject.transform.position;
        var arrowPos = choiceArrowObj.transform.position;
        arrowPos.x = charaImgPos.x;
        choiceArrowObj.transform.position = arrowPos;
    }

    /// <summary>
    /// 現在の経過ターン数からパーティーのインデックスを計算し返す
    /// </summary>
    /// <returns></returns>
    private int GetPartyIndex()
    {
        return (turnCount - 1) % 4;
    }

    /// <summary>
    /// フロア数の表示処理
    /// </summary>
    private void FloorCountDisplay()
    {
        floorCntUI.text = "フロア数" + floorCount.ToString() + " / " + questData.stage_master.max_floor;
    }

    /// <summary>
    /// 現在インスタンスが存在する敵の取得
    /// </summary>
    /// <returns></returns>
    public InstanceEnemy[] GetInstanceEnemys()
    {
        return enemyParent.GetComponentsInChildren<InstanceEnemy>();
    }

    /// <summary>
    /// プレイヤーの移動終了時の処理
    /// </summary>
    public void PlayerTurnEnd()
    {
        // ヒット数の初期化
        hitCount = 0;
        hitCntUI.text = hitCount.ToString();
        // ターン数を進める
        turnCount++;
    }

    /// <summary>
    /// プレイヤーボールの情報を更新する処理
    /// </summary>
    public void PlayerInfoUpdate()
    {
        // 次のキャラクターを選択させる
        PartyArrowDisplay();
        // 武器情報の表示
        AttackWeaponEnable((int)selectedWeapons[GetPartyIndex()]);
        // プレイヤーの状態更新
        player.PlayerTurnStart();
    }

    /// <summary>
    /// 攻撃方法の選択
    /// 引数はintだが、Weapon型をキャストした0or1の値
    /// </summary>
    public void AttackWeaponEnable(int _index)
    {
        // 一旦オフの状態で初期化
        for (int i = 0; i < attackButton.Length; i++)
        {
            attackButton[i].transform.localScale = Vector3.one;
            attackButton[i].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            attackButton[i].transform.GetChild(1).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        }

        // 指定されたidでUIを変更
        // 大きくする
        var ls = attackButton[_index].transform.localScale;
        ls *= 1.2f;
        attackButton[_index].transform.localScale = ls;
        // グレーアウトを外す
        attackButton[_index].GetComponent<Image>().color = Color.white;
        attackButton[_index].transform.GetChild(1).GetComponent<Image>().color = Color.white;


        // 攻撃データの更新
        // キャラデータのためのインデックス取得
        var partyIndex = GetPartyIndex();
        // 武器IDの取得
        var weaponId = partyCharas[partyIndex].weapon_ids[_index];
        // 武器データの取得
        JsonWeaponMaster weaponData = new JsonWeaponMaster();
        for (int i = 0; i < questData.weapon_masters.Length; i++)
        {
            if (weaponId == questData.weapon_masters[i].id)
            {
                weaponData = questData.weapon_masters[i];
            }
        }

        // 武器の画像取得
        var path = PATH_WEAPON_ICON + weaponData.name_id;
        // 選んだ情報を保存
        selectedWeapons[partyIndex] = (WEAPON)_index;

        var img = Resources.Load<Sprite>(path);

        player.SetCharaData(partyCharas[partyIndex], weaponData, img);
    }

    /// <summary>
    /// 攻撃方法のUIの表示制御
    /// trueでUIがオン
    /// </summary>
    private void AttackUIEnable(bool _flg)
    {
        attackUI.SetActive(_flg);

        if (_flg)
        {
            for(int i = 0; i < questData.weapon_masters.Length;i++)
            {
                for(int j = 0; j < partyCharas[GetPartyIndex()].weapon_ids.Length;j++)
                {
                    if (questData.weapon_masters[i].id == partyCharas[GetPartyIndex()].weapon_ids[j])
                    {
                        attackButton[j].transform.GetChild(1).GetComponent<Image>().sprite = Resources.Load<Sprite>(PATH_WEAPON_ICON + questData.weapon_masters[i].name_id);
                    }
                }
            }
            
        }

    }

    /// <summary>
    /// ボールが敵にヒットしたときに呼び出される、カウントを増やすだけ
    /// </summary>
    public void Hit()
    {
        hitCount++;
        hitCntUI.text = hitCount.ToString();
    }

    /// <summary>
    /// ヒット数UIの表示
    /// </summary>
    private void HitCountUIEneble(bool _flg)
    {
        hitCntUI.gameObject.SetActive(_flg);
    }

    /// <summary>
    /// 編成情報の初期化、キャラデータや画像を取得
    /// </summary>
    private void PartyInit()
    {
        // キャラデータ取得
        // キャラのidからキャラデータを取得
        // パーティーid分ループ

        for (int i = 0; i < questData.character_masters.Length; i++)
        {
            partyCharas[i] = questData.character_masters[i];
        }

        // キャラ画像を入れる画像の取得
        var tmp = GameObject.Find("CharaImages").transform;


        for (int i = 0; i < charaImages.Length; i++)
        {
            charaImages[i] = tmp.GetChild(i).GetComponent<Image>();
        }


        // キャラ画像を入れる
        for (int i = 0; i < charaImages.Length; i++)
        {
            // キャラの画像をパス指定で取得
            var path = PATH_CHARACTER_ICON + (partyCharas[i].name_id);
            charaImages[i].sprite = Resources.Load<Sprite>(path);
        }
    }

    /// <summary>
    /// クエスト開始時のフェードインの演出をいれるための処理
    /// 大部分はCameraMoveに記述
    /// </summary>
    private void InitCamera(Vector3 _target)
    {
        // フェードイン用に画像を透明にしていく
        var tmpCol01 = fadeInImage.color;
        var tmpCol02 = questName.color;
        var tmpCol03 = questFloorNum.color;
        tmpCol01.a -= Time.deltaTime;
        tmpCol02.a -= Time.deltaTime;
        tmpCol03.a -= Time.deltaTime;


        fadeInImage.color = tmpCol01;
        questName.color = tmpCol02;
        questFloorNum.color = tmpCol03;
        CameraMove(_target);
    }

    /// <summary>
    /// 指定座標までカメラを移動させる
    /// </summary>
    private void CameraMove(Vector3 _target)
    {
        // z座標を切り捨てるためにvector2で宣言
        Vector2 dif = _target - cameraMain.transform.position;
        // カメラを動かす
        cameraMain.transform.position += (Vector3)dif / CAM_MOVE_TIME * Time.deltaTime;
        // オブジェクトもすべて動かす
        allObjectParent.transform.position += (Vector3)dif / CAM_MOVE_TIME * Time.deltaTime;

        // 距離が一定より小さいor通りすぎたら固定して壁を戻す
        if (dif.magnitude < CAM_MOVE_LIMIT || dif.y < 0)
        {
            cameraMain.transform.position = _target;
            allObjectParent.transform.position = new Vector3(_target.x, _target.y, allObjectParent.transform.position.z);
            // カメラ移動完了、壁の移動開始
            Fase = GameFase.CAMERA_MOVE_END;
        }
    }

    /// <summary>
    /// 壁を画面外、内に動かす処理
    /// カメラの移動前後にしか呼ばれない
    /// </summary>
    private void WallsMove()
    {
        Vector3 target;

        if (Fase == GameFase.CAMERA_MOVE_START)
        {
            target = WALL_SIZE_MAX;
        }
        else
        {
            target = WALL_SIZE_MIN;
        }

        // 目標サイズとの差を計算して差が小さければサイズを固定
        var dif = wallParentObj.transform.localScale - target;
        if (Math.Abs(dif.magnitude) < WALL_SIZE_DIFF_LIMIT)
        {
            // 固定
            wallParentObj.transform.localScale = target;

            // 壁の移動方向によってフェーズ処理分岐
            if (Fase == GameFase.CAMERA_MOVE_START)
            {
                // 画面外へ動いたらカメラ移動開始
                Fase = GameFase.CAMERA_MOVING;
                // 移動する者の座標初期化
                cameraMain.transform.position = START_CAMERA_POS;
                allObjectParent.transform.position = new Vector3(START_CAMERA_POS.x, START_CAMERA_POS.y, allObjectParent.transform.position.z);
            }
            else
            {
                // 画面内へ動いたらゲームに戻る
                Fase = GameFase.GAMING;

                // フロアの敵生成
                FloorInit(questData.stage_master);

                // 敵の表示
                enemyParent.SetActive(true);

                // プレイヤーの情報を更新する
                PlayerInfoUpdate();

                // UIを戻す
                AttackUIEnable(true);
                HitCountUIEneble(true);
                return;
            }
            return;
        }
        else
        {
            // 画面外に出すならscaleを増やす、逆なら減らす
            int val = (Fase == GameFase.CAMERA_MOVE_START) ? 1 : -1;
            // サイズを徐々に増やす
            wallParentObj.transform.localScale += Vector3.one * val * Time.deltaTime;
        }
    }

    /// <summary>
    /// フロアの初期化処理
    /// フロアの敵の生成
    /// </summary>
    private void FloorInit(JsonStageMaster _stage)
    {
        // 生成する敵のリスト
        List<JsonEnemyInfo> floorEnemies = new List<JsonEnemyInfo>();

        var allEnemies = _stage.enemy_infos;

        // 現在のフロアと比較してリストに追加
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].floor == floorCount)
            {
                floorEnemies.Add(allEnemies[i]);
            }
        }

        // 生成してパラメータの設定
        // マスターの取り出し
        var eMasters = questData.enemy_masters;
        // 取り出したマスターデータ格納用変数
        JsonEnemyMaster eMaster = new JsonEnemyMaster();

        // 生成リスト分ループ
        for (int i = 0; i < floorEnemies.Count; i++)
        {
            var enemy = floorEnemies[i];
            // マスターデータ分ループしてデータ取り出し
            // マスターのインデックスとidが必ずしも一致しないため全探索
            for (int j = 0; j < eMasters.Length; j++)
            {
                if (eMasters[j].id == enemy.id)
                {
                    eMaster = eMasters[j];
                    break;
                }
                else
                {
                    // 配列最後でなければ繰り返し
                    if (j != eMasters.Length - 1)
                    {
                        continue;
                    }

                    // 配列最後ならマスターデータにないのでエラー処理
                    print("生成しようとしているID：" + enemy.id + "はマスターデータに存在しません!!");
#if UNITY_EDITOR
                    //ゲームプレイ終了
                    UnityEditor.EditorApplication.isPlaying = false;
#else
    Common.LoadScene("Main3D");
#endif
                }
            }

            // プールされたオブジェクトを取得
            var instance = enemyParent.transform.GetChild(i).gameObject;
            instance.transform.position = Vector3.zero;

            // パラメータを入れるためにコンポーネント取得
            var component = instance.GetComponent<InstanceEnemy>();

            // 実際のパラメータをマスターとステージ情報から計算
            // パラメータ作成
            EnemyInitDatas datas = new EnemyInitDatas();
            datas.hp        = (int)(eMaster.hp * enemy.hp_rate);
            datas.atk       = eMaster.atk * floorEnemies[i].atk_rate;
            datas.atk_ptns  = eMaster.atk_pattern_ids;
            datas.pos       = enemy.position;
            datas.size      = enemy.size;
            datas.wk        = eMaster.weak;
            datas.isB       = enemy.is_boss;
            datas.path      = PATH_ENEMY_BODY + eMaster.name_id;
            print("敵の名前パス" + datas.path);

            component.EnemyInit(datas);
        }

        //todo:未実装：敵生成時に自分が敵とかぶっていたら自分の座標をずらす


        // ボスフラグを初期化
        isBossDestroyed = false;
    }

    /// <summary>
    /// 次のフロアへ移動する際に玉から呼び出し
    /// </summary>
    public void FloorToNext()
    {
        // フロアの最奥で全員倒すとクリア
        if (floorCount == questData.stage_master.max_floor)
        {
            // クエストクリア処理
            QuestClear();
            return;
        }

        // カメラ移動準備を行う
        Fase = GameFase.CAMERA_MOVE_START;
        enemyParent.SetActive(false);

        // フロア数の加算と表示
        floorCount++;
        FloorCountDisplay();
    }

    /// <summary>
    /// フロアボスが倒されたかどうか
    /// </summary>
    /// <returns></returns>
    public bool GetIsBossDestroyed() { return isBossDestroyed; }

    /// <summary>
    /// ボス撃破時の処理、ボスが死んだら呼び出し
    /// </summary>
    public void BossDestroyed()
    {
        isBossDestroyed = true;
    }

    /// <summary>
    /// 敵の攻撃オブジェクトを生成する処理
    /// </summary>
    public GameObject EnemyAtkInst(GameObject _prefab, Vector3 _pos, Quaternion _rot)
    {
        return Instantiate(_prefab, _pos, _rot, enemyAttackObjParent);
    }

    /// <summary>
    /// 敵の攻撃オブジェクトが有効かどうか取得
    /// 有効でなければ敵の攻撃は終了している
    /// </summary>
    /// <returns></returns>
    public bool IsEnemyAtkObjActive()
    {
        if (enemyParent.GetComponentsInChildren<GameObject>() == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// プレイヤーがダメージを受けたときの処理
    /// HPを減らしてUIに反映
    /// </summary>
    public void PlayerDamage(int _dmg)
    {
        partyCurrentHp -= _dmg;
        if (partyCurrentHp <= 0)
        {
            partyCurrentHp = 0;
        }
        playerCurrentHpGauge.fillAmount = Common.NormalizedFunc(partyCurrentHp, 0, partyMaxHp, 0, 1);

        playerHpText.text = partyCurrentHp + "　" + partyMaxHp;
    }

    /// <summary>
    /// プレイヤーの回復処理
    /// </summary>
    /// <param name="_val"></param>
    public void PlayerHeal(int _val)
    {
        partyCurrentHp += _val;
        // HPが増えすぎないよう調整
        if (partyCurrentHp >= partyMaxHp)
        {
            partyCurrentHp = partyMaxHp;
        }
        playerCurrentHpGauge.fillAmount = Common.NormalizedFunc(partyCurrentHp, 0, partyMaxHp, 0, 1);
        playerHpText.text = partyCurrentHp + " / " + partyMaxHp;
    }

    /// <summary>
    /// パーティーのHPの赤ゲージを動かす
    /// </summary>
    private void HpGaugeSync()
    {
        var hpDif = playerCurrentHpGauge.fillAmount - playerDamageHpGauge.fillAmount;

        // ゲージが同期したら終了
        if (hpDif == 0)
        {
            return;
        }

        // 増減方向
        int sign;
        if (hpDif < 0)
        {
            sign = -1;
        }
        else
        {
            sign = 1;
        }

        playerDamageHpGauge.fillAmount += sign * Time.deltaTime / 2;


        var lim = 0.0001f * sign;

        if (sign == 1)
        {
            if (hpDif < lim)
            {
                playerDamageHpGauge.fillAmount = playerCurrentHpGauge.fillAmount;
            }
        }
        else
        {
            if (hpDif > -lim)
            {
                playerDamageHpGauge.fillAmount = playerCurrentHpGauge.fillAmount;
            }
        }
    }

    /// <summary>
    /// リザルトパネル表示処理、ボタンから呼び出し
    /// </summary>
    public void PushToResaultButton()
    {
        if (Fase == GameFase.GAME_CLEAR)
        {
            resultPanel.SetActive(true);
            Fase = GameFase.RESULT_START;
            StartCoroutine(Result());
        }
        else
        {
            Common.LoadScene(Common.SCENE_NAME_MAIN);
        }
    }

    /// <summary>
    /// リザルト表示処理
    /// </summary>
    private IEnumerator Result()
    {
        // パラメータ設定
        WWWForm form = new WWWForm();
        // リクエストパラメーターをセット
        form.AddField("user_id", PlayerPrefs.GetString(Common.KEY_USER_ID));
        form.AddField("stage_id", questData.stage_master.id);
        form.AddField("player_info", JsonUtility.ToJson(MainData.instance.playerInfo));

        print("送る前のデータ：" + JsonUtility.ToJson(MainData.instance.playerInfo));

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_RESULT;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        var res = coroutine.Current.ToString();

        print("送る前のデータ：" + JsonUtility.ToJson(MainData.instance.playerInfo));
        print("送った後のリザルト：" + res);

        if (res == null)
        {
            print("通信エラー：リザルトのレスポンスデータがない");
        }

        // シリアライズ
        var resultData = JsonUtility.FromJson<JsonResultData>(res);

        if (resultData.result == Common.SUCCES)
        {
            // 表示するランク
            var newData = resultData.player_info;

            // リザルト画面初期化
            rewardsPanel.SetActive(false);
            toMainButton.SetActive(false);

            // 現在のランクポイント表示
            yield return StartCoroutine(CurrnetRankPointDisplay(MainData.instance.playerInfo, questData.stage_master));

            // クリア後のランクポイントまで上昇させる
            yield return StartCoroutine(NewRankPointDisplay(MainData.instance.playerInfo, newData, questData.stage_master, resultData.limit_rank_points));

            // その他クリア時報酬の表示.初回クリアの石やドロップなど
            yield return StartCoroutine(RewardsDisplay(MainData.instance.playerInfo));


            // メインデータ書き換え
            MainData.instance.playerInfo.total_rank_point = resultData.player_info.total_rank_point;
            MainData.instance.playerInfo.next_rank_point = resultData.player_info.next_rank_point;
            MainData.instance.playerInfo.rank = resultData.player_info.rank;
            MainData.instance.playerInfo.energy = resultData.player_info.energy;
            MainData.instance.playerInfo.coin = resultData.player_info.coin;
            MainData.instance.playerInfo.crystal = resultData.player_info.crystal;
            MainData.instance.playerInfo.progress_rates = resultData.player_info.progress_rates;

            // ホームに戻るボタン表示
            toMainButton.SetActive(true);
        }
        else
        {
            yield return null;
        }
    }

    /// <summary>
    /// 現在のランク表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator CurrnetRankPointDisplay(JsonPlayerInfo _old, JsonStageMaster _stg)
    {
        // リザルト初期化

        var get = _stg.rank_point;
        var next = _old.next_rank_point;
        // Sランクだったらカンスト
        if (_old.rank == "S")
        {
            get = 0;
            next = 0;
        }

        // ランク表示
        rankText.text = _old.rank;
        rankText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // 取得ポイント表示
        getRankPointText.text = get.ToString();
        getRankPointText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // 次のランクまでのポイント表示
        nextRankPointText.text = next.ToString();
        nextRankPointText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
    }

    /// <summary>
    /// 更新後のランク表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator NewRankPointDisplay(JsonPlayerInfo _old, JsonPlayerInfo _new, JsonStageMaster _stg, int[] _limits)
    {
        int get = _stg.rank_point;
        int next = _old.next_rank_point;
        string rankDisp = _old.rank;
        // Sランクだったら演出スキップ
        if (_old.rank == "S")
        {
            yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
        }
        else
        {
            while (true)
            {
                // タップでスキップ処理
                if (isResultTap)
                {
                    get = 0;
                    next = _new.next_rank_point;
                    rankDisp = _new.rank;

                    getRankPointText.text = get.ToString();
                    nextRankPointText.text = next.ToString();
                    rankText.text = rankDisp;

                    break;
                }

                // 演出の増加量.目標までの距離に応じて調整
                int val = 1;
                if (get > 200)
                {
                    val = get / 4;
                }
                else if (get > 100)
                {
                    val = 10;
                }
                // 減算していく
                get -= val;
                next -= val;


                // 次のランクまでのポイントが負で表示上のランクアップ演出
                if (next <= 0)
                {
                    ResultRankUp(ref get, ref next, ref rankDisp, _limits);
                }

                // 画面に出力
                getRankPointText.text = get.ToString();
                nextRankPointText.text = next.ToString();
                rankText.text = rankDisp;

                // 取得ポイントがなくなったら終了
                if (get <= 0)
                {
                    break;
                }
                yield return null;
            }
        }
        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
    }

    /// <summary>
    /// ランクアップの演出の処理
    /// 行が多いので関数化
    /// </summary>
    private void ResultRankUp(ref int _getPt, ref int _nextPt, ref string _rank, int[] _limits)
    {

        // オーバー分を保持
        var dif = -_nextPt;

        switch (_rank)
        {

            case "F":
                _nextPt = _limits[1];
                _rank = "E";
                break;
            case "E":
                _nextPt = _limits[2];
                _rank = "D";
                break;
            case "D":
                _nextPt = _limits[3];
                _rank = "C";

                break;
            case "C":
                _nextPt = _limits[4];
                _rank = "B";

                break;
            case "B":
                _nextPt = _limits[5];
                _rank = "A";

                break;
            case "A":
                _getPt = 0;
                _nextPt = 0;
                _rank = "S";
                break;
        }
        // オーバー分を戻す
        _nextPt -= dif;
    }

    /// <summary>
    /// クエストクリア時のドロップアイテム表示
    /// 前期発表時点は未実装
    /// </summary>
    /// <returns></returns>
    private IEnumerator RewardsDisplay(JsonPlayerInfo _old)
    {
        rewardsPanel.SetActive(true);
        crystalObj.SetActive(false);
        emptyObj.SetActive(false);
        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // 初回クリアかどうか判定
        // 進行度から検索

        for (int i = 0; i < _old.progress_rates.Length; i++)
        {
            // ステージのマスターのidと一致したら取り出し
            if (_old.progress_rates[i].stage_id == questData.stage_master.id)
            {
                // is_clearがfalseなら初回クリア
                if (_old.progress_rates[i].is_clear == false)
                {
                    print("初回クリア報酬あり");
                    var getCrystal = questData.stage_master.crystal;
                    crystalObj.SetActive(true);
                    crystalObj.GetComponentInChildren<Text>().text = getCrystal.ToString();
                    emptyObj.SetActive(false);
                }
                // 初回クリアではないので報酬なし
                else
                {
                    print("初回クリア報酬なし");
                    crystalObj.SetActive(false);
                    emptyObj.SetActive(true);
                }

                break;
            }
        }

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // ドロップアイテムの表示、未実装
        dropPanel.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
    }

    /// <summary>
    /// 敗北しているかを取得する関数
    /// </summary>
    /// <returns></returns>
    public bool GetIsDefeat()
    {
        return partyCurrentHp <= 0;
    }

    /// <summary>
    /// 生成した敵から呼び出す
    /// </summary>
    /// <returns></returns>
    public JsonEnemyAttackMaster[] GetJsonEnemyAttackMasters()
    {
        return questData.enemy_atk_masters;
    }

    /// <summary>
    /// クエストのクリア後に呼ばれる処理
    /// </summary>
    private void QuestClear()
    {
        Fase = GameFase.GAME_CLEAR;
        // リザルトの表示
        toResultPanel.SetActive(true);
        // todo:クリア用に変更
        toResultText.text = "ステージ\nクリア！";
    }

    /// <summary>
    /// クエスト失敗時の処理
    /// </summary>
    public void GameOver()
    {
        
        Fase = GameFase.GAME_OVER;

        // リザルトパネル表示
        toResultPanel.SetActive(true);
        // todo:テキストを敗北用に変更
        toResultText.text = "ざんねん";
        toResultButton.transform.GetChild(0).GetComponent<Text>().text = "ホームへ";
        
    }

    /// <summary>
    /// プレイヤーの入力待機時のみ表示
    /// </summary>
    public void MenuButtonEnable()
    {
        menuButton.SetActive(true);
    }

    /// <summary>
    /// プレイヤーの入力待機時以外は非表示
    /// </summary>
    public void MenuButtonDisable()
    {
        menuButton.SetActive(false);
    }


    /// <summary>
    /// メニューボタン押下時の処理
    /// </summary>
    public void PushMenuButton()
    {
        menuPanel.SetActive(true);
        isPause = true;
    }

    /// <summary>
    /// メニューからゲームに戻る
    /// </summary>
    public void PushBackGameButton()
    {
        menuPanel.SetActive(false);
        isPause = false;
    }

    /// <summary>
    /// メイン画面に戻るボタン押下時の処理
    /// </summary>
    public void PushBackMainButton()
    {
        Common.LoadScene(Common.SCENE_NAME_MAIN);
    }

}