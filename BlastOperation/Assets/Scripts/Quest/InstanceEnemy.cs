using Const;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Json;

/// <summary>
/// �X�e�[�W�ɃC���X�^���X����G�N���X
/// </summary>
public class InstanceEnemy : MonoBehaviour, IDamageable
{
    /// <summary>
    /// �󂯂��_���[�W�̕\���p�̗񋓁A����������\��
    /// </summary>
    enum Value
    {
        PLUS = 1,
        MINUS = -1,
    }

    [Tooltip("���g�U�̓������ː�")]
    private const int WATER_BULLET_NUM = 3;


    [Tooltip("�Q�[�W�����I������臒l")]
    private const float LIMIT_GAUGE = 0.01f;
    [Tooltip("�A�j���[�^�[")]
    private Animator animator;
    [Tooltip("�}�l�[�W���[")]
    private QuestManager qm;


    [Tooltip("�̗�"), SerializeField]
    private int hp;
    [Tooltip("�ő�̗�"), SerializeField]
    private int hpMax;
    [Tooltip("���̃^�[�����Ɏ󂯂��_���[�W")]
    private int dmgThisTime;
    [Tooltip("�\���p�̃_���[�W�A���X�ɑ�����")]
    private int dmgThisTimeDisplay;
    [Tooltip("�Q�[�W�̑�������")]
    private Value dmgValue;
    [Tooltip("�U����"), SerializeField]
    private float atk;
    [Tooltip("�U���p�^�[��"), SerializeField]
    private int[] atkPtns;
    [Tooltip("��_"), SerializeField]
    private string weak;
    [Tooltip("�{�X���ǂ����̃t���O"), SerializeField]
    private bool isBoss;
    [Tooltip("���̃^�[�����Ɏ���ł��邩�ǂ���")]
    private bool isDead;

    [Tooltip("�����̑̂�Sr"), SerializeField]
    private GameObject bodySprite;
    [Tooltip("HP�Q�[�W�̃I�u�W�F�N�g�A��"), SerializeField]
    private Image hpGaugeGr;
    [Tooltip("HP�Q�[�W�̃I�u�W�F�N�g�A��"), SerializeField]
    private Image hpGaugeRd;
    [Tooltip("HP�Q�[�W�̃I�u�W�F�N�g�A��"), SerializeField]
    private Image hpGaugeBk;
    [Tooltip("�_���[�W�\���p�̃I�u�W�F�N�g"), SerializeField]
    private Text damageDisplayobj;
    [Tooltip("�ԃQ�[�W�ɕύX���K�v���ǂ����t���O")]
    private bool isRdChanged;


    [Tooltip("�v���C���[�f�[�^")]
    private Player playerBall;
    [Tooltip("�U��ID:1�̃v���n�u")]
    private GameObject waterBulletPrefab;



    // Start is called before the first frame update
    void Start()
    {
        // �������݂̂̏���������
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
            // �_���[�W���l�̕\��
            DamageValDisplay();
        }

