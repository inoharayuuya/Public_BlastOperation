using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Const;
using UnityEngine.UI;

public class QuestDetailManager : MonoBehaviour
{
    // �g�p����p�[�e�B�ԍ�
    private const int USE_PARTY_NUMBER = 0; 

    // �N�G�X�g�ڍ׉��
    [SerializeField] private GameObject questDetail;

    // �X�e�[�^�X�}�l�[�W���[�擾
    private StatusManager statusManager;

    /// <summary>
    /// �N�G�X�g�{�^�������������@�N�G�X�g�ɂ���ďڍ׃e�L�X�g��ύX
    /// </summary>
    /// <param name="_questId"></param>
    public void TapStageButton(int _questId)
    {
        OrganizationManager.isGet = true;
        OrganizationManager.isFirst = false;

        // �X�e�[�^�X�}�l�[�W���[�擾
        statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();

        // �N�G�X�g���ύX
        var questTitleText = questDetail.transform.Find("QuestTitleText").GetComponent<Text>();
        questTitleText.text = statusManager.questName[_questId-1];

        // �N�G�X�g�ڍוύX
        var questDetailText = questDetail.transform.Find("QuestDetailText").GetComponent<Text>();
        questDetailText.text = statusManager.questDetail[_questId-1];

        // �X�e�[�WID�ƃp�[�e�B���ۑ�
        
        PlayerPrefs.SetInt(Common.KEY_STAGE_ID,_questId);
        PlayerPrefs.SetInt(Common.KEY_PARTY_ID,USE_PARTY_NUMBER);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// �N�G�X�g�o���{�^������������
    /// </summary>
    public void TapGoButton()
    {
        Common.LoadScene(Common.SCENE_NAME_QUEST);
    }

    // Start is called before the first frame update
    void Start()
    {
        statusManager = GameObject.Find("StatusManager").GetComponent<StatusManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
