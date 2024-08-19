using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationEventFunc : MonoBehaviour
{
    // クエスト関連
    // クエスト一覧リスト
    [SerializeField] private GameObject questListPanel;
    [SerializeField] private GameObject questDetailPanel;
    [SerializeField] private GameObject questConfilmPanel;
    [SerializeField] private Button detailBackButton;
    [SerializeField] private Button okButton;


    // メニューボタン無効化用
    [SerializeField] private GameObject hideImage;

    // オブジェクトを非表示にする
    private void ObjectAnActive()
    {
        this.gameObject.SetActive(false);
    }

    /// <summary>
    /// クエスト一覧リストを表示
    /// </summary>
    private void QuestListActive()
    {
        //this.GetComponent<Animator>().SetBool("Back", false);
        questListPanel.SetActive(true);
    }

    /// <summary>
    /// クエスト詳細を非表示
    /// </summary>
    private void QuestDetailAnActive()
    {
        //this.GetComponent<Animator>().SetBool("Back", false);
        questDetailPanel.SetActive(false);
    }

    /// <summary>
    /// クエスト確認パネルを表示
    /// </summary>
    private void QuestConfilmPanelActive()
    {
        //questDetailPanel.GetComponent<Animator>().SetTrigger("Back");
        questDetailPanel.SetActive(false);
        questConfilmPanel.SetActive(true);
        hideImage.SetActive(false);

    }


    /// <summary>
    /// ボタンの有効化
    /// </summary>
    private void ButtonActive()
    {
        detailBackButton.interactable = true;
        okButton.interactable = true;

        hideImage.SetActive(false);
    }

    /// <summary>
    /// ボタン無効化
    /// </summary>
    private void ButtonAnActive()
    {
        okButton.interactable = false;
        detailBackButton.interactable = false;

        hideImage.SetActive(true);
    }

    /// <summary>
    /// メニューボタン無効化
    /// </summary>
    private void MenuButtonAnActive()
    {
        hideImage.SetActive(true);

    }

    /// <summary>
    /// メニューボタン有効化
    /// </summary>
    private void MenuButtonActive()
    {
        hideImage.SetActive(false);

    }

    // ガチャ演出開始フラグ
    private void GachaAnimationStart()
    {
        GachaManager.isStart = true;
    }
}
