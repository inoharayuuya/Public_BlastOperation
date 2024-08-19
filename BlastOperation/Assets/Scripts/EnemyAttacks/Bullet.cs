using Const;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // 楕円型の弾


    [Tooltip("移動ベクトル")]
    private Vector3 moveVec;
    [Tooltip("移動速度")]
    private float moveSpeed;
    [Tooltip("自分のサイズ情報"), SerializeField]
    private Vector3 size;
    [Tooltip("前端の円")]
    private Transform forCirTr;
    [Tooltip("後端の円")]
    private Transform behCirTr;

    [Tooltip("プレイヤーの当たり判定を持つオブジェクト")]
    private Player player;

    [Tooltip("攻撃力")]
    private float atk;
    [Tooltip("攻撃属性")]
    private string attr;

    [Tooltip("画面右上の座標")]
    private Vector2 windowTopRightPos;
    [Tooltip("画面左下の座標")]
    private Vector2 windowBottomLeftPos;

    /// <summary>
    /// 
    /// </summary>
    public void Main()
    {

    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init(Vector3 _vec, float _ang, float _speed, float _atk, string _attr)
    {
        // 引数の受け取り
        //moveVec =  Quaternion.Euler(0, 0, _ang) * _vec.normalized;
        moveVec = _vec;
        moveSpeed = _speed;
        atk = _atk;
        attr = _attr;
        //print("受け取ったアングル：" + _ang);
        transform.rotation = Quaternion.Euler(0, 0, _ang);

        // 各変数の初期化
        player = GameObject.Find("PlayerBall").GetComponent<Player>();
        size = transform.GetChild(0).localScale * transform.localScale.x;
        forCirTr = transform.GetChild(1).GetChild(0);
        behCirTr = transform.GetChild(1).GetChild(1);

        // 画面サイズの取得
        windowBottomLeftPos = Camera.main.ViewportToWorldPoint(Vector2.zero);
        windowTopRightPos = Camera.main.ViewportToWorldPoint(Vector2.one);
    }

    // Update is called once per frame
    protected void Update()
    {
        // 移動処理
        Move();

        // 当たり判定処理
        if (Collision())
        {
            player.Damage(atk, attr);
            Destroy(gameObject);
            return;
        }

        // 画面内判定
        JudgeDisplay();

    }

    /// <summary>
    /// 移動処理、追従弾が派生クラスであるため、仮想関数
    /// </summary>
    protected virtual void Move()
    {
        transform.position += moveVec * moveSpeed * Time.deltaTime;
    }

    /// <summary>
    /// 弾の当たり判定は
    /// <para>線との距離+円二個で楕円を近似</para>
    /// <para>1.全体の円で仮近似、当たり判定</para>
    /// <para>2.円のどちらかと当たっているか</para>
    /// <para>3.線と当たり判定</para>
    /// </summary>
    protected bool Collision()
    {

        // 仮の当たり判定
        var dummyDif = ((Vector2)(player.transform.position - transform.position)).magnitude;
        //print("dif:" + dif);
        // 大まかな当たり判定で処理を省略
        if (dummyDif < size.y / 2 + Common.SIZE_PLAYER / 2)
        {
            // 当たる可能性あり
            //print("近い");
            // 弾の中部に当たっているか
            var for2Player = player.transform.position - forCirTr.position;
            var beh2Player = player.transform.position - behCirTr.position;


            var for2beh = forCirTr.position - behCirTr.position;

            // 外積計算に必要なベクトルを算出
            var cross = Mathf.Abs(Vector3.Cross(for2Player, for2beh.normalized).z);
            // 弾の中心の線からプレイヤーの距離
            if (cross < Common.SIZE_PLAYER / 2)
            {
                //print("中部にヒット");
                return true;
            }



            // 弾の前端か後端に当たっているかどうか
            var forDif = for2Player.magnitude;
            var behDif = beh2Player.magnitude;

            if (
                forDif < forCirTr.transform.localScale.x / 2 * transform.localScale.x + Common.SIZE_PLAYER / 2 ||
                behDif < behCirTr.transform.localScale.x / 2 * transform.localScale.y + Common.SIZE_PLAYER / 2
                )
            {
                print("前後方にヒット");
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 画面内にいるかどうか判定
    /// </summary>
    private void JudgeDisplay()
    {
        if(
            transform.position.y < windowBottomLeftPos.y ||
            transform.position.x < windowBottomLeftPos.x ||
            transform.position.y > windowTopRightPos.y   ||
            transform.position.x > windowTopRightPos.x
            )
        {
            Destroy(gameObject);
        }
    }
}
