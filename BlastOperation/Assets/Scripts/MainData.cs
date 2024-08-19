using Json;
using Const;
using UnityEngine;

public class MainData : MonoBehaviour
{
    public static MainData instance;

    public string userId;   // ユーザーID
    public string apiUrl;   // APIのURL

    public JsonPlayerInfo        playerInfo;                                                    // プレイヤー情報
    public JsonCharacterMaster[] characterMasters;                                              // キャラクターのマスターデータ
    public JsonStageMaster[]     stageMasters     = new JsonStageMaster[Common.STAGE_NUMBER];   // ステージのマスターデータ

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
