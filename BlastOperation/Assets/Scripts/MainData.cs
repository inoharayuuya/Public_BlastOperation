using Json;
using Const;
using UnityEngine;

public class MainData : MonoBehaviour
{
    public static MainData instance;

    public string userId;   // ���[�U�[ID
    public string apiUrl;   // API��URL

    public JsonPlayerInfo        playerInfo;                                                    // �v���C���[���
    public JsonCharacterMaster[] characterMasters;                                              // �L�����N�^�[�̃}�X�^�[�f�[�^
    public JsonStageMaster[]     stageMasters     = new JsonStageMaster[Common.STAGE_NUMBER];   // �X�e�[�W�̃}�X�^�[�f�[�^

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
        }
    }
}
