using System.Collections;
using Unity.VisualScripting;
using UnityEngine;



public class HomeCharaManager : MonoBehaviour
{
    // スクロールのスピードデフォルト値、最小値、減速の値
    private const float DEF_SPEED = 1.5f;
    private const float MIN_SPEED = 0.5f;
    private const float SLOW_SPEED = 0.97f;
    // startPosを更新する間隔
    private const float START_POS_TIME = 0.1f;

    // 回転方向右向きか左向きか
    private const int DIR_RIGHT = -1;
    private const int DIR_LEFT = 1;

    // 正規化したx座標の中心
    private const float NORMALIZED_X_CENTER = 0.5f;

    // スクロールのスピード
    [SerializeField] private float speed;

    // タップ時のポジション
    private Vector3 startPos;
    // 指を離したときのポジション
    private Vector3 endPos;

    // タップしているかどうかのフラグ
    private bool isTap;
    // 動かしているかのフラグ
    private bool isMove;

    // startPosを更新するまでの時間
    private float startPosTime;

    // 相対ベクター
    private float direction;


    // キャラのオブジェクト
    [SerializeField] private GameObject[] charas = new GameObject[CHARA_NUMBER];

    private int dir;
    private bool isDirDecition;
    private float tmp = 300f;
    private int index;

    private readonly int[] charaAngles = { 0,120,-120};
    private bool isCenter;

    // タップ領域を触っているかどうか
    private bool isTapArea;

    #region 前バージョンの回転処理

    // ホームに配置するキャラの数
    private const int CHARA_NUMBER = 3;

    // タップ時の座標
    public Vector3 sMousePos;
    // タップ終了時の座標
    public Vector3 eMousePos;

    // ドラッグを始めたかどうかのフラグ
    public bool isDrag;
    // 指が離された(ストップ)かどうかのフラグ
    private bool isStop;

    private int objNum = 0;

    // スワイプ判定の閾値
    [SerializeField] private float tapJudge;


    /// <summary>
    /// 指を動かしたかどうかを判定する
    /// 始めにタップした座標から動いていればisMoveがオンで移動処理へ
    /// </summary>
    private void MoveJudge()
    {
        if (isDrag)
        {
            // スクリーン座標から世界座標に変換しておく
            //eMousePos =Camera.main.ScreenToWorldPoint(Input.mousePosition);
            eMousePos = Input.mousePosition;

            var directionX = eMousePos.x - sMousePos.x;

            // 回転方向 右
            if (directionX > tapJudge)
            {

                isMove = true;

                //dir = LEFT;
                isDrag = false;
            }
            else if (directionX < -tapJudge)
            {

                isMove = true;

                //dir = RIGHT;
                isDrag = false;
            }
        }

        if (isStop)
        {
            if (charas[objNum].transform.position.z > -299.9f || charas[objNum].transform.position.z <= -300.0f)
            {


                Debug.Log("a");
                isMove = true;
            }
            else
            {
                isMove = false;
                isStop = false;
            }
        }
    }

    IEnumerator StartPosUpdate()
    {

        yield return new WaitForSeconds(0.5f);
        sMousePos = Input.mousePosition;
        //isMove = false;
    }

    /// <summary>
    /// イベントトリガーでPointerDownの時に呼び出す
    /// ドラッグのフラグをオンにして判定を開始
    /// </summary>
    public void PointerDown()
    {
        // スクリーン座標から世界座標に変換しておく
        //sMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        sMousePos = Input.mousePosition;

        //.Log("s " +  sMousePos.x);

        isDrag = true;
    }

    /// <summary>
    /// イベントトリガーでPointerUpの時に呼び出す
    /// フラグをオフにして移動を停止
    /// </summary>
    public void PointerUp()
    {
        isDrag = false;
        var tmp = new Vector3(0, 0, 300);

        // 指を離したときに一番前にいるキャラを中心に設定
        for (int i = 0; i < charas.Length; i++)
        {
            var vec = charas[i].transform.position;

            if (vec.z < tmp.z)
            {
                tmp.z = vec.z;
                objNum = i;
            }
        }

        isMove = false;
        isStop = true;

    }

    /// <summary>
    /// 親の回転を変更させる処理
    /// </summary>
    private void RotationChange()
    {
        // transformを取得
        Transform myTransform = this.transform;

        // ワールド座標を基準に、回転を取得
        Vector3 worldAngle = myTransform.eulerAngles;
        worldAngle.y += 0.1f; // ワールド座標を基準に、y軸を軸にした回転を10度に変更
        myTransform.eulerAngles = worldAngle; // 回転角度を設定
    }

    #endregion

    private void Awake()
    {
        // シーンごとに初期化しとく(追加するなら)
        Application.targetFrameRate = 60;

    }

    /// <summary>
    /// キャラ3体の座標を計算して指定する処理
    /// </summary>
    private void SetPosition()
    {
        // キャラ3体の座標を指定する処理
        for (int i = 1; i < 3; i++)
        {
            // 度数法で120度、240度 90引いてるのは始点の調整
            var deg = (120 * i) - 90;

            // 120度、240度を弧度法に変換
            var rad = Mathf.Deg2Rad * deg;

            // 中心座標と基準座標のベクトル取得用
            Vector3 center = transform.position;
            Vector3 standard = charas[0].transform.position;

            // キャラ2,3体目の座標を指定
            var pos = charas[i].transform.position;

            var cVec = center - standard;

            // 半径取得
            var r = cVec.z;

            pos.x = center.x + r * Mathf.Cos(rad);
            pos.z = center.z + r * Mathf.Sin(rad);
            charas[i].transform.position = pos;
        }
    }

