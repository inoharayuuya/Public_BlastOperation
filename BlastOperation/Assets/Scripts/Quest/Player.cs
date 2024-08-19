using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;
using Json;

/// <summary>
/// クエスト中操作するプレイヤー（ボール）
/// </summary>
public class Player : MonoBehaviour, IDamageable
{
    [Tooltip("壁の厚み")]
    private const float WALL_WIDTH = 1.0f;

    public enum PlayerState
    {
        /// <summary>メニューを開いている</summary>
        PAUSE,
        /// <summary>フロア移動待ち</summary>
        WAIT_FLOOR,
        /// <summary>プレイヤーの入力待ち</summary>
        WAIT_INPUT,
        /// <summary>発射後、停止待ち</summary>
        MOVE,
        /// <summary>発射後、停止待ち</summary>
        STOP,
        /// <summary>敵の処理待ち</summary>
        WAIT_ENEMY,
        /// <summary>敗北時</summary>
        DEFEAT,
    }


    /// <summary>
    /// 四方向の壁の法線ベクトル、U,D,L,Rの順に格納
    /// </summary>
    private readonly Vector3[] NORMAL_VECTOR =
    {
        Vector3.down,
        Vector3.up,
        Vector3.right,
        Vector3.left
    };

    /// <summary>
    /// 四方向の壁
    /// U,D,L,Rの順に格納
    /// </summary>
    [SerializeField]
    private GameObject[] walls;

    // コンポーネント
    private QuestManager qm;
    private SpriteRenderer spriteSr;




    [Tooltip("発射方向の矢印")]
    private Transform moveArrow;
    [Tooltip("矢印の大きさの限界値")]
    private const float ARROW_MAX_SIZE = 3.3f;
    [Tooltip("引っ張った距離に応じて速度にかかる倍率")]
    private float pullRate;
    [Tooltip("タップ開始地点")]
    private Vector3 tapStPos;
    [Tooltip("タップ終了地点")]
    private Vector3 tapEdPos;
    [Tooltip("運動ベクトル（単位ベクトル）")]
    private Vector3 moveNmVec;
    [Tooltip("運動ベクトルにかかる大きさ")]
    private const float MOVE_SPEED = 10;
    [Tooltip("タップ開始したかどうかのフラグ")]
    public bool isTap { get; private set; }
    [Tooltip("プレイヤーの状態変数")]
    public PlayerState state {  get; private set; }
    // 初速度に加わる倍率、引っ張った距離に応じて変動
    [Tooltip("初速度の倍率の最小値")]
    private const float START_MOVE_SPEED_MIN = 0.8f;
    [Tooltip("初速度の倍率の最大値")]
    private const float START_MOVE_SPEED_MAX = 1.25f;
    [Tooltip("現在の加速度、1から0までで変化")]
    private float accelarate;


    [Header("減速に関する定数")]

    [Tooltip("移動中常にかかる減速")]
    private const float DEC_RATE_MOVE = 0.88f;
    [Tooltip("ボス撃破時にかかる減速")]
    private const float DEC_RATE_BOSS = 0.2f;
    [Tooltip("止まる直前にかかる減速")]
    private const float DEC_RATE_STOP = 0.55f;
    [Tooltip("壁に当たった時にかかる減速")]
    private const float DEC_RATE_HIT_WALL = 0.90f;
    [Tooltip("敵と当たった時にかかる減速")]
    private const float DEC_RATE_HIT_ENEMY = 0.80f;


    [Header("停止の基準速度")]

    [Tooltip("ボールが停止し始める速度")]
    private const float LIMIT_DEC = 0.1f;
    [Tooltip("ボールが停止する速度")]
    private const float LIMIT_STOP = 0.01f;
    [Tooltip("現在フロアにいる敵の配列"), SerializeField]
    private InstanceEnemy[] enemies;
    [Tooltip("全滅時の遅延時間")]
    private const float DELAY_NEXT_TIME = 0.2f;


    [Header("プレイヤーの画像")]

    [Tooltip("攻撃判定に必要なデータ")]
    private CharaData charaData;
    [Tooltip("現在選択されているキャラの、現在選択されている武器の画像")]
    private SpriteRenderer weaponSr;

