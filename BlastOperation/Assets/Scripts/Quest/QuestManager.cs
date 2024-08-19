using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

using Const;
using Json;

public class QuestManager : MonoBehaviour
{
    #region �e�X�g�f�[�^


    #endregion

    #region �񋓑�

    /// <summary>
    /// ����̔ԍ�
    /// </summary>
    public enum WEAPON
    {
        WEAPON_A,
        WEAPON_B
    }

    /// <summary>
    /// �Q�[�����̃t�F�[�Y
    /// </summary>
    public enum GameFase
    {
        /// <summary>����������</summary>
        GAME_INIT,


        /// <summary>�t���A�J�n��</summary>
        FLOOR_START,
        /// <summary>�J�����ړ�����</summary>
        CAMERA_MOVE_START,
        /// <summary>�J�����ړ���</summary>
        CAMERA_MOVING,
        /// <summary>�J�����ړ��I��</summary>
        CAMERA_MOVE_END,
        /// <summary>�Q�[����</summary>
        GAMING,


        /// <summary>�Q�[���N���A��A���͂Ń��U���g�ɑJ��</summary>
        GAME_CLEAR,
        /// <summary>�Q�[���I�[�o�[��A���͂Ń��U���g�ɑJ��</summary>
        GAME_OVER,


        /// <summary>���U���g�\���J�n�A�ʐM����</summary>
        RESULT_START,
    }

    #endregion

    #region �ϐ�

    [Tooltip("����摜�̃p�X")]
    private const string PATH_WEAPON_ICON = "Images/Icons/Weapon/";
    [Tooltip("�L�����A�C�R���̃p�X")]
    private const string PATH_CHARACTER_ICON = "Images/Characters/Face/";
    [Tooltip("�G�摜�̃p�X")]
    private const string PATH_ENEMY_BODY = "Images/Characters/Body/";


    [Tooltip("�p�[�e�B�[�̏��")]
    private JsonCharacterMaster[] partyCharas = new JsonCharacterMaster[PARTY_LIMIT];
    [Tooltip("�V���A���C�Y��̃N�G�X�g�f�[�^")]
    private JsonQuestData questData;



    [Header("�N�G�X�g���n")]
    [Space(10)]


    [Tooltip("�G�̐e�I�u�W�F�N�g")]
    private GameObject enemyParent;
    [Tooltip("1�^�[�����̃q�b�g��")]
    private int hitCount;
    [Tooltip("�o�߃^�[����")]
    private int turnCount;
    [Tooltip("���݂̃t���A��")]
    private int floorCount;
    [Tooltip("���݂̃Q�[�����")]
    public GameFase Fase { get; private set; }
    [Tooltip("�G�v���n�u")]
    private GameObject enemyPrefab;
    [Tooltip("�G�̍U���I�u�W�F�N�g�̐e")]
    private Transform enemyAttackObjParent;
    [Tooltip("�t���A�{�X��|�������ǂ���")]
    private bool isBossDestroyed;

    [Tooltip("���������I���������ǂ���")]
    private bool isInitEnd;
    [Tooltip("�������I���x��")]
    private const float DELAY_INIT_END = 1f;



    [Header("UI�n")]
    [Space(10)]

    [Tooltip("UI�̓����Ă���L�����o�X"), SerializeField]
    private GameObject canvasUI;
    [Tooltip("�U�����@��UI"), SerializeField]
    private GameObject attackUI;
    [Tooltip("�q�b�g����UI"), SerializeField]
    private Text hitCntUI;
    [Tooltip("�t���A����UI"), SerializeField]
    private Text floorCntUI;
    [Tooltip("�U�����@�{�^���̔z��"), SerializeField]
    private GameObject[] attackButton;
    [Tooltip("�J�����ɕ\�������I�u�W�F�N�g�̐e")]
    private GameObject allObjectParent;
    [Tooltip("�v���C���[�̃I�u�W�F�N�g")]
    private Player player;
    [Tooltip("UI�\���p�̃^�C�}�[")]
    private float timerUI;
    [Tooltip("�^�b�v����������UI�������鎞��")]
    private const float LIMIT_UI_BLINK = 0.5f;
    [Tooltip("�t�F�[�h�C���p��Image"), SerializeField]
    private Image fadeInImage;
    [Tooltip("�N�G�X�g��"), SerializeField]
    private Text questName;
    [Tooltip("�N�G�X�g�̃t���A��"), SerializeField]
    private Text questFloorNum;
    [Tooltip("���j���[�p�l��"), SerializeField]
    private GameObject menuPanel;
    [Tooltip("���j���[�{�^��"), SerializeField]
    private GameObject menuButton;
    [Tooltip("�|�[�Y��")]
    public bool isPause {  get; private set; }



    [Header("���U���g�p")]
    [Space(10)]