    /// <summary>
    /// 画面をタップしているとき、離したときの処理
    /// </summary>
    private void TapCheck()
    {
        // タップ領域を触っていれば
        if (isTapArea)
        {
            // 画面がタップされたとき
            if (Input.GetMouseButtonDown(0))
            {
                // startPosに画面が押された座標を代入する
                startPos = Input.mousePosition;

                // 画面が押されているのでtrueにする
                isTap = true;

                // 動けるようになるのでtrue
                isMove = true;
            }

            // 画面がタップされている間
            if (Input.GetMouseButton(0))
            {
                // endPosには毎フレーム画面が押されている座標を代入する
                endPos = Input.mousePosition;

                // 更新時間がくるまでdeltaTimeで計算
                startPosTime -= Time.deltaTime;

                // スピードは押されるたびに初期化する
                speed = DEF_SPEED;

                isCenter = false;

                // startPosTimeが0になるたびにstartPosを現在のendPosで初期化
                if (startPosTime <= 0)
                {
                    // startPosTimeの初期化
                    startPosTime = START_POS_TIME;
                    startPos = endPos;
                }
            }
        }

        // 画面から指が離されたとき
        if (Input.GetMouseButtonUp(0))
        {
            // 指が離されたのでfalse
            isTap = false;
            isTapArea = false;
        }
    }

    /// <summary>
    /// キャラを動かす処理
    /// </summary>
    private void CharaMove()
    {
        // 画面が押されていない間は減速し続ける
        if (!isTap && !isCenter)
        {
            // 減速
            speed *= SLOW_SPEED;

            // 止める処理
            if (speed < MIN_SPEED)
            {
                // 通常の移動を消したいのでfalse
                isMove = false;


                // ここに中心にする処理
                // 一番正面に近いオブジェクトを探す
                for (int i = 0; i < charas.Length; i++)
                {
                    if (charas[i].transform.position.z < tmp)
                    {
                        tmp = charas[i].transform.position.z;
                        index = i;
                    }

                    Debug.Log("ナンバー　" + index);
                }

                // 一番正面に近かったオブジェクトの、正規化したz座標を取得
                var obj = charas[index].GetComponent<HomeCharaMove>();

                // 正規化した座標取得
                var normalizedZ = obj.GetNormalizedZ(charas[index]);
                var normalizedX = obj.GetNormalizedX(charas[index]);

                // 正面の角度を設定
                var frontAngle = 0;

                //Debug.Log("norX " + normalizedX);

                // 一度だけ判定
                //if (speed == 0)
                // 回転方向判定(一度だけ)
                if (!isDirDecition)
                {
                    // 回転の方向を見る
                    if (normalizedX > NORMALIZED_X_CENTER)
                    {
                        dir = DIR_LEFT;
                    }
                    else
                    {
                        dir = DIR_RIGHT;
                    }
                    
                    isDirDecition = true;

                }

                // transformを取得
                Transform myTransform = this.transform;
                // ワールド座標を基準に、回転を取得
                Vector3 worldAngle = myTransform.eulerAngles;

                // TODO ここの値おかしいからなおす
                // 正面に来るまで移動させる
                if (dir == DIR_LEFT)
                {
                    if ( normalizedX > NORMALIZED_X_CENTER && !isCenter)
                    {
                        // y軸を軸にした回転を変更
                        worldAngle.y += (100f * dir) * Time.deltaTime;
                    }
                    else
                    {
                        isCenter = true;
                        speed = 0;
                        worldAngle.y = charaAngles[index];
                        myTransform.eulerAngles = worldAngle;
                    }
                }
                else
                {
                    if ( normalizedX < NORMALIZED_X_CENTER)
                    {
                        // y軸を軸にした回転を変更
                        worldAngle.y += (100f * dir) * Time.deltaTime;

                    }
                    else
                    {
                        isCenter = true;
                        speed = 0;
                        worldAngle.y = charaAngles[index];
                        myTransform.eulerAngles = worldAngle;
                    }
                }

                // 回転角度を設定
                myTransform.eulerAngles = worldAngle;

            }
            else if(speed == 0)
            {
                // transformを取得
                Transform myTransform = this.transform;
                // ワールド座標を基準に、回転を取得
                Vector3 worldAngle = myTransform.eulerAngles;

                
            }
        }
        else
        {
            tmp = 300;
            isDirDecition = false;
            
        }

        // startPosとendPosを計算して距離を求める
        direction = (endPos.x - startPos.x) * speed;

        if (isMove)
        {
            // transformを取得
            Transform myTransform = this.transform;
            // ワールド座標を基準に、回転を取得
            Vector3 worldAngle = myTransform.eulerAngles;

            // 計算結果がプラスの場合(0を含まない)
            if (direction > 0)
            {
                // y軸を軸にした回転を変更
                worldAngle.y += (-direction * speed) * Time.deltaTime;

                //dir = DIR_RIGHT;
            }
            else
            {
                // y軸を軸にした回転を変更
                worldAngle.y += (-direction * speed) * Time.deltaTime;
                //dir = DIR_LEFT;
            }

            // 回転角度を設定
            myTransform.eulerAngles = worldAngle;

        }

    }

    /// <summary>
    /// タップ領域を触っている状態の時にフラグをオン
    /// </summary>
    public void TapArea()
    {
        isTapArea = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        speed = DEF_SPEED;

        // キャラ3体の座標を指定する処理
        SetPosition();
    }

    // Update is called once per frame
    void Update()
    {
        // 一番正面に近かったオブジェクトの、正規化したz座標を取得
        var obj = charas[0].GetComponent<HomeCharaMove>();

        // 正規化した座標取得
        var normalizedX = obj.GetNormalizedX(charas[0]);


        // タップしているか、離されているか判定
        TapCheck();

        // キャラを動かす処理
        CharaMove();

    }
}
