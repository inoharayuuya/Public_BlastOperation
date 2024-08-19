using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Const;
using UnityEngine.UI;

public class ActiveUIManager : MonoBehaviour
{
    // �萔
    public const int MAX_ORG = 4;  // 1�Ґ��ɓ���L�����̐�
    private const float TIME_POINTER_DOWN = 2.0f;// ���������ĉ��b�o������\�����邩�Ɏg�p

    // �g���L�����o�X�A�p�l��
    [SerializeField] private GameObject[] useUI = new GameObject[12];

    // �g���L�����o�X
    [SerializeField] GameObject canvasHome;
    [SerializeField] GameObject canvasChara;
    [SerializeField] GameObject canvasGacha;

    // �g���p�l����
    [SerializeField] GameObject homeChara;         // �z�[���ɔz�u����Ă���L����
    [SerializeField] GameObject tapArea;           // �z�[���L�����̃^�b�v�̈�
    [SerializeField] GameObject menuPanel;         // ���j���[�p�l��
    [SerializeField] GameObject charaPanel;        // �L�����p�l��
    [SerializeField] GameObject organizationPanel; // �Ґ��p�l��
    [SerializeField] GameObject charaListPanel;    // �L�����ꗗ�p�l��
    [SerializeField] GameObject orgConfilmDialog;  // �Ґ��m�F�_�C�A���O
    [SerializeField] GameObject charaDetailPanel;  // �L�����ڍ׃p�l��
    [SerializeField] GameObject questPanel;        // �N�G�X�g�I���p�l��
    [SerializeField] GameObject questSelect;       // �A�j���[�V��������N�G�X�g�I�����
    [SerializeField] Button questSelectBackButton; // �N�G�X�g�I����ʂ̖߂�{�^��
    [SerializeField] GameObject questListPanel;    // �N�G�X�g�ꗗ�p�l��
    [SerializeField] GameObject questConfilmPanel; // �N�G�X�g�ŏI�m�F�p�l��

    // �Ґ��L����
    public GameObject orgChara;
    // �Ґ����[�h���ǂ����̃t���O
    public bool isOrg;
    // �ꊇ�Ґ����[�h���ǂ����̃t���O
    public bool isBulkOrg;

    // �L�������������Ɏg�p����^�C�}�[
    private float pointerDownTimer;
    // �L��������������t���O
    private bool isDown;

    // �L�����ꗗ�p
    private bool isList;

    // �ꊇ�Ґ��̃L������\������̈�
    public GameObject bulkOrgBg;
    // �ꊇ�Ґ��̃L��������c������J�E���g
    public int bulkCnt;
    // �L�����̃v���t�@�u
    [SerializeField] GameObject charaTemplate;

    // �Ґ����̃L����
    public GameObject[] org= new GameObject[MAX_ORG];

    // �N�G�X�g�ڍ׃e���v���[�g�̃v���t�@�u
    [SerializeField] GameObject detailTemplate;

    // �N�G�X�g�I�𒆂��ǂ���
    private bool isQuest;

    // �Ґ��p�̔z��

    #region �p�u���b�N�֐� - �{�^��OnClick���Ă΂�鏈��

    #region �S���ڋ���

    /// <summary>
    /// �S�Ă�UI���\���ɂ���֐�
    /// </summary>
    private void AnActiveAll()
    {
        for (int i = 0; i < useUI.Length; i++)
        {

            useUI[i].SetActive(false);
        }

        // �Ґ��t���O�I�t
        isOrg = false;
        isBulkOrg = false;
        isQuest = false;
        // �ꗗ�t���O�I�t
        isList = false;

    }

    /// <summary>
    /// �e�߂�{�^������������
    /// </summary>
    /// <param name="_activePanel">�\������p�l��(�ЂƂO�ɕ\�����Ă����p�l��)</param>
    public void TapBackButton(GameObject _activePanel)
    {
        // ���݊J���Ă���p�l���͔�\��
        //_anactivePanel.SetActive(false);

        // �ЂƂO�ɊJ���Ă����p�l����\��
        _activePanel.SetActive(true);

        // �Ґ����[�h�I�t
        isOrg = false;
        isBulkOrg = false;
    }

    public void TapQuestBackButton()
    {
        isQuest = false;
        homeChara.SetActive(true);
    }

    #endregion


    #region �z�[���֘A

    /// <summary>
    /// �z�[���{�^���������ɌĂяo������
    /// </summary>
    public void TapHomeButton()
    {
        questSelect.GetComponent<Animator>().SetTrigger("Back");
        questSelectBackButton.interactable = true;

        // ���ׂĂ�UI���\��
        AnActiveAll();

        // �z�[���̃L�����o�X��\��
        canvasHome.SetActive(true);

        // �z�[���̃L������\��
        homeChara.SetActive(true);
        tapArea.SetActive(true);
    }

    /// <summary>
    /// ���j���[�{�^������������
    /// </summary>
    public void TapMenuButton()
    {
        tapArea.SetActive(false);
        menuPanel.SetActive(true);
    }

