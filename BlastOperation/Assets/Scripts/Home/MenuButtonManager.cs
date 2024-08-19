using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuButtonManager : MonoBehaviour
{
    // メニューボタンの数
    private const int BUTTON_NUMBER = 3;

    // ボタンの名前の定数
    private const int BUTTON_CHARA = 0;
    private const int BUTTON_HOME  = 1;
    private const int BUTTON_GACHA = 2;

    // 選択中のフラグ名の定数
    private const string BUTTON_SELECT = "isSelect";

    // トグル取得
    private Toggle[] toggles = new Toggle[BUTTON_NUMBER];
    // アニメーター取得
    private Animator[] animators = new Animator[BUTTON_NUMBER];

    // 各ボタン取得
    [SerializeField] private GameObject[] buttons = new GameObject[BUTTON_NUMBER];

    /// <summary>
    /// 初期化処理　トグルとアニメーターのコンポーネント取得
    /// </summary>
    private void GetComponent()
    {
        // トグル、アニメーターのコンポーネント取得
        for (int i = 0; i < BUTTON_NUMBER; i++)
        {
            toggles[i] = buttons[i].GetComponent<Toggle>();
            animators[i] = buttons[i].GetComponent<Animator>();
        }
    }

    /// <summary>
    /// どのボタンが選択中かを見て、フラグの値を設定
    /// </summary>
    private void JudgeButtonSelect()
    {
        // 各ボタンが選択中の場合、他のトグルは選ばれていない状態に
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
        // 初期化処理　必要なコンポーネント取得
        GetComponent();
    }

    // Update is called once per frame
    void Update()
    {
        // ボタンの選択状態によりフラグに値を設定
        JudgeButtonSelect();

    }
}
