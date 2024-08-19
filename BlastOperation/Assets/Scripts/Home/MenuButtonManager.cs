using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    // ���j���[�{�^���̐�
    private const int BUTTON_NUMBER = 3;

    // �{�^���̖��O�̒萔
    private const int BUTTON_CHARA = 0;
    private const int BUTTON_HOME  = 1;
    private const int BUTTON_GACHA = 2;

    // �I�𒆂̃t���O���̒萔
    private const string BUTTON_SELECT = "isSelect";

    // �g�O���擾
    private Toggle[] toggles = new Toggle[BUTTON_NUMBER];
    // �A�j���[�^�[�擾
    private Animator[] animators = new Animator[BUTTON_NUMBER];

    // �e�{�^���擾
    [SerializeField] private GameObject[] buttons = new GameObject[BUTTON_NUMBER];

    /// <summary>
    /// �����������@�g�O���ƃA�j���[�^�[�̃R���|�[�l���g�擾
    /// </summary>
    private void GetComponent()
    {
        // �g�O���A�A�j���[�^�[�̃R���|�[�l���g�擾
        for (int i = 0; i < BUTTON_NUMBER; i++)
        {
            toggles[i] = buttons[i].GetComponent<Toggle>();
            animators[i] = buttons[i].GetComponent<Animator>();
        }
    }

    /// <summary>
    /// �ǂ̃{�^�����I�𒆂������āA�t���O�̒l��ݒ�
    /// </summary>
    private void JudgeButtonSelect()
    {
        // �e�{�^�����I�𒆂̏ꍇ�A���̃g�O���͑I�΂�Ă��Ȃ���Ԃ�
        if (toggles[BUTTON_CHARA].isOn)
        {
            animators[BUTTON_CHARA].SetBool(BUTTON_SELECT, true);

            animators[BUTTON_HOME].SetBool(BUTTON_SELECT, false);
            animators[BUTTON_GACHA].SetBool(BUTTON_SELECT, false);
        }
        else if (toggles[BUTTON_HOME].isOn)
        {
            animators[BUTTON_HOME].SetBool(BUTTON_SELECT, true);

            animators[BUTTON_CHARA].SetBool(BUTTON_SELECT, false);
            animators[BUTTON_GACHA].SetBool(BUTTON_SELECT, false);
        }
        else
        {
            animators[BUTTON_GACHA].SetBool(BUTTON_SELECT, true);

            animators[BUTTON_CHARA].SetBool(BUTTON_SELECT, false);
            animators[BUTTON_HOME].SetBool(BUTTON_SELECT, false);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // �����������@�K�v�ȃR���|�[�l���g�擾
        GetComponent();
    }

    // Update is called once per frame
    void Update()
    {
        // �{�^���̑I����Ԃɂ��t���O�ɒl��ݒ�
        JudgeButtonSelect();

    }
}
