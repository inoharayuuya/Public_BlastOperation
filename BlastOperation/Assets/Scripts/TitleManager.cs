using UnityEngine;
using Const;
using Json;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// �p�l���̖��O���Ǘ�����\����
/// </summary>
public enum PanelName
{
    ACCOUNT,
    RESPONSE,
};

public class TitleManager : MonoBehaviour
{
    // ���[�U�[������͂���C���v�b�g�t�B�[���h
    [SerializeField] InputField nameInputField;

    // �p�l�����Z�b�g
    [SerializeField] GameObject[] panels;

    // �A�J�E���g�V�K�o�^�p�l�����Z�b�g
    [SerializeField] GameObject accoutnButton;

    // ���O�C�����̃��[�U�[����\������e�L�X�g
    [SerializeField] Text loginNameText;

    // �G���[���b�Z�[�W�\���p
    [SerializeField] Text dialog;

    private bool isCommunication;  // �ʐM�����ǂ���
    private bool auto_flg;         // �p�X���[�h�������������邩�ǂ���

    private string password;  // �p�X���[�h

    /// <summary>
    /// ��ʃ^�b�v�Ń��C���V�[���֑J��
    /// </summary>
    public void TapScreen()
    {
        // PlayerPrefs�̃L�[��T��
        if (PlayerPrefs.HasKey(Common.KEY_USER_ID))
        {
            // �L�[�̎擾
            MainData.instance.userId = PlayerPrefs.GetString(Common.KEY_USER_ID);

            // ���C���f�[�^�擾API�ƒʐM
            StartCoroutine(LoadMainData());
        }
        else
        {
            // �A�J�E���g�o�^�p�l����\��
            panels[(int)PanelName.ACCOUNT].SetActive(true);
        }
    }

    /// <summary>
    /// �A�J�E���g�{�^���������̏���
    /// �A�J�E���g�؂�ւ�(���O����́�������΃��O�C���A�Ȃ���ΐV�K�쐬)
    /// </summary>
    public void TapAccountButton()
    {
        // ���͂��ꂽ���O���擾
        var name = nameInputField.text;
        if (name == "")  // ���͂��ꂽ���O��NULL�̏ꍇ�͌㑱�̏����������Ȃ�
        {
            return;
        }

        // �A�J�E���g�o�^�p�l�����\��
        panels[(int)PanelName.ACCOUNT].SetActive(false);

        // PlayerPrefs�ɖ��O��ۑ�
        MainData.instance.playerInfo.name = name;
        //PlayerPrefs.Save();

        // ���O�ɕ\��
        Debug.Log("account_name�F" + name);

        // �ʐM����
        StartCoroutine(CreateAccount());
    }

    /// <summary>
    /// ���O�A�E�gAPI�Ăяo��
    /// </summary>
    public void TapLogoutButton()
    {
        // �A�J�E���g�o�^�p�l����\��
        //panels[(int)PanelName.ACCOUNT].SetActive(true);

        // �ʐM����
        StartCoroutine(Logout());
    }

    /// <summary>
    /// �O���t�@�C���ǂݍ���
    /// </summary>
    void LoadPass()
    {
#if UNITY_ANDROID
        MainData.instance.apiUrl = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";
#else
        FileInfo fi = new FileInfo(Application.dataPath + "/StreamingAssets/api_url 1.txt");
        try
        {
            // ��s���ǂݍ���
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                MainData.instance.apiUrl = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            print("�O���t�@�C���ǂݍ��ݎ��s");
            return;
        }
#endif
        print("api_url:" + MainData.instance.apiUrl);
    }

