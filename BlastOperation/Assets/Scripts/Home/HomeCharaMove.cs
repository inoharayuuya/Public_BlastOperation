using Const;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class HomeCharaMove : MonoBehaviour
{
    // z座標の最小値、最大値
    private const float MIN_POS_Z = 300;
    private const float MAX_POS_Z = -300;
    // x座標の最大値、最小値
    private const float MIN_POS_X = 60;
    private const float MAX_POS_X = 660;
    // 暗さの最大値
    private const float MAX_DARK = 0.5f;

    // 色変更用
    private float col;

    #region 前バージョンの処理

    // 回転のクォータニオン
    Quaternion angleAxis;

    // 円運動の中心点
    [SerializeField] private Vector3 center;
    // 回転軸
    [SerializeField] private Vector3 axis = Vector3.up;
    // 一回転までの円運動周期
    [SerializeField] private float period;

    // 一度だけ実行用
    bool isOnce;

    // マネージャー取得
    HomeCharaManager hm;

    /// <summary>
    /// 画像の移動処理
    /// </summary>
    private void Move()
    {
        // 現在のポジション
        /*var pos = transform.position;

        // z座標を正規化
        normalizedZ = Common.NormalizedFunc(pos.z, MIN_POS_Z, MAX_POS_Z, 0, 1.0f);

        // フラグがオンの時に移動
        if (hm.isMove)
        {
            // 円運動の位置計算
            pos -= center;
            pos = angleAxis * pos;
            pos += center;
        }

        transform.position = pos;

        // 回転のクォータニオン作成
        angleAxis = Quaternion.AngleAxis(((360 / period) * hm.dir) * Time.deltaTime, axis);
        //angleAxis = Quaternion.AngleAxis(((360 * hm.dir) / period) * Time.deltaTime, axis);
        */
    }

    #endregion

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Init()
    {
        // カラーは最大値(白)
        col = 1f;
    }

    /// <summary>
    /// キャラ画像の明るさをz座標により変更する処理
    /// </summary>
    private void ChangeColor()
    {
        // カラーをz座標によって変更
        col = GetNormalizedZ(this.gameObject);

        // 暗さが一定に達していればそれ以上暗くしない
        if (col < MAX_DARK)
        {
            col = MAX_DARK;
        }

        // イメージコンポーネントのカラー取得、カラーを代入
        this.gameObject.GetComponent<Image>().color = new Color(col, col, col);
    }
    /// <summary>
    /// 画像の回転はせず、常に正面を向かせる処理
    /// </summary>
    private void MatchTheAngle()
    {
        // transformを取得
        Transform myTransform = this.transform;

        // ワールド座標を基準に、回転を取得
        Vector3 worldAngle = myTransform.eulerAngles;

        // y軸の回転は0で固定(正面を向く)
        worldAngle.y = 0;

        // 回転角度を設定
        myTransform.eulerAngles = worldAngle;
    }

    /// <summary>
    /// 正規化したz座標の値を返す処理
    /// </summary>
    /// <returns>正規化したz座標</returns>
    public float GetNormalizedZ(GameObject _obj)
    {
        // ベクター取得
        var pos = _obj.transform.position;
        //Debug.Log("posx = " + pos.x);

        // z座標を正規化
        var normalizedZ = Common.NormalizedFunc(pos.z, MIN_POS_Z, MAX_POS_Z, 0, 1.0f);

        // 正規化したz座標を返す
        return normalizedZ;
    }

    /// <summary>
    /// 正規化したx座標の値を返す処理
    /// </summary>
    /// <returns>正規化したz座標</returns>
    public float GetNormalizedX(GameObject _obj)
    {
        // ベクター取得
        var pos = _obj.transform.position;

        // z座標を正規化
        var normalizedX = Common.NormalizedFunc(pos.x, MIN_POS_X, MAX_POS_X, 0, 1.0f);

        // 正規化したz座標を返す
        return normalizedX;
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期化処理
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        var normalizedX = GetNormalizedX(this.gameObject);
        //.Log("norX " + normalizedX);


        // 角度変更処理
        MatchTheAngle();

        // 色の変更処理
        ChangeColor();
    }
}
