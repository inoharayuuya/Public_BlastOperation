using System.Collections;
using System.Collections.Generic;
using Const;
using System;
using UnityEngine;
using UnityEngine.UI;

using Json;

public class StatusManager : MonoBehaviour
{
    // �z�[���ɕ\������L�����̐�
    private const int HOMECHARA_NUMBER = 3;

    // �N�G�X�g��
    private const int QUEST_NUMBER = 3;

    //private const string USER_ID = "8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918";
    //private const string TEST_URL = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";


    // �X�e�[�^�X�̃e�L�X�g
    [SerializeField] private Text staminaText;
    [SerializeField] private Text crystalText;
    [SerializeField] private Text nameText;
    [SerializeField] private Text rankText;
    [SerializeField] private Text cardRankText;

    // �����N�J�[�h�̃e�L�X�g
    [SerializeField] private InputField rNameText;
    [SerializeField] private InputField rCommentText;

    // �z�[���̃L����3��
    [SerializeField] private Image[] homeCharaImages = new Image[HOMECHARA_NUMBER];

    // �N�G�X�g
    [SerializeField] private GameObject[] questObjcts = new GameObject[QUEST_NUMBER];

    // �N�G�X�g�ڍׂɓn���悤�̔z��
    public string[] questName = new string[QUEST_NUMBER]; 
    public string[] questDetail = new string[QUEST_NUMBER];

    // �����L�����N�^�[
    [SerializeField] private GameObject hasCharacterView;  // �\���̈�
    [SerializeField] private GameObject charaTemplate;     // �L�����N�^�[�̃e���v���[�g

    // �Ґ�
    [SerializeField] private GameObject orgCharacterView;  // �Ґ��L�����\���̈�
    [SerializeField] private GameObject orgCharaTemplate;  // �Ґ��L�����̃e���v���[�g

    public string jsonHasChara;

    // ���[�f�B���O�p�l��
    [SerializeField] private GameObject startLoading;
    [SerializeField] private GameObject loadingSmall;

