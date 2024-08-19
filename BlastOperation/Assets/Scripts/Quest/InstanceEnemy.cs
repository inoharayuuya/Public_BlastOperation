using Const;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Json;

/// <summary>
/// ステージにインスタンスする敵クラス
/// </summary>
public class InstanceEnemy : MonoBehaviour, IDamageable
{
    /// <summary>
    /// 受けたダメージの表示用の列挙、増減方向を表す
    /// </summary>
    enum Value
    {
        PLUS = 1,
        MINUS = -1,
    }

    [Tooltip("水拡散の同時発射数")]
    private const int WATER_BULLET_NUM = 3;


    [Tooltip("ゲージ増減終了時の閾値")]
    private const float LIMIT_GAUGE = 0.01f;
    [Tooltip("アニメーター")]
    private Animator animator;
    [Tooltip("マネージャー")]
    private QuestManager qm;


    [Tooltip("体力"), SerializeField]
    private int hp;
    [Tooltip("最大体力"), SerializeField]
    private int hpMax;
    [Tooltip("このターン中に受けたダメージ")]
    private int dmgThisTime;
    [Tooltip("表示用のダメージ、徐々に増える")]
    private int dmgThisTimeDisplay;
    [Tooltip("ゲージの増減方向")]
    private Value dmgValue;
    [Tooltip("攻撃力"), SerializeField]
    private float atk;
    [Tooltip("攻撃パターン"), SerializeField]
    private int[] atkPtns;
    [Tooltip("弱点"), SerializeField]
    private string weak;
    [Tooltip("ボスかどうかのフラグ"), SerializeField]
    private bool isBoss;
    [Tooltip("このターン中に死んでいるかどうか")]
    private bool isDead;

    [Tooltip("自分の体のSr"), SerializeField]
    private GameObject bodySprite;
    [Tooltip("HPゲージのオブジェクト、緑"), SerializeField]
    private Image hpGaugeGr;
    [Tooltip("HPゲージのオブジェクト、赤"), SerializeField]
    private Image hpGaugeRd;
    [Tooltip("HPゲージのオブジェクト、黒"), SerializeField]
    private Image hpGaugeBk;
    [Tooltip("ダメージ表示用のオブジェクト"), SerializeField]
    private Text damageDisplayobj;
    [Tooltip("赤ゲージに変更が必要かどうかフラグ")]
    private bool isRdChanged;


    [Tooltip("プレイヤーデータ")]
    private Player playerBall;
    [Tooltip("攻撃ID:1のプレハブ")]
    private GameObject waterBulletPrefab;



    // Start is called before the first frame update
    void Start()
    {
        // 生成時のみの初期化処理
        animator = GetComponent<Animator>();
        qm = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        waterBulletPrefab = (GameObject)Resources.Load("Prefabs/EnemyAttackObjects/Bullet");
        playerBall = GameObject.Find("PlayerBall").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        if (dmgThisTime > 0)
        {
            // ダメージ数値の表示
            DamageValDisplay();
        }

        // 赤ゲージのfillAmountが緑と同じでない場合
        if (!isRdChanged)
        {
            // 赤ゲージを動かす
            HpGaugeSync();
        }
    }

    /// <summary>
    /// ダメージ処理、
    /// </summary>
    /// <param name="_atk">攻撃力</param>
    /// <param name="_attr">属性</param>
    public void Damage(float _atk, string _attr)
    {
        int damage;

        if (weak == _attr)
        {
            damage = (int)(_atk * Common.ATTR_WEAK_RATE);
        }
        else
        {
            damage = (int)_atk;
        }
        hp -= damage;


        // ダメージの表示
        /*
         * ・緑ゲージは当たった瞬間に切り替え
         * ・赤ゲージはターン終了時に徐々に緑に追いつく
         * ・数字は当たった瞬間からターン終了時まで徐々に増えてく
         */
        // ダメージ量の表示
        dmgThisTime += damage;
        // HPゲージの減少
        var amount = (float)(hp / (float)hpMax);
        hpGaugeGr.fillAmount = amount;


        if (hp <= 0)
        {
            hp = 0;

            // 死亡処理は後で呼ぶ
            // 死んでいることにする
            if (!isDead)
            {
                isDead = true;
                if (isBoss)
                {
                    qm.BossDestroyed();
                }
            }
        }

        animator.SetTrigger("Damage");

    }

