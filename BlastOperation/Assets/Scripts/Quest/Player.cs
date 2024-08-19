using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;
using Json;

/// <summary>
/// �N�G�X�g�����삷��v���C���[�i�{�[���j
/// </summary>
public class Player : MonoBehaviour, IDamageable
{
    [Tooltip("�ǂ̌���")]
    private const float WALL_WIDTH = 1.0f;

    public enum PlayerState
    {
        /// <summary>���j���[���J���Ă���</summary>
        PAUSE,
        /// <summary>�t���A�ړ��҂�</summary>
        WAIT_FLOOR,
        /// <summary>�v���C���[�̓��͑҂�</summary>
        WAIT_INPUT,
        /// <summary>���ˌ�A��~�҂�</summary>
        MOVE,
        /// <summary>���ˌ�A��~�҂�</summary>
        STOP,
        /// <summary>�G�̏����҂�</summary>
        WAIT_ENEMY,
        /// <summary>�s�k��</summary>
        DEFEAT,
    }


    /// <summary>
    /// �l�����̕ǂ̖@���x�N�g���AU,D,L,R�̏��Ɋi�[
    /// </summary>
    private readonly Vector3[] NORMAL_VECTOR =
    {
        Vector3.down,
        Vector3.up,
        Vector3.right,
        Vector3.left
    };

    /// <summary>
    /// �l�����̕�
    /// U,D,L,R�̏��Ɋi�[
    /// </summary>
    [SerializeField]
    private GameObject[] walls;

    // �R���|�[�l���g
    private QuestManager qm;
    private SpriteRenderer spriteSr;




    [Tooltip("���˕����̖��")]
    private Transform moveArrow;
    [Tooltip("���̑傫���̌��E�l")]
    private const float ARROW_MAX_SIZE = 3.3f;
    [Tooltip("���������������ɉ����đ��x�ɂ�����{��")]
    private float pullRate;
    [Tooltip("�^�b�v�J�n�n�_")]
    private Vector3 tapStPos;
    [Tooltip("�^�b�v�I���n�_")]
    private Vector3 tapEdPos;
    [Tooltip("�^���x�N�g���i�P�ʃx�N�g���j")]
    private Vector3 moveNmVec;
    [Tooltip("�^���x�N�g���ɂ�����傫��")]
    private const float MOVE_SPEED = 10;
    [Tooltip("�^�b�v�J�n�������ǂ����̃t���O")]
    public bool isTap { get; private set; }
    [Tooltip("�v���C���[�̏�ԕϐ�")]
    public PlayerState state {  get; private set; }
    // �����x�ɉ����{���A���������������ɉ����ĕϓ�
    [Tooltip("�����x�̔{���̍ŏ��l")]
    private const float START_MOVE_SPEED_MIN = 0.8f;
    [Tooltip("�����x�̔{���̍ő�l")]
    private const float START_MOVE_SPEED_MAX = 1.25f;
    [Tooltip("���݂̉����x�A1����0�܂łŕω�")]
    private float accelarate;


    [Header("�����Ɋւ���萔")]

    [Tooltip("�ړ�����ɂ����錸��")]
    private const float DEC_RATE_MOVE = 0.88f;
    [Tooltip("�{�X���j���ɂ����錸��")]
    private const float DEC_RATE_BOSS = 0.2f;
    [Tooltip("�~�܂钼�O�ɂ����錸��")]
    private const float DEC_RATE_STOP = 0.55f;
    [Tooltip("�ǂɓ����������ɂ����錸��")]
    private const float DEC_RATE_HIT_WALL = 0.90f;
    [Tooltip("�G�Ɠ����������ɂ����錸��")]
    private const float DEC_RATE_HIT_ENEMY = 0.80f;


    [Header("��~�̊���x")]