    /// <summary>
    /// �����L�����擾
    /// </summary>
    /// <returns></returns>
    public IEnumerator HasCharaAPI()
    {
        WWWForm form = new WWWForm();

        //var jsonHasChara = JsonUtility.ToJson(jsonStatus.player_info.has_characters);

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", MainData.instance.userId);
        form.AddField("player_info", JsonUtility.ToJson(MainData.instance.playerInfo));

        print("�����L����USERID:" + MainData.instance.userId);
        print("�����L�������N�G�X�g�v���C���[���:" + JsonUtility.ToJson(MainData.instance.playerInfo));


        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_CHARA;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();
        print("�L�����擾API" + res);
        // Json�f�[�^���V���A���C�Y (�V���A���C�Y��Json����class)
        var jsonCharas = JsonUtility.FromJson<JsonCharaData>(res);
        

        // �X�N���[���̈�̃I�u�W�F�N�g�폜

        // �q�I�u�W�F�N�g���擾
        var count = hasCharacterView.transform.childCount;
        if (count != 0)
        {
            for (int i = 0; i < count; i++)
            {
                GameObject child = hasCharacterView.transform.GetChild(i).gameObject;
                Destroy(child);
            }
        }

        if (jsonCharas.result == Common.SUCCES)
        {
            Debug.Log("����");

            // �����L�����N�^�[���k���`�F�b�N
            for (int i = 0; i < jsonCharas.character_ids.Length; i++)
            {
                // �k�����`�F�b�N �����L������id���L�����}�X�ɂ��邩�ǂ�������
                if (jsonCharas.character_ids[i] != 0)
                {
                    for (int j = 0; j < jsonCharas.character_masters.Length; j++)
                    {
                        // �L�����}�X��j�Ԗڂ��k������Ȃ����`�F�b�N
                        if (jsonCharas.character_masters[j] != null)
                        {
                            // �擾�����z�[���L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                            if (jsonCharas.character_ids[i] == jsonCharas.character_masters[j].id)
                            {
                                for (int k = 0; k < jsonCharas.character_masters.Length; k++)
                                {
                                    if (jsonCharas.character_masters[k] != null)
                                    {
                                        // �擾�����z�[���L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                                        if (jsonCharas.character_ids[i] == jsonCharas.character_masters[k].id)
                                        {

                                            Debug.Log("�͂������id�@�@" + jsonCharas.character_ids[i]);

                                            // �v���t�@�u���X�N���[���̈�ɐ���
                                            var tmp = Instantiate(charaTemplate, hasCharacterView.transform);

                                            //�g�p����L�����摜���擾 
                                            var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + jsonCharas.character_masters[k].name_id);
                                            // sprite�ύX
                                            tmp.GetComponent<Image>().sprite = charaSprite;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            yield break;
        }
        else
        {
            Debug.Log("���s");
        }

    }

    /// <summary>
    /// �X�e�[�^�X�擾
    /// </summary>
    /// <returns></returns>
    public IEnumerator StatusAPI()
    {
        //yield return null;
        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", MainData.instance.userId);

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_GET_MAIN_DATA;

        print(url + "�ŒʐM�J�n");
        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();
        // ���X�|���X��\��
        //print(res);


        // �f�o�b�O�p
        //var res = (Resources.Load("GachaJsonOnce",typeof(TextAsset))as TextAsset).text;
        //var res = (Resources.Load("StatusJson", typeof(TextAsset)) as TextAsset).text;
        // Json�f�[�^���V���A���C�Y (�V���A���C�Y��Json����class)
        var jsonStatus = JsonUtility.FromJson<JsonMainData>(res);

        jsonHasChara = JsonUtility.ToJson(jsonStatus.player_info);



        // �ʐM����������
        if (jsonStatus.result == Common.SUCCES)
        {
            OrganizationManager.isGet = true;
            OrganizationManager.isFirst = true;  // true��������

            print("�ʐM����");
            print("�R�����g  " + jsonStatus.player_info.comment);

            // �X�e�[�^�X��\��
            staminaText.text = (jsonStatus.player_info.energy).ToString() + "�m" + (jsonStatus.player_info.energy).ToString();
            crystalText.text = (jsonStatus.player_info.crystal).ToString();
            nameText.text = jsonStatus.player_info.name;
            rankText.text = jsonStatus.player_info.rank;
            cardRankText.text = jsonStatus.player_info.rank;

            // �M���h�J�[�h�̃e�L�X�g�X�V
            rNameText.text = jsonStatus.player_info.name;
            rCommentText.text = jsonStatus.player_info.comment;

            // �p�[�e�BID�̎擾
            var partyId = jsonStatus.player_info.party_id;

            // �z�[���̃L���������k���`�F�b�N TODO
            for (int i = 0; i < HOMECHARA_NUMBER; i++)
            {
                // �k�����`�F�b�N �z�[���L������id���L�����}�X�ɂ��邩�ǂ�������
                if (jsonStatus.player_info.party_infos[partyId].box_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // �L�����}�X��j�Ԗڂ��k������Ȃ����`�F�b�N
                        if (jsonStatus.character_masters[j] != null)
                        {
                            //TODO�@���̏�����ʐM���Ď擾���Ă���id�Ɣ�r�ɕύX

                            // �擾�����z�[���L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                            if (jsonStatus.player_info.party_infos[partyId].box_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                //�g�p����L�����摜���擾 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Body/" + jsonStatus.character_masters[j].name_id);
                                // sprite�ύX
                                homeCharaImages[i].GetComponent<Image>().sprite = charaSprite;
                            }
                        }
                    }
                }
            }

            // �Ґ��̃L���������k���`�F�b�N
            for (int i = 0; i < jsonStatus.player_info.party_infos[0].box_ids.Length; i++)
            {
                Destroy(OrganizationManager.orgChara[i]);

                // �k�����`�F�b�N �Ґ��L������id���L�����}�X�ɂ��邩�ǂ�������
                if (jsonStatus.player_info.party_infos[partyId].box_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // �L�����}�X��j�Ԗڂ��k������Ȃ����`�F�b�N
                        if (jsonStatus.character_masters[j] != null)
                        {
                            // �擾�����Ґ��L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                            if (jsonStatus.player_info.party_infos[partyId].box_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                // �v���t�@�u���X�N���[���̈�ɐ���
                                var tmp = Instantiate(orgCharaTemplate, orgCharacterView.transform);

                                //�g�p����L�����摜���擾 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Face/" + jsonStatus.character_masters[j].name_id);
                                // sprite�ύX
                                tmp.GetComponent<Image>().sprite = charaSprite;
                                OrganizationManager.orgChara[i] = tmp;
                            }
                        }
                    }
                }
            }

            // �����L�����N�^�[���k���`�F�b�N
            /*for (int i = 0; i < jsonStatus.player_info.has_character_ids.Length; i++)
            {
                // �k�����`�F�b�N �����L������id���L�����}�X�ɂ��邩�ǂ�������
                if (jsonStatus.player_info.has_character_ids[i] != 0)
                {
                    // 
                    for (int j = 0; j < jsonStatus.character_masters.Length; j++)
                    {
                        // �L�����}�X��j�Ԗڂ��k������Ȃ����`�F�b�N
                        if (jsonStatus.character_masters[j] != null)
                        {
                            // �擾�����z�[���L������id���L�����}�X��id�ƈ�v���Ă�����f�[�^�����o��
                            if (jsonStatus.player_info.has_character_ids[i] == jsonStatus.character_masters[j].id)
                            {
                                Debug.Log("�͂������id�@�@" + jsonStatus.player_info.has_character_ids[i]);

                                // �v���t�@�u���X�N���[���̈�ɐ���
                                var tmp = Instantiate(charaTemplate, hasCharacterView.transform);

                                //�g�p����L�����摜���擾 
                                var charaSprite = Resources.Load<Sprite>("Images/Characters/Body/" + jsonStatus.character_masters[j].name_id);
                                // sprite�ύX
                                tmp.GetComponent<Image>().sprite = charaSprite;
                            }
                        }
                    }
                }
            }*/

            // �N�G�X�g���X�V
            for (int i = 0; i < QUEST_NUMBER; i++)
            {
                // �N�G�X�g���ύX
                var questNameText = questObjcts[i].transform.Find("QuestNameText").GetComponent<Text>();
                questNameText.text = jsonStatus.stage_masters[i].name;

                // �N�G�X�g�ڍׂɓn���悤�̔z��Ɋi�[
                questName[i] = jsonStatus.stage_masters[i].name;
                questDetail[i] = jsonStatus.stage_masters[i].note;

                // �N���A���Ă�����摜�\��
                if (jsonStatus.player_info.progress_rates[i].is_clear)
                {
                    GameObject clearImage = questObjcts[i].transform.Find("ClearImage").gameObject;
                    clearImage.SetActive(true);
                }
            }
        }
        else
        {
            print("���擾�����ς�" + res);
            yield break;
        }
    }

    public void TestButton()
    {
        // �ʐM�J�n
        StartCoroutine(StatusAPI());
    }

    public void TapCharaListButton()
    {
        loadingSmall.SetActive(true);
        // �ʐM�J�n
        StartCoroutine(HasCharaAPI());
        loadingSmall.SetActive(false);
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        startLoading.SetActive(true);
        yield return StartCoroutine(StatusAPI());
        startLoading.GetComponent<Animator>().SetTrigger("Out");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    

}