    [Tooltip("���U���g�p�̃p�l��"), SerializeField]
    private GameObject resultPanel;
    [Tooltip("���U���g�ɐi�ރ{�^���p�̃p�l��"), SerializeField]
    private GameObject toResultPanel;
    [Tooltip("���U���g�̃{�^��"), SerializeField]
    private GameObject toResultButton;
    [Tooltip("���U���g�̃e�L�X�g"), SerializeField]
    private Text toResultText;
    [Tooltip("�l�������|�C���g�e�L�X�g"), SerializeField]
    private Text getRankPointText;
    [Tooltip("�����N�e�L�X�g"), SerializeField]
    private Text rankText;
    [Tooltip("���̃����N�܂ł̃|�C���g�e�L�X�g"), SerializeField]
    private Text nextRankPointText;
    [Tooltip("��V�̃p�l��"), SerializeField]
    private GameObject rewardsPanel;
    [Tooltip("���C����ʂɖ߂�{�^��"), SerializeField]
    private GameObject toMainButton;
    [Tooltip("���U���g�\�����̃f�B���C")]
    private const float DELAY_DISPLAY_RESULT = 0.3f;
    [Tooltip("����N���A�̐�"), SerializeField]
    private GameObject crystalObj;
    [Tooltip("����N���A��V�Ȃ�"), SerializeField]
    private GameObject emptyObj;
    [Tooltip("�h���b�v�A�C�e���A������"), SerializeField]
    private GameObject dropPanel;
    [Tooltip("���U���g�X�L�b�v")]
    private bool isResultTap;




    [Header("�p�[�e�B�[���n")]
    [Space(10)]

    [Tooltip("�O�^�[���ɑI�΂ꂽ�`�[���L�����S���̍U�����@"), SerializeField]
    private WEAPON[] selectedWeapons;

    [Tooltip("�p�[�e�B�[�̍ő呍HP")]
    private int partyMaxHp;
    [Tooltip("�p�[�e�B�[�̌��݂̑�HP")]
    private int partyCurrentHp;
    [Tooltip("���ݑI�𒆂̃L������\��������I�u�W�F�N�g")]
    private GameObject choiceArrowObj;
    [Tooltip("�p�[�e�B�[�̃L�����摜�\���p�z��")]
    private Image[] charaImages = new Image[PARTY_LIMIT];
    [Tooltip("�p�[�e�B�[�̍ő吔")]
    private const int PARTY_LIMIT = 4;
    [Tooltip("�v���C���[�̌���HP�Q�[�W"), SerializeField]
    private Image playerCurrentHpGauge;
    [Tooltip("�v���C���[�̃_���[�WHP�Q�[�W"), SerializeField]
    private Image playerDamageHpGauge;
    [Tooltip("HP�̃e�L�X�g"), SerializeField]
    private Text playerHpText;