    /// <summary>
    /// ���j���[����{�^������������
    /// </summary>
    public void TapMenuBackButton()
    {
        tapArea.SetActive(true);
        menuPanel.GetComponent<Animator>().SetTrigger("Out");
    }

    /// <summary>
    /// �^�C�g���ɖ߂�{�^������������
    /// </summary>
    public void TapToTitleButton()
    {
        Common.LoadScene(Common.SCENE_NAME_TITLE);
    }

    #endregion

    #region �N�G�X�g�֘A

    /// <summary>
    /// �z�[���̃N�G�X�g�{�^������������
    /// </summary>
    public void TapQuestButton()
    {
        menuPanel.SetActive(false);

        isQuest = true;
        homeChara.SetActive(false);

        questPanel.SetActive(true);
        tapArea.SetActive(false);
    }

    #endregion

    /// <summary>
    /// �N�G�X�g�I���̃m�[�}���N�G�X�g������
    /// </summary>
    public void TapNormalQuestButton()
    {
        questListPanel.SetActive(true);
    }


    /// <summary>
    /// �N�G�X�g�ꗗ����N�G�X�g������
    /// </summary>
    public void TapQuest()
    {
        detailTemplate.SetActive(true);
    }

    /// <summary>
    /// �N�G�X�g�󂯂�{�^��������
    /// </summary>
    public void TapOKButton()
    {
        canvasChara.SetActive(true);
        charaListPanel.SetActive(true);
        bulkOrgBg.SetActive(true);

        OrganizationManager.isGet = true;

        questConfilmPanel.SetActive(true);
        questListPanel.SetActive(false);
    }

    public void CanvasAnActive()
    {

        canvasChara.SetActive(false);
        charaListPanel.SetActive(false);
        bulkOrgBg.SetActive(false);
        homeChara.SetActive(false);
        tapArea.SetActive(false);
    }

    #region �L�����֘A

    /// <summary>
    /// �L�����{�^���������ɌĂяo������
    /// </summary>
    public void TapCharaButton()
    {
        questSelectBackButton.interactable = true;
        questSelect.GetComponent<Animator>().SetTrigger("Back");

        // ���ׂĂ�UI���\��
        AnActiveAll();

        // �L�����̃L�����o�X��\��
        canvasChara.SetActive(true);
        charaPanel.SetActive(true);
        homeChara.SetActive(false);
        tapArea.SetActive(false);
    }

    /// <summary>
    /// �e�L�����������̏���
    /// �Ґ����[�h�̗L���ŏ����𕪊�
    /// </summary>
    public void TapChara()
    {
        // �Ґ����[�h��
        if (isOrg)
        {
            Debug.Log("�Ґ����[�h��");

            // �Ґ��m�F�_�C�A���O�\��
            orgConfilmDialog.SetActive(true);
        }

        // �Ґ����[�h�ȊO
        if(isList)
        {
            Debug.Log("�Ґ����[�h����Ȃ�");

            // �L�����̏ڍו\��
            //charaDetailPanel.SetActive(true);
        }

        // �ꊇ�Ґ����[�h
        /*if (isBulkOrg)
        {
            if (bulkCnt < MAX_ORG) {
                // �v���t�@�u�𐶐�
                var temp = Instantiate(charaTemplate, bulkOrgBg.transform);
                temp.tag = "Bulk";
                
                bulkCnt++;
            }

        }*/
    }

    // �ꊇ�Ґ��ŃL������ǉ����鏈��
    public void AddBulkOrg(GameObject obj)
    {
        if (bulkCnt < MAX_ORG)
        {
            // �v���t�@�u�𐶐�
            var temp = Instantiate(charaTemplate, bulkOrgBg.transform);
            temp.tag = "Bulk";
            temp.GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;

            //org[bulkCnt] = temp;
            // �V�����L������ݒ�
            org[bulkCnt].GetComponent<Image>().sprite = obj.GetComponent<Image>().sprite;
            
            bulkCnt++;
        }
    }



    /// <summary>
    /// �e�L�����������ꂽ�ꍇ�̏��� ����������p
    /// CharaTemplateManager�ŁAeventtrigger�ɓo�^
    /// </summary>
    public void PointerDownChara()
    {
        Debug.Log("������");
        isDown = true;
    }

    /// <summary>
    /// �e�L��������w�������ꂽ�ꍇ�̏��� ����������p
    /// CharaTemplateManager�ŁAeventtrigger�ɓo�^
    /// </summary>
    public void PointerUpChara()
    {
        isDown = false;
        Debug.Log("������");


    }

    #region �Ґ��֘A

    /// <summary>
    /// �L�����Ґ��{�^������������
    /// </summary>
    public void TapOrganizationButton()
    {
        isList = false;

        // 
        charaPanel.SetActive(false);

        //
        organizationPanel.SetActive(true);

        // �Ґ��t���O�I��
        isOrg = true;
    }