    private void Awake()
    {
        LoadPass();  // �O���t�@�C���ǂݍ���
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
        }
        DispName();
        SearchKey();
    }

    /// <summary>
    /// ������
    /// </summary>
    private void Init()
    {
        // �C�x���g���Z�b�g
        nameInputField.onValidateInput += Common.OnValidateInput;

        // ���O�\���̈�̔�\��
        loginNameText.enabled = false;

        // �ʐM�����̃t���O��������
        isCommunication = false;
    }

    /// <summary>
    /// ���O�C�����̃A�J�E���g����\��
    /// </summary>
    private void DispName()
    {
        // ���O�C�����̖��O���擾
        if (MainData.instance.playerInfo.name != "")
        {
            // PlayerPrefs���疼�O���擾
            var name = MainData.instance.playerInfo.name;

            // ���O�\���̈�̕\��
            loginNameText.gameObject.SetActive(true);

            // ���O�\���̈�ɃA�J�E���g�����Z�b�g
            loginNameText.text = name + "�Ń��O�C����";
        }
        else
        {
            // ���O�\���̈�̔�\��
            loginNameText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// �L�[��T��
    /// </summary>
    private void SearchKey()
    {
        // PlayerPrefs�̃L�[��T��
        if (PlayerPrefs.HasKey(Common.KEY_USER_ID))
        {
            // �A�J�E���g�؂�ւ��{�^����\��
            accoutnButton.SetActive(true);
        }
        else
        {
            // �A�J�E���g�؂�ւ��{�^�����\��
            accoutnButton.SetActive(false);
        }
    }

    /// <summary>
    /// �A�J�E���g�V�K�o�^API�Ƃ̒ʐM�p
    /// </summary>
    private IEnumerator CreateAccount()
    {
        // �ʐM�J�n
        isCommunication = false;

        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();
        auto_flg = true;

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("name", MainData.instance.playerInfo.name);
        form.AddField("auto_flg", Convert.ToInt32(auto_flg).ToString());

        // ���N�G�X�g�p�����[�^��\��
        print("account_name:" + MainData.instance.playerInfo.name);
        print("auto_flg:" + auto_flg.ToString());

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_CREATE_ACCOUNT;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();

        // ���X�|���X��\��
        print("�A�J�E���g�V�K�쐬" + res);

        // Json�f�[�^���V���A���C�Y
        var json_create_account = JsonUtility.FromJson<JsonCreateAccunt>(res);
        if (json_create_account.result == "NG")
        {
            print("�A�J�E���g�̐V�K�o�^�Ɏ��s���܂����B�n�߂����蒼���Ă��������B");
            yield break;
        }

        // �ʐM���ʂ̕\��
        print(json_create_account.result);

        // �p�X���[�h��ێ�
        password = json_create_account.password;

        // �ʐM�I��
        isCommunication = true;

        // ���O�C��API�����s
        StartCoroutine(Login());
    }

    /// <summary>
    /// �A�J�E���g�V�K�o�^API�Ƃ̒ʐM�p
    /// </summary>
    private IEnumerator Login()
    {
        // �ʐM�J�n
        isCommunication = false;

        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("name", MainData.instance.playerInfo.name);
        form.AddField("password", password);
        form.AddField("auto_flg", Convert.ToInt32(auto_flg).ToString());

        // ���N�G�X�g�p�����[�^��\��
        print("account_name:" + MainData.instance.playerInfo.name);
        print("password:" + password);
        print("auto_flg:" + auto_flg.ToString());

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_LOGIN;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();

        // ���X�|���X��\��
        print(res);

        // Json�f�[�^���V���A���C�Y
        var json_login = JsonUtility.FromJson<JsonLogin>(res);
        if (json_login.result == "NG")
        {
            print("���O�C���Ɏ��s���܂����B�n�߂����蒼���Ă��������B");
            yield break;
        }

        // �ʐM���ʂ�\��
        print("���O�C�����" + json_login.result);

        // ���[�U�[ID��ۑ�
        MainData.instance.userId = json_login.user_id;
        PlayerPrefs.SetString(Common.KEY_USER_ID, MainData.instance.userId);
        PlayerPrefs.Save();

        // �ʐM�I��
        isCommunication = true;

        // ���C���f�[�^�擾API�ƒʐM
        StartCoroutine(LoadMainData());
    }

    /// <summary>
    /// �A�J�E���g�V�K�o�^API�Ƃ̒ʐM�p
    /// </summary>
    private IEnumerator Logout()
    {
        // �ʐM�J�n
        isCommunication = false;

        // ���N�G�X�g�p�����[�^�[���擾
        string tmp = null;
        if (MainData.instance.userId != "")
        {
            // ���[�U�[ID���擾
            tmp = MainData.instance.userId;
        }
        else
        {
            print("���O�C�����Ă��܂���");
            yield break;
        }

        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();
        var user_id = tmp;

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", user_id);

        // ���N�G�X�g�p�����[�^��\��
        print("user_id:" + user_id);

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_LOGOUT;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();

        // ���X�|���X��\��
        print(res);

        // Json�f�[�^���V���A���C�Y
        var json_logout = JsonUtility.FromJson<JsonLogout>(res);
        if (json_logout.result == "NG")
        {
            print("���O�A�E�g�Ɏ��s���܂����B�n�߂����蒼���Ă��������B");
            yield break;
        }

        // �ʐM���ʂ�\��
        print(json_logout.result);

        // �ʐM�I��
        isCommunication = true;

        // �K�v�Ȃ��Ȃ����L�[���폜
        PlayerPrefs.DeleteKey(Common.KEY_USER_ID);
    }

    /// <summary>
    /// ���C���f�[�^�擾API�Ƃ̒ʐM�p
    /// </summary>
    private IEnumerator LoadMainData()
    {
        // �ʐM�J�n
        isCommunication = false;

        // �p�����[�^�ݒ�
        WWWForm form = new WWWForm();
        auto_flg = true;

        // ���N�G�X�g�p�����[�^�[���Z�b�g
        form.AddField("user_id", MainData.instance.userId);

        // ���N�G�X�g�p�����[�^��\��
        print("user_id:" + MainData.instance.userId);

        // URL�̐ݒ�
        string url = MainData.instance.apiUrl + Common.EP_GET_MAIN_DATA;

        // �ʐM�J�n
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // ���X�|���X�󂯎��
        var res = coroutine.Current.ToString();

        // ���X�|���X��\��
        print("response:" + res);

        // Json�f�[�^���V���A���C�Y
        var json_main_data = JsonUtility.FromJson<JsonMainData>(res);
        if (json_main_data.result == "NG")
        {
            print("�A�J�E���g�̐V�K�o�^�Ɏ��s���܂����B�n�߂����蒼���Ă��������B");
            PlayerPrefs.DeleteAll();
            yield break;
        }

        // ���C���f�[�^�̕ۑ�
        SetMainData(json_main_data);

        // �ʐM���ʂ̕\��
        print(json_main_data.result);

        // �ʐM�I��
        isCommunication = true;

        Common.LoadScene(Common.SCENE_NAME_MAIN);
    }

    /// <summary>
    /// ���C���f�[�^�̃Z�b�g
    /// </summary>
    private void SetMainData(JsonMainData json_main_data)
    {
        MainData.instance.playerInfo       = json_main_data.player_info;
        MainData.instance.characterMasters = json_main_data.character_masters;
        MainData.instance.stageMasters     = json_main_data.stage_masters;
    }
}
