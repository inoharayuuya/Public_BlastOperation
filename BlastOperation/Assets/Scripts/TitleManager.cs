using UnityEngine;
using Const;
using Json;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// パネルの名前を管理する構造体
/// </summary>
public enum PanelName
{
    ACCOUNT,
    RESPONSE,
};

public class TitleManager : MonoBehaviour
{
    // ユーザー名を入力するインプットフィールド
    [SerializeField] InputField nameInputField;

    // パネルをセット
    [SerializeField] GameObject[] panels;

    // アカウント新規登録パネルをセット
    [SerializeField] GameObject accoutnButton;

    // ログイン中のユーザー名を表示するテキスト
    [SerializeField] Text loginNameText;

    // エラーメッセージ表示用
    [SerializeField] Text dialog;

    private bool isCommunication;  // 通信中かどうか
    private bool auto_flg;         // パスワードを自動生成するかどうか

    private string password;  // パスワード

    /// <summary>
    /// 画面タップでメインシーンへ遷移
    /// </summary>
    public void TapScreen()
    {
        // PlayerPrefsのキーを探す
        if (PlayerPrefs.HasKey(Common.KEY_USER_ID))
        {
            // キーの取得
            MainData.instance.userId = PlayerPrefs.GetString(Common.KEY_USER_ID);

            // メインデータ取得APIと通信
            StartCoroutine(LoadMainData());
        }
        else
        {
            // アカウント登録パネルを表示
            panels[(int)PanelName.ACCOUNT].SetActive(true);
        }
    }

    /// <summary>
    /// アカウントボタン押下時の処理
    /// アカウント切り替え(名前を入力→見つかればログイン、なければ新規作成)
    /// </summary>
    public void TapAccountButton()
    {
        // 入力された名前を取得
        var name = nameInputField.text;
        if (name == "")  // 入力された名前がNULLの場合は後続の処理をさせない
        {
            return;
        }

        // アカウント登録パネルを非表示
        panels[(int)PanelName.ACCOUNT].SetActive(false);

        // PlayerPrefsに名前を保存
        MainData.instance.playerInfo.name = name;
        //PlayerPrefs.Save();

        // ログに表示
        Debug.Log("account_name：" + name);

        // 通信処理
        StartCoroutine(CreateAccount());
    }

    /// <summary>
    /// ログアウトAPI呼び出し
    /// </summary>
    public void TapLogoutButton()
    {
        // アカウント登録パネルを表示
        //panels[(int)PanelName.ACCOUNT].SetActive(true);

        // 通信処理
        StartCoroutine(Logout());
    }

    /// <summary>
    /// 外部ファイル読み込み
    /// </summary>
    void LoadPass()
    {
#if UNITY_ANDROID
        MainData.instance.apiUrl = "http://10.22.53.100/r06/3m/SrvBlastOperation/api/";
#else
        FileInfo fi = new FileInfo(Application.dataPath + "/StreamingAssets/api_url 1.txt");
        try
        {
            // 一行毎読み込み
            using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                MainData.instance.apiUrl = sr.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            print("外部ファイル読み込み失敗");
            return;
        }
#endif
        print("api_url:" + MainData.instance.apiUrl);
    }

    private void Awake()
    {
        LoadPass();  // 外部ファイル読み込み
    }