    // Start is called before the first frame update
    void Start()
    {
        // 初期化
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!qm.isPause)
        {
            // 発射前
            if (state == PlayerState.WAIT_INPUT)
            {
                // 入力受付
                GetOperation();

                // 矢印の表示
                ArrowDisp();
            }

            // 発射後
            if (state == PlayerState.MOVE)
            {
                // 移動処理
                Move();
            }
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Init()
    {
        // 状態変数初期化
        state = PlayerState.WAIT_FLOOR;
        // タップされてない
        isTap = false;
        // 加速度
        accelarate = 1;
        // 引っ張った距離の倍率
        pullRate = 1;
        // タップ開始、終了地点
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;

        // 玉の画像コンポーネント取得
        spriteSr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        // マネージャー取得
        qm = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        // 発射方向表示のオブジェクト取得
        moveArrow = transform.GetChild(0);
        moveArrow.gameObject.SetActive(false);

        // 自分のサイズを取得
        transform.GetChild(1).localScale = Vector3.one * Common.SIZE_PLAYER;

        // 武器の画像取得
        weaponSr = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// プレイヤーの操作を受け取る、一定以上引っ張っていたら発射
    /// </summary>
    private void GetOperation()
    {
        // ゲームフェーズ以外で入力を受け付けない
        if (qm.Fase != QuestManager.GameFase.GAMING)
        {
            return;
        }

        // 狙いを定める
        if (Input.GetMouseButtonDown(0))
        {
            tapStPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isTap = true;
        }

        // タップ開始時と終了時のベクトルの大きさを判定
        if (Input.GetMouseButtonUp(0))
        {
            // タップ終了
            isTap = false;
            // 座標記録
            tapEdPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // 移動方向の決定
            moveNmVec = (tapStPos - tapEdPos);

            // 一定以上大きいので発射
            if (moveNmVec.magnitude > Common.TAP_VECTOR_LIMIT)
            {
                // 引っ張ったら動く
                // 引っ張った距離の倍率をかけた初速度を与える
                accelarate = 1 * pullRate;
                // 移動ベクトルの正規化
                moveNmVec = moveNmVec.normalized;
                // 状態を更新
                state = PlayerState.MOVE;
                // 仮の色変更
                spriteSr.color = Color.white;
                enemies = qm.GetInstanceEnemys();
                // メニューボタンの非表示
                qm.MenuButtonDisable();
            }
            else
            {
                //print("発射キャンセル");
                //qm.AttackUIEnable(true);
            }
        }
    }

    /// <summary>
    /// 移動処理、減速と反射、攻撃を行う
    /// </summary>
    private void Move()
    {
        // 減速、停止処理
        StartCoroutine(Decelarating());

        // 壁との当たり判定
        JudgeWallHit();

        // 敵との当たり判定
        JudgeEnemyHit();
    }

    /// <summary>
    /// 徐々に減速し、一定以上遅ければ停止する処理
    /// </summary>
    private IEnumerator Decelarating()
    {
        var tmpVec = moveNmVec * charaData.speed * Time.deltaTime * accelarate;

        // なめらかに停止させたいので一定の速度以下になると減速率上昇

        float rate = 0;
        // ボスが倒れた
        if (qm.GetIsBossDestroyed())
        {

            // 強制停止
            if (tmpVec.magnitude < LIMIT_STOP)
            {
                //Stop(true);
                //return;
                //print("ボス倒している");
                yield return StartCoroutine(Stop(true));

            }
            rate = DEC_RATE_BOSS;

        }
        else
        {
            // 通常時の減速
            if (tmpVec.magnitude > LIMIT_DEC)
            {
                rate = DEC_RATE_MOVE;
            }
            // 一定以上遅くなると停止し始めるために大きめに減速
            else if (tmpVec.magnitude > LIMIT_STOP)
            {
                rate = DEC_RATE_STOP;
            }
            // 強制停止
            else
            {
                // 状態の更新
                state = PlayerState.STOP;
                StartCoroutine(Stop());
            }
        }

        // 徐々に減速する
        // 一秒間に速度がrate倍になるようにフレームに直して計算
        // 毎フレーム数値をかけていく
        accelarate *= Mathf.Pow(rate, Time.deltaTime);

        //print(tmpVec.magnitude);
        transform.position += tmpVec;
    }

    /// <summary>
    /// 弾が止まる処理
    /// </summary>
    private IEnumerator Stop(bool _isBossDes = false)
    {
        //print("強制停止");

        // フロアボスを倒した場合は、敵すべての死亡処理を呼んで次のフロアへ
        if (_isBossDes)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                // 死亡処理
                if (enemies[i].gameObject.activeSelf != false)
                {
                    enemies[i].Dead();
                }
            }

            // 初期化処理
            PlayerTurnEnd();

            // 遅延を入れる
            yield return StartCoroutine(Common.Delay(DELAY_NEXT_TIME, qm.FloorToNext));
        }
        // ボスは倒していない
        else
        {
            // 敵が全滅したかどうか判定
            bool isAllDown = true;

            for (int i = 0; i < enemies.Length; i++)
            {
                // アクティブな敵がいる＝まだ倒し切れていない
                if (enemies[i].gameObject.activeSelf)
                {
                    if (enemies[i].GetIsDead())
                    {
                        yield return StartCoroutine(enemies[i].Dead());
                    }
                    else
                    {
                        // 全滅フラグをオフ
                        isAllDown = false;
                    }
                }
            }
            // 少し待機

            // 敵が全滅していたら次のフロア
            if (isAllDown)
            {
                //print("次のフロアへ移動");

                // 初期化
                PlayerTurnEnd();

                // 遅延を入れる
                StartCoroutine(Common.Delay(DELAY_NEXT_TIME, qm.FloorToNext));
            }
            else
            {
                state = PlayerState.WAIT_ENEMY;
                // 敵のターン終了時の処理、HPの表示更新と攻撃
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].gameObject.activeSelf)
                    {
                        // 一体ずつ呼び出す
                        enemies[i].EnemyTurnInit();
                    }
                }

