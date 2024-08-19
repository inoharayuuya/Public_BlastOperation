using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEventFunc : MonoBehaviour
{
    // �N�G�X�g�֘A
    // �N�G�X�g�ꗗ���X�g
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private GameObject questDetailPanel;
    [SerializeField] private GameObject questConfilmPanel;
    [SerializeField] private Button detailBackButton;
    [SerializeField] private Button okButton;


    // ���j���[�{�^���������p
    [SerializeField] private GameObject hideImage;

    // �I�u�W�F�N�g���\���ɂ���
    private void ObjectAnActive()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// �N�G�X�g�ꗗ���X�g��\��
    /// </summary>
    private void QuestListActive()
    {
        //this.GetComponent<Animator>().SetBool("Back", false);
        questListPanel.SetActive(true);
    }

    /// <summary>
    /// �N�G�X�g�ڍׂ��\��
    /// </summary>
    private void QuestDetailAnActive()
    {
        //this.GetComponent<Animator>().SetBool("Back", false);
        questDetailPanel.SetActive(false);
    }

    /// <summary>
    /// �N�G�X�g�m�F�p�l����\��
    /// </summary>
    private void QuestConfilmPanelActive()
    {
        //questDetailPanel.GetComponent<Animator>().SetTrigger("Back");
        questDetailPanel.SetActive(false);
        questConfilmPanel.SetActive(true);
        hideImage.SetActive(false);

    }


    /// <summary>
    /// �{�^���̗L����
    /// </summary>
    private void ButtonActive()
    {
        detailBackButton.interactable = true;
        okButton.interactable = true;

        hideImage.SetActive(false);
    }

    /// <summary>
    /// �{�^��������
    /// </summary>
    private void ButtonAnActive()
    {
        okButton.interactable = false;
        detailBackButton.interactable = false;

        hideImage.SetActive(true);
    }

    /// <summary>
    /// ���j���[�{�^��������
    /// </summary>
    private void MenuButtonAnActive()
    {
        hideImage.SetActive(true);

    }

    /// <summary>
    /// ���j���[�{�^���L����
    /// </summary>
    private void MenuButtonActive()
    {
        hideImage.SetActive(false);

    }

    // �K�`�����o�J�n�t���O
    private void GachaAnimationStart()
    {
        GachaManager.isStart = true;
    }
}