    // Start is called before the first frame update
    void Start()
    {
        Init();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            PlayerPrefs.DeleteAll();
        }
        DispName();
        SearchKey();
    }

    /// <summary>
    /// 初期化
    /// </summary>
    private void Init()
    {
        // イベントをセット
        nameInputField.onValidateInput += Common.OnValidateInput;

        // 名前表示領域の非表示
        loginNameText.enabled = false;

        // 通信処理のフラグを初期化
        isCommunication = false;
    }

    /// <summary>
    /// ログイン中のアカウント名を表示
    /// </summary>
    private void DispName()
    {
        // ログイン中の名前を取得
        if (MainData.instance.playerInfo.name != "")
        {
            // PlayerPrefsから名前を取得
            var name = MainData.instance.playerInfo.name;

            // 名前表示領域の表示
            loginNameText.gameObject.SetActive(true);

            // 名前表示領域にアカウント名をセット
            loginNameText.text = name + "でログイン中";
        }
        else
        {
            // 名前表示領域の非表示
            loginNameText.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// キーを探す
    /// </summary>
    private void SearchKey()
    {
        // PlayerPrefsのキーを探す
        if (PlayerPrefs.HasKey(Common.KEY_USER_ID))
        {
            // アカウント切り替えボタンを表示
            accoutnButton.SetActive(true);
        }
        else
        {
            // アカウント切り替えボタンを非表示
            accoutnButton.SetActive(false);
        }
    }

    /// <summary>
    /// アカウント新規登録APIとの通信用
    /// </summary>
    private IEnumerator CreateAccount()
    {
        // 通信開始
        isCommunication = false;

        // パラメータ設定
        WWWForm form = new WWWForm();
        auto_flg = true;

        // リクエストパラメーターをセット
        form.AddField("name", MainData.instance.playerInfo.name);
        form.AddField("auto_flg", Convert.ToInt32(auto_flg).ToString());

        // リクエストパラメータを表示
        print("account_name:" + MainData.instance.playerInfo.name);
        print("auto_flg:" + auto_flg.ToString());

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_CREATE_ACCOUNT;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();

        // レスポンスを表示
        print("アカウント新規作成" + res);

        // Jsonデータをシリアライズ
        var json_create_account = JsonUtility.FromJson<JsonCreateAccunt>(res);
        if (json_create_account.result == "NG")
        {
            print("アカウントの新規登録に失敗しました。始めからやり直してください。");
            yield break;
        }

        // 通信結果の表示
        print(json_create_account.result);

        // パスワードを保持
        password = json_create_account.password;

        // 通信終了
        isCommunication = true;

        // ログインAPIを実行
        StartCoroutine(Login());
    }

    /// <summary>
    /// アカウント新規登録APIとの通信用
    /// </summary>
    private IEnumerator Login()
    {
        // 通信開始
        isCommunication = false;

        // パラメータ設定
        WWWForm form = new WWWForm();

        // リクエストパラメーターをセット
        form.AddField("name", MainData.instance.playerInfo.name);
        form.AddField("password", password);
        form.AddField("auto_flg", Convert.ToInt32(auto_flg).ToString());

        // リクエストパラメータを表示
        print("account_name:" + MainData.instance.playerInfo.name);
        print("password:" + password);
        print("auto_flg:" + auto_flg.ToString());

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_LOGIN;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();

        // レスポンスを表示
        print(res);

        // Jsonデータをシリアライズ
        var json_login = JsonUtility.FromJson<JsonLogin>(res);
        if (json_login.result == "NG")
        {
            print("ログインに失敗しました。始めからやり直してください。");
            yield break;
        }

        // 通信結果を表示
        print("ログイン情報" + json_login.result);

        // ユーザーIDを保存
        MainData.instance.userId = json_login.user_id;
        PlayerPrefs.SetString(Common.KEY_USER_ID, MainData.instance.userId);
        PlayerPrefs.Save();

        // 通信終了
        isCommunication = true;

        // メインデータ取得APIと通信
        StartCoroutine(LoadMainData());
    }

    /// <summary>
    /// アカウント新規登録APIとの通信用
    /// </summary>
    private IEnumerator Logout()
    {
        // 通信開始
        isCommunication = false;

        // リクエストパラメーターを取得
        string tmp = null;
        if (MainData.instance.userId != "")
        {
            // ユーザーIDを取得
            tmp = MainData.instance.userId;
        }
        else
        {
            print("ログインしていません");
            yield break;
        }

        // パラメータ設定
        WWWForm form = new WWWForm();
        var user_id = tmp;

        // リクエストパラメーターをセット
        form.AddField("user_id", user_id);

        // リクエストパラメータを表示
        print("user_id:" + user_id);

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_LOGOUT;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();

        // レスポンスを表示
        print(res);

        // Jsonデータをシリアライズ
        var json_logout = JsonUtility.FromJson<JsonLogout>(res);
        if (json_logout.result == "NG")
        {
            print("ログアウトに失敗しました。始めからやり直してください。");
            yield break;
        }

        // 通信結果を表示
        print(json_logout.result);

        // 通信終了
        isCommunication = true;

        // 必要なくなったキーを削除
        PlayerPrefs.DeleteKey(Common.KEY_USER_ID);
    }

    /// <summary>
    /// メインデータ取得APIとの通信用
    /// </summary>
    private IEnumerator LoadMainData()
    {
        // 通信開始
        isCommunication = false;

        // パラメータ設定
        WWWForm form = new WWWForm();
        auto_flg = true;

        // リクエストパラメーターをセット
        form.AddField("user_id", MainData.instance.userId);

        // リクエストパラメータを表示
        print("user_id:" + MainData.instance.userId);

        // URLの設定
        string url = MainData.instance.apiUrl + Common.EP_GET_MAIN_DATA;

        // 通信開始
        var coroutine = HTTPManager.PostCommunication(url, form);
        yield return StartCoroutine(coroutine);

        // レスポンス受け取り
        var res = coroutine.Current.ToString();

        // レスポンスを表示
        print("response:" + res);

        // Jsonデータをシリアライズ
        var json_main_data = JsonUtility.FromJson<JsonMainData>(res);
        if (json_main_data.result == "NG")
        {
            print("アカウントの新規登録に失敗しました。始めからやり直してください。");
            PlayerPrefs.DeleteAll();
            yield break;
        }

        // メインデータの保存
        SetMainData(json_main_data);

        // 通信結果の表示
        print(json_main_data.result);

        // 通信終了
        isCommunication = true;

        Common.LoadScene(Common.SCENE_NAME_MAIN);
    }

    /// <summary>
    /// メインデータのセット
    /// </summary>
    private void SetMainData(JsonMainData json_main_data)
    {
        MainData.instance.playerInfo       = json_main_data.player_info;
        MainData.instance.characterMasters = json_main_data.character_masters;
        MainData.instance.stageMasters     = json_main_data.stage_masters;
    }
}