    [Header("�J�����ړ��n")]
    [Space(10)]
    [Tooltip("���C���J����")]
    private Camera cameraMain;
    [Tooltip("�J����������Ԃ̍��W")]
    private readonly Vector3 START_CAMERA_POS = new Vector3(0, 0, -10);
    [Tooltip("�J�����Q�[�����̍��W")]
    private readonly Vector3 MAIN_CAMERA_POS = new Vector3(0, 10, -10);
    [Tooltip("�J�����̈ړ�����")]
    private const float CAM_MOVE_TIME = 0.75f;
    [Tooltip("�J�����ړ�����~����臒l")]
    private const float CAM_MOVE_LIMIT = 0.1f;
    [Tooltip("�ǂ̐e�I�u�W�F�N�g")]
    private GameObject wallParentObj;
    [Tooltip("�ǂ̍ő�T�C�Y")]
    private readonly Vector3 WALL_SIZE_MAX = new Vector3(1.3f, 1.3f, 1.3f);
    [Tooltip("�ǂ̍ŏ��T�C�Y")]
    private readonly Vector3 WALL_SIZE_MIN = new Vector3(1f, 1f, 1f);
    [Tooltip("�ǈړ����I������臒l")]
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
            // ���C���̏���
            StartCoroutine(FaseControll());
        }

    }


    /// <summary>
    /// ����������
    /// </summary>
    private IEnumerator Init()
    {
        // �ʐM�J�n�O�̍����
        fadeInImage.gameObject.SetActive(true);
        questName.gameObject.SetActive(false);
        questFloorNum.gameObject.SetActive(false);
        isPause = false;

        // �N�G�X�g�J�n���̒ʐM����
        yield return StartCoroutine(QuestStart());

        // �X�^�~�i�����炷
        MainData.instance.playerInfo.energy -= questData.stage_master.use_energy;


        // �v���C���[�̎擾
        player = GameObject.Find("PlayerBall").GetComponent<Player>();
        // �G�̐e���擾
        enemyParent = GameObject.Find("Enemies");
        // ��\��
        enemyParent.SetActive(false);

        // �J�����̎擾
        cameraMain = Camera.main;


        // �Q�[���̏�Ԃ̏�����
        Fase = GameFase.GAME_INIT;
        QuestInfoDisplay(questData.stage_master);

        // �G�̏�����
        QuestEnemyInit(questData.stage_master);


        // �N�G�X�g���̏�����
        hitCount = 0;
        turnCount = 1;
        floorCount = 1;



        choiceArrowObj = GameObject.Find("ChoiceArrow");

        // �p�[�e�B�[�̏�����
        PartyInit();
        // ���̈ʒu��Ґ���1�̖ڂɂ���
        StartCoroutine(Common.Delay(1, PartyArrowDisplay));

        // �U�����@�̏�����
        selectedWeapons = new WEAPON[Common.PARTY_LIMIT];
        AttackWeaponEnable((int)WEAPON.WEAPON_A);



        // �I�u�W�F�N�g�̐e�擾

        allObjectParent = GameObject.Find("AllObjectParent");
        wallParentObj = GameObject.Find("Walls");
        wallParentObj.transform.localScale = WALL_SIZE_MAX;



        // ���U���g�̏�����
        toResultPanel.SetActive(false);
        resultPanel.SetActive(false);
        toMainButton.SetActive(false);
        getRankPointText.gameObject.SetActive(false);
        rankText.gameObject.SetActive(false);
        nextRankPointText.gameObject.SetActive(false);
        rewardsPanel.SetActive(false);
        toMainButton.SetActive(false);
        isResultTap = false;


        // �t�F�[�h�C���p��Image������
        fadeInImage.color = Color.black;
        fadeInImage.gameObject.SetActive(true);

        // �t���A���\��
        FloorCountDisplay();

        // ������HP��ݒ�
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
    /// �N�G�X�g�J�n�̒ʐM����
    /// </summary>
    private IEnumerator QuestStart()
    {
        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();
        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", PlayerPrefs.GetString(Common.KEY_USER_ID));
        form.AddField("stage_id", PlayerPrefs.GetInt(Common.KEY_STAGE_ID));
        form.AddField("party_id", PlayerPrefs.GetInt(Common.KEY_PARTY_ID));

        // URL�̐ݒ�
        var url = MainData.instance.apiUrl + Common.EP_GET_STAGE;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X���o��
        var res = coroutine.Current.ToString();

        // �f�[�^�̃V���A���C�Y
        questData = JsonUtility.FromJson<JsonQuestData>(res);

        // �ʐM�ɐ������Ă����珈�����s
        if (questData.result == Common.SUCCES)
        {
            Fase = GameFase.FLOOR_START;
            // �p�[�e�B�[�̃L������������
            partyCharas = questData.character_masters;

            questName.gameObject.SetActive(true);
            questFloorNum.gameObject.SetActive(false);

            yield return null;
        }
        else
        {
            print("�ʐM�G���[:" + questData.message);
            yield return null;
        }
    }


    /// <summary>
    /// �Q�[���J�n���̓G�I�u�W�F�N�g����������
    /// �N�G�X�g�̓G�̍ő吔�œG���ŏ��ɐ����A�v�[�����Ă���
    /// </summary>
    private void QuestEnemyInit(JsonStageMaster _stage)
    {
        // �G�v���n�u�̓ǂݍ���
        enemyPrefab = Resources.Load<GameObject>("Prefabs/Enemy");

        // �S�t���A�̓G�̍ő吔���v�Z

        var floorMax = _stage.max_floor;

        // ���t���A�ڂ̓G���ő吔���𒲂ׂ邽�߂̔z��
        var enemiesCnt = new int[floorMax];

        int i;
        // �G�̍ő吔�𐔂���A�ǂ��̃t���A���ő吔�����ׂ�
        // �N�G�X�g�̓G�̑����������[�v
        for (i = 0; i < _stage.enemy_infos.Length; i++)
        {
            // null�`�F�b�N
            if (_stage.enemy_infos[i] != null)
            {
                // ���̓G�͂ǂ̃t���A�ɏo�邩
                var index = _stage.enemy_infos[i].floor - 1;
                // ���̓G���o��t���A�Ԗڂ����Z
                enemiesCnt[index]++;
            }


        }
        // �����I������̂ōő吔���Z�o
        int max = 0;
        for (i = 0; i < enemiesCnt.Length; i++)
        {
            if (enemiesCnt[i] > max)
            {
                max = enemiesCnt[i];
            }
        }

        // �N�G�X�g�Ŏg���ő�̓G�̐����킩�����̂œG�I�u�W�F�N�g�𐶐�
        for (i = 0; i < max; i++)
        {
            var obj = Instantiate(enemyPrefab, enemyParent.transform);
        }
    }

    /// <summary>
    /// �����������I��
    /// </summary>
    private void GameInitEnd()
    {
        isInitEnd = true;
        Fase = GameFase.FLOOR_START;
    }

    /// <summary>
    /// fase�𔻒肵�ăQ�[�����̃t�F�[�Y�𐧌�
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
                // �J�����ړ������A�ǂ���ʊO�Ɉړ�
                WallsMove();
                break;
            case GameFase.CAMERA_MOVING:
                // �J�������ړ�
                AttackUIEnable(false);
                HitCountUIEneble(false);
                CameraMove(MAIN_CAMERA_POS);
                break;
            case GameFase.CAMERA_MOVE_END:
                // �J�����ړ������A�ǂ���ʓ��Ɉړ�
                WallsMove();
                break;
            case GameFase.GAMING:
                // �G�Ɩ����̃t�F�[�Y����
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
    /// �Q�[���J�n���̏��\��
    /// </summary>
    private void QuestInfoDisplay(JsonStageMaster _stage)
    {
        questName.gameObject.SetActive(true);
        questFloorNum.gameObject.SetActive(true);

        questName.text = _stage.name;
        questFloorNum.text = "�t���A���F" + _stage.max_floor.ToString();
    }



    /// <summary>
    /// UI�̕\���𐧌�
    /// </summary>
    private void GameUIDisplay()
    {
        // �^�b�v���͈�莞�ԂŔ�\��
        if (player.isTap)
        {
            timerUI += Time.deltaTime;
            if (timerUI > LIMIT_UI_BLINK)
            {
                AttackUIEnable(false);
            }
        }
        // ��^�b�v���̓^�C�}�[��0
        else
        {
            AttackUIEnable(true);
            timerUI = 0;
        }


        // �ҋ@����HP�Q�[�W���X�V����
        if (player.state == Player.PlayerState.WAIT_INPUT || player.state == Player.PlayerState.MOVE)
        {
            HpGaugeSync();
        }
        // ���˒��͖������Ŕ�\��
        if (player.state == Player.PlayerState.MOVE || player.state == Player.PlayerState.WAIT_ENEMY)
        {
            AttackUIEnable(false);
        }
    }

    /// <summary>
    /// ���̕\���A�o�߃^�[������4�Ŋ������]���index���w��
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
    /// ���݂̌o�߃^�[��������p�[�e�B�[�̃C���f�b�N�X���v�Z���Ԃ�
    /// </summary>
    /// <returns></returns>
    private int GetPartyIndex()
    {
        return (turnCount - 1) % 4;
    }

    /// <summary>
    /// �t���A���̕\������
    /// </summary>
    private void FloorCountDisplay()
    {
        floorCntUI.text = "�t���A��" + floorCount.ToString() + " / " + questData.stage_master.max_floor;
    }

    /// <summary>
    /// ���݃C���X�^���X�����݂���G�̎擾
    /// </summary>
    /// <returns></returns>
    public InstanceEnemy[] GetInstanceEnemys()
    {
        return enemyParent.GetComponentsInChildren<InstanceEnemy>();
    }

    /// <summary>
    /// �v���C���[�̈ړ��I�����̏���
    /// </summary>
    public void PlayerTurnEnd()
    {
        // �q�b�g���̏�����
        hitCount = 0;
        hitCntUI.text = hitCount.ToString();
        // �^�[������i�߂�
        turnCount++;
    }

    /// <summary>
    /// �v���C���[�{�[���̏����X�V���鏈��
    /// </summary>
    public void PlayerInfoUpdate()
    {
        // ���̃L�����N�^�[��I��������
        PartyArrowDisplay();
        // ������̕\��
        AttackWeaponEnable((int)selectedWeapons[GetPartyIndex()]);
        // �v���C���[�̏�ԍX�V
        player.PlayerTurnStart();
    }

    /// <summary>
    /// �U�����@�̑I��
    /// ������int�����AWeapon�^���L���X�g����0or1�̒l
    /// </summary>
    public void AttackWeaponEnable(int _index)
    {
        // ��U�I�t�̏�Ԃŏ�����
        for (int i = 0; i < attackButton.Length; i++)
        {
            attackButton[i].transform.localScale = Vector3.one;
            attackButton[i].GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
            attackButton[i].transform.GetChild(1).GetComponent<Image>().color = new Color(0.25f, 0.25f, 0.25f, 0.5f);
        }

        // �w�肳�ꂽid��UI��ύX
        // �傫������
        var ls = attackButton[_index].transform.localScale;
        ls *= 1.2f;
        attackButton[_index].transform.localScale = ls;
        // �O���[�A�E�g���O��
        attackButton[_index].GetComponent<Image>().color = Color.white;
        attackButton[_index].transform.GetChild(1).GetComponent<Image>().color = Color.white;


        // �U���f�[�^�̍X�V
        // �L�����f�[�^�̂��߂̃C���f�b�N�X�擾
        var partyIndex = GetPartyIndex();
        // ����ID�̎擾
        var weaponId = partyCharas[partyIndex].weapon_ids[_index];
        // ����f�[�^�̎擾
        JsonWeaponMaster weaponData = new JsonWeaponMaster();
        for (int i = 0; i < questData.weapon_masters.Length; i++)
        {
            if (weaponId == questData.weapon_masters[i].id)
            {
                weaponData = questData.weapon_masters[i];
            }
        }

        // ����̉摜�擾
        var path = PATH_WEAPON_ICON + weaponData.name_id;
        // �I�񂾏���ۑ�
        selectedWeapons[partyIndex] = (WEAPON)_index;

        var img = Resources.Load<Sprite>(path);

        player.SetCharaData(partyCharas[partyIndex], weaponData, img);
    }

    /// <summary>
    /// �U�����@��UI�̕\������
    /// true��UI���I��
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
    /// �{�[�����G�Ƀq�b�g�����Ƃ��ɌĂяo�����A�J�E���g�𑝂₷����
    /// </summary>
    public void Hit()
    {
        hitCount++;
        hitCntUI.text = hitCount.ToString();
    }

    /// <summary>
    /// �q�b�g��UI�̕\��
    /// </summary>
    private void HitCountUIEneble(bool _flg)
    {
        hitCntUI.gameObject.SetActive(_flg);
    }

    /// <summary>
    /// �Ґ����̏������A�L�����f�[�^��摜���擾
    /// </summary>
    private void PartyInit()
    {
        // �L�����f�[�^�擾
        // �L������id����L�����f�[�^���擾
        // �p�[�e�B�[id�����[�v

        for (int i = 0; i < questData.character_masters.Length; i++)
        {
            partyCharas[i] = questData.character_masters[i];
        }

        // �L�����摜������摜�̎擾
        var tmp = GameObject.Find("CharaImages").transform;


        for (int i = 0; i < charaImages.Length; i++)
        {
            charaImages[i] = tmp.GetChild(i).GetComponent<Image>();
        }


        // �L�����摜������
        for (int i = 0; i < charaImages.Length; i++)
        {
            // �L�����̉摜���p�X�w��Ŏ擾
            var path = PATH_CHARACTER_ICON + (partyCharas[i].name_id);
            charaImages[i].sprite = Resources.Load<Sprite>(path);
        }
    }

    /// <summary>
    /// �N�G�X�g�J�n���̃t�F�[�h�C���̉��o������邽�߂̏���
    /// �啔����CameraMove�ɋL�q
    /// </summary>
    private void InitCamera(Vector3 _target)
    {
        // �t�F�[�h�C���p�ɉ摜�𓧖��ɂ��Ă���
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
    /// �w����W�܂ŃJ�������ړ�������
    /// </summary>
    private void CameraMove(Vector3 _target)
    {
        // z���W��؂�̂Ă邽�߂�vector2�Ő錾
        Vector2 dif = _target - cameraMain.transform.position;
        // �J�����𓮂���
        cameraMain.transform.position += (Vector3)dif / CAM_MOVE_TIME * Time.deltaTime;
        // �I�u�W�F�N�g�����ׂē�����
        allObjectParent.transform.position += (Vector3)dif / CAM_MOVE_TIME * Time.deltaTime;

        // ����������菬����or�ʂ肷������Œ肵�ĕǂ�߂�
        if (dif.magnitude < CAM_MOVE_LIMIT || dif.y < 0)
        {
            cameraMain.transform.position = _target;
            allObjectParent.transform.position = new Vector3(_target.x, _target.y, allObjectParent.transform.position.z);
            // �J�����ړ������A�ǂ̈ړ��J�n
            Fase = GameFase.CAMERA_MOVE_END;
        }
    }

    /// <summary>
    /// �ǂ���ʊO�A���ɓ���������
    /// �J�����̈ړ��O��ɂ����Ă΂�Ȃ�
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

        // �ڕW�T�C�Y�Ƃ̍����v�Z���č�����������΃T�C�Y���Œ�
        var dif = wallParentObj.transform.localScale - target;
        if (Math.Abs(dif.magnitude) < WALL_SIZE_DIFF_LIMIT)
        {
            // �Œ�
            wallParentObj.transform.localScale = target;

            // �ǂ̈ړ������ɂ���ăt�F�[�Y��������
            if (Fase == GameFase.CAMERA_MOVE_START)
            {
                // ��ʊO�֓�������J�����ړ��J�n
                Fase = GameFase.CAMERA_MOVING;
                // �ړ�����҂̍��W������
                cameraMain.transform.position = START_CAMERA_POS;
                allObjectParent.transform.position = new Vector3(START_CAMERA_POS.x, START_CAMERA_POS.y, allObjectParent.transform.position.z);
            }
            else
            {
                // ��ʓ��֓�������Q�[���ɖ߂�
                Fase = GameFase.GAMING;

                // �t���A�̓G����
                FloorInit(questData.stage_master);

                // �G�̕\��
                enemyParent.SetActive(true);

                // �v���C���[�̏����X�V����
                PlayerInfoUpdate();

                // UI��߂�
                AttackUIEnable(true);
                HitCountUIEneble(true);
                return;
            }
            return;
        }
        else
        {
            // ��ʊO�ɏo���Ȃ�scale�𑝂₷�A�t�Ȃ猸�炷
            int val = (Fase == GameFase.CAMERA_MOVE_START) ? 1 : -1;
            // �T�C�Y�����X�ɑ��₷
            wallParentObj.transform.localScale += Vector3.one * val * Time.deltaTime;
        }
    }

    /// <summary>
    /// �t���A�̏���������
    /// �t���A�̓G�̐���
    /// </summary>
    private void FloorInit(JsonStageMaster _stage)
    {
        // ��������G�̃��X�g
        List<JsonEnemyInfo> floorEnemies = new List<JsonEnemyInfo>();

        var allEnemies = _stage.enemy_infos;

        // ���݂̃t���A�Ɣ�r���ă��X�g�ɒǉ�
        for (int i = 0; i < allEnemies.Length; i++)
        {
            if (allEnemies[i].floor == floorCount)
            {
                floorEnemies.Add(allEnemies[i]);
            }
        }

        // �������ăp�����[�^�̐ݒ�
        // �}�X�^�[�̎��o��
        var eMasters = questData.enemy_masters;
        // ���o�����}�X�^�[�f�[�^�i�[�p�ϐ�
        JsonEnemyMaster eMaster = new JsonEnemyMaster();

        // �������X�g�����[�v
        for (int i = 0; i < floorEnemies.Count; i++)
        {
            var enemy = floorEnemies[i];
            // �}�X�^�[�f�[�^�����[�v���ăf�[�^���o��
            // �}�X�^�[�̃C���f�b�N�X��id���K��������v���Ȃ����ߑS�T��
            for (int j = 0; j < eMasters.Length; j++)
            {
                if (eMasters[j].id == enemy.id)
                {
                    eMaster = eMasters[j];
                    break;
                }
                else
                {
                    // �z��Ō�łȂ���ΌJ��Ԃ�
                    if (j != eMasters.Length - 1)
                    {
                        continue;
                    }

                    // �z��Ō�Ȃ�}�X�^�[�f�[�^�ɂȂ��̂ŃG���[����
                    print("�������悤�Ƃ��Ă���ID�F" + enemy.id + "�̓}�X�^�[�f�[�^�ɑ��݂��܂���!!");
#if UNITY_EDITOR
                    //�Q�[���v���C�I��
                    UnityEditor.EditorApplication.isPlaying = false;
#else
    Common.LoadScene("Main3D");
#endif
                }
            }

            // �v�[�����ꂽ�I�u�W�F�N�g���擾
            var instance = enemyParent.transform.GetChild(i).gameObject;
            instance.transform.position = Vector3.zero;

            // �p�����[�^�����邽�߂ɃR���|�[�l���g�擾
            var component = instance.GetComponent<InstanceEnemy>();

            // ���ۂ̃p�����[�^���}�X�^�[�ƃX�e�[�W��񂩂�v�Z
            // �p�����[�^�쐬
            EnemyInitDatas datas = new EnemyInitDatas();
            datas.hp        = (int)(eMaster.hp * enemy.hp_rate);
            datas.atk       = eMaster.atk * floorEnemies[i].atk_rate;
            datas.atk_ptns  = eMaster.atk_pattern_ids;
            datas.pos       = enemy.position;
            datas.size      = enemy.size;
            datas.wk        = eMaster.weak;
            datas.isB       = enemy.is_boss;
            datas.path      = PATH_ENEMY_BODY + eMaster.name_id;
            print("�G�̖��O�p�X" + datas.path);

            component.EnemyInit(datas);
        }

        //todo:�������F�G�������Ɏ������G�Ƃ��Ԃ��Ă����玩���̍��W�����炷


        // �{�X�t���O��������
        isBossDestroyed = false;
    }

    /// <summary>
    /// ���̃t���A�ֈړ�����ۂɋʂ���Ăяo��
    /// </summary>
    public void FloorToNext()
    {
        // �t���A�̍ŉ��őS���|���ƃN���A
        if (floorCount == questData.stage_master.max_floor)
        {
            // �N�G�X�g�N���A����
            QuestClear();
            return;
        }

        // �J�����ړ��������s��
        Fase = GameFase.CAMERA_MOVE_START;
        enemyParent.SetActive(false);

        // �t���A���̉��Z�ƕ\��
        floorCount++;
        FloorCountDisplay();
    }

    /// <summary>
    /// �t���A�{�X���|���ꂽ���ǂ���
    /// </summary>
    /// <returns></returns>
    public bool GetIsBossDestroyed() { return isBossDestroyed; }

    /// <summary>
    /// �{�X���j���̏����A�{�X�����񂾂�Ăяo��
    /// </summary>
    public void BossDestroyed()
    {
        isBossDestroyed = true;
    }

    /// <summary>
    /// �G�̍U���I�u�W�F�N�g�𐶐����鏈��
    /// </summary>
    public GameObject EnemyAtkInst(GameObject _prefab, Vector3 _pos, Quaternion _rot)
    {
        return Instantiate(_prefab, _pos, _rot, enemyAttackObjParent);
    }

    /// <summary>
    /// �G�̍U���I�u�W�F�N�g���L�����ǂ����擾
    /// �L���łȂ���ΓG�̍U���͏I�����Ă���
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
    /// �v���C���[���_���[�W���󂯂��Ƃ��̏���
    /// HP�����炵��UI�ɔ��f
    /// </summary>
    public void PlayerDamage(int _dmg)
    {
        partyCurrentHp -= _dmg;
        if (partyCurrentHp <= 0)
        {
            partyCurrentHp = 0;
        }
        playerCurrentHpGauge.fillAmount = Common.NormalizedFunc(partyCurrentHp, 0, partyMaxHp, 0, 1);

        playerHpText.text = partyCurrentHp + "�@" + partyMaxHp;
    }

    /// <summary>
    /// �v���C���[�̉񕜏���
    /// </summary>
    /// <param name="_val"></param>
    public void PlayerHeal(int _val)
    {
        partyCurrentHp += _val;
        // HP�����������Ȃ��悤����
        if (partyCurrentHp >= partyMaxHp)
        {
            partyCurrentHp = partyMaxHp;
        }
        playerCurrentHpGauge.fillAmount = Common.NormalizedFunc(partyCurrentHp, 0, partyMaxHp, 0, 1);
        playerHpText.text = partyCurrentHp + " / " + partyMaxHp;
    }

    /// <summary>
    /// �p�[�e�B�[��HP�̐ԃQ�[�W�𓮂���
    /// </summary>
    private void HpGaugeSync()
    {
        var hpDif = playerCurrentHpGauge.fillAmount - playerDamageHpGauge.fillAmount;

        // �Q�[�W������������I��
        if (hpDif == 0)
        {
            return;
        }

        // ��������
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
    /// ���U���g�p�l���\�������A�{�^������Ăяo��
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
    /// ���U���g�\������
    /// </summary>
    private IEnumerator Result()
    {
        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();
        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", PlayerPrefs.GetString(Common.KEY_USER_ID));
        form.AddField("stage_id", questData.stage_master.id);
        form.AddField("player_info", JsonUtility.ToJson(MainData.instance.playerInfo));

        print("����O�̃f�[�^�F" + JsonUtility.ToJson(MainData.instance.playerInfo));

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_RESULT;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        var res = coroutine.Current.ToString();

        print("����O�̃f�[�^�F" + JsonUtility.ToJson(MainData.instance.playerInfo));
        print("��������̃��U���g�F" + res);

        if (res == null)
        {
            print("�ʐM�G���[�F���U���g�̃��X�|���X�f�[�^���Ȃ�");
        }

        // �V���A���C�Y
        var resultData = JsonUtility.FromJson<JsonResultData>(res);

        if (resultData.result == Common.SUCCES)
        {
            // �\�����郉���N
            var newData = resultData.player_info;

            // ���U���g��ʏ�����
            rewardsPanel.SetActive(false);
            toMainButton.SetActive(false);

            // ���݂̃����N�|�C���g�\��
            yield return StartCoroutine(CurrnetRankPointDisplay(MainData.instance.playerInfo, questData.stage_master));

            // �N���A��̃����N�|�C���g�܂ŏ㏸������
            yield return StartCoroutine(NewRankPointDisplay(MainData.instance.playerInfo, newData, questData.stage_master, resultData.limit_rank_points));

            // ���̑��N���A����V�̕\��.����N���A�̐΂�h���b�v�Ȃ�
            yield return StartCoroutine(RewardsDisplay(MainData.instance.playerInfo));


            // ���C���f�[�^��������
            MainData.instance.playerInfo.total_rank_point = resultData.player_info.total_rank_point;
            MainData.instance.playerInfo.next_rank_point = resultData.player_info.next_rank_point;
            MainData.instance.playerInfo.rank = resultData.player_info.rank;
            MainData.instance.playerInfo.energy = resultData.player_info.energy;
            MainData.instance.playerInfo.coin = resultData.player_info.coin;
            MainData.instance.playerInfo.crystal = resultData.player_info.crystal;
            MainData.instance.playerInfo.progress_rates = resultData.player_info.progress_rates;

            // �z�[���ɖ߂�{�^���\��
            toMainButton.SetActive(true);
        }
        else
        {
            yield return null;
        }
    }

    /// <summary>
    /// ���݂̃����N�\��
    /// </summary>
    /// <returns></returns>
    private IEnumerator CurrnetRankPointDisplay(JsonPlayerInfo _old, JsonStageMaster _stg)
    {
        // ���U���g������

        var get = _stg.rank_point;
        var next = _old.next_rank_point;
        // S�����N��������J���X�g
        if (_old.rank == "S")
        {
            get = 0;
            next = 0;
        }

        // �����N�\��
        rankText.text = _old.rank;
        rankText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // �擾�|�C���g�\��
        getRankPointText.text = get.ToString();
        getRankPointText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // ���̃����N�܂ł̃|�C���g�\��
        nextRankPointText.text = next.ToString();
        nextRankPointText.gameObject.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
    }

    /// <summary>
    /// �X�V��̃����N�\��
    /// </summary>
    /// <returns></returns>
    private IEnumerator NewRankPointDisplay(JsonPlayerInfo _old, JsonPlayerInfo _new, JsonStageMaster _stg, int[] _limits)
    {
        int get = _stg.rank_point;
        int next = _old.next_rank_point;
        string rankDisp = _old.rank;
        // S�����N�������牉�o�X�L�b�v
        if (_old.rank == "S")
        {
            yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
        }
        else
        {
            while (true)
            {
                // �^�b�v�ŃX�L�b�v����
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

                // ���o�̑�����.�ڕW�܂ł̋����ɉ����Ē���
                int val = 1;
                if (get > 200)
                {
                    val = get / 4;
                }
                else if (get > 100)
                {
                    val = 10;
                }
                // ���Z���Ă���
                get -= val;
                next -= val;


                // ���̃����N�܂ł̃|�C���g�����ŕ\����̃����N�A�b�v���o
                if (next <= 0)
                {
                    ResultRankUp(ref get, ref next, ref rankDisp, _limits);
                }

                // ��ʂɏo��
                getRankPointText.text = get.ToString();
                nextRankPointText.text = next.ToString();
                rankText.text = rankDisp;

                // �擾�|�C���g���Ȃ��Ȃ�����I��
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
    /// �����N�A�b�v�̉��o�̏���
    /// �s�������̂Ŋ֐���
    /// </summary>
    private void ResultRankUp(ref int _getPt, ref int _nextPt, ref string _rank, int[] _limits)
    {

        // �I�[�o�[����ێ�
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
        // �I�[�o�[����߂�
        _nextPt -= dif;
    }

    /// <summary>
    /// �N�G�X�g�N���A���̃h���b�v�A�C�e���\��
    /// �O�����\���_�͖�����
    /// </summary>
    /// <returns></returns>
    private IEnumerator RewardsDisplay(JsonPlayerInfo _old)
    {
        rewardsPanel.SetActive(true);
        crystalObj.SetActive(false);
        emptyObj.SetActive(false);
        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // ����N���A���ǂ�������
        // �i�s�x���猟��

        for (int i = 0; i < _old.progress_rates.Length; i++)
        {
            // �X�e�[�W�̃}�X�^�[��id�ƈ�v��������o��
            if (_old.progress_rates[i].stage_id == questData.stage_master.id)
            {
                // is_clear��false�Ȃ珉��N���A
                if (_old.progress_rates[i].is_clear == false)
                {
                    print("����N���A��V����");
                    var getCrystal = questData.stage_master.crystal;
                    crystalObj.SetActive(true);
                    crystalObj.GetComponentInChildren<Text>().text = getCrystal.ToString();
                    emptyObj.SetActive(false);
                }
                // ����N���A�ł͂Ȃ��̂ŕ�V�Ȃ�
                else
                {
                    print("����N���A��V�Ȃ�");
                    crystalObj.SetActive(false);
                    emptyObj.SetActive(true);
                }

                break;
            }
        }

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);

        // �h���b�v�A�C�e���̕\���A������
        dropPanel.SetActive(true);

        yield return new WaitForSeconds(DELAY_DISPLAY_RESULT);
    }

    /// <summary>
    /// �s�k���Ă��邩���擾����֐�
    /// </summary>
    /// <returns></returns>
    public bool GetIsDefeat()
    {
        return partyCurrentHp <= 0;
    }

    /// <summary>
    /// ���������G����Ăяo��
    /// </summary>
    /// <returns></returns>
    public JsonEnemyAttackMaster[] GetJsonEnemyAttackMasters()
    {
        return questData.enemy_atk_masters;
    }

    /// <summary>
    /// �N�G�X�g�̃N���A��ɌĂ΂�鏈��
    /// </summary>
    private void QuestClear()
    {
        Fase = GameFase.GAME_CLEAR;
        // ���U���g�̕\��
        toResultPanel.SetActive(true);
        // todo:�N���A�p�ɕύX
        toResultText.text = "�X�e�[�W\n�N���A�I";
    }

    /// <summary>
    /// �N�G�X�g���s���̏���
    /// </summary>
    public void GameOver()
    {
        
        Fase = GameFase.GAME_OVER;

        // ���U���g�p�l���\��
        toResultPanel.SetActive(true);
        // todo:�e�L�X�g��s�k�p�ɕύX
        toResultText.text = "����˂�";
        toResultButton.transform.GetChild(0).GetComponent<Text>().text = "�z�[����";
        
    }

    /// <summary>
    /// �v���C���[�̓��͑ҋ@���̂ݕ\��
    /// </summary>
    public void MenuButtonEnable()
    {
        menuButton.SetActive(true);
    }

    /// <summary>
    /// �v���C���[�̓��͑ҋ@���ȊO�͔�\��
    /// </summary>
    public void MenuButtonDisable()
    {
        menuButton.SetActive(false);
    }


    /// <summary>
    /// ���j���[�{�^���������̏���
    /// </summary>
    public void PushMenuButton()
    {
        menuPanel.SetActive(true);
        isPause = true;
    }

    /// <summary>
    /// ���j���[����Q�[���ɖ߂�
    /// </summary>
    public void PushBackGameButton()
    {
        menuPanel.SetActive(false);
        isPause = false;
    }

    /// <summary>
    /// ���C����ʂɖ߂�{�^���������̏���
    /// </summary>
    public void PushBackMainButton()
    {
        Common.LoadScene(Common.SCENE_NAME_MAIN);
    }

}