                // 敵の行動が終わるまでなにもしない

                yield return new WaitForSeconds(3f);

                // 敵の攻撃が終了したので
                EnemyActionEnd();
            }
        }
    }

    /// <summary>
    /// 敵の攻撃処理が完了したときの処理
    /// </summary>
    public void EnemyActionEnd()
    {
        // 情報の更新
        PlayerTurnEnd();
        qm.PlayerInfoUpdate();

        // メニューボタン表示
        qm.MenuButtonEnable();
    }

    /// <summary>
    /// ターン終了時の処理
    /// カメラ移動準備、敗北の判定
    /// </summary>
    private void PlayerTurnEnd()
    {

        if (qm.GetIsDefeat())
        {
            //敗北処理
            qm.GameOver();
            state = PlayerState.DEFEAT;
            return;
        }

        // 状態の更新
        state = PlayerState.WAIT_FLOOR;

        // 仮で発射状態を可視化
        spriteSr.color = Color.green;
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;

        // 移動終了、ターン数の加算
        qm.PlayerTurnEnd();
    }

    /// <summary>
    /// 壁と当たって反射するか判定
    /// </summary>
    private void JudgeWallHit()
    {
        // 壁との当たり判定と反射
        // 参考サイト：https://nn-hokuson.hatenablog.com/entry/2018/03/30/201715

        // 当たり判定処理、四方向の壁それぞれを判定
        for (int i = 0; i < walls.Length; i++)
        {
            // 当たっていたらtrue
            if (MoveCircle2WallHit(moveNmVec, transform.position, Common.SIZE_PLAYER, NORMAL_VECTOR[i], walls[i].transform.position, WALL_WIDTH))
            {
                // 反射する
                Reflect(NORMAL_VECTOR[i], ref moveNmVec);
                accelarate *= DEC_RATE_HIT_WALL;
            }
        }
    }

    /// <summary>
    /// 敵と当たって反射するか判定
    /// 円と円の当たり判定
    /// </summary>
    private void JudgeEnemyHit()
    {
        // 敵の情報取得して、敵と反射する
        for (int i = 0; i < enemies.Length; i++)
        {
            // インスタンスエネミーかどうか
            if (enemies[i].TryGetComponent(out InstanceEnemy component))
            {
                //print("エネミーだ");
                // nullチェックと有効かどうかと、
                if (enemies[i] != null && enemies[i].gameObject.activeSelf == true)
                {
                    // 距離と半径の和を比較
                    if (MoveCircle2CircleHit(transform, Common.SIZE_PLAYER, moveNmVec, enemies[i].transform, enemies[i].transform.localScale.x, out var diff))
                    {
                        // 敵の法線ベクトルはdiffの大きさが１のベクトル
                        Reflect(diff.normalized, ref moveNmVec);

                        // 攻撃判定
                        // 属性が回復なら回復の処理
                        if (charaData.attr == Common.ATTR_HEAL)
                        {
                            //回復処理
                            qm.PlayerHeal(charaData.atk);

                            //print("回復した!");
                        }
                        // ダメージ処理
                        else if (charaData.attr == Common.ATTR_MAGICAL || charaData.attr == Common.ATTR_PHYSICAL)
                        {
                            //攻撃力を引数で渡す
                            component.Damage(charaData.atk, charaData.attr);
                        }

                        // Hit数の加算
                        qm.Hit();
                        accelarate *= DEC_RATE_HIT_ENEMY;
                    }
                    else
                    {
                        //print("遠い");

                    }
                }
            }
        }
    }

    /// <summary>
    /// 発射方向を示す矢印の表示
    /// </summary>
    private void ArrowDisp()
    {
        // 進行方向ベクトルを取得
        var tmpMoveVec = tapStPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // 発射時と同じ条件で表示のオンオフを切り替え
        // 発射されるなら表示し計算
        if (isTap && tmpMoveVec.magnitude > Common.TAP_VECTOR_LIMIT)
        {
            // xのプラス方向を0度とし、なす角を算出
            var tmpAng = Mathf.Atan2(tmpMoveVec.y, tmpMoveVec.x) * Mathf.Rad2Deg;

            // yのプラス方向を基準にしたいので値を修正
            tmpAng -= 90f;
            //print("ang：" + tmpAng);
            moveArrow.transform.rotation = Quaternion.Euler(0, 0, tmpAng);
            moveArrow.gameObject.SetActive(true);

            // 矢印の大きさを変更してタップ開始地点をわかりやすくする
            var tmpVecMag = tmpMoveVec.magnitude;
            // 大きさを正規化
            if (tmpVecMag > ARROW_MAX_SIZE)
            {
                tmpVecMag = ARROW_MAX_SIZE;
            }
            // 矢印の大きさを変更
            var tmp = moveArrow.transform.localScale;
            tmp.y = tmpVecMag;
            moveArrow.transform.localScale = tmp;
            // 初速度に与える倍率の計算
            // START_MOVE_SPEED_MAX～START_MOVE_SPEED_MINで正規化
            var v0 = Common.NormalizedFunc(tmpVecMag, 0, ARROW_MAX_SIZE, START_MOVE_SPEED_MIN, START_MOVE_SPEED_MAX);
            //print("速度倍率：" + v0);
            pullRate = v0;
        }
        // 発射されないので矢印の表示をしない
        else
        {
            moveArrow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 2つのベクトルが向き合っているかどうかを返す
    /// </summary>
    private bool IsVectorOpposite(Vector3 _v1, Vector3 _v2)
    {
        return (Vector3.Dot(_v1, _v2) > 0);
    }

    /// <summary>
    /// 厚みを持つ壁と動く円の当たり判定の処理
    /// </summary>
    /// <param name="_wallNmUnVec">壁の反射面に対する法線ベクトルの単位ベクトル</param>
    /// <param name="_wallPos">壁の座標</param>
    /// <param name="_wallWid">壁の厚さ</param>
    /// <param name="_crMvNmUnVec">動く円の運動ベクトルの単位ベクトル</param>
    /// <param name="_crPos">動く円の座標</param>
    /// <param name="_crWid">円の直径</param>
    /// <returns></returns>
    private bool MoveCircle2WallHit(Vector3 _crMvNmUnVec, Vector3 _crPos, float _crWid, Vector3 _wallNmUnVec, Vector3 _wallPos, float _wallWid)
    {
        // 当たる相手の法線ベクトルと自分の運動ベクトルの内積が0より大きい
        // （ベクトル同士が向き合っていない）なら当たり判定は取らない
        if (IsVectorOpposite(_wallNmUnVec, _crMvNmUnVec))
        {
            return false;
        }

        // 壁とボールの距離を計算
        var diff = _crPos - _wallPos;
        // 内積を計算し、ボールから壁の垂線の距離を計算
        // 壁の厚みとプレイヤーのサイズを参照
        float h = Vector3.Dot(diff, _wallNmUnVec);


        var r = (_crWid / 2) + (_wallWid / 2);

        // 円の半径と壁の厚みを足した長さより短ければ当たった
        return (h - r < 0);
    }

    /// <summary>
    /// 動く円と円の当たり判定
    /// </summary>
    /// <param name="_circle">動く円のTransform</param>
    /// <param name="_cSize">動く円の直径</param>
    /// <param name="_cMoveNmVec">動く円の運動ベクトルの単位ベクトル</param>
    /// <param name="_target">相手の円</param>
    /// <param name="_tSize">相手の円の直径</param>
    /// <param name="diff">反射面の法線ベクトル</param>
    /// <returns></returns>
    private bool MoveCircle2CircleHit(Transform _circle, float _cSize, Vector3 _cMoveNmVec, Transform _target, float _tSize, out Vector3 diff)
    {
        // 敵と座標を比較
        diff = (_circle.position - _target.position);
        // 当たり判定は2Dのためzを消す
        diff.z = 0;

        // 当たる相手の法線ベクトルと自分の運動ベクトルの内積が0より大きい
        // （ベクトル同士が向き合っていない）なら当たり判定は取らない
        if (IsVectorOpposite(diff.normalized, _cMoveNmVec))
        {
            return false;
        }

        // 敵と自分の半径の和
        var r = (_tSize / 2) + (_cSize / 2);

        // 距離と半径の和を比較
        // マイナスなら当たり判定発生
        return (diff.magnitude - r < 0);
    }

    /// <summary>
    /// 反射処理,引数の運動ベクトルを反射後の向きに変える
    /// </summary>
    /// <param name="_normal"></param>
    /// <param name="_moveVec"></param>
    private void Reflect(Vector3 _normal, ref Vector3 _moveVec)
    {
        // 当たった相手の法線ベクトル
        Vector3 n = _normal;
        // 移動ベクトルの、反射面(点)に水平な成分の算出
        float h = Mathf.Abs(Vector3.Dot(_moveVec, n));


        // 反射角を計算
        Vector3 d = _moveVec + 2 * n * h;

        // 移動方向を反射させる、減速処理は当たり判定のところで行う
        _moveVec = d.normalized;
    }

    /// <summary>
    /// 攻撃データの設定、キャラと武器のデータから攻撃力を決定
    /// </summary>
    public void SetCharaData(JsonCharacterMaster _chara, JsonWeaponMaster _weapon, Sprite _img)
    {
        charaData.atk = (int)(_weapon.atk_rate * _chara.atk);
        charaData.attr = _weapon.attr;
        charaData.speed = _weapon.speed_rate * _chara.speed;
        weaponSr.sprite = _img;
    }

    /// <summary>
    /// 敵からのダメージ
    /// </summary>
    public void Damage(float _atk, string _attr)
    {
        // ダメージ計算
        if (_attr == Common.ATTR_PHYSICAL)
        {
            _atk *= 2;
        }
        var dmg = _atk;

        // ダメージ処理
        qm.PlayerDamage((int)dmg);

    }

    /// <summary>
    /// プレイヤーのターン開始時の処理
    /// カメラの移動終了時にqmから呼び出し
    /// </summary>
    public void PlayerTurnStart()
    {
        if(state == PlayerState.WAIT_FLOOR)
        {
            state = PlayerState.WAIT_INPUT;
            spriteSr.color = Color.green;
        }
    }

    /// <summary>
    /// ポーズ時にqmから呼び出し
    /// </summary>
    public void GamePause()
    {
        state = PlayerState.PAUSE;
    }

    /// <summary>
    /// ポーズ解除時にqmから呼び出し
    /// </summary>
    public void GameUnPause()
    {
        state = PlayerState.WAIT_INPUT;
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;
    }
}