    [Tooltip("�{�[������~���n�߂鑬�x")]
    private const float LIMIT_DEC = 0.1f;
    [Tooltip("�{�[������~���鑬�x")]
    private const float LIMIT_STOP = 0.01f;
    [Tooltip("���݃t���A�ɂ���G�̔z��"), SerializeField]
    private InstanceEnemy[] enemies;
    [Tooltip("�S�Ŏ��̒x������")]
    private const float DELAY_NEXT_TIME = 0.2f;


    [Header("�v���C���[�̉摜")]

    [Tooltip("�U������ɕK�v�ȃf�[�^")]
    private CharaData charaData;
    [Tooltip("���ݑI������Ă���L�����́A���ݑI������Ă��镐��̉摜")]
    private SpriteRenderer weaponSr;

    // Start is called before the first frame update
    void Start()
    {
        // ������
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if (!qm.isPause)
        {
            // ���ˑO
            if (state == PlayerState.WAIT_INPUT)
            {
                // ���͎�t
                GetOperation();

                // ���̕\��
                ArrowDisp();
            }

            // ���ˌ�
            if (state == PlayerState.MOVE)
            {
                // �ړ�����
                Move();
            }
        }
    }

    /// <summary>
    /// ����������
    /// </summary>
    private void Init()
    {
        // ��ԕϐ�������
        state = PlayerState.WAIT_FLOOR;
        // �^�b�v����ĂȂ�
        isTap = false;
        // �����x
        accelarate = 1;
        // ���������������̔{��
        pullRate = 1;
        // �^�b�v�J�n�A�I���n�_
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;

        // �ʂ̉摜�R���|�[�l���g�擾
        spriteSr = transform.GetChild(1).GetComponent<SpriteRenderer>();
        // �}�l�[�W���[�擾
        qm = GameObject.Find("QuestManager").GetComponent<QuestManager>();
        // ���˕����\���̃I�u�W�F�N�g�擾
        moveArrow = transform.GetChild(0);
        moveArrow.gameObject.SetActive(false);

        // �����̃T�C�Y���擾
        transform.GetChild(1).localScale = Vector3.one * Common.SIZE_PLAYER;

        // ����̉摜�擾
        weaponSr = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// �v���C���[�̑�����󂯎��A���ȏ���������Ă����甭��
    /// </summary>
    private void GetOperation()
    {
        // �Q�[���t�F�[�Y�ȊO�œ��͂��󂯕t���Ȃ�
        if (qm.Fase != QuestManager.GameFase.GAMING)
        {
            return;
        }

        // �_�����߂�
        if (Input.GetMouseButtonDown(0))
        {
            tapStPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            isTap = true;
        }

        // �^�b�v�J�n���ƏI�����̃x�N�g���̑傫���𔻒�
        if (Input.GetMouseButtonUp(0))
        {
            // �^�b�v�I��
            isTap = false;
            // ���W�L�^
            tapEdPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // �ړ������̌���
            moveNmVec = (tapStPos - tapEdPos);

            // ���ȏ�傫���̂Ŕ���
            if (moveNmVec.magnitude > Common.TAP_VECTOR_LIMIT)
            {
                // �����������瓮��
                // ���������������̔{���������������x��^����
                accelarate = 1 * pullRate;
                // �ړ��x�N�g���̐��K��
                moveNmVec = moveNmVec.normalized;
                // ��Ԃ��X�V
                state = PlayerState.MOVE;
                // ���̐F�ύX
                spriteSr.color = Color.white;
                enemies = qm.GetInstanceEnemys();
                // ���j���[�{�^���̔�\��
                qm.MenuButtonDisable();
            }
            else
            {
                //print("���˃L�����Z��");
                //qm.AttackUIEnable(true);
            }
        }
    }

    /// <summary>
    /// �ړ������A�����Ɣ��ˁA�U�����s��
    /// </summary>
    private void Move()
    {
        // �����A��~����
        StartCoroutine(Decelarating());

        // �ǂƂ̓����蔻��
        JudgeWallHit();

        // �G�Ƃ̓����蔻��
        JudgeEnemyHit();
    }

    /// <summary>
    /// ���X�Ɍ������A���ȏ�x����Β�~���鏈��
    /// </summary>
    private IEnumerator Decelarating()
    {
        var tmpVec = moveNmVec * charaData.speed * Time.deltaTime * accelarate;

        // �Ȃ߂炩�ɒ�~���������̂ň��̑��x�ȉ��ɂȂ�ƌ������㏸

        float rate = 0;
        // �{�X���|�ꂽ
        if (qm.GetIsBossDestroyed())
        {

            // ������~
            if (tmpVec.magnitude < LIMIT_STOP)
            {
                //Stop(true);
                //return;
                //print("�{�X�|���Ă���");
                yield return StartCoroutine(Stop(true));

            }
            rate = DEC_RATE_BOSS;

        }
        else
        {
            // �ʏ펞�̌���
            if (tmpVec.magnitude > LIMIT_DEC)
            {
                rate = DEC_RATE_MOVE;
            }
            // ���ȏ�x���Ȃ�ƒ�~���n�߂邽�߂ɑ傫�߂Ɍ���
            else if (tmpVec.magnitude > LIMIT_STOP)
            {
                rate = DEC_RATE_STOP;
            }
            // ������~
            else
            {
                // ��Ԃ̍X�V
                state = PlayerState.STOP;
                StartCoroutine(Stop());
            }
        }

        // ���X�Ɍ�������
        // ��b�Ԃɑ��x��rate�{�ɂȂ�悤�Ƀt���[���ɒ����Čv�Z
        // ���t���[�����l�������Ă���
        accelarate *= Mathf.Pow(rate, Time.deltaTime);

        //print(tmpVec.magnitude);
        transform.position += tmpVec;
    }

    /// <summary>
    /// �e���~�܂鏈��
    /// </summary>
    private IEnumerator Stop(bool _isBossDes = false)
    {
        //print("������~");

        // �t���A�{�X��|�����ꍇ�́A�G���ׂĂ̎��S�������Ă�Ŏ��̃t���A��
        if (_isBossDes)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                // ���S����
                if (enemies[i].gameObject.activeSelf != false)
                {
                    enemies[i].Dead();
                }
            }

            // ����������
            PlayerTurnEnd();

            // �x��������
            yield return StartCoroutine(Common.Delay(DELAY_NEXT_TIME, qm.FloorToNext));
        }
        // �{�X�͓|���Ă��Ȃ�
        else
        {
            // �G���S�ł������ǂ�������
            bool isAllDown = true;

            for (int i = 0; i < enemies.Length; i++)
            {
                // �A�N�e�B�u�ȓG�����遁�܂��|���؂�Ă��Ȃ�
                if (enemies[i].gameObject.activeSelf)
                {
                    if (enemies[i].GetIsDead())
                    {
                        yield return StartCoroutine(enemies[i].Dead());
                    }
                    else
                    {
                        // �S�Ńt���O���I�t
                        isAllDown = false;
                    }
                }
            }
            // �����ҋ@

            // �G���S�ł��Ă����玟�̃t���A
            if (isAllDown)
            {
                //print("���̃t���A�ֈړ�");

                // ������
                PlayerTurnEnd();

                // �x��������
                StartCoroutine(Common.Delay(DELAY_NEXT_TIME, qm.FloorToNext));
            }
            else
            {
                state = PlayerState.WAIT_ENEMY;
                // �G�̃^�[���I�����̏����AHP�̕\���X�V�ƍU��
                for (int i = 0; i < enemies.Length; i++)
                {
                    if (enemies[i].gameObject.activeSelf)
                    {
                        // ��̂��Ăяo��
                        enemies[i].EnemyTurnInit();
                    }
                }

                // �G�̍s�����I���܂łȂɂ����Ȃ�

                yield return new WaitForSeconds(3f);

                // �G�̍U�����I�������̂�
                EnemyActionEnd();
            }
        }
    }

    /// <summary>
    /// �G�̍U�����������������Ƃ��̏���
    /// </summary>
    public void EnemyActionEnd()
    {
        // ���̍X�V
        PlayerTurnEnd();
        qm.PlayerInfoUpdate();

        // ���j���[�{�^���\��
        qm.MenuButtonEnable();
    }

    /// <summary>
    /// �^�[���I�����̏���
    /// �J�����ړ������A�s�k�̔���
    /// </summary>
    private void PlayerTurnEnd()
    {

        if (qm.GetIsDefeat())
        {
            //�s�k����
            qm.GameOver();
            state = PlayerState.DEFEAT;
            return;
        }

        // ��Ԃ̍X�V
        state = PlayerState.WAIT_FLOOR;

        // ���Ŕ��ˏ�Ԃ�����
        spriteSr.color = Color.green;
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;

        // �ړ��I���A�^�[�����̉��Z
        qm.PlayerTurnEnd();
    }

    /// <summary>
    /// �ǂƓ������Ĕ��˂��邩����
    /// </summary>
    private void JudgeWallHit()
    {
        // �ǂƂ̓����蔻��Ɣ���
        // �Q�l�T�C�g�Fhttps://nn-hokuson.hatenablog.com/entry/2018/03/30/201715

        // �����蔻�菈���A�l�����̕ǂ��ꂼ��𔻒�
        for (int i = 0; i < walls.Length; i++)
        {
            // �������Ă�����true
            if (MoveCircle2WallHit(moveNmVec, transform.position, Common.SIZE_PLAYER, NORMAL_VECTOR[i], walls[i].transform.position, WALL_WIDTH))
            {
                // ���˂���
                Reflect(NORMAL_VECTOR[i], ref moveNmVec);
                accelarate *= DEC_RATE_HIT_WALL;
            }
        }
    }

    /// <summary>
    /// �G�Ɠ������Ĕ��˂��邩����
    /// �~�Ɖ~�̓����蔻��
    /// </summary>
    private void JudgeEnemyHit()
    {
        // �G�̏��擾���āA�G�Ɣ��˂���
        for (int i = 0; i < enemies.Length; i++)
        {
            // �C���X�^���X�G�l�~�[���ǂ���
            if (enemies[i].TryGetComponent(out InstanceEnemy component))
            {
                //print("�G�l�~�[��");
                // null�`�F�b�N�ƗL�����ǂ����ƁA
                if (enemies[i] != null && enemies[i].gameObject.activeSelf == true)
                {
                    // �����Ɣ��a�̘a���r
                    if (MoveCircle2CircleHit(transform, Common.SIZE_PLAYER, moveNmVec, enemies[i].transform, enemies[i].transform.localScale.x, out var diff))
                    {
                        // �G�̖@���x�N�g����diff�̑傫�����P�̃x�N�g��
                        Reflect(diff.normalized, ref moveNmVec);

                        // �U������
                        // �������񕜂Ȃ�񕜂̏���
                        if (charaData.attr == Common.ATTR_HEAL)
                        {
                            //�񕜏���
                            qm.PlayerHeal(charaData.atk);

                            //print("�񕜂���!");
                        }
                        // �_���[�W����
                        else if (charaData.attr == Common.ATTR_MAGICAL || charaData.attr == Common.ATTR_PHYSICAL)
                        {
                            //�U���͂������œn��
                            component.Damage(charaData.atk, charaData.attr);
                        }

                        // Hit���̉��Z
                        qm.Hit();
                        accelarate *= DEC_RATE_HIT_ENEMY;
                    }
                    else
                    {
                        //print("����");

                    }
                }
            }
        }
    }

    /// <summary>
    /// ���˕������������̕\��
    /// </summary>
    private void ArrowDisp()
    {
        // �i�s�����x�N�g�����擾
        var tmpMoveVec = tapStPos - Camera.main.ScreenToWorldPoint(Input.mousePosition);
        // ���ˎ��Ɠ��������ŕ\���̃I���I�t��؂�ւ�
        // ���˂����Ȃ�\�����v�Z
        if (isTap && tmpMoveVec.magnitude > Common.TAP_VECTOR_LIMIT)
        {
            // x�̃v���X������0�x�Ƃ��A�Ȃ��p���Z�o
            var tmpAng = Mathf.Atan2(tmpMoveVec.y, tmpMoveVec.x) * Mathf.Rad2Deg;

            // y�̃v���X��������ɂ������̂Œl���C��
            tmpAng -= 90f;
            //print("ang�F" + tmpAng);
            moveArrow.transform.rotation = Quaternion.Euler(0, 0, tmpAng);
            moveArrow.gameObject.SetActive(true);

            // ���̑傫����ύX���ă^�b�v�J�n�n�_���킩��₷������
            var tmpVecMag = tmpMoveVec.magnitude;
            // �傫���𐳋K��
            if (tmpVecMag > ARROW_MAX_SIZE)
            {
                tmpVecMag = ARROW_MAX_SIZE;
            }
            // ���̑傫����ύX
            var tmp = moveArrow.transform.localScale;
            tmp.y = tmpVecMag;
            moveArrow.transform.localScale = tmp;
            // �����x�ɗ^����{���̌v�Z
            // START_MOVE_SPEED_MAX�`START_MOVE_SPEED_MIN�Ő��K��
            var v0 = Common.NormalizedFunc(tmpVecMag, 0, ARROW_MAX_SIZE, START_MOVE_SPEED_MIN, START_MOVE_SPEED_MAX);
            //print("���x�{���F" + v0);
            pullRate = v0;
        }
        // ���˂���Ȃ��̂Ŗ��̕\�������Ȃ�
        else
        {
            moveArrow.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 2�̃x�N�g�������������Ă��邩�ǂ�����Ԃ�
    /// </summary>
    private bool IsVectorOpposite(Vector3 _v1, Vector3 _v2)
    {
        return (Vector3.Dot(_v1, _v2) > 0);
    }

    /// <summary>
    /// ���݂����ǂƓ����~�̓����蔻��̏���
    /// </summary>
    /// <param name="_wallNmUnVec">�ǂ̔��˖ʂɑ΂���@���x�N�g���̒P�ʃx�N�g��</param>
    /// <param name="_wallPos">�ǂ̍��W</param>
    /// <param name="_wallWid">�ǂ̌���</param>
    /// <param name="_crMvNmUnVec">�����~�̉^���x�N�g���̒P�ʃx�N�g��</param>
    /// <param name="_crPos">�����~�̍��W</param>
    /// <param name="_crWid">�~�̒��a</param>
    /// <returns></returns>
    private bool MoveCircle2WallHit(Vector3 _crMvNmUnVec, Vector3 _crPos, float _crWid, Vector3 _wallNmUnVec, Vector3 _wallPos, float _wallWid)
    {
        // �����鑊��̖@���x�N�g���Ǝ����̉^���x�N�g���̓��ς�0���傫��
        // �i�x�N�g�����m�����������Ă��Ȃ��j�Ȃ瓖���蔻��͎��Ȃ�
        if (IsVectorOpposite(_wallNmUnVec, _crMvNmUnVec))
        {
            return false;
        }

        // �ǂƃ{�[���̋������v�Z
        var diff = _crPos - _wallPos;
        // ���ς��v�Z���A�{�[������ǂ̐����̋������v�Z
        // �ǂ̌��݂ƃv���C���[�̃T�C�Y���Q��
        float h = Vector3.Dot(diff, _wallNmUnVec);


        var r = (_crWid / 2) + (_wallWid / 2);

        // �~�̔��a�ƕǂ̌��݂𑫂����������Z����Γ�������
        return (h - r < 0);
    }

    /// <summary>
    /// �����~�Ɖ~�̓����蔻��
    /// </summary>
    /// <param name="_circle">�����~��Transform</param>
    /// <param name="_cSize">�����~�̒��a</param>
    /// <param name="_cMoveNmVec">�����~�̉^���x�N�g���̒P�ʃx�N�g��</param>
    /// <param name="_target">����̉~</param>
    /// <param name="_tSize">����̉~�̒��a</param>
    /// <param name="diff">���˖ʂ̖@���x�N�g��</param>
    /// <returns></returns>
    private bool MoveCircle2CircleHit(Transform _circle, float _cSize, Vector3 _cMoveNmVec, Transform _target, float _tSize, out Vector3 diff)
    {
        // �G�ƍ��W���r
        diff = (_circle.position - _target.position);
        // �����蔻���2D�̂���z������
        diff.z = 0;

        // �����鑊��̖@���x�N�g���Ǝ����̉^���x�N�g���̓��ς�0���傫��
        // �i�x�N�g�����m�����������Ă��Ȃ��j�Ȃ瓖���蔻��͎��Ȃ�
        if (IsVectorOpposite(diff.normalized, _cMoveNmVec))
        {
            return false;
        }

        // �G�Ǝ����̔��a�̘a
        var r = (_tSize / 2) + (_cSize / 2);

        // �����Ɣ��a�̘a���r
        // �}�C�i�X�Ȃ瓖���蔻�蔭��
        return (diff.magnitude - r < 0);
    }

    /// <summary>
    /// ���ˏ���,�����̉^���x�N�g���𔽎ˌ�̌����ɕς���
    /// </summary>
    /// <param name="_normal"></param>
    /// <param name="_moveVec"></param>
    private void Reflect(Vector3 _normal, ref Vector3 _moveVec)
    {
        // ������������̖@���x�N�g��
        Vector3 n = _normal;
        // �ړ��x�N�g���́A���˖�(�_)�ɐ����Ȑ����̎Z�o
        float h = Mathf.Abs(Vector3.Dot(_moveVec, n));


        // ���ˊp���v�Z
        Vector3 d = _moveVec + 2 * n * h;

        // �ړ������𔽎˂�����A���������͓����蔻��̂Ƃ���ōs��
        _moveVec = d.normalized;
    }

    /// <summary>
    /// �U���f�[�^�̐ݒ�A�L�����ƕ���̃f�[�^����U���͂�����
    /// </summary>
    public void SetCharaData(JsonCharacterMaster _chara, JsonWeaponMaster _weapon, Sprite _img)
    {
        charaData.atk = (int)(_weapon.atk_rate * _chara.atk);
        charaData.attr = _weapon.attr;
        charaData.speed = _weapon.speed_rate * _chara.speed;
        weaponSr.sprite = _img;
    }

    /// <summary>
    /// �G����̃_���[�W
    /// </summary>
    public void Damage(float _atk, string _attr)
    {
        // �_���[�W�v�Z
        if (_attr == Common.ATTR_PHYSICAL)
        {
            _atk *= 2;
        }
        var dmg = _atk;

        // �_���[�W����
        qm.PlayerDamage((int)dmg);

    }

    /// <summary>
    /// �v���C���[�̃^�[���J�n���̏���
    /// �J�����̈ړ��I������qm����Ăяo��
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
    /// �|�[�Y����qm����Ăяo��
    /// </summary>
    public void GamePause()
    {
        state = PlayerState.PAUSE;
    }

    /// <summary>
    /// �|�[�Y��������qm����Ăяo��
    /// </summary>
    public void GameUnPause()
    {
        state = PlayerState.WAIT_INPUT;
        tapStPos = Vector3.zero;
        tapEdPos = Vector3.zero;
    }
}