    /// <summary>
    /// �Ґ����̃L��������������
    /// </summary>
    public void TapOrgChara(GameObject _gameObject)
    {
        

        // �^�b�v�����Ґ��L�������擾
        orgChara = _gameObject;

        if (isQuest)
        {
            organizationPanel.SetActive(false);
        }
        else
        {
            // ���ׂĂ�UI���\��
            AnActiveAll();
        }

        // �Ґ��t���O�I��
        isOrg = true;

        // �L�����L�����o�X��\��
        canvasChara.SetActive(true);

        // �L�����ꗗ�p�l����\��
        charaListPanel.SetActive(true);
    }

    /// <summary>
    /// �ꊇ�Ґ��{�^������������
    /// </summary>
    public void TapBulkOrgButton()
    {
        // ���ׂĂ�UI���\��
        AnActiveAll();

        // �Ґ��t���O�I��
        isOrg = false;

        // �ꊇ�Ґ��t���O�I��
        isBulkOrg = true;

        // �L�����L�����o�X��\��
        canvasChara.SetActive(true);

        // �L�����ꗗ�p�l����\��
        charaListPanel.SetActive(true);

        // �ꊇ�Ґ���\��
        bulkOrgBg.SetActive(true);
    }

    /// <summary>
    /// �Ґ��m�F�_�C�A���O��"�͂�"�{�^������������
    /// </summary>
    public void TapOrgDialogYesButton()
    {
        // �_�C�A���O��\��
        orgConfilmDialog.SetActive(false);

        // �L�����ꗗ�p�l����\��
        charaListPanel.SetActive(false);

        // �N�G�X�g���[�h������
        if (!isQuest) {
            // �Ґ��p�l���\��
            organizationPanel.SetActive(true);
        }
        else
        {
            canvasHome.SetActive(true);
            questSelect.SetActive(true);
            questSelect.GetComponent<Animator>().SetTrigger("Idle");
            questPanel.SetActive(true);
            questConfilmPanel.SetActive(true);
            canvasChara.SetActive(false);
        }

        OrganizationManager.isFirst = false;
        OrganizationManager.isGet = true;
    }

    /// <summary>
    /// �Ґ��m�F�_�C�A���O��"������"�{�^������������
    /// </summary>
    public void TapOrgDialogNoButton()
    {
        // �_�C�A���O��\��
        //OrgConfilmDialog.SetActive(false);
    }

    #endregion

    #region �ꗗ�֘A

    /// <summary>
    /// �L�����ꗗ�{�^������������
    /// </summary>
    public void TapCharaListButton()
    {
        isList = true;
        isOrg = false;
        isBulkOrg = false;

        // 
        charaPanel.SetActive(false);

        //
        charaListPanel.SetActive(true);
    }

    /// <summary>
    /// �L�����ꗗ�p�l���̖߂�{�^����������
    /// </summary>
    public void TapCharaListBackButton()
    {
            charaListPanel.SetActive(false);

        if (!isQuest)
        {
            // �Ґ����[�h��
            if (isOrg)
            {
                // �Ґ���ʂ֖߂�
                organizationPanel.SetActive(true);
            }
            // �Ґ����[�h�ȊO
            else
            {
                // �L�����p�l���֖߂�
                charaPanel.SetActive(true);
            }
        }
        else
        {
            canvasChara.SetActive(false);
        }
    }

    #endregion

    #endregion


    #region �K�`���֘A

    /// <summary>
    /// �K�`���{�^���������ɌĂяo������
    /// </summary>
    public void TapGachaButton()
    {
        questSelectBackButton.interactable = true;
        questSelect.GetComponent<Animator>().SetTrigger("Back");


        // ���ׂĂ�UI���\��
        AnActiveAll();

        // �K�`���̃L�����o�X��\��
        canvasGacha.SetActive(true);
        homeChara.SetActive(false);
        tapArea.SetActive(false);

    }

    #endregion

    #endregion



    void Start()
    {


            // �ꊇ�Ґ��̏���
            for (int i = 0; i < MAX_ORG; i++)
        {
            // �e���v���[�g�v���t�@�u�𐶐�
            var temp = Instantiate(charaTemplate, bulkOrgBg.transform);

            // �ꊇ�Ґ��̌����ڂ��Ґ��Ɠ����ɂ���
            temp.tag = "Bulk";

            // �Ґ����̃L�����擾
            if (org[i] != null)
            {
                var orgChara = org[i];
                temp.GetComponent<Image>().sprite = orgChara.GetComponent<Image>().sprite;
                bulkCnt++;
            }
            else
            {
                Destroy(temp);
            }

        }
    }


    void Update()
    {
        //Debug.Log("�������Ƃ��ӂ炂�@" + isQuest);

        if (isDown)
        {
            pointerDownTimer += Time.deltaTime;
        }
        else
        {
            pointerDownTimer = 0;
        }

        if(pointerDownTimer >= TIME_POINTER_DOWN)
        {
            // �L�����̏ڍו\��
            //charaDetailPanel.SetActive(true);
        }

    }
}