    /// <summary>
    /// 敵データの初期化処理、フロア初期化時にqmから呼び出し
    /// </summary>
    public void EnemyInit(EnemyInitDatas _datas)
    {
        hp = _datas.hp;
        hpMax = hp;
        atk = _datas.atk;
        atkPtns = _datas.atk_ptns;
        transform.position = transform.root.position + _datas.pos;
        transform.localScale = Vector3.one * _datas.size;
        weak = _datas.wk;
        isBoss = _datas.isB;
        print("受け取ったパス:" + _datas.path);

        var tmp = Resources.Load<Sprite>(_datas.path);

        if (tmp != null)
        {
            print("取得できてる");
        }
        else
        {
            print("取得できてない");
        }

        bodySprite.GetComponent<SpriteRenderer>().sprite = tmp;

        DamageReset();
        isDead = false;
        isRdChanged = true;
        gameObject.SetActive(true);
        hpGaugeGr.fillAmount = 1;
        hpGaugeRd.fillAmount = 1;
        transform.localScale = Vector3.one;


    }

    /// <summary>
    /// ターン終了時の処理、ダメージの初期化と攻撃処理に入る
    /// </summary>
    public void EnemyTurnInit()
    {
        // ターン中ダメージのリセット
        DamageReset();
        // 赤ゲージ操作用の処理、緑ゲージとの差を見て増減量を記録
        if (hpGaugeGr.fillAmount != hpGaugeRd.fillAmount)
        {
            isRdChanged = false;
            if (hpGaugeGr.fillAmount > hpGaugeRd.fillAmount)
            {
                // 緑ゲージの方が大きいから、赤ゲージを増やす
                dmgValue = Value.PLUS;
            }
            else
            {
                // 赤ゲージの方が大きいから、赤ゲージを減らす
                dmgValue = Value.MINUS;
            }
        }

        // 攻撃処理
        StartCoroutine(Attack());
    }

    /// <summary>
    /// 攻撃処理
    /// </summary>
    private IEnumerator Attack()
    {
        // idの抽選
        var rndIndex = UnityEngine.Random.Range(0, atkPtns.Length);

        Vector2 toPlayerVec = (playerBall.transform.position - transform.position).normalized;

        // 抽選されたIDで行動を分岐

        var atkPtnId = atkPtns[rndIndex];
        var enemyAtk = GetJsonEnemyAttackMaster(atkPtnId);

        switch ((EnemyAttackPattern) atkPtnId)
        {

            //  1:水の弾、拡散
            case EnemyAttackPattern.BULLET_WATER_SPREAD:

                // 仮で定数
                var sp = 2;
                var atk = this.atk * enemyAtk.atk_rate;
                var attr = enemyAtk.atk_attr;
                yield return StartCoroutine(WaterBullet(toPlayerVec, sp, atk, attr));

                break;
            //  2:周囲に斬撃
            case EnemyAttackPattern.SLASH_AREA:
                break;
            //  3:ナイフ投げ
            case EnemyAttackPattern.KNIFE_THROW:
                break;
            //  4:火のブレス（小）
            case EnemyAttackPattern.BREATH_FIRE_SMALL:
                break;
            //  5:しっぽ薙ぎ払い（小）
            case EnemyAttackPattern.TALE_AREA_SMALL:
                break;
            //  6:火のブレス（大、薙ぎ払い）
            case EnemyAttackPattern.BREATH_FIRE_SWEEP_BIG:
                break;
            //  7:火のブレス（大、周囲）
            case EnemyAttackPattern.BREATH_FIRE_AROUND_BIG:
                break;
            //  8:しっぽ薙ぎ払い（大）
            case EnemyAttackPattern.TALE_AREA_BIG:
                break;
            //  9:回復（単体）
            case EnemyAttackPattern.HEAL_SINGLE:
                break;
            // 10:回復（全体）
            case EnemyAttackPattern.HEAL_MULTI:
                break;
            // 11:魔法の弾、追従
            case EnemyAttackPattern.BULLET_MAGIC_FOLLOW:
                break;
            default:
                print("エラー：データにない攻撃パターン");
                yield return null;
                break;
        }
    }

    /// <summary>
    /// idから攻撃マスターを取得する関数
    /// </summary>
    /// <param name="_id"></param>
    /// <returns></returns>
    private JsonEnemyAttackMaster GetJsonEnemyAttackMaster(int _id)
    {
        var masters = qm.GetJsonEnemyAttackMasters();
        for (int i = 0; i < masters.Length; i++)
        {
            if (masters[i] != null && masters[i].atk_id == _id)
            {
                return masters[i];
            }
        }

        return null;
    }


