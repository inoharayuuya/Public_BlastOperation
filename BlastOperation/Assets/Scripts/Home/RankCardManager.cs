using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankCardManager : MonoBehaviour
{
    // ランクカードの中身
    [SerializeField] private GameObject rankCard;
    private Animator rankCardAnimator; 

    // 各ダイアログ
    // 内容を保存するかどうか確認するダイアログ
    [SerializeField] private GameObject contentChangeDialog;
    // 内容を保存した時の確認ダイアログ
    [SerializeField] private GameObject submitDialog;

    // 各入力欄、アイコンボタン
    [SerializeField] private InputField nameInputField;
    [SerializeField] private InputField profileInputField;
    [SerializeField] private Button charaImageButton;

    // 内容を変更したかどうか、保存したかどうかのフラグ
    private bool isEdit;
    private bool isSubmit;

    // 確認ダイアログ表示時にランクカード閉じるボタン押下した時の挙動を管理するフラグ
    private bool isDialogActive;

    // キャラ選択画面
    [SerializeField] private GameObject charaSelect;
    private Animator charaSelectAnimator;
    private Sprite[] charaSprites;
    // アイコン表示領域
    [SerializeField] private GameObject iconScrollView;
    // アイコンテンプレ―ト
    [SerializeField] private GameObject iconCharaTemplate;

    // 変更前情報
    private string  oldNameText;
    private string  oldProfileText;
    Sprite oldCharaSprite;

    /// <summary>
    /// オブジェクト非表示処理
    /// </summary>
    private void ObjectAnActive()
    {
        rankCardAnimator.SetTrigger("Out");

        contentChangeDialog.SetActive(false);
        submitDialog.SetActive(false);
    }

    /// <summary>
    /// 変更前情報を取得する処理
    /// </summary>
    private void GetValueBeforeChange()
    {

        // 既に入力されていた値を取得しておく
        oldNameText = nameInputField.text;
        oldProfileText = profileInputField.text;
        oldCharaSprite = charaImageButton.GetComponent<Image>().sprite;

        isEdit = false;
    }

    /// <summary>
    /// ランクボタン押下時処理 ランクカード表示
    /// </summary>
    public void TapRankButton()
    {
        // 変更前情報を取得
        GetValueBeforeChange();

        rankCard.SetActive(true);
    }

    /// <summary>
    /// ランクカード閉じるボタン押下時処理 ランクカードを非表示
    /// </summary>
    public void TapRankCardExitButton()
    {

        // 編集していたら保存するか確認
        if (isEdit)
        {
            // 確認ダイアログを表示
            contentChangeDialog.SetActive(true);
            // 確認ダイアログが表示されている状態
            isDialogActive = true;
        }
        else
        {
            // ランクカード非表示
            ObjectAnActive();
        }
        
    }

    /// <summary>
    /// 確認ダイアログの変更しないボタン押下時
    /// </summary>
    public void TapNotSubmitButton()
    {
        // 変更前の情報に戻す
        nameInputField.text = oldNameText;
        profileInputField.text = oldProfileText;
        charaImageButton.GetComponent<Image>().sprite = oldCharaSprite;

        // ランクカードを非表示
        ObjectAnActive();
    }

    /// <summary>
    /// 確認ダイアログの変更するボタン押下時
    /// </summary>
    public void TapSubmitButton()
    {
        // 保存したことを伝えるダイアログ表示
        submitDialog.SetActive(true);
    }

    /// <summary>
    /// 確認ダイアログのキャンセルボタン(ランクカードを閉じる野をキャンセル)押下時
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
    /// 保存したことを伝えるダイアログのOKボタン押下時処理
    /// </summary>
    public void TapOkButton()
    {
        // ランクカードを非表示
        ObjectAnActive();
    }

    /// <summary>
    /// ランクカード内容保存ボタン押下時処理
    /// </summary>
    public void TapRankCardSubmitButton()
    {
        
        // TODO ここに編集後のパラメーター変更処理
        if (isEdit)
        {
            // 保存したことを伝えるダイアログ表示
            submitDialog.SetActive(true);
        }
    }

    /// <summary>
    /// 内容が変更された場合に呼び出す 編集したフラグを立てる
    /// </summary>
    public void OnValueChanged()
    {
        isEdit = true;
    }

    /// <summary>
    /// キャラアイコン押下時処理　キャラ選択画面出す
    /// </summary>
    public void TapCharaIconButton()
    {
        charaSelect.SetActive(true);
    }

    /// <summary>
    /// アイコンキャラを選択した時の処理
    /// </summary>
    public void TapNewCharaIcon(GameObject _obj)
    {
        // idを取得
        // TODO　ここで通信処理でid取得
        var id = int.Parse(_obj.name);

        // タグをもとにスプライトを割り振る
        charaImageButton.GetComponent<Image>().sprite = charaSprites[id];

        // 編集フラグをオン
        isEdit = true;

        // 選択画面非表示
        //charaSelect.SetActive(false);
        charaSelectAnimator.SetTrigger("Out");

    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    private void Init()
    {
        // アニメーター取得
        rankCardAnimator = rankCard.GetComponent<Animator>();
        charaSelectAnimator = charaSelect.GetComponent<Animator>();

        // Assets/Resources/Images/IconSprites/ 配下 すべてのスプライトを取得する
        charaSprites = Resources.LoadAll<Sprite>("Images/IconSprites/");

        // スクロール領域にアイコンを表示
        for (int i = 0; i < charaSprites.Length; i++)
        {
            var temp = Instantiate(iconCharaTemplate, iconScrollView.transform);
            temp.GetComponent<Image>().sprite = charaSprites[i];
            temp.name = i.ToString();
        }

        // プレイヤー名取得
        // TODO 通信して取得？
        //nameInputField.text = "プレイヤー名";
    }

    // Start is called before the first frame update
    void Start()
    {
        // 初期化処理
        Init();

    }

}
