using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class QuestAnimationsManager : MonoBehaviour
{
    // �A�j���[�^�[�擾
    private Animator animator;

    // �N�G�X�g�I���{�^��
    [SerializeField] private Button normalQuestButton;
    [SerializeField] private Button eventQuestButton;

    // �e�߂�{�^��
    [SerializeField] private Button questBackButton;

    // �e�p�l��
    [SerializeField] private GameObject questSelectPanel;
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private GameObject questDetailPanel;
    [SerializeField] private GameObject questDetailImage;
    [SerializeField] private GameObject eventQuestMessage;

    /// <summary>
    /// �m�[�}���N�G�X�g�{�^��������
    /// </summary>
    public void TapNormalQuestButton()
    {
        eventQuestMessage.SetActive(false);

        // �{�^�����~
        normalQuestButton.interactable = false;
        questBackButton.interactable = false;

        // �Y�[���̃A�j���[�V����
        animator.SetTrigger("Zoom");

        // �X���C�h�A�E�g
        questSelectPanel.GetComponent<Animator>().SetTrigger("Back");

    }

    // �N�G�X�g�I���ɖ߂�{�^��������
    public void TapBackButton()
    {
        // �{�^���L����
        normalQuestButton.interactable = true;
        questBackButton.interactable = true;

        // �A�j���[�V�������͂��߂����
        animator.SetTrigger("Back");

        // �X���C�h�C��
        questSelectPanel.GetComponent<Animator>().SetTrigger("Show");

    }

    // �N�G�X�g�I�����Ƀ��X�g���\���ɂ���
    public void ListAnActive()
    {
        // �X���C�h�A�E�g
        questListPanel.GetComponent<Animator>().SetTrigger("Back");
        // �X���C�h�C��
        questDetailPanel.GetComponent<Animator>().SetTrigger("Show");


    }

    // �N�G�X�g�ڍׂ��烊�X�g��
    public void ListActive()
    {
        // �X���C�h�A�E�g
        //questDetailPanel.GetComponent<Animator>().SetTrigger("Back");
        //questDetailImage.GetComponent<Animator>().SetTrigger("Back");
        questDetailPanel.SetActive(false);

        // �X���C�h�C��
        questListPanel.GetComponent<Animator>().SetTrigger("Show");

    }

    // �ڍ׉�ʂ�OK�{�^��������
    public void TapOKButton()
    {
        questDetailImage.GetComponent<Animator>().SetTrigger("Stamp");

    }

    /// <summary>
    /// �C�x���g�N�G�X�g����������
    /// </summary>
    public void TapEventQuestButton()
    {
        eventQuestMessage.SetActive(true);
        eventQuestMessage.GetComponent<Animator>().SetTrigger("Show");
    }

    // Start is called before the first frame update
    void Start()
    {
        // �A�j���[�^�[�擾
        animator = GetComponent<Animator>();
    }


}