    private IEnumerator WaterBullet(Vector3 _target, float _sp, float _atk, string _attr)
    {
        int i = 0;
        // 発射方向にランダムを持たせる
        var rnd = UnityEngine.Random.Range(0, 10);
        // 何発か発射する
        while (i < WATER_BULLET_NUM)
        {
            for(int j = 0; j < WATER_BULLET_NUM; j++)
            {
                // 生成
                var bul = qm.EnemyAtkInst(waterBulletPrefab, transform.position, Quaternion.identity);

                // 角度の設定
                // 三方向の発射をするために角度を計算
                var angle = (j - (WATER_BULLET_NUM / 2)) * 10 + rnd;

                // 移動方向の傾け
                var moveVec = Quaternion.Euler(0, 0, angle) * _target.normalized;
                // オブジェクト自体の傾き計算
                var rotateAngle = GetAngleBetweenVectors(Vector2.up, moveVec);
                // ターゲットに傾ける
                bul.GetComponent<Bullet>().Init(moveVec, rotateAngle, _sp, _atk, _attr);
            }
            i++;
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// 二つのベクトルのなすオイラー角を符号付きで返す
    /// 負なら時計回り
    /// </summary>
    /// <param name="_from"></param>
    /// <param name="_to"></param>
    /// <returns></returns>
    private float GetAngleBetweenVectors(Vector2 _from, Vector2 _to)
    {
        _from = _from.normalized;
        _to = _to.normalized;

        // 内積計算
        float dot = Vector2.Dot(_from, _to);

        // cosXを求める
        // 値を-1から1に制限
        float cosAngle = Mathf.Clamp(dot, -1.0f, 1.0f);

        // ラジアン角の計算
        float angleRad = Mathf.Acos(cosAngle);

        // オイラー角の計算
        float angleDeg = angleRad * Mathf.Rad2Deg;


        // 符号の判定
        float crossZ = _from.x * _to.y - _from.y * _to.x;
        if (crossZ < 0)
        {
            angleDeg = -angleDeg;
        }

        return　angleDeg;
    }



    /// <summary>
    /// ダメージの表示初期化処理
    /// </summary>
    private void DamageReset()
    {
        dmgThisTime = 0;
        dmgThisTimeDisplay = 0;
        damageDisplayobj.text = "";
    }

    /// <summary>
    /// 赤ゲージを徐々に緑に近づける処理
    /// </summary>
    private void HpGaugeSync()
    {
        hpGaugeRd.fillAmount += Time.deltaTime * (int)dmgValue;

        var dif = hpGaugeGr.fillAmount - hpGaugeRd.fillAmount;
        var lim = LIMIT_GAUGE * (int)dmgValue;

        if (dmgValue == Value.PLUS)
        {
            if (dif < lim)
            {
                hpGaugeRd.fillAmount = hpGaugeGr.fillAmount;
            }
        }
        else
        {
            if (dif > -lim)
            {
                hpGaugeRd.fillAmount = hpGaugeGr.fillAmount;
            }
        }


        // ゲージが同期したら終了
        if (hpGaugeGr.fillAmount == hpGaugeRd.fillAmount)
        {
            isRdChanged = true;
        }
    }

    /// <summary>
    /// ダメージを数値として表示する
    /// 実際の数値まで上昇するように表示
    /// </summary>
    private void DamageValDisplay()
    {
        // 表示している数字と実際の値を比較して加算する
        if (dmgThisTime > dmgThisTimeDisplay)
        {
            var tmp = (int)(dmgThisTime * Time.deltaTime);
            if(tmp < 1)
            {
                tmp = 1;
            }
            dmgThisTimeDisplay += tmp;
        }
        else
        {
            dmgThisTimeDisplay = dmgThisTime;
        }
        // ダメージを表示
        damageDisplayobj.text = dmgThisTimeDisplay.ToString("N0");
    }

    /// <summary>
    /// 死んでいるかどうかを取得
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead()
    {
        return isDead;
    }

    /// <summary>
    /// 死亡処理
    /// </summary>
    public IEnumerator Dead()
    {
        // ダメージの表示が間に合っていないと待機
        while (dmgThisTimeDisplay != dmgThisTime)
        {
            yield return null;
        }

        // 死亡アニメーションの発火
        animator.SetTrigger("Dead");

        // 自分のアクティブを確認して処理を終了する
        while (true)
        {
            if(gameObject.activeSelf)
            {
                yield return null;
            }
            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 死亡アニメーション終了時処理
    /// アニメーションイベントから呼び出し
    /// </summary>
    public void DeadAnimationEnd()
    {
        gameObject.SetActive(false);
    }


}
