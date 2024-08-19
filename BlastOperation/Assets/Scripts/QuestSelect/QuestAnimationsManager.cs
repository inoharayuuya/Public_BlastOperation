using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.UI;

public class QuestAnimationsManager : MonoBehaviour
{
    // アニメーター取得
    private Animator animator;

    // クエスト選択ボタン
    [SerializeField] private Button normalQuestButton;
    [SerializeField] private Button eventQuestButton;

    // 各戻るボタン
    [SerializeField] private Button questBackButton;

    // 各パネル
    [SerializeField] private GameObject questSelectPanel;
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private GameObject questDetailPanel;
    [SerializeField] private GameObject questDetailImage;
    [SerializeField] private GameObject eventQuestMessage;

    /// <summary>
    /// ノーマルクエストボタン押下時
    /// </summary>
    public void TapNormalQuestButton()
    {
        eventQuestMessage.SetActive(false);

        // ボタンを停止
        normalQuestButton.interactable = false;
        questBackButton.interactable = false;

        // ズームのアニメーション
        animator.SetTrigger("Zoom");

        // スライドアウト
        questSelectPanel.GetComponent<Animator>().SetTrigger("Back");

    }

    // クエスト選択に戻るボタン押下時
    public void TapBackButton()
    {
        // ボタン有効化
        normalQuestButton.interactable = true;
        questBackButton.interactable = true;

        // アニメーションをはじめからに
        animator.SetTrigger("Back");

        // スライドイン
        questSelectPanel.GetComponent<Animator>().SetTrigger("Show");

    }

    // クエスト選択時にリストを非表示にする
    public void ListAnActive()
    {
        // スライドアウト
        questListPanel.GetComponent<Animator>().SetTrigger("Back");
        // スライドイン
        questDetailPanel.GetComponent<Animator>().SetTrigger("Show");


    }

    // クエスト詳細からリストへ
    public void ListActive()
    {
        // スライドアウト
        //questDetailPanel.GetComponent<Animator>().SetTrigger("Back");
        //questDetailImage.GetComponent<Animator>().SetTrigger("Back");
        questDetailPanel.SetActive(false);

        // スライドイン
        questListPanel.GetComponent<Animator>().SetTrigger("Show");

    }

    // 詳細画面のOKボタン押下時
    public void TapOKButton()
    {
        questDetailImage.GetComponent<Animator>().SetTrigger("Stamp");

    }

    /// <summary>
    /// イベントクエスト押下時処理
    /// </summary>
    public void TapEventQuestButton()
    {
        eventQuestMessage.SetActive(true);
        eventQuestMessage.GetComponent<Animator>().SetTrigger("Show");
    }

    // Start is called before the first frame update
    void Start()
    {
        // アニメーター取得
        animator = GetComponent<Animator>();
    }


}
