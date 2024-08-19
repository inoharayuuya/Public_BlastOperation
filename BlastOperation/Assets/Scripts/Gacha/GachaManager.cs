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
    // ��ʂ̕��A����
    private const int SCREEN_WIDTH  = 360;
    private const int SCREEN_HEIGHT = 720;

    // 1�A�A10�A
    private const int GACHA_NUMBER_ONCE = 0;
    private const int GACHA_NUMBER_TEN  = 1;

    private const int MAX_CHARA_NUMBER = 10;
    private const int HASCHARA_NUMBER = 30;
    private const int MAX_WEAPON = 2;
    private const float WAIT_TIME = 0.5f;

    // �����N�ɂ���ĐF��ύX���鎞�̐F�̐�
    private const int RANK_SPRITES_NUMBER = 3;
    // ���A��A���̔z��ԍ�
    private const int RANK_SPRITES_COPPER = 0;
    private const int RANK_SPRITES_SILVER = 1;
    private const int RANK_SPRITES_GOLD   = 2;

    // �L�����\���Ɏg�p����萔
    private const int CHARA_WIDTH = 100;      // �L�����摜�̕�
    private const int POSITION_PLUS_X = 130;  // �\������x���W�ύX�p
    private const int POSITION_PLUS_Y = -100; // �\������y���W�ύX�p

    // �f�o�b�O�p
    //private const string USER_ID = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918";
    //private const string TEST_URL = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";

    // �K�`���̃L�����N�^�[�e���v���[�g
    [SerializeField] private GameObject gachaCharaTemplate;

    // ���c�ǋL.�N���X�^�����������p
    [SerializeField] private Text crystalText;

    // �\���̈�
    [SerializeField] private GameObject showArea;

    // �����N�p�̃X�v���C�g
    [SerializeField] private Sprite[] rankSprites = new Sprite[RANK_SPRITES_NUMBER];

    // �L�����\�����W�p
    private Vector3 pos;

    // �ʐM�����t���O
    private bool isSucces;
    // �K�`�����o�X�^�[�g�t���O
    static public bool isStart;
    // �K�`�����o�I���t���O
    private bool isEnd;
    // �K�`�����o�^�[���X�^�[�g�t���O
    private bool isTurnStart;
    // ����񂷂�(1�A��10�A)���̃t���O
    private int gachaNumber;

    // �K�`���Ŋl�������L����(�e���v���[�g)���i�[����p�̔z��
    private GameObject[] getCharas = new GameObject[MAX_CHARA_NUMBER];

    // �K�`���I���{�^��
    [SerializeField] private GameObject exitButton;

    // �K�`�����o�p�l��
    [SerializeField] private GameObject gachaPanel;
    [SerializeField] private GameObject gachaAnimPanel;
    [SerializeField] private GameObject tapArea;
    [SerializeField] private GameObject tapText;
    [SerializeField] private GameObject moneyImage;

    // ���[�f�B���O
    [SerializeField] private GameObject loadingSmall;

    // 10�A�����ۂ̃t���O
    private bool isSingle;

    /// <summary>
    /// ��A�{�^������������
    /// </summary>
    public void TapOnceButton()
    {
        // �K�`���̉񐔂�1��
        gachaNumber = GACHA_NUMBER_ONCE;

        // �ʐM�J�n
        StartCoroutine(GachaAPI());
    }

    /// <summary>
    /// �\�A�{�^������������
    /// </summary>
    public void TapTenButton()
    {
        // �K�`���̉񐔂�10��
        gachaNumber = GACHA_NUMBER_TEN;

        loadingSmall.SetActive(true);
        // �ʐM�J�n
        StartCoroutine(GachaAPI());
        loadingSmall.SetActive(false);
    }

    /// <summary>
    /// �K�`���L�����̃v���t�@�u�𐶐����鏈��
    /// </summary>
    private void CreatePrefab(int _i,int _j,JsonGachaData _jsonGacha)
    {
        // 1�A�̏ꍇ�͐�������x���W�𒆉��ɕύX
        if(gachaNumber == GACHA_NUMBER_ONCE)
        {
            pos.x = SCREEN_WIDTH - CHARA_WIDTH;
        }

        // �v���t�@�u����
        var chara = Instantiate(gachaCharaTemplate, new Vector3(pos.x + (CHARA_WIDTH), pos.y, 0), Quaternion.identity, showArea.transform);

        // �q�I�u�W�F�N�g(�����N�̐F��Image)���擾
        var child = chara.transform.GetChild(0);

        // �����N���ƂɐF��Image��ύX
        switch (_jsonGacha.character_masters[_j].rank)
        {
            // ��
            case "F":
            case "B":
                // �X�v���C�g���擾���A�ύX
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_COPPER];
                break;

            // ��
            case "A":
                // �X�v���C�g���擾���A�ύX
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_SILVER];
                break;

            // ��
            case "S":
                // �X�v���C�g���擾���A�ύX
                child.GetComponent<Image>().sprite = rankSprites[RANK_SPRITES_GOLD];
                break;

        }

        //�g�p����L�����摜���擾 
        var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + _jsonGacha.character_masters[_j].name_id);
        // �L�����̉摜��ύX
        chara.GetComponent<Image>().sprite = charaSprite;

        // ���͉E�ׂ�ɐ������邽��x���W���v���X
        pos.x += POSITION_PLUS_X;

        // �����܂ł�������y���W�ύX
        if (_i == 4)
        {
            // x�͍��[�Ay�͏�������
            pos.x = 0;
            pos.y = SCREEN_HEIGHT + POSITION_PLUS_Y;
        }

        // �l���L�����z��Ɋi�[
        getCharas[_i] = chara;
    }


    public IEnumerator GachaAPI()
    {
        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();

        isSingle = true;

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", MainData.instance.userId);
        form.AddField("is_single", Convert.ToInt32(isSingle).ToString());

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_GACHA;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();
        // ���X�|���X��\��
        //print(res);
        
        

        // �f�o�b�O�p
        //var res = (Resources.Load("GachaJsonOnce",typeof(TextAsset))as TextAsset).text;
        //var res = (Resources.Load("GachaJson",typeof(TextAsset))as TextAsset).text;
        // Json�f�[�^���V���A���C�Y (�V���A���C�Y��Json����class)
        var jsonGacha = JsonUtility.FromJson<JsonGachaData>(res);

        // �ʐM����������
        if (jsonGacha.result == Common.SUCCES)
        {
            isSucces = true;
            // ��������������
            MainData.instance.playerInfo.crystal = jsonGacha.player_info.crystal;
            
            // UI�ɔ��f
            crystalText.text = (jsonGacha.player_info.crystal).ToString();

            print("�ʐM����");

            // �K�`�����o�J�n
            gachaPanel.SetActive(true);
            tapArea.SetActive(true);
            tapText.SetActive(true);
            //moneyImage.SetActive(false);

            // �K�`���ň����������k���`�F�b�N�@10�A��1�A������
            for (int i = 0; i < jsonGacha.character_ids.Length; i++)
            {
                // �k�����`�F�b�N �������̃L������id���L�����}�X�ɂ��邩�ǂ�������
                if (jsonGacha.character_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonGacha.character_masters.Length; j++)
                    {
                        // �L�����}�X��j�Ԗڂ��k������Ȃ����`�F�b�N
                        if (jsonGacha.character_masters[j] != null)
                        {
                            // �K�`���ŃQ�b�g�����L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                            if (jsonGacha.character_ids[i] == jsonGacha.character_masters[j].id)
                            {
                                // �v���t�@�u��������
                                CreatePrefab(i,j,jsonGacha);

                                print(i + "�̖�  �@name_id : " + jsonGacha.character_masters[j].name_id + "�@�@�����N : " + jsonGacha.character_masters[j].rank);
                            }
                        }
                    }
                }
            }


            // �v���C���[���̏�������
            // �ǉ��L�������̃��[�v

            // �����L�����ƒǉ��L���������X�g�ɕϊ�

            var hasCharaList = MainData.instance.playerInfo.has_character_ids.ToList();
            var addCharaList = jsonGacha.player_info.has_character_ids.ToList();

            // �����L�������X�V
            foreach(var addChara in addCharaList)
            {
                hasCharaList.Add(addChara);
            }

            // ���������𒴉߂���Ɛ؂�̂�
            if(hasCharaList.Count > Common.MAX_HAS_CHARA_NUMBER)
            {
                hasCharaList = hasCharaList.GetRange(0, Common.MAX_HAS_CHARA_NUMBER);
            }

            // �v���C���[���̏�������
            MainData.instance.playerInfo.has_character_ids = hasCharaList.ToArray();

            yield break;
        }


    }

    /// <summary>
    /// �K�`�����o����
    /// </summary>
    public IEnumerator GachaAnimation()
    {
        // �K�`�����o�J�n��
        if (isStart)
        {

            // ���A��
            if (gachaNumber == GACHA_NUMBER_ONCE)
            {
                // 1�A�̏���
                // WAIT_TIME�b�҂�����A���̏��������s
                yield return new WaitForSeconds(WAIT_TIME);

                getCharas[0].SetActive(true);

                // �t���O������
                isStart = false;

                // �Ђ�����Ԃ����o�X�^�[�g
                isTurnStart = true;
            }
            else
            {
                // 10�A�̏���
                for (int i = 0; i < MAX_CHARA_NUMBER; i++)
                {
                    // WAIT_TIME�b�҂�����A���̏��������s
                    yield return new WaitForSeconds(WAIT_TIME);

                    getCharas[i].SetActive(true);
                    //getCharas[i].GetComponent<Animator>().SetTrigger("Show");

                    // �����ɂȂ�����A�j���[�V�����؂�ւ�
                    if (i >= MAX_CHARA_NUMBER/2)
                    {
                        // �A�j���[�^�[�擾
                        var animator = getCharas[i].GetComponent<Animator>();
                        animator.SetBool("isHalf", true);
                    }

                    if (i == MAX_CHARA_NUMBER-1)
                    {
                        // �t���O������
                        isStart = false;

                        // �Ђ�����Ԃ����o�X�^�[�g
                        isTurnStart = true;

                        // �A�j���[�V����������
                        //animator.SetBool("isHalf", false);
                    }
                }
            }
        }
    }

    /// <summary>
    /// �K�`�����o�^�[������
    /// </summary>
    public IEnumerator GachaAnimationTurn()
    {
        // �K�`�����o�J�n��
        if (isTurnStart)
        {
            // ���A��
            if (gachaNumber == GACHA_NUMBER_ONCE)
            {
                // 1�A�̏���
                // WAIT_TIME�b�҂�����A���̏��������s
                yield return new WaitForSeconds(WAIT_TIME);

                // �F�����ɂ��鏈��
                var child = getCharas[0].transform.GetChild(0);
                child.GetComponent<Image>().color = new Color(255, 255, 255, 0);

                // �t���O������
                isTurnStart = false;

                // ���o�I���t���O
                isEnd = true;
            }
            else
            {
                // 10�A�̏���
                for (int i = 0; i < MAX_CHARA_NUMBER; i++)
                {
                    // WAIT_TIME�b�҂�����A���̏��������s
                    yield return new WaitForSeconds(WAIT_TIME);

                    // �F�����ɂ��鏈��
                    //var child = getCharas[i].transform.GetChild(0);
                    //child.GetComponent<Image>().color=new Color(255,255,255,0);

                    // �A�j���[�^�[�擾
                    if (getCharas[i] != null)
                    {
                        var animator = getCharas[i].GetComponent<Animator>();
                        animator.SetTrigger("Turn");
                    }


                    if (i == MAX_CHARA_NUMBER - 1)
                    {
                        // �t���O������
                        isTurnStart = false;

                        


                    }
                }
                // ���o�I���t���O
                isEnd = true;
            }
        }
    }

    /// <summary>
    /// �K�`����ʂŉ�ʃ^�b�v������
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
        // �摜�̕\���ʒu�p
        pos.x= 0;
        pos.y = SCREEN_HEIGHT + CHARA_WIDTH;

    }

    void Update()
    {
        // �K�`�����o�X�^�[�g
        StartCoroutine(GachaAnimation());

        // �K�`�����o�^�[��
        StartCoroutine(GachaAnimationTurn());

        // �K�`�����o�I���㏈��
        if (isEnd)
        {
            // �{�^���\��
            exitButton.SetActive(true);

        }

    }

    /// <summary>
    /// �K�`���I���{�^������������
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
            // showArea�̎q�I�u�W�F�N�g�폜
            for (int i = MAX_CHARA_NUMBER; i > 0; i--)
            {
                Destroy(showArea.transform.GetChild(i - 1).gameObject);
            }
        }
        isEnd = false;

        // �摜�̕\���ʒu������
        pos.x = 0;
        pos.y = SCREEN_HEIGHT + CHARA_WIDTH;
    }

    

}
