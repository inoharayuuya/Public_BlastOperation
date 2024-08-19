using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankCardManager : MonoBehaviour
{
    // �����N�J�[�h�̒��g
    [SerializeField] private GameObject rankCard;
    private Animator rankCardAnimator; 

    // �e�_�C�A���O
    // ���e��ۑ����邩�ǂ����m�F����_�C�A���O
    [SerializeField] private GameObject contentChangeDialog;
    // ���e��ۑ��������̊m�F�_�C�A���O
    [SerializeField] private GameObject submitDialog;

    // �e���͗��A�A�C�R���{�^��
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField profileInputField;
    [SerializeField] private Button charaImageButton;

    // ���e��ύX�������ǂ����A�ۑ��������ǂ����̃t���O
    private bool isEdit;
    private bool isSubmit;

    // �m�F�_�C�A���O�\�����Ƀ����N�J�[�h����{�^�������������̋������Ǘ�����t���O
    private bool isDialogActive;

    // �L�����I�����
    [SerializeField] private GameObject charaSelect;
    private Animator charaSelectAnimator;
    private Sprite[] charaSprites;
    // �A�C�R���\���̈�
    [SerializeField] private GameObject iconScrollView;
    // �A�C�R���e���v���\�g
    [SerializeField] private GameObject iconCharaTemplate;

    // �ύX�O���
    private string  oldNameText;
    private string  oldProfileText;
    Sprite oldCharaSprite;

    /// <summary>
    /// �I�u�W�F�N�g��\������
    /// </summary>
    private void ObjectAnActive()
    {
        rankCardAnimator.SetTrigger("Out");

        contentChangeDialog.SetActive(false);
        submitDialog.SetActive(false);
    }

    /// <summary>
    /// �ύX�O�����擾���鏈��
    /// </summary>
    private void GetValueBeforeChange()
    {

        // ���ɓ��͂���Ă����l���擾���Ă���
        oldNameText = nameInputField.text;
        oldProfileText = profileInputField.text;
        oldCharaSprite = charaImageButton.GetComponent<Image>().sprite;

        isEdit = false;
    }

    /// <summary>
    /// �����N�{�^������������ �����N�J�[�h�\��
    /// </summary>
    public void TapRankButton()
    {
        // �ύX�O�����擾
        GetValueBeforeChange();

        rankCard.SetActive(true);
    }

    /// <summary>
    /// �����N�J�[�h����{�^������������ �����N�J�[�h���\��
    /// </summary>
    public void TapRankCardExitButton()
    {

        // �ҏW���Ă�����ۑ����邩�m�F
        if (isEdit)
        {
            // �m�F�_�C�A���O��\��
            contentChangeDialog.SetActive(true);
            // �m�F�_�C�A���O���\������Ă�����
            isDialogActive = true;
        }
        else
        {
            // �����N�J�[�h��\��
            ObjectAnActive();
        }
        
    }

    /// <summary>
    /// �m�F�_�C�A���O�̕ύX���Ȃ��{�^��������
    /// </summary>
    public void TapNotSubmitButton()
    {
        // �ύX�O�̏��ɖ߂�
        nameInputField.text = oldNameText;
        profileInputField.text = oldProfileText;
        charaImageButton.GetComponent<Image>().sprite = oldCharaSprite;

        // �����N�J�[�h���\��
        ObjectAnActive();
    }

    /// <summary>
    /// �m�F�_�C�A���O�̕ύX����{�^��������
    /// </summary>
    public void TapSubmitButton()
    {
        // �ۑ��������Ƃ�`����_�C�A���O�\��
        submitDialog.SetActive(true);
    }

    /// <summary>
    /// �m�F�_�C�A���O�̃L�����Z���{�^��(�����N�J�[�h��������L�����Z��)������
    /// </summary>
    public void TapCancelButton()
    {
        if (isDialogActive)
        {
            //contentChangeDialog.SetActive(false);
            isDialogActive = false;
        }
    }
    
    /// <summary>
    /// �ۑ��������Ƃ�`����_�C�A���O��OK�{�^������������
    /// </summary>
    public void TapOkButton()
    {
        // �����N�J�[�h���\��
        ObjectAnActive();
    }

    /// <summary>
    /// �����N�J�[�h���e�ۑ��{�^������������
    /// </summary>
    public void TapRankCardSubmitButton()
    {
        
        // TODO �����ɕҏW��̃p�����[�^�[�ύX����
        if (isEdit)
        {
            // �ۑ��������Ƃ�`����_�C�A���O�\��
            submitDialog.SetActive(true);
        }
    }

    /// <summary>
    /// ���e���ύX���ꂽ�ꍇ�ɌĂяo�� �ҏW�����t���O�𗧂Ă�
    /// </summary>
    public void OnValueChanged()
    {
        isEdit = true;
    }

    /// <summary>
    /// �L�����A�C�R�������������@�L�����I����ʏo��
    /// </summary>
    public void TapCharaIconButton()
    {
        charaSelect.SetActive(true);
    }

    /// <summary>
    /// �A�C�R���L������I���������̏���
    /// </summary>
    public void TapNewCharaIcon(GameObject _obj)
    {
        // id���擾
        // TODO�@�����ŒʐM������id�擾
        var id = int.Parse(_obj.name);

        // �^�O�����ƂɃX�v���C�g������U��
        charaImageButton.GetComponent<Image>().sprite = charaSprites[id];

        // �ҏW�t���O���I��
        isEdit = true;

        // �I����ʔ�\��
        //charaSelect.SetActive(false);
        charaSelectAnimator.SetTrigger("Out");

    }

    /// <summary>
    /// ����������
    /// </summary>
    private void Init()
    {
        // �A�j���[�^�[�擾
        rankCardAnimator = rankCard.GetComponent<Animator>();
        charaSelectAnimator = charaSelect.GetComponent<Animator>();

        // Assets/Resources/Images/IconSprites/ �z�� ���ׂẴX�v���C�g���擾����
        charaSprites = Resources.LoadAll<Sprite>("Images/IconSprites/");

        // �X�N���[���̈�ɃA�C�R����\��
        for (int i = 0; i < charaSprites.Length; i++)
        {
            var temp = Instantiate(iconCharaTemplate, iconScrollView.transform);
            temp.GetComponent<Image>().sprite = charaSprites[i];
            temp.name = i.ToString();
        }

        // �v���C���[���擾
        // TODO �ʐM���Ď擾�H
        //nameInputField.text = "�v���C���[��";
    }

    // Start is called before the first frame update
    void Start()
    {
        // ����������
        Init();

    }

}
