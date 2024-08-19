using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace Const
{
    #region 列挙体
    /// <summary>
    /// 攻撃パターンの列挙
    /// </summary>
    public enum EnemyAttackPattern
    {
        BULLET_WATER_SPREAD = 1,
        SLASH_AREA,
        KNIFE_THROW,
        BREATH_FIRE_SMALL,
        TALE_AREA_SMALL,
        BREATH_FIRE_SWEEP_BIG,
        BREATH_FIRE_AROUND_BIG,
        TALE_AREA_BIG,
        HEAL_SINGLE,
        HEAL_MULTI,
        BULLET_MAGIC_FOLLOW
    }

    #endregion

    #region 構造体

    /// <summary>
    /// 敵生成時に必要な情報の構造体、生成時のInitの引数で渡す
    /// </summary>
    public struct EnemyInitDatas
    {
        /// <summary>体力</summary>
        public int hp;
        /// <summary>攻撃力</summary>
        public float atk;
        /// <summary>攻撃パターン</summary>
        public int[] atk_ptns;
        /// <summary>出現位置</summary>
        public Vector3 pos;
        /// <summary>大きさ</summary>
        public float size;
        /// <summary>弱点</summary>
        public string wk;
        /// <summary>ボスフラグ</summary>
        public bool isB;
        /// <summary>画像のパス</summary>
        public string path;
    }

    /// <summary>
    /// 攻撃、ダメージ判定に使う情報の構造体、ダメージインターフェースの引数で渡す
    /// </summary>
    public struct CharaData
    {
        public int atk;
        public string attr;
        public float speed;
    }

    #endregion


    /// <summary>
    /// 汎用クラス
    /// </summary>
    public class Common
    {
    	// ボールの操作をキャンセルするときの長さ
        public const float TAP_VECTOR_LIMIT = 0.5f;
        // 編成に入れられるキャラクターの最大数
        public const int PARTY_LIMIT = 4;
        // シーン名定数
        public const string SCENE_NAME_TITLE = "Title";
        public const string SCENE_NAME_QUEST = "Quest";
        public const string SCENE_NAME_MAIN = "Main";

        // 攻撃属性
        public const string ATTR_PHYSICAL = "物理";
        public const string ATTR_MAGICAL = "魔法";
        public const string ATTR_HEAL = "回復";
        public const float ATTR_WEAK_RATE = 2f;

        // １キャラの武器の数
        public const int WEAPON_LIMIT = 2;
        public const int MAX_CHARA_NUMBER = 10;
        public const int MAX_HAS_CHARA_NUMBER = 30;

        // ガチャ
        public const int MAX_GACHA_CHARA_NUMBER = 10;

        // ステージ数
        public const int STAGE_NUMBER = 4;
        // 編成数
        public const int ORG_NUMBER = 2;

        // プレイヤーのボールのサイズ（）
        public const float SIZE_PLAYER = 0.65f;

        // API通信関連
        public static string apiUrl = "";  // APIのリンク

        public const string SUCCES = "OK";
        public const string FAILED = "NG";

        public const string EP_CREATE_ACCOUNT   = "create_account";    // アカウント新規登録APIのエンドポイント
        public const string EP_LOGIN            = "login";             // ログインAPIのエンドポイント
        public const string EP_LOGOUT           = "logout";            // ログアウトAPIのエンドポイント
        public const string EP_GET_MAIN_DATA    = "get_main_data";     // メインデータ取得APIのエンドポイント
        public const string EP_SET_ACCOUNT_INFO = "set_account_info";  // アカウント情報登録APIのエンドポイント
        public const string EP_SET_IN_GAME      = "set_in_game";       // ゲーム中データ保存APIのエンドポイント
        public const string EP_GACHA            = "gacha";             // ガチャ実行APIのエンドポイント
        public const string EP_CHARA            = "get_character_info";// キャラ情報取得APIのエンドポイント
        public const string EP_GET_STAGE        = "get_stage_info";    //ステージ情報の取得 
        public const string EP_RESULT           = "result";            // リザルトの通信


        public const string KEY_ACCOUNT_NAME = "account_name";  // アカウント名を格納しているPrefsのキー名
        public const string KEY_USER_ID      = "user_id";       // ユーザーIDを格納しているPrefsのキー名
        public const string KEY_STAGE_ID     = "stage_id";       // ユーザーIDを格納しているPrefsのキー名
        public const string KEY_PARTY_ID     = "party_id";       // ユーザーIDを格納しているPrefsのキー名

        
        public static void LoadScene(string _sceneName)
        {
            SceneManager.LoadScene(_sceneName);
        }


        /// <summary>
        /// 指定フレーム後に指定の関数を呼び出す
        /// </summary>
        /// <param name="_delayFrame">遅延フレーム数</param>
        /// <param name="_action">呼び出す関数</param>
        /// <returns></returns>
        public static IEnumerator Delay(int _delayFrame, Action _action)
        {
            for (var i = 0; i < _delayFrame; i++)
            {
                yield return null;
            }
            _action();
        }

        /// <summary>
        /// 指定秒後に指定の関数を呼び出す
        /// </summary>
        /// <param name="_waitTime">遅延秒数</param>
        /// <param name="_action">呼び出す関数</param>
        /// <returns></returns>
        public static IEnumerator Delay(float _waitTime, Action _action)
        {
            yield return new WaitForSeconds(_waitTime);
            _action();
        }


        /// <summary>
        /// 0～1に正規化された値(_val)を_min～_maxに正規化して返す関数
        /// </summary>
        /// <param name="_val">0～1に正規化された値</param>
        /// <param name="_min">正規化する値の最小値</param>
        /// <param name="_max">正規化する値の最大値</param>
        /// <returns></returns>
        public static float NormalizedFunc(float _val, float _min, float _max)
        {
            return _val * (_max - _min) + _min;
        }

        /// <summary>
        /// _min1～_max1に正規化された値(_val)を_min2～_max2に正規化して返す関数
        /// </summary>
        /// <param name="_val">正規化したい値</param>
        /// <param name="_min1">_valがとる値の範囲の最小値</param>
        /// <param name="_max1">_valがとる値の範囲の最大値</param>
        /// <param name="_min2">正規化する値の最小値</param>
        /// <param name="_max2">正規化する値の最大値</param>
        /// <returns></returns>
        public static float NormalizedFunc(float _val, float _min1, float _max1, float _min2, float _max2)
        {
            // _valを0～1で正規化
            _val = (_val - _min1) / (_max1 - _min1);
            //_min2～_max2で正規化された値を返す
            return NormalizedFunc(_val, _min2, _max2);
        }

        /// <summary>
        /// バリデーション
        /// 入力された文字のチェックと変換
        /// 日本語と英数字以外入力できない
        /// </summary>
        public static char OnValidateInput(string text, int index, char addedChar)
        {
            //「ぁ」～「より」までと、「ー」「ダブルハイフン」をひらがなとする
            //「ダブルハイフン」から「コト」までと、カタカナフリガナ拡張と、
            //濁点と半濁点を全角カタカナとする
            //中点と長音記号も含む
            //CJK統合漢字、CJK互換漢字、CJK統合漢字拡張Aの範囲にあるか調べる
            if (
                '\u3041' <= addedChar && addedChar <= '\u309F' ||
                '\u30A0' <= addedChar && addedChar <= '\u30FF' ||
                '\u31F0' <= addedChar && addedChar <= '\u31FF' ||
                '\u3099' <= addedChar && addedChar <= '\u309C' ||
                '\u4E00' <= addedChar && addedChar <= '\u9FCF' ||
                '\uF900' <= addedChar && addedChar <= '\uFAFF' ||
                '\u3400' <= addedChar && addedChar <= '\u4DBF' ||
                addedChar == '\u30FC' || addedChar == '\u30A0'
               )
            {
                // 日本語の場合、入力された文字をそのまま返す
                return addedChar;
            }
            else
            {
                // アルファベットまたは数字でなければ入力しない
                if (char.IsLetterOrDigit(addedChar))
                {
                    // 英数字の場合、入力された文字をそのまま返す
                    return addedChar;
                }
                else
                {
                    // それ以外の場合、NULLを返す
                    return '\0';
                }
            }
        }
    }

    /// <summary>
    /// 通信処理
    /// </summary>
    public static class HTTPManager
    {
        /// <summary>
        /// GET通信処理
        /// </summary>
        public static IEnumerator GetCommunication(string url)
        {
            using (var req = UnityWebRequest.Get(url))
            {
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("通信完了");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.Log("通信エラー");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.Log("プロトコルエラー");
                        Debug.Log(req.error);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("エラー");
                        Debug.Log(req.error);
                        break;
                }
            }
        }

        /// <summary>
        /// POST通信処理
        /// </summary>
        public static IEnumerator PostCommunication(string url, WWWForm form)
        {
            using (var req = UnityWebRequest.Post(url, form))
            {
                yield return req.SendWebRequest();
                switch (req.result)
                {
                    case UnityWebRequest.Result.Success:
                        Debug.Log("通信完了");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ConnectionError:
                        Debug.Log("通信エラー");
                        yield return req.downloadHandler.text;
                        break;

                    case UnityWebRequest.Result.ProtocolError:
                        Debug.Log("プロトコルエラー");
                        Debug.Log(req.error);
                        yield return req.downloadHandler.text;
                        break;

                    default:
                        Debug.Log("エラー");
                        Debug.Log(req.error);
                        break;
                }
            }
        }
    }
}