        // �ԃQ�[�W��fillAmount���΂Ɠ����łȂ��ꍇ
        if (!isRdChanged)
        {
            // �ԃQ�[�W�𓮂���
            HpGaugeSync();
        }
    }

    /// <summary>
    /// �_���[�W�����A
    /// </summary>
    /// <param name="_atk">�U����</param>
    /// <param name="_attr">����</param>
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


        // �_���[�W�̕\��
        /*
         * �E�΃Q�[�W�͓��������u�Ԃɐ؂�ւ�
         * �E�ԃQ�[�W�̓^�[���I�����ɏ��X�ɗ΂ɒǂ���
         * �E�����͓��������u�Ԃ���^�[���I�����܂ŏ��X�ɑ����Ă�
         */
        // �_���[�W�ʂ̕\��
        dmgThisTime += damage;
        // HP�Q�[�W�̌���
        var amount = (float)(hp / (float)hpMax);
        hpGaugeGr.fillAmount = amount;


        if (hp <= 0)
        {
            hp = 0;

            // ���S�����͌�ŌĂ�
            // ����ł��邱�Ƃɂ���
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
    /// �G�f�[�^�̏����������A�t���A����������qm����Ăяo��
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
        print("�󂯎�����p�X:" + _datas.path);

        var tmp = Resources.Load<Sprite>(_datas.path);

        if (tmp != null)
        {
            print("�擾�ł��Ă�");
        }
        else
        {
            print("�擾�ł��ĂȂ�");
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
    /// �^�[���I�����̏����A�_���[�W�̏������ƍU�������ɓ���
    /// </summary>
    public void EnemyTurnInit()
    {
        // �^�[�����_���[�W�̃��Z�b�g
        DamageReset();
        // �ԃQ�[�W����p�̏����A�΃Q�[�W�Ƃ̍������đ����ʂ��L�^
        if (hpGaugeGr.fillAmount != hpGaugeRd.fillAmount)
        {
            isRdChanged = false;
            if (hpGaugeGr.fillAmount > hpGaugeRd.fillAmount)
            {
                // �΃Q�[�W�̕����傫������A�ԃQ�[�W�𑝂₷
                dmgValue = Value.PLUS;
            }
            else
            {
                // �ԃQ�[�W�̕����傫������A�ԃQ�[�W�����炷
                dmgValue = Value.MINUS;
            }
        }

        // �U������
        StartCoroutine(Attack());
    }

    /// <summary>
    /// �U������
    /// </summary>
    private IEnumerator Attack()
    {
        // id�̒��I
        var rndIndex = UnityEngine.Random.Range(0, atkPtns.Length);

        Vector2 toPlayerVec = (playerBall.transform.position - transform.position).normalized;

        // ���I���ꂽID�ōs���𕪊�

        var atkPtnId = atkPtns[rndIndex];
        var enemyAtk = GetJsonEnemyAttackMaster(atkPtnId);

        switch ((EnemyAttackPattern) atkPtnId)
        {

            //  1:���̒e�A�g�U
            case EnemyAttackPattern.BULLET_WATER_SPREAD:

                // ���Œ萔
                var sp = 2;
                var atk = this.atk * enemyAtk.atk_rate;
                var attr = enemyAtk.atk_attr;
                yield return StartCoroutine(WaterBullet(toPlayerVec, sp, atk, attr));

                break;
            //  2:���͂Ɏa��
            case EnemyAttackPattern.SLASH_AREA:
                break;
            //  3:�i�C�t����
            case EnemyAttackPattern.KNIFE_THROW:
                break;
            //  4:�΂̃u���X�i���j
            case EnemyAttackPattern.BREATH_FIRE_SMALL:
                break;
            //  5:�����ۓガ�����i���j
            case EnemyAttackPattern.TALE_AREA_SMALL:
                break;
            //  6:�΂̃u���X�i��A�ガ�����j
            case EnemyAttackPattern.BREATH_FIRE_SWEEP_BIG:
                break;
            //  7:�΂̃u���X�i��A���́j
            case EnemyAttackPattern.BREATH_FIRE_AROUND_BIG:
                break;
            //  8:�����ۓガ�����i��j
            case EnemyAttackPattern.TALE_AREA_BIG:
                break;
            //  9:�񕜁i�P�́j
            case EnemyAttackPattern.HEAL_SINGLE:
                break;
            // 10:�񕜁i�S�́j
            case EnemyAttackPattern.HEAL_MULTI:
                break;
            // 11:���@�̒e�A�Ǐ]
            case EnemyAttackPattern.BULLET_MAGIC_FOLLOW:
                break;
            default:
                print("�G���[�F�f�[�^�ɂȂ��U���p�^�[��");
                yield return null;
                break;
        }
    }

    /// <summary>
    /// id����U���}�X�^�[���擾����֐�
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
        // ���˕����Ƀ����_������������
        var rnd = UnityEngine.Random.Range(0, 10);
        // ���������˂���
        while (i < WATER_BULLET_NUM)
        {
            for(int j = 0; j < WATER_BULLET_NUM; j++)
            {
                // ����
                var bul = qm.EnemyAtkInst(waterBulletPrefab, transform.position, Quaternion.identity);

                // �p�x�̐ݒ�
                // �O�����̔��˂����邽�߂Ɋp�x���v�Z
                var angle = (j - (WATER_BULLET_NUM / 2)) * 10 + rnd;

                // �ړ������̌X��
                var moveVec = Quaternion.Euler(0, 0, angle) * _target.normalized;
                // �I�u�W�F�N�g���̂̌X���v�Z
                var rotateAngle = GetAngleBetweenVectors(Vector2.up, moveVec);
                // �^�[�Q�b�g�ɌX����
                bul.GetComponent<Bullet>().Init(moveVec, rotateAngle, _sp, _atk, _attr);
            }
            i++;
            yield return new WaitForSeconds(0.3f);
        }
    }

    /// <summary>
    /// ��̃x�N�g���̂Ȃ��I�C���[�p�𕄍��t���ŕԂ�
    /// ���Ȃ玞�v���
    /// </summary>
    /// <param name="_from"></param>
    /// <param name="_to"></param>
    /// <returns></returns>
    private float GetAngleBetweenVectors(Vector2 _from, Vector2 _to)
    {
        _from = _from.normalized;
        _to = _to.normalized;

        // ���όv�Z
        float dot = Vector2.Dot(_from, _to);

        // cosX�����߂�
        // �l��-1����1�ɐ���
        float cosAngle = Mathf.Clamp(dot, -1.0f, 1.0f);

        // ���W�A���p�̌v�Z
        float angleRad = Mathf.Acos(cosAngle);

        // �I�C���[�p�̌v�Z
        float angleDeg = angleRad * Mathf.Rad2Deg;


        // �����̔���
        float crossZ = _from.x * _to.y - _from.y * _to.x;
        if (crossZ < 0)
        {
            angleDeg = -angleDeg;
        }

        return�@angleDeg;
    }



    /// <summary>
    /// �_���[�W�̕\������������
    /// </summary>
    private void DamageReset()
    {
        dmgThisTime = 0;
        dmgThisTimeDisplay = 0;
        damageDisplayobj.text = "";
    }

    /// <summary>
    /// �ԃQ�[�W�����X�ɗ΂ɋ߂Â��鏈��
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


        // �Q�[�W������������I��
        if (hpGaugeGr.fillAmount == hpGaugeRd.fillAmount)
        {
            isRdChanged = true;
        }
    }

    /// <summary>
    /// �_���[�W�𐔒l�Ƃ��ĕ\������
    /// ���ۂ̐��l�܂ŏ㏸����悤�ɕ\��
    /// </summary>
    private void DamageValDisplay()
    {
        // �\�����Ă��鐔���Ǝ��ۂ̒l���r���ĉ��Z����
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
        // �_���[�W��\��
        damageDisplayobj.text = dmgThisTimeDisplay.ToString("N0");
    }

    /// <summary>
    /// ����ł��邩�ǂ������擾
    /// </summary>
    /// <returns></returns>
    public bool GetIsDead()
    {
        return isDead;
    }

    /// <summary>
    /// ���S����
    /// </summary>
    public IEnumerator Dead()
    {
        // �_���[�W�̕\�����Ԃɍ����Ă��Ȃ��Ƒҋ@
        while (dmgThisTimeDisplay != dmgThisTime)
        {
            yield return null;
        }

        // ���S�A�j���[�V�����̔���
        animator.SetTrigger("Dead");

        // �����̃A�N�e�B�u���m�F���ď������I������
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
    /// ���S�A�j���[�V�����I��������
    /// �A�j���[�V�����C�x���g����Ăяo��
    /// </summary>
    public void DeadAnimationEnd()
    {
        gameObject.SetActive(false);
    }


}